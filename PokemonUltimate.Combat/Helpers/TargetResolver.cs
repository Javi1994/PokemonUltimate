using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Combat.Helpers
{
    /// <summary>
    /// Resolves valid targets for moves based on TargetScope and battlefield state.
    /// </summary>
    public static class TargetResolver
    {
        /// <summary>
        /// Gets all valid targets for a move based on its TargetScope.
        /// </summary>
        /// <param name="user">The slot using the move. Cannot be null.</param>
        /// <param name="move">The move data. Cannot be null.</param>
        /// <param name="field">The battlefield. Cannot be null.</param>
        /// <returns>List of valid target slots. May be empty if no valid targets.</returns>
        /// <exception cref="ArgumentNullException">If user, move, or field is null.</exception>
        public static List<BattleSlot> GetValidTargets(BattleSlot user, MoveData move, BattleField field)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), ErrorMessages.PokemonCannotBeNull);
            if (move == null)
                throw new ArgumentNullException(nameof(move), ErrorMessages.MoveCannotBeNull);
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            var potentialTargets = new List<BattleSlot>();

            // 1. Initial filtering based on TargetScope
            switch (move.TargetScope)
            {
                case TargetScope.Self:
                    return new List<BattleSlot> { user };

                case TargetScope.SingleEnemy:
                    potentialTargets.AddRange(GetOpposingSlots(user, field));
                    break;

                case TargetScope.SingleAlly:
                    potentialTargets.AddRange(GetAllySlots(user, field));
                    potentialTargets.Remove(user); // Exclude self unless move explicitly allows it
                    break;

                case TargetScope.AllEnemies:
                    potentialTargets.AddRange(GetOpposingSlots(user, field));
                    break;

                case TargetScope.AllAllies:
                    potentialTargets.AddRange(GetAllySlots(user, field));
                    break;

                case TargetScope.AllOthers:
                    potentialTargets.AddRange(field.GetAllActiveSlots());
                    potentialTargets.Remove(user);
                    break;

                case TargetScope.AllAdjacent:
                    // In singles, all slots are adjacent. In doubles/triples, this would check position.
                    // For now, treat as all active slots except self.
                    potentialTargets.AddRange(field.GetAllActiveSlots());
                    potentialTargets.Remove(user);
                    break;

                case TargetScope.AllAdjacentEnemies:
                    potentialTargets.AddRange(GetOpposingSlots(user, field));
                    break;

                case TargetScope.Any:
                    potentialTargets.AddRange(field.GetAllActiveSlots());
                    potentialTargets.Remove(user);
                    break;

                case TargetScope.RandomEnemy:
                    // Return all enemies, caller will pick random
                    potentialTargets.AddRange(GetOpposingSlots(user, field));
                    break;

                case TargetScope.Field:
                    // Field moves don't target specific slots
                    return new List<BattleSlot>();

                case TargetScope.UserAndAllies:
                    potentialTargets.Add(user);
                    potentialTargets.AddRange(GetAllySlots(user, field));
                    break;

                default:
                    // Unknown scope, return empty
                    return new List<BattleSlot>();
            }

            // 2. Filter out empty and fainted slots (standard rule)
            // Note: Some moves like "Revive" would skip this, but that's handled by the move effect
            potentialTargets = potentialTargets.Where(s => !s.IsEmpty && !s.HasFainted).ToList();

            // 3. Handle redirection (Follow Me, Rage Powder, etc.)
            // TODO: Implement redirection logic when volatile status system is complete
            // For now, return targets as-is

            return potentialTargets;
        }

        /// <summary>
        /// Gets all opposing slots (enemy side) relative to the user's side.
        /// </summary>
        private static IEnumerable<BattleSlot> GetOpposingSlots(BattleSlot user, BattleField field)
        {
            var oppositeSide = field.GetOppositeSide(user.Side);
            return oppositeSide.GetActiveSlots();
        }

        /// <summary>
        /// Gets all ally slots (same side) relative to the user.
        /// </summary>
        private static IEnumerable<BattleSlot> GetAllySlots(BattleSlot user, BattleField field)
        {
            return user.Side.GetActiveSlots();
        }
    }
}

