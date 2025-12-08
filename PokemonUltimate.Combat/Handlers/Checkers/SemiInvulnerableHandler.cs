using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Definition;
using PokemonUltimate.Combat.Infrastructure.Constants;
using PokemonUltimate.Combat.Infrastructure.Messages.Definition;
using PokemonUltimate.Combat.Utilities.Extensions;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Domain.Instances.Pokemon;
using PokemonUltimate.Localization.Constants;

namespace PokemonUltimate.Combat.Handlers.Checkers
{
    /// <summary>
    /// Handler para semi-invulnerabilidad.
    /// Maneja la lógica de movimientos semi-invulnerables (Dig, Dive, Fly, etc.).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public class SemiInvulnerableHandler : ICheckerHandler
    {
        private readonly Infrastructure.Messages.Definition.IBattleMessageFormatter _messageFormatter;

        /// <summary>
        /// Crea una nueva instancia del handler de semi-invulnerabilidad.
        /// </summary>
        /// <param name="messageFormatter">El formateador de mensajes. Si es null, crea uno por defecto.</param>
        public SemiInvulnerableHandler(Infrastructure.Messages.Definition.IBattleMessageFormatter messageFormatter = null)
        {
            _messageFormatter = messageFormatter ?? new Infrastructure.Messages.BattleMessageFormatter();
        }

        /// <summary>
        /// Resultado de la verificación de semi-invulnerabilidad.
        /// </summary>
        public class SemiInvulnerableResult
        {
            /// <summary>
            /// Indica si el objetivo está semi-invulnerable y el movimiento falló.
            /// </summary>
            public bool MissedDueToSemiInvulnerable { get; set; }

            /// <summary>
            /// Mensaje de fallo si el movimiento falló. Null si no falló.
            /// </summary>
            public string MissMessage { get; set; }
        }

        /// <summary>
        /// Verifica si un movimiento puede golpear a un objetivo semi-invulnerable.
        /// </summary>
        /// <param name="target">El slot objetivo. No puede ser null.</param>
        /// <param name="move">El movimiento que se intenta usar. No puede ser null.</param>
        /// <returns>El resultado de la verificación.</returns>
        public SemiInvulnerableResult CheckSemiInvulnerable(BattleSlot target, MoveData move)
        {
            var result = new SemiInvulnerableResult { MissedDueToSemiInvulnerable = false };

            // Skip semi-invulnerable check if target is fainted (move still executes)
            if (!target.IsActive())
                return result;

            if (!target.HasVolatileStatus(VolatileStatus.SemiInvulnerable))
                return result;

            // Check if move can hit semi-invulnerable target
            bool canHit = CanHitSemiInvulnerable(target.SemiInvulnerableMoveName, move);
            if (!canHit)
            {
                result.MissedDueToSemiInvulnerable = true;
                result.MissMessage = _messageFormatter.Format(LocalizationKey.BattleMissed);
            }

            return result;
        }

        /// <summary>
        /// Verifica si un movimiento puede golpear a un objetivo semi-invulnerable.
        /// </summary>
        /// <param name="semiInvulnerableMoveName">El nombre del movimiento semi-invulnerable que está usando el objetivo.</param>
        /// <param name="move">El movimiento que se intenta usar. No puede ser null.</param>
        /// <returns>True si el movimiento puede golpear, false en caso contrario.</returns>
        public bool CanHitSemiInvulnerable(string semiInvulnerableMoveName, MoveData move)
        {
            if (string.IsNullOrEmpty(semiInvulnerableMoveName))
                return false;

            // Always-hit moves can hit semi-invulnerable targets
            if (move.NeverMisses)
                return true;

            // Check specific move combinations
            var moveNameLower = semiInvulnerableMoveName.ToLower();

            if (moveNameLower == "dig")
            {
                return MoveConstants.DigCounterMoveNames.Contains(move.Name) ||
                       move.NeverMisses;
            }

            if (moveNameLower == "dive")
            {
                return MoveConstants.DiveCounterMoveNames.Contains(move.Name) ||
                       move.NeverMisses;
            }

            if (moveNameLower == "fly" || moveNameLower == "bounce")
            {
                // Thunder hits Fly/Bounce in rain (handled by weather perfect accuracy)
                // For now, only always-hit moves can hit
                return move.NeverMisses;
            }

            if (moveNameLower == "shadow force" || moveNameLower == "phantom force")
            {
                // Only always-hit moves can hit
                return move.NeverMisses;
            }

            return false;
        }

        /// <summary>
        /// Formatea un mensaje para la fase de carga de un movimiento semi-invulnerable.
        /// </summary>
        /// <param name="moveName">El nombre del movimiento.</param>
        /// <param name="pokemon">El Pokemon que usa el movimiento. No puede ser null.</param>
        /// <param name="move">El movimiento. No puede ser null.</param>
        /// <returns>El mensaje formateado.</returns>
        public string FormatSemiInvulnerableMessage(string moveName, PokemonInstance pokemon, MoveData move)
        {
            string moveNameLower = moveName.ToLower();
            string action;

            if (moveNameLower == "fly")
                action = "flew up high!";
            else if (moveNameLower == "dig")
                action = "dug underground!";
            else if (moveNameLower == "dive")
                action = "dove underwater!";
            else if (moveNameLower == "bounce")
                action = "bounced up!";
            else if (moveNameLower == "shadow force" || moveNameLower == "phantom force")
                action = "vanished instantly!";
            else
                return _messageFormatter.FormatMoveUsed(pokemon, move);

            return $"{pokemon.DisplayName} {action}";
        }
    }
}
