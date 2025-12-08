using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Combat.Utilities;
using PokemonUltimate.Combat.Utilities.Extensions;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.AI
{
    /// <summary>
    /// Simple AI that selects a random valid move.
    /// Used for testing and basic enemy behavior.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.7: Integration
    /// **Documentation**: See `docs/features/2-combat-system/2.7-integration/architecture.md`
    /// </remarks>
    public class RandomAI : ActionProviderBase
    {
        private readonly Random _random;
        private readonly TargetResolver _targetResolver; // Reuse resolver to avoid allocation overhead

        /// <summary>
        /// Creates a new RandomAI instance.
        /// </summary>
        /// <param name="targetResolver">The target resolver for resolving move targets. Cannot be null.</param>
        /// <param name="seed">Optional seed for random number generator. If null, uses time-based seed.</param>
        public RandomAI(TargetResolver targetResolver, int? seed = null)
        {
            _targetResolver = targetResolver ?? throw new ArgumentNullException(nameof(targetResolver));
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        /// <summary>
        /// Gets a random action for the Pokemon slot.
        /// </summary>
        /// <param name="field">The current battlefield state. Cannot be null.</param>
        /// <param name="mySlot">The slot requesting an action. Cannot be null.</param>
        /// <returns>A UseMoveAction with a random valid move, or null if no moves available.</returns>
        /// <exception cref="ArgumentNullException">If field or mySlot is null.</exception>
        public override Task<BattleAction> GetAction(BattleField field, BattleSlot mySlot)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);
            if (mySlot == null)
                throw new ArgumentNullException(nameof(mySlot), ErrorMessages.PokemonCannotBeNull);

            // Return null if slot is empty or Pokemon is fainted
            if (!mySlot.IsActive())
                return Task.FromResult<BattleAction>(null);

            // Get moves with PP > 0
            var availableMoves = mySlot.Pokemon.Moves
                .Where(m => m.HasPP)
                .ToList();

            if (availableMoves.Count == 0)
                return Task.FromResult<BattleAction>(null);

            // Pick a random move
            var selectedMove = availableMoves[_random.Next(availableMoves.Count)];

            // Get basic valid targets (without redirections - those are applied by TargetResolutionStep)
            var validTargets = _targetResolver.GetBasicTargets(mySlot, selectedMove.Move, field);

            if (validTargets.Count == 0)
            {
                // No valid targets (e.g., Field move or all targets fainted)
                // For Field moves, we still create the action (target can be null or self)
                // For now, return null if no targets
                return Task.FromResult<BattleAction>(null);
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
            return Task.FromResult<BattleAction>(new UseMoveAction(mySlot, target, selectedMove));
        }

        /// <summary>
        /// Selects a random Pokemon for automatic switches (when Pokemon faint).
        /// Uses the default random selection behavior.
        /// </summary>
        public override Task<PokemonInstance> SelectAutoSwitch(BattleField field, BattleSlot mySlot, IReadOnlyList<PokemonInstance> availablePokemon)
        {
            if (availablePokemon == null || availablePokemon.Count == 0)
                return Task.FromResult<PokemonInstance>(null);

            // Select a random Pokemon from available switches
            var selectedPokemon = availablePokemon[_random.Next(availablePokemon.Count)];
            return Task.FromResult(selectedPokemon);
        }
    }
}

