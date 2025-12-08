using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Effects;
using PokemonUltimate.Core.Domain.Instances.Move;
using PokemonUltimate.Localization.Constants;
using PokemonUltimate.Localization.Services;

namespace PokemonUltimate.Combat.Actions.Checkers
{
    /// <summary>
    /// Verificador de comportamiento Focus Punch (focusing state y hit check).
    /// Maneja el estado de focusing y verifica si el Pokemon fue golpeado mientras enfocaba.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/MEJORAS_PROPUESTAS.md`
    /// </remarks>
    public class FocusPunchBehaviorChecker
    {
        /// <summary>
        /// Verifica si el movimiento tiene efecto Focus Punch.
        /// </summary>
        /// <param name="move">El movimiento a verificar. Puede ser null.</param>
        /// <returns>True si el movimiento tiene efecto Focus Punch, false en caso contrario.</returns>
        public bool HasFocusPunchBehavior(MoveData move)
        {
            if (move == null)
                return false;

            return move.Effects.Any(e => e is FocusPunchEffect);
        }

        /// <summary>
        /// Procesa el comportamiento Focus Punch al inicio del movimiento.
        /// Marca al usuario como focusing y verifica si fue golpeado mientras enfocaba.
        /// </summary>
        /// <param name="user">El slot del usuario del movimiento. No puede ser null.</param>
        /// <param name="actions">La lista de acciones a la que agregar mensajes. No puede ser null.</param>
        /// <param name="moveInstance">La instancia del movimiento. No puede ser null.</param>
        /// <returns>True si el movimiento debe cancelarse (focus perdido), false si debe continuar.</returns>
        /// <exception cref="System.ArgumentNullException">Si user, actions o moveInstance es null.</exception>
        public bool ProcessFocusPunchStart(BattleSlot user, List<BattleAction> actions, MoveInstance moveInstance)
        {
            if (user == null)
                throw new System.ArgumentNullException(nameof(user));
            if (actions == null)
                throw new System.ArgumentNullException(nameof(actions));
            if (moveInstance == null)
                throw new System.ArgumentNullException(nameof(moveInstance));

            // Marcar como focusing al inicio del turno
            // En una batalla real, esto ocurriría cuando se recopilan las acciones,
            // pero lo hacemos aquí por simplicidad
            user.AddVolatileStatus(Core.Data.Enums.VolatileStatus.Focusing);

            // Verificar si fue golpeado mientras enfocaba (debe verificarse después de marcar como focusing)
            if (user.WasHitWhileFocusing)
            {
                // Consumir PP aunque el movimiento falle
                moveInstance.Use();
                var provider = LocalizationService.Instance;
                actions.Add(new MessageAction(provider.GetString(LocalizationKey.MoveFocusLost, user.Pokemon.DisplayName)));
                user.RemoveVolatileStatus(Core.Data.Enums.VolatileStatus.Focusing);
                return true; // Cancelar movimiento
            }

            return false; // Continuar con el movimiento
        }

        /// <summary>
        /// Limpia el estado focusing si el movimiento falla (miss, bloqueado, etc.).
        /// </summary>
        /// <param name="user">El slot del usuario del movimiento. Puede ser null.</param>
        public void CleanupOnFailure(BattleSlot user)
        {
            if (user == null)
                return;

            if (user.HasVolatileStatus(Core.Data.Enums.VolatileStatus.Focusing))
            {
                user.RemoveVolatileStatus(Core.Data.Enums.VolatileStatus.Focusing);
            }
        }

        /// <summary>
        /// Limpia el estado focusing si el movimiento tiene éxito.
        /// </summary>
        /// <param name="user">El slot del usuario del movimiento. Puede ser null.</param>
        public void CleanupOnSuccess(BattleSlot user)
        {
            if (user == null)
                return;

            if (user.HasVolatileStatus(Core.Data.Enums.VolatileStatus.Focusing))
            {
                user.RemoveVolatileStatus(Core.Data.Enums.VolatileStatus.Focusing);
            }
        }
    }
}
