using System;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Utilities.Extensions;
using PokemonUltimate.Combat.View.Definition;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Domain.Instances.Move;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.Actions.Validation
{
    /// <summary>
    /// Validadores reutilizables para acciones de combate.
    /// Centraliza validaciones comunes para reducir código repetitivo.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/MEJORAS_PROPUESTAS.md`
    /// </remarks>
    public static class ActionValidators
    {
        /// <summary>
        /// Valida que el campo de batalla no sea null.
        /// </summary>
        /// <param name="field">El campo de batalla a validar.</param>
        /// <exception cref="ArgumentNullException">Si field es null.</exception>
        public static void ValidateField(BattleField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);
        }

        /// <summary>
        /// Valida que el slot objetivo no sea null ni vacío.
        /// </summary>
        /// <param name="target">El slot objetivo a validar.</param>
        /// <returns>True si el slot es válido y no está vacío, false si está vacío.</returns>
        /// <exception cref="ArgumentNullException">Si target es null.</exception>
        public static bool ValidateTarget(BattleSlot target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), ErrorMessages.PokemonCannotBeNull);

            return !target.IsEmpty;
        }

        /// <summary>
        /// Valida que el slot objetivo esté activo (no vacío y no desmayado).
        /// </summary>
        /// <param name="target">El slot objetivo a validar.</param>
        /// <returns>True si el slot está activo, false en caso contrario.</returns>
        /// <exception cref="ArgumentNullException">Si target es null.</exception>
        public static bool ValidateActiveTarget(BattleSlot target)
        {
            if (!ValidateTarget(target))
                return false;

            return target.IsActive();
        }

        /// <summary>
        /// Valida que la vista no sea null.
        /// </summary>
        /// <param name="view">La vista a validar.</param>
        /// <exception cref="ArgumentNullException">Si view es null.</exception>
        public static void ValidateView(IBattleView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
        }

        /// <summary>
        /// Valida múltiples condiciones y retorna si la acción debe ejecutarse.
        /// Valida el campo y el objetivo en una sola llamada.
        /// </summary>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <param name="target">El slot objetivo. No puede ser null.</param>
        /// <param name="checkActive">Si es true, también valida que el objetivo esté activo.</param>
        /// <returns>True si la acción debe ejecutarse, false en caso contrario.</returns>
        /// <exception cref="ArgumentNullException">Si field o target es null.</exception>
        public static bool ShouldExecute(BattleField field, BattleSlot target, bool checkActive = false)
        {
            ValidateField(field);

            if (checkActive)
                return ValidateActiveTarget(target);

            return ValidateTarget(target);
        }

        /// <summary>
        /// Valida que el usuario esté activo para ejecutar una acción.
        /// </summary>
        /// <param name="user">El slot del usuario a validar.</param>
        /// <returns>True si el usuario está activo, false en caso contrario.</returns>
        /// <exception cref="ArgumentNullException">Si user es null.</exception>
        public static bool ValidateUserActive(BattleSlot user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), ErrorMessages.PokemonCannotBeNull);

            return user.IsActive();
        }

        /// <summary>
        /// Valida que tanto el usuario como el objetivo estén activos y válidos para ejecutar una acción visual.
        /// </summary>
        /// <param name="user">El slot del usuario a validar.</param>
        /// <param name="target">El slot objetivo a validar.</param>
        /// <returns>True si ambos están activos y válidos, false en caso contrario.</returns>
        /// <exception cref="ArgumentNullException">Si user o target es null.</exception>
        public static bool ValidateUserAndTargetForVisual(BattleSlot user, BattleSlot target)
        {
            if (!ValidateUserActive(user))
                return false;

            if (!ValidateTarget(user))
                return false;

            if (!ValidateTarget(target))
                return false;

            return true;
        }

        #region Constructor Validation Methods

        /// <summary>
        /// Valida que el usuario no sea null. Para uso en constructores.
        /// </summary>
        /// <param name="user">El slot del usuario a validar.</param>
        /// <param name="paramName">El nombre del parámetro para el mensaje de error.</param>
        /// <exception cref="ArgumentNullException">Si user es null.</exception>
        public static void ValidateUser(BattleSlot user, string paramName = "user")
        {
            if (user == null)
                throw new ArgumentNullException(paramName, ErrorMessages.PokemonCannotBeNull);
        }

        /// <summary>
        /// Valida que el objetivo no sea null. Para uso en constructores.
        /// </summary>
        /// <param name="target">El slot objetivo a validar.</param>
        /// <param name="paramName">El nombre del parámetro para el mensaje de error.</param>
        /// <exception cref="ArgumentNullException">Si target es null.</exception>
        public static void ValidateTargetNotNull(BattleSlot target, string paramName = "target")
        {
            if (target == null)
                throw new ArgumentNullException(paramName, ErrorMessages.PokemonCannotBeNull);
        }

        /// <summary>
        /// Valida que la instancia de movimiento no sea null. Para uso en constructores.
        /// </summary>
        /// <param name="moveInstance">La instancia de movimiento a validar.</param>
        /// <param name="paramName">El nombre del parámetro para el mensaje de error.</param>
        /// <exception cref="ArgumentNullException">Si moveInstance es null.</exception>
        public static void ValidateMoveInstance(MoveInstance moveInstance, string paramName = "moveInstance")
        {
            if (moveInstance == null)
                throw new ArgumentNullException(paramName, ErrorMessages.MoveCannotBeNull);
        }

        /// <summary>
        /// Valida que el mensaje no sea null. Para uso en constructores.
        /// </summary>
        /// <param name="message">El mensaje a validar.</param>
        /// <param name="paramName">El nombre del parámetro para el mensaje de error.</param>
        /// <exception cref="ArgumentNullException">Si message es null.</exception>
        public static void ValidateMessage(string message, string paramName = "message")
        {
            if (message == null)
                throw new ArgumentNullException(paramName);
        }

        /// <summary>
        /// Valida que el contexto de daño no sea null. Para uso en constructores.
        /// </summary>
        /// <param name="context">El contexto de daño a validar.</param>
        /// <param name="paramName">El nombre del parámetro para el mensaje de error.</param>
        /// <exception cref="ArgumentNullException">Si context es null.</exception>
        public static void ValidateDamageContext(DamageContext context, string paramName = "context")
        {
            if (context == null)
                throw new ArgumentNullException(paramName, ErrorMessages.ContextCannotBeNull);
        }

        /// <summary>
        /// Valida que el lado de batalla no sea null. Para uso en constructores.
        /// </summary>
        /// <param name="side">El lado de batalla a validar.</param>
        /// <param name="paramName">El nombre del parámetro para el mensaje de error.</param>
        /// <exception cref="ArgumentNullException">Si side es null.</exception>
        public static void ValidateBattleSide(BattleSide side, string paramName = "targetSide")
        {
            if (side == null)
                throw new ArgumentNullException(paramName);
        }

        /// <summary>
        /// Valida que la instancia de Pokemon no sea null. Para uso en constructores.
        /// </summary>
        /// <param name="pokemon">La instancia de Pokemon a validar.</param>
        /// <param name="paramName">El nombre del parámetro para el mensaje de error.</param>
        /// <exception cref="ArgumentNullException">Si pokemon es null.</exception>
        public static void ValidatePokemonInstance(PokemonInstance pokemon, string paramName = "pokemon")
        {
            if (pokemon == null)
                throw new ArgumentNullException(paramName, ErrorMessages.PokemonCannotBeNull);
        }

        /// <summary>
        /// Valida que el slot no sea null. Para uso en constructores.
        /// </summary>
        /// <param name="slot">El slot a validar.</param>
        /// <param name="paramName">El nombre del parámetro para el mensaje de error.</param>
        /// <exception cref="ArgumentNullException">Si slot es null.</exception>
        public static void ValidateSlot(BattleSlot slot, string paramName = "slot")
        {
            if (slot == null)
                throw new ArgumentNullException(paramName, ErrorMessages.SlotCannotBeNull);
        }

        #endregion
    }
}
