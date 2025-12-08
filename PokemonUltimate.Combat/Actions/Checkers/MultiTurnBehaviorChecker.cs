using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Effects;
using PokemonUltimate.Core.Domain.Instances.Move;

namespace PokemonUltimate.Combat.Actions.Checkers
{
    /// <summary>
    /// Verificador de comportamiento Multi-Turn (carga/ataque).
    /// Maneja movimientos que requieren un turno de carga antes del ataque.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/MEJORAS_PROPUESTAS.md`
    /// </remarks>
    public class MultiTurnBehaviorChecker
    {
        /// <summary>
        /// Verifica si el movimiento tiene efecto Multi-Turn.
        /// </summary>
        /// <param name="move">El movimiento a verificar. Puede ser null.</param>
        /// <returns>True si el movimiento tiene efecto Multi-Turn, false en caso contrario.</returns>
        public bool HasMultiTurnBehavior(MoveData move)
        {
            if (move == null)
                return false;

            return move.Effects.Any(e => e is MultiTurnEffect);
        }

        /// <summary>
        /// Procesa el comportamiento Multi-Turn.
        /// Determina si es turno de carga o turno de ataque.
        /// </summary>
        /// <param name="user">El slot del usuario del movimiento. No puede ser null.</param>
        /// <param name="move">El movimiento a procesar. No puede ser null.</param>
        /// <param name="moveInstance">La instancia del movimiento. No puede ser null.</param>
        /// <param name="actions">La lista de acciones a la que agregar mensajes. No puede ser null.</param>
        /// <returns>True si es turno de carga (cancelar ejecución), false si es turno de ataque (continuar).</returns>
        /// <exception cref="System.ArgumentNullException">Si user, move, moveInstance o actions es null.</exception>
        public bool ProcessMultiTurn(BattleSlot user, MoveData move, MoveInstance moveInstance, List<BattleAction> actions)
        {
            if (user == null)
                throw new System.ArgumentNullException(nameof(user));
            if (move == null)
                throw new System.ArgumentNullException(nameof(move));
            if (moveInstance == null)
                throw new System.ArgumentNullException(nameof(moveInstance));
            if (actions == null)
                throw new System.ArgumentNullException(nameof(actions));

            // Verificar si el usuario ya está cargando este movimiento
            if (user.HasVolatileStatus(Core.Data.Enums.VolatileStatus.Charging) && user.ChargingMoveName == move.Name)
            {
                // Este es el turno de ataque - limpiar charging y continuar con ejecución normal
                user.RemoveVolatileStatus(Core.Data.Enums.VolatileStatus.Charging);
                user.ClearChargingMove();
                return false; // Continuar con ejecución
            }

            // Este es el turno de carga - marcar como charging y retornar temprano
            user.AddVolatileStatus(Core.Data.Enums.VolatileStatus.Charging);
            user.SetChargingMove(move.Name);
            moveInstance.Use(); // Consumir PP

            var multiTurnEffect = move.Effects.OfType<MultiTurnEffect>().First();
            string chargeMessage = !string.IsNullOrEmpty(multiTurnEffect.ChargeMessage)
                ? $"{user.Pokemon.DisplayName} {multiTurnEffect.ChargeMessage}"
                : $"{user.Pokemon.DisplayName} is charging!";
            actions.Add(new MessageAction(chargeMessage));

            return true; // Cancelar ejecución (turno de carga)
        }

        /// <summary>
        /// Limpia el estado charging si el movimiento falla.
        /// </summary>
        /// <param name="user">El slot del usuario del movimiento. Puede ser null.</param>
        public void CleanupOnFailure(BattleSlot user)
        {
            if (user == null)
                return;

            if (user.HasVolatileStatus(Core.Data.Enums.VolatileStatus.Charging))
            {
                user.RemoveVolatileStatus(Core.Data.Enums.VolatileStatus.Charging);
                user.ClearChargingMove();
            }
        }
    }
}
