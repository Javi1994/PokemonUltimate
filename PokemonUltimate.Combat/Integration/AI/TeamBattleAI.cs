using System;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Combat.Infrastructure.Events;
using PokemonUltimate.Combat.Infrastructure.Helpers;
using PokemonUltimate.Combat.Infrastructure.Logging;
using PokemonUltimate.Combat.Infrastructure.Providers;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Domain.Instances.Pokemon;
using PokemonUltimate.Core.Infrastructure.Localization;
using PokemonUltimate.Core.Utilities.Extensions;

namespace PokemonUltimate.Combat.Integration.AI
{
    /// <summary>
    /// AI designed for team battles that automatically switches when Pokemon faint.
    /// Handles both strategic switching (low HP) and mandatory switching (fainted Pokemon).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.7: Integration
    /// **Documentation**: See `docs/features/2-combat-system/2.7-integration/architecture.md`
    /// </remarks>
    public class TeamBattleAI : IActionProvider
    {
        private readonly Random _random;
        private readonly double _switchThreshold; // HP percentage below which to consider switching
        private readonly double _switchChance; // Probability of switching when conditions are met
        private readonly IBattleLogger _logger;
        private IBattleEventBus _eventBus;

        /// <summary>
        /// Creates a new TeamBattleAI instance.
        /// </summary>
        /// <param name="switchThreshold">HP percentage threshold (0.0-1.0) below which switching is considered. Default: 0.3 (30% HP).</param>
        /// <param name="switchChance">Probability (0.0-1.0) of switching when conditions are met. Default: 0.5 (50%).</param>
        /// <param name="seed">Optional seed for random number generator. If null, uses time-based seed.</param>
        /// <param name="logger">Optional logger for AI decision logging. If null, no logging is performed.</param>
        /// <param name="eventBus">Optional event bus for AI decision events. If null, events are not published.</param>
        public TeamBattleAI(double switchThreshold = 0.3, double switchChance = 0.5, int? seed = null, IBattleLogger logger = null, IBattleEventBus eventBus = null)
        {
            if (switchThreshold < 0.0 || switchThreshold > 1.0)
                throw new ArgumentException(ErrorMessages.PercentMustBeBetween0And1, nameof(switchThreshold));
            if (switchChance < 0.0 || switchChance > 1.0)
                throw new ArgumentException(ErrorMessages.PercentMustBeBetween0And1, nameof(switchChance));

            _switchThreshold = switchThreshold;
            _switchChance = switchChance;
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
            _logger = logger;
            _eventBus = eventBus;
        }

        /// <summary>
        /// Sets the event bus for this AI.
        /// Can be called after initialization to enable event publishing.
        /// </summary>
        /// <param name="eventBus">The event bus to use.</param>
        public void SetEventPublisher(IBattleEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        /// <summary>
        /// Gets an action for the Pokemon slot.
        /// Automatically switches if Pokemon is fainted or empty slot.
        /// Considers strategic switching when HP is low.
        /// </summary>
        /// <param name="field">The current battlefield state. Cannot be null.</param>
        /// <param name="mySlot">The slot requesting an action. Cannot be null.</param>
        /// <returns>A BattleAction (UseMoveAction or SwitchAction), or null if no action available.</returns>
        /// <exception cref="ArgumentNullException">If field or mySlot is null.</exception>
        public Task<BattleAction> GetAction(BattleField field, BattleSlot mySlot)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);
            if (mySlot == null)
                throw new ArgumentNullException(nameof(mySlot), ErrorMessages.PokemonCannotBeNull);

            var side = mySlot.Side;
            if (side == null)
                return Task.FromResult<BattleAction>(null);

            // CRITICAL: If slot is empty or Pokemon is fainted, MUST switch
            if (mySlot.IsEmpty || (mySlot.Pokemon != null && mySlot.Pokemon.IsFainted))
            {
                var autoSwitch = TryCreateAutoSwitch(field, mySlot, side);
                if (autoSwitch != null)
                {
                    var reason = $"Pokemon fainted or slot empty, switching to {autoSwitch.NewPokemon.DisplayName}";
                    PublishAIDecisionEvent(mySlot, "MANDATORY_SWITCH", reason, autoSwitch.NewPokemon);
                    return Task.FromResult<BattleAction>(autoSwitch);
                }

                // No Pokemon available to switch - battle should end
                PublishAIDecisionEvent(mySlot, "NO_SWITCH_AVAILABLE", "No Pokemon available to switch - party exhausted");
                return Task.FromResult<BattleAction>(null);
            }

