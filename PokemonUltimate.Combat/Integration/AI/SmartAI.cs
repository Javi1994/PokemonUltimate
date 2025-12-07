using System;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Extensions;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Combat.Infrastructure.Helpers;
using PokemonUltimate.Combat.Infrastructure.Providers;
using PokemonUltimate.Core.Data.Constants;

namespace PokemonUltimate.Combat.Integration.AI
{
    /// <summary>
    /// Smart AI that can make strategic decisions including switching Pokemon.
    /// Considers HP, type effectiveness, and available switches when making decisions.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.7: Integration
    /// **Documentation**: See `docs/features/2-combat-system/2.7-integration/architecture.md`
    /// </remarks>
    public class SmartAI : IActionProvider
    {
        private readonly Random _random;
        private readonly double _switchThreshold; // HP percentage below which to consider switching
        private readonly double _switchChance; // Probability of switching when conditions are met

        /// <summary>
        /// Creates a new SmartAI instance.
        /// </summary>
        /// <param name="switchThreshold">HP percentage threshold (0.0-1.0) below which switching is considered. Default: 0.3 (30% HP).</param>
        /// <param name="switchChance">Probability (0.0-1.0) of switching when conditions are met. Default: 0.5 (50%).</param>
        /// <param name="seed">Optional seed for random number generator. If null, uses time-based seed.</param>
        public SmartAI(double switchThreshold = 0.3, double switchChance = 0.5, int? seed = null)
        {
            if (switchThreshold < 0.0 || switchThreshold > 1.0)
                throw new ArgumentException(ErrorMessages.PercentMustBeBetween0And1, nameof(switchThreshold));
            if (switchChance < 0.0 || switchChance > 1.0)
                throw new ArgumentException(ErrorMessages.PercentMustBeBetween0And1, nameof(switchChance));

            _switchThreshold = switchThreshold;
            _switchChance = switchChance;
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        /// <summary>
        /// Gets an action for the Pokemon slot, considering switching when appropriate.
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

            // Return null if slot is empty or Pokemon is fainted
            if (!mySlot.IsActive())
                return Task.FromResult<BattleAction>(null);

            // Check if we should consider switching
            var shouldConsiderSwitch = ShouldConsiderSwitching(mySlot, field);
            if (shouldConsiderSwitch)
            {
                var switchAction = TryCreateSwitchAction(field, mySlot);
                if (switchAction != null)
                    return Task.FromResult<BattleAction>(switchAction);
            }

            // Otherwise, use a move
            return Task.FromResult<BattleAction>(CreateMoveAction(field, mySlot));
        }

        /// <summary>
        /// Determines if switching should be considered based on current Pokemon's HP.
        /// </summary>
        private bool ShouldConsiderSwitching(BattleSlot mySlot, BattleField field)
        {
            var pokemon = mySlot.Pokemon;
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
        private SwitchAction TryCreateSwitchAction(BattleField field, BattleSlot mySlot)
        {
            var side = mySlot.Side;
            if (side == null)
                return null;

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
    }
}
