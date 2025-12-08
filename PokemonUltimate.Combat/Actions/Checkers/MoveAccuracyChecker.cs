using PokemonUltimate.Combat.Actions.Registry;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Messages.Definition;
using PokemonUltimate.Combat.Utilities;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Localization.Constants;

namespace PokemonUltimate.Combat.Actions.Checkers
{
    /// <summary>
    /// Verificador de precisión de movimientos.
    /// Maneja la verificación de precisión y el cleanup de estados cuando un movimiento falla.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/MEJORAS_PROPUESTAS.md`
    /// </remarks>
    public class MoveAccuracyChecker
    {
        private readonly AccuracyChecker _accuracyChecker;
        private readonly BehaviorCheckerRegistry _behaviorRegistry;
        private readonly IBattleMessageFormatter _messageFormatter;

        /// <summary>
        /// Crea una nueva instancia del verificador de precisión.
        /// </summary>
        /// <param name="accuracyChecker">El verificador de precisión. No puede ser null.</param>
        /// <param name="behaviorRegistry">El registry de checkers. No puede ser null.</param>
        /// <param name="messageFormatter">El formateador de mensajes. Si es null, crea uno por defecto.</param>
        public MoveAccuracyChecker(
            AccuracyChecker accuracyChecker,
            BehaviorCheckerRegistry behaviorRegistry,
            IBattleMessageFormatter messageFormatter = null)
        {
            _accuracyChecker = accuracyChecker ?? throw new System.ArgumentNullException(nameof(accuracyChecker));
            _behaviorRegistry = behaviorRegistry ?? throw new System.ArgumentNullException(nameof(behaviorRegistry));
            _messageFormatter = messageFormatter ?? new Infrastructure.Messages.BattleMessageFormatter();
        }

        /// <summary>
        /// Resultado de la verificación de precisión.
        /// </summary>
        public class AccuracyResult
        {
            /// <summary>
            /// Indica si el movimiento acertó.
            /// </summary>
            public bool Hit { get; set; }

            /// <summary>
            /// Mensaje de fallo si el movimiento falló. Null si acertó.
            /// </summary>
            public string MissMessage { get; set; }
        }

        /// <summary>
        /// Verifica si un movimiento acerta.
        /// </summary>
        /// <param name="user">El slot del usuario. No puede ser null.</param>
        /// <param name="target">El slot objetivo. No puede ser null.</param>
        /// <param name="move">El movimiento. No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <returns>El resultado de la verificación de precisión.</returns>
        public AccuracyResult CheckAccuracy(BattleSlot user, BattleSlot target, MoveData move, BattleField field)
        {
            var result = new AccuracyResult { Hit = true };

            if (_accuracyChecker.CheckHit(user, target, move, field))
                return result;

            result.Hit = false;
            result.MissMessage = _messageFormatter.Format(LocalizationKey.BattleMissed);
            return result;
        }

        /// <summary>
        /// Limpia los estados volátiles cuando un movimiento falla (falla, bloqueado, etc.).
        /// </summary>
        /// <param name="user">El slot del usuario. No puede ser null.</param>
        /// <param name="hasFocusPunchEffect">Indica si el movimiento tiene efecto Focus Punch.</param>
        /// <param name="hasMultiTurnEffect">Indica si el movimiento tiene efecto Multi-Turn.</param>
        public void CleanupOnFailure(BattleSlot user, bool hasFocusPunchEffect, bool hasMultiTurnEffect)
        {
            var focusPunchChecker = _behaviorRegistry.GetFocusPunchChecker();
            var multiTurnChecker = _behaviorRegistry.GetMultiTurnChecker();

            // Remove focusing status if move failed (using Behavior Checker)
            if (hasFocusPunchEffect)
            {
                focusPunchChecker.CleanupOnFailure(user);
            }

            // Clear charging if move failed (using Behavior Checker)
            if (hasMultiTurnEffect)
            {
                multiTurnChecker.CleanupOnFailure(user);
            }
        }
    }
}
