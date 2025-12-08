using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage.Processors;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Combat.Moves.MoveModifier;
using PokemonUltimate.Combat.Moves.Processors;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Effects;

namespace PokemonUltimate.Combat.Moves.Orchestrator
{
    /// <summary>
    /// Orquestador de efectos de movimientos.
    /// Coordina el procesamiento de todos los efectos del movimiento delegando a procesadores especializados.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// </remarks>
    public class MoveEffectOrchestrator
    {
        private readonly CombatEffectHandlerRegistry _handlerRegistry;
        private readonly MoveDamageProcessor _damageProcessor;
        private readonly SemiInvulnerableMoveProcessor _semiInvulnerableProcessor;
        private readonly MoveModificationApplier _modificationApplier;

        /// <summary>
        /// Crea una nueva instancia del orquestador de efectos.
        /// </summary>
        /// <param name="handlerRegistry">El registry unificado de handlers. No puede ser null.</param>
        /// <param name="damageProcessor">El procesador de daño. No puede ser null.</param>
        /// <param name="semiInvulnerableProcessor">El procesador de movimientos semi-invulnerables. No puede ser null.</param>
        /// <param name="modificationApplier">El aplicador de modificaciones. No puede ser null.</param>
        public MoveEffectOrchestrator(
            CombatEffectHandlerRegistry handlerRegistry,
            MoveDamageProcessor damageProcessor,
            SemiInvulnerableMoveProcessor semiInvulnerableProcessor,
            MoveModificationApplier modificationApplier)
        {
            _handlerRegistry = handlerRegistry ?? throw new System.ArgumentNullException(nameof(handlerRegistry));
            _damageProcessor = damageProcessor ?? throw new System.ArgumentNullException(nameof(damageProcessor));
            _semiInvulnerableProcessor = semiInvulnerableProcessor ?? throw new System.ArgumentNullException(nameof(semiInvulnerableProcessor));
            _modificationApplier = modificationApplier ?? throw new System.ArgumentNullException(nameof(modificationApplier));
        }

        /// <summary>
        /// Analiza el estado del movimiento semi-invulnerable.
        /// </summary>
        /// <param name="user">El slot del usuario. No puede ser null.</param>
        /// <param name="move">El movimiento. No puede ser null.</param>
        /// <returns>La información del movimiento semi-invulnerable.</returns>
        public SemiInvulnerableMoveProcessor.SemiInvulnerableInfo AnalyzeSemiInvulnerableMove(BattleSlot user, MoveData move)
        {
            return _semiInvulnerableProcessor.AnalyzeSemiInvulnerableMove(user, move);
        }

        /// <summary>
        /// Resultado del procesamiento de todos los efectos del movimiento.
        /// </summary>
        public class EffectProcessingResult
        {
            /// <summary>
            /// El daño total infligido.
            /// </summary>
            public int TotalDamage { get; set; }

            /// <summary>
            /// Indica si se debe detener el procesamiento (por ejemplo, en turno de carga semi-invulnerable).
            /// </summary>
            public bool ShouldStop { get; set; }
        }

        /// <summary>
        /// Procesa todos los efectos del movimiento, incluyendo daño.
        /// </summary>
        /// <param name="user">El slot del usuario. No puede ser null.</param>
        /// <param name="target">El slot objetivo. No puede ser null.</param>
        /// <param name="move">El movimiento. No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <param name="actions">La lista de acciones a la que agregar efectos. No puede ser null.</param>
        /// <returns>El resultado del procesamiento de efectos.</returns>
        public EffectProcessingResult ProcessAllEffects(
            BattleSlot user,
            BattleSlot target,
            MoveData move,
            BattleField field,
            List<BattleAction> actions)
        {
            var result = new EffectProcessingResult();

            // Analyze semi-invulnerable move
            var semiInvulnerableInfo = _semiInvulnerableProcessor.AnalyzeSemiInvulnerableMove(user, move);

            int damageDealt = 0;

            // Process SemiInvulnerable move first (special handling)
            if (semiInvulnerableInfo.HasEffect)
            {
                bool shouldStop = _semiInvulnerableProcessor.ProcessSemiInvulnerableMove(
                    user, move, semiInvulnerableInfo, actions);

                if (shouldStop)
                {
                    // If it's charge turn, no damage is dealt
                    if (!semiInvulnerableInfo.IsAttackTurn)
                    {
                        result.TotalDamage = 0;
                    }
                    else
                    {
                        // Attack turn - process damage
                        damageDealt = _damageProcessor.ProcessDamage(user, target, move, field, actions);
                        result.TotalDamage = damageDealt;
                    }

                    result.ShouldStop = true;
                    return result;
                }
            }

            // Process damage (if not already processed)
            if (damageDealt == 0)
            {
                damageDealt = _damageProcessor.ProcessDamage(user, target, move, field, actions);
            }
            result.TotalDamage = damageDealt;

            // Process all other effects using unified handler registry
            foreach (var effect in move.Effects)
            {
                // Skip DamageEffect (already processed)
                if (effect is DamageEffect)
                    continue;

                // Skip SemiInvulnerableEffect (already processed above)
                if (effect is SemiInvulnerableEffect)
                    continue;

                // Process effect using unified handler registry
                var effectActions = _handlerRegistry.ProcessMoveEffect(effect, user, target, move, field, damageDealt);
                actions.AddRange(effectActions);
            }

            return result;
        }
    }
}
