using PokemonUltimate.Combat.Field;
using PokemonUltimate.Content.Catalogs.Status;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Actions.Checkers
{
    /// <summary>
    /// Verificador de aplicación de condiciones de estado.
    /// Valida si un status puede aplicarse a un Pokemon considerando todas las condiciones.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/MEJORAS_PROPUESTAS.md`
    /// </remarks>
    public class StatusApplicationChecker
    {
        /// <summary>
        /// Resultado de la verificación de aplicación de status.
        /// </summary>
        public class StatusApplicationResult
        {
            /// <summary>
            /// Indica si el status puede aplicarse.
            /// </summary>
            public bool CanApply { get; }

            /// <summary>
            /// Razón por la cual no se puede aplicar (si aplica).
            /// </summary>
            public string Reason { get; }

            private StatusApplicationResult(bool canApply, string reason = null)
            {
                CanApply = canApply;
                Reason = reason;
            }

            public static StatusApplicationResult Allowed() => new StatusApplicationResult(true);
            public static StatusApplicationResult Blocked(string reason) => new StatusApplicationResult(false, reason);
        }

        /// <summary>
        /// Verifica si un status puede aplicarse a un Pokemon.
        /// Considera: status existente, inmunidades de tipo, Safeguard, etc.
        /// </summary>
        /// <param name="target">El slot objetivo. No puede ser null.</param>
        /// <param name="status">El status que se intenta aplicar.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <returns>Resultado de la verificación con información sobre si puede aplicarse.</returns>
        public StatusApplicationResult CanApplyStatus(BattleSlot target, PersistentStatus status, BattleField field)
        {
            if (target == null)
                return StatusApplicationResult.Blocked("Target is null");
            if (field == null)
                return StatusApplicationResult.Blocked("Field is null");
            if (target.IsEmpty)
                return StatusApplicationResult.Blocked("Target is empty");

            var pokemon = target.Pokemon;

            // Check if clearing status (Status.None)
            if (status == PersistentStatus.None)
            {
                return StatusApplicationResult.Allowed();
            }

            // Check if Pokemon already has a status
            if (pokemon.Status != PersistentStatus.None)
            {
                return StatusApplicationResult.Blocked("Pokemon already has a status");
            }

            // Get status effect data to check immunities
            var statusData = StatusCatalog.GetByStatus(status);
            if (statusData != null)
            {
                // Check type immunities (e.g., Fire types immune to Burn)
                if (statusData.IsTypeImmune(pokemon.Species.PrimaryType) ||
                    (pokemon.Species.SecondaryType.HasValue && statusData.IsTypeImmune(pokemon.Species.SecondaryType.Value)))
                {
                    return StatusApplicationResult.Blocked("Pokemon is immune to this status type");
                }
            }

            // Safeguard prevents status application
            if (target.Side.HasSideCondition(SideCondition.Safeguard))
            {
                var safeguardData = target.Side.GetSideConditionData(SideCondition.Safeguard);
                if (safeguardData != null && safeguardData.PreventsStatus)
                {
                    return StatusApplicationResult.Blocked("Status blocked by Safeguard");
                }
            }

            return StatusApplicationResult.Allowed();
        }

        /// <summary>
        /// Aplica el status al Pokemon si es posible.
        /// </summary>
        /// <param name="target">El slot objetivo. No puede ser null.</param>
        /// <param name="status">El status a aplicar.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <returns>True si el status fue aplicado, false en caso contrario.</returns>
        public bool TryApplyStatus(BattleSlot target, PersistentStatus status, BattleField field)
        {
            var result = CanApplyStatus(target, status, field);
            if (result.CanApply)
            {
                target.Pokemon.Status = status;
                return true;
            }
            return false;
        }
    }
}
