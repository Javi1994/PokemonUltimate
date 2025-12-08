using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Messages.Definition;
using PokemonUltimate.Combat.Utilities.Extensions;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Localization.Constants;

namespace PokemonUltimate.Combat.Actions.Checkers
{
    /// <summary>
    /// Verificador de protección de movimientos.
    /// Valida si un movimiento puede ser bloqueado por efectos como Protect.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/MEJORAS_PROPUESTAS.md`
    /// </remarks>
    public class ProtectionChecker
    {
        private readonly IBattleMessageFormatter _messageFormatter;

        /// <summary>
        /// Crea una nueva instancia del verificador de protección.
        /// </summary>
        /// <param name="messageFormatter">El formateador de mensajes. Si es null, crea uno por defecto.</param>
        public ProtectionChecker(IBattleMessageFormatter messageFormatter = null)
        {
            _messageFormatter = messageFormatter ?? new Infrastructure.Messages.BattleMessageFormatter();
        }

        /// <summary>
        /// Resultado de la verificación de protección.
        /// </summary>
        public class ProtectionResult
        {
            /// <summary>
            /// Indica si el movimiento está protegido.
            /// </summary>
            public bool IsProtected { get; set; }

            /// <summary>
            /// Mensaje de protección si está protegido. Null si no está protegido.
            /// </summary>
            public string ProtectionMessage { get; set; }
        }

        /// <summary>
        /// Verifica si un movimiento está protegido por efectos como Protect.
        /// </summary>
        /// <param name="target">El slot objetivo. No puede ser null.</param>
        /// <param name="move">El movimiento que se intenta usar. No puede ser null.</param>
        /// <param name="canBeBlocked">Indica si el movimiento puede ser bloqueado.</param>
        /// <returns>El resultado de la verificación de protección.</returns>
        public ProtectionResult CheckProtection(BattleSlot target, MoveData move, bool canBeBlocked)
        {
            var result = new ProtectionResult { IsProtected = false };

            // Skip protection check if target is fainted (move still executes)
            if (!target.IsActive())
                return result;

            if (!target.HasVolatileStatus(VolatileStatus.Protected) || !canBeBlocked)
                return result;

            // Check if move bypasses Protect (e.g., Feint)
            if (!move.BypassesProtect)
            {
                result.IsProtected = true;
                result.ProtectionMessage = _messageFormatter.Format(LocalizationKey.BattleProtected, target.Pokemon.DisplayName);
            }

            return result;
        }
    }
}
