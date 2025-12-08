using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Effects;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Moves.Processors
{
    /// <summary>
    /// Procesa movimientos semi-invulnerables (Fly, Dig, Dive, etc.).
    /// Maneja turnos de carga y ataque durante la ejecución de movimientos.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// </remarks>
    public class SemiInvulnerableMoveProcessor
    {
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        /// <summary>
        /// Crea una nueva instancia del procesador de movimientos semi-invulnerables.
        /// </summary>
        public SemiInvulnerableMoveProcessor(CombatEffectHandlerRegistry handlerRegistry)
        {
            _handlerRegistry = handlerRegistry ?? throw new System.ArgumentNullException(nameof(handlerRegistry));
        }

        /// <summary>
        /// Información sobre el movimiento semi-invulnerable.
        /// </summary>
        public class SemiInvulnerableInfo
        {
            /// <summary>
            /// Indica si el movimiento tiene efecto semi-invulnerable.
            /// </summary>
            public bool HasEffect { get; set; }

            /// <summary>
            /// Indica si es el turno de ataque (turno 2).
            /// </summary>
            public bool IsAttackTurn { get; set; }
        }

        /// <summary>
        /// Analiza el estado del movimiento semi-invulnerable.
        /// </summary>
        /// <param name="user">El slot del usuario. No puede ser null.</param>
        /// <param name="move">El movimiento. No puede ser null.</param>
        /// <returns>La información del movimiento semi-invulnerable.</returns>
        public SemiInvulnerableInfo AnalyzeSemiInvulnerableMove(BattleSlot user, MoveData move)
        {
            var info = new SemiInvulnerableInfo();

            bool hasSemiInvulnerableEffect = move.Effects.Any(e => e is SemiInvulnerableEffect);
            if (!hasSemiInvulnerableEffect)
                return info;

            info.HasEffect = true;

            // Check if user is already charging this move and ready to attack
            if (user.HasVolatileStatus(VolatileStatus.SemiInvulnerable) &&
                user.SemiInvulnerableMoveName == move.Name &&
                !user.IsSemiInvulnerableCharging)
            {
                info.IsAttackTurn = true;
            }

            return info;
        }

        /// <summary>
        /// Procesa el movimiento semi-invulnerable y retorna si se debe detener el procesamiento.
        /// </summary>
        /// <param name="user">El slot del usuario. No puede ser null.</param>
        /// <param name="move">El movimiento. No puede ser null.</param>
        /// <param name="semiInvulnerableInfo">La información del movimiento semi-invulnerable.</param>
        /// <param name="actions">La lista de acciones a la que agregar efectos.</param>
        /// <returns>True si se debe detener el procesamiento (turno de carga), false si se continúa.</returns>
        public bool ProcessSemiInvulnerableMove(
            BattleSlot user,
            MoveData move,
            SemiInvulnerableInfo semiInvulnerableInfo,
            List<BattleAction> actions)
        {
            if (!semiInvulnerableInfo.HasEffect)
                return false;

            if (semiInvulnerableInfo.IsAttackTurn)
            {
                // This is turn 2 - execute attack and clear semi-invulnerable status
                user.RemoveVolatileStatus(VolatileStatus.SemiInvulnerable);
                user.ClearSemiInvulnerableMove();
                // Damage will be processed, stop processing other effects
                return true;
            }
            else
            {
                // This is turn 1 - charge turn (no damage, just message)
                user.AddVolatileStatus(VolatileStatus.SemiInvulnerable);
                user.SetSemiInvulnerableMove(move.Name, isCharging: true);
                // Message varies by move (e.g., "flew up high!" for Fly) using Semi-Invulnerable Handler
                var semiInvulnerableHandler = _handlerRegistry.GetSemiInvulnerableHandler();
                string message = semiInvulnerableHandler.FormatSemiInvulnerableMessage(move.Name, user.Pokemon, move);
                actions.Add(new MessageAction(message));
                // Charge turn handled - stop processing effects
                return true;
            }
        }
    }
}
