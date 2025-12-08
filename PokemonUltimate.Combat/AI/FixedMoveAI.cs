using System;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Constants;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Combat.Utilities;
using PokemonUltimate.Combat.Utilities.Extensions;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Domain.Instances.Move;

namespace PokemonUltimate.DeveloperTools.Runners
{
    /// <summary>
    /// AI provider that always selects a specific move.
    /// Used for testing specific moves in MoveRunner.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.6: Move Debugger
    /// **Documentation**: See `docs/features/6-development-tools/6.6-move-debugger/README.md`
    /// </remarks>
    public class FixedMoveAI : ActionProviderBase
    {
        private readonly MoveInstance _moveToUse;

        /// <summary>
        /// Creates a new FixedMoveAI that always uses the specified move.
        /// </summary>
        /// <param name="moveToUse">The move instance to always use. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If moveToUse is null.</exception>
        public FixedMoveAI(MoveInstance moveToUse)
        {
            _moveToUse = moveToUse ?? throw new ArgumentNullException(nameof(moveToUse));
        }

        /// <summary>
        /// Gets the action using the fixed move.
        /// </summary>
        /// <param name="field">The current battlefield state. Cannot be null.</param>
        /// <param name="mySlot">The slot requesting an action. Cannot be null.</param>
        /// <returns>A UseMoveAction with the fixed move, or null if the move is not available or no valid targets.</returns>
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

            // Check if the Pokemon has this move and it has PP
            var pokemonMove = mySlot.Pokemon.Moves.FirstOrDefault(m =>
                m.Move.Id == _moveToUse.Move.Id && m.HasPP);

            if (pokemonMove == null)
            {
                // Move not available (not learned or no PP)
                return Task.FromResult<BattleAction>(null);
            }

            // Get basic valid targets (without redirections - those are applied by TargetResolutionStep)
            var targetResolver = new TargetResolver();
            var validTargets = targetResolver.GetBasicTargets(mySlot, pokemonMove.Move, field);

            if (validTargets.Count == 0)
            {
                // No valid targets (e.g., Field move or all targets fainted)
                return Task.FromResult<BattleAction>(null);
            }

            // Pick the first valid target (or self if only one target and it's self)
            BattleSlot target;
            if (validTargets.Count == 1)
            {
                target = validTargets[0];
            }
            else
            {
                // For multi-target moves, pick the first enemy target, or first target if no enemy
                target = validTargets.FirstOrDefault(t =>
                    field.EnemySide.Slots.Contains(t)) ?? validTargets[0];
            }

            // Return UseMoveAction with the fixed move
            return Task.FromResult<BattleAction>(new UseMoveAction(mySlot, target, pokemonMove));
        }
    }
}