            // Strategic switching: Consider switching when HP is low
            var shouldConsiderSwitch = ShouldConsiderSwitching(mySlot, field);
            if (shouldConsiderSwitch)
            {
                var switchAction = TryCreateSwitchAction(field, mySlot, side);
                if (switchAction != null)
                {
                    var hpPercent = (int)((double)mySlot.Pokemon.CurrentHP / mySlot.Pokemon.MaxHP * 100);
                    var reason = $"HP low ({hpPercent}%), switching to {switchAction.NewPokemon.DisplayName}";
                    PublishAIDecisionEvent(mySlot, "STRATEGIC_SWITCH", reason, switchAction.NewPokemon);
                    return Task.FromResult<BattleAction>(switchAction);
                }
            }

            // Otherwise, use a move
            var moveAction = CreateMoveAction(field, mySlot);
            if (moveAction is UseMoveAction useMove)
            {
                // Get localized move name
                var localizationProvider = LocalizationService.Instance;
                var moveName = useMove.Move.GetDisplayName(localizationProvider);
                var targetName = useMove.Target.Pokemon?.DisplayName ?? "Unknown";
                var reason = $"Using {moveName} on {targetName}";
                PublishAIDecisionEvent(mySlot, "ATTACK", reason, useMove.Target.Pokemon, moveName);
            }
            return Task.FromResult<BattleAction>(moveAction);
        }

        /// <summary>
        /// Attempts to create an automatic switch when Pokemon is fainted or slot is empty.
        /// </summary>
        private SwitchAction TryCreateAutoSwitch(BattleField field, BattleSlot slot, BattleSide side)
        {
            var availableSwitches = side.GetAvailableSwitches().ToList();
            if (availableSwitches.Count == 0)
                return null; // No Pokemon available

            // Select first available Pokemon (or random for variety)
            var selectedPokemon = availableSwitches[_random.Next(availableSwitches.Count)];
            return new SwitchAction(slot, selectedPokemon);
        }

        /// <summary>
        /// Determines if switching should be considered based on current Pokemon's HP.
        /// </summary>
        private bool ShouldConsiderSwitching(BattleSlot mySlot, BattleField field)
        {
            var pokemon = mySlot.Pokemon;
            if (pokemon == null || pokemon.IsFainted)
                return false;

            var hpPercent = (double)pokemon.CurrentHP / pokemon.MaxHP;

            // Consider switching if HP is below threshold
            if (hpPercent <= _switchThreshold)
            {
                // Random chance to actually switch
                return _random.NextDouble() < _switchChance;
            }

            return false;
        }

        /// <summary>
        /// Attempts to create a switch action if there are available Pokemon to switch to.
        /// </summary>
        private SwitchAction TryCreateSwitchAction(BattleField field, BattleSlot mySlot, BattleSide side)
        {
            var availableSwitches = side.GetAvailableSwitches().ToList();
            if (availableSwitches.Count == 0)
                return null; // No Pokemon available to switch

            // Select a random Pokemon from available switches
            var selectedPokemon = availableSwitches[_random.Next(availableSwitches.Count)];

            return new SwitchAction(mySlot, selectedPokemon);
        }

        /// <summary>
        /// Creates a move action using a random valid move.
        /// </summary>
        private BattleAction CreateMoveAction(BattleField field, BattleSlot mySlot)
        {
            // Get moves with PP > 0
            var availableMoves = mySlot.Pokemon.Moves
                .Where(m => m.HasPP)
                .ToList();

            if (availableMoves.Count == 0)
                return null; // No moves available

            // Pick a random move
            var selectedMove = availableMoves[_random.Next(availableMoves.Count)];

            // Get valid targets for this move
            var targetResolver = new TargetResolver();
            var validTargets = targetResolver.GetValidTargets(mySlot, selectedMove.Move, field);

            if (validTargets.Count == 0)
            {
                // No valid targets (e.g., Field move or all targets fainted)
                return null;
            }

            // Pick a random target (or use self if only one target and it's self)
            BattleSlot target;
            if (validTargets.Count == 1)
            {
                target = validTargets[0];
            }
            else
            {
                target = validTargets[_random.Next(validTargets.Count)];
            }

            // Return UseMoveAction
            return new UseMoveAction(mySlot, target, selectedMove);
        }

        /// <summary>
        /// Publishes an AI decision event.
        /// </summary>
        private void PublishAIDecisionEvent(BattleSlot slot, string decisionType, string reason, PokemonInstance targetPokemon = null, string moveName = null)
        {
            if (slot?.Pokemon == null || slot.Side == null)
                return;

            // Publish event if event bus is available (logging handled by EventBasedBattleLogger)
            if (_eventBus != null)
            {
                var @event = new BattleEvent(
                    BattleEventType.AIDecisionMade,
                    turnNumber: 0, // Will be set by caller if needed
                    isPlayerSide: slot.Side.IsPlayer,
                    pokemon: slot.Pokemon,
                    data: new BattleEventData
                    {
                        DecisionType = decisionType,
                        DecisionReason = reason,
                        MoveName = moveName,
                        TargetPokemon = targetPokemon
                    });

                _eventBus.PublishEvent(@event);
            }
        }
    }
}
