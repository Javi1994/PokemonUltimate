using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Messages.Definition;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Effects;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Moves.MoveModifier
{
    /// <summary>
    /// Aplica modificaciones temporales a movimientos (Pursuit, Helping Hand, etc.).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// </remarks>
    public class MoveModificationApplier
    {
        private readonly IBattleMessageFormatter _messageFormatter;

        /// <summary>
        /// Crea una nueva instancia del aplicador de modificaciones.
        /// </summary>
        public MoveModificationApplier(IBattleMessageFormatter messageFormatter)
        {
            _messageFormatter = messageFormatter ?? throw new System.ArgumentNullException(nameof(messageFormatter));
        }

        /// <summary>
        /// Aplica modificaciones temporales al movimiento y retorna el movimiento modificado.
        /// </summary>
        /// <param name="user">El slot del usuario. No puede ser null.</param>
        /// <param name="target">El slot objetivo. No puede ser null.</param>
        /// <param name="move">El movimiento original. No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <param name="actions">La lista de acciones a la que agregar mensajes. No puede ser null.</param>
        /// <returns>El movimiento con modificaciones aplicadas.</returns>
        public MoveData ApplyModifications(
            BattleSlot user,
            BattleSlot target,
            MoveData move,
            BattleField field,
            List<BattleAction> actions)
        {
            MoveData modifiedMove = move;

            // Check for Pursuit effect - doubles power if target is switching
            bool hasPursuitEffect = move.Effects.Any(e => e is PursuitEffect);
            if (hasPursuitEffect && target.HasVolatileStatus(VolatileStatus.SwitchingOut))
            {
                // Apply power multiplier using MoveModifier
                var pursuitModifier = MoveModifier.MultiplyPower(2.0f);
                modifiedMove = pursuitModifier.ApplyModifications(move);
                actions.Add(new MessageAction(_messageFormatter.FormatMoveUsedDuringSwitch(target.Pokemon, user.Pokemon, move)));
            }

            // Future: Add other modifications here (Helping Hand, etc.)

            return modifiedMove;
        }
    }
}
