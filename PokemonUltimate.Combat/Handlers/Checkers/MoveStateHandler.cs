using System.Linq;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Definition;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Effects;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Handlers.Checkers
{
    /// <summary>
    /// Handler para estados de movimientos.
    /// Maneja la cancelación de estados conflictivos de movimientos (charging, semi-invulnerable).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public class MoveStateHandler : ICheckerHandler
    {
        /// <summary>
        /// Cancela estados conflictivos de movimientos (charging diferente movimiento, semi-invulnerable con diferente movimiento).
        /// </summary>
        /// <param name="user">El slot del usuario. No puede ser null.</param>
        /// <param name="move">El movimiento que se va a usar. No puede ser null.</param>
        public void CancelConflictingMoveStates(BattleSlot user, MoveData move)
        {
            // Cancel charging if using a different move
            if (user.HasVolatileStatus(VolatileStatus.Charging) && user.ChargingMoveName != move.Name)
            {
                user.RemoveVolatileStatus(VolatileStatus.Charging);
                user.ClearChargingMove();
            }

            // Cancel semi-invulnerable if using a different move
            // Note: If using the same move with SemiInvulnerableEffect, we don't cancel here
            // because ProcessEffects will handle it (either charge turn or attack turn)
            bool hasSemiInvulnerableEffect = move.Effects.Any(e => e is SemiInvulnerableEffect);
            if (user.HasVolatileStatus(VolatileStatus.SemiInvulnerable) &&
                user.SemiInvulnerableMoveName != move.Name)
            {
                user.RemoveVolatileStatus(VolatileStatus.SemiInvulnerable);
                user.ClearSemiInvulnerableMove();
            }
            // If using the same move with SemiInvulnerableEffect, don't cancel - ProcessEffects will handle it
        }

        /// <summary>
        /// Determina si un movimiento es un movimiento de dispersión (golpea múltiples objetivos).
        /// </summary>
        /// <param name="move">El movimiento a verificar. No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <param name="validTargetCount">El número de objetivos válidos.</param>
        /// <returns>True si el movimiento es de dispersión.</returns>
        public bool IsSpreadMove(MoveData move, BattleField field, int validTargetCount)
        {
            // A move is a spread move if:
            // 1. It can target multiple Pokemon (based on TargetScope)
            // 2. There are multiple valid targets
            if (validTargetCount <= 1)
                return false;

            switch (move.TargetScope)
            {
                case TargetScope.AllEnemies:
                case TargetScope.AllAdjacent:
                case TargetScope.AllAdjacentEnemies:
                case TargetScope.AllOthers:
                    return true;

                case TargetScope.AllAllies:
                    // Only spread if there are multiple allies (excluding self)
                    return validTargetCount > 1;

                default:
                    return false;
            }
        }
    }
}
