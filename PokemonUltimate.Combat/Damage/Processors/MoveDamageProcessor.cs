using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Actions.Registry;
using PokemonUltimate.Combat.Damage.Definition;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Messages.Definition;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Combat.Moves.MoveModifier;
using PokemonUltimate.Combat.Utilities.Definition;
using PokemonUltimate.Combat.Utilities.Extensions;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Effects;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Damage.Processors
{
    /// <summary>
    /// Procesa el daño de movimientos, incluyendo multi-hit, spread moves y modificaciones temporales.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// </remarks>
    public class MoveDamageProcessor
    {
        private readonly IDamagePipeline _damagePipeline;
        private readonly IRandomProvider _randomProvider;
        private readonly ITargetResolver _targetResolver;
        private readonly BehaviorCheckerRegistry _behaviorRegistry;
        private readonly IBattleMessageFormatter _messageFormatter;
        private readonly MoveModificationApplier _modificationApplier;

        /// <summary>
        /// Crea una nueva instancia del procesador de daño de movimientos.
        /// </summary>
        public MoveDamageProcessor(
            IDamagePipeline damagePipeline,
            IRandomProvider randomProvider,
            ITargetResolver targetResolver,
            BehaviorCheckerRegistry behaviorRegistry,
            IBattleMessageFormatter messageFormatter,
            MoveModificationApplier modificationApplier)
        {
            _damagePipeline = damagePipeline ?? throw new System.ArgumentNullException(nameof(damagePipeline));
            _randomProvider = randomProvider ?? throw new System.ArgumentNullException(nameof(randomProvider));
            _targetResolver = targetResolver ?? throw new System.ArgumentNullException(nameof(targetResolver));
            _behaviorRegistry = behaviorRegistry ?? throw new System.ArgumentNullException(nameof(behaviorRegistry));
            _messageFormatter = messageFormatter ?? throw new System.ArgumentNullException(nameof(messageFormatter));
            _modificationApplier = modificationApplier ?? throw new System.ArgumentNullException(nameof(modificationApplier));
        }

        /// <summary>
        /// Procesa el daño de un movimiento considerando todos los efectos especiales.
        /// </summary>
        /// <param name="user">El slot del usuario. No puede ser null.</param>
        /// <param name="target">El slot objetivo. No puede ser null.</param>
        /// <param name="move">El movimiento. No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <param name="actions">La lista de acciones a la que agregar acciones de daño. No puede ser null.</param>
        /// <returns>El daño total infligido.</returns>
        public int ProcessDamage(
            BattleSlot user,
            BattleSlot target,
            MoveData move,
            BattleField field,
            List<BattleAction> actions)
        {
            // Check if there are any valid active targets
            var potentialTargets = _targetResolver.GetValidTargets(user, move, field);
            bool hasActiveTargets = target.IsActive() || potentialTargets.Any(t => t.IsActive());
            if (!hasActiveTargets)
                return 0;

            // Apply temporary modifications (Pursuit, Helping Hand, etc.)
            MoveData moveForDamage = _modificationApplier.ApplyModifications(user, target, move, field, actions);

            // Find DamageEffect or check if move should deal damage based on category
            var damageEffect = move.Effects.OfType<DamageEffect>().FirstOrDefault();

            // If no explicit DamageEffect, check if move should deal damage based on category and power
            // Moves with Physical/Special category and Power > 0 should deal damage even without explicit DamageEffect
            bool shouldDealDamage = damageEffect != null ||
                                   (move.Category != MoveCategory.Status && move.Power > 0);

            if (!shouldDealDamage)
                return 0;

            // Check for Multi-Hit effect
            bool hasMultiHitEffect = move.Effects.Any(e => e is MultiHitEffect);
            MultiHitEffect multiHitEffect = null;
            if (hasMultiHitEffect)
            {
                multiHitEffect = move.Effects.OfType<MultiHitEffect>().First();
            }

            if (hasMultiHitEffect && multiHitEffect != null)
            {
                // Multi-hit move: hit multiple times
                return ProcessMultiHitDamage(user, target, moveForDamage, field, multiHitEffect, actions);
            }
            else
            {
                // Single-target or spread move
                return ProcessSingleOrSpreadDamage(user, target, moveForDamage, field, actions);
            }
        }

        /// <summary>
        /// Procesa el daño de un movimiento multi-hit.
        /// </summary>
        private int ProcessMultiHitDamage(
            BattleSlot user,
            BattleSlot target,
            MoveData move,
            BattleField field,
            MultiHitEffect multiHitEffect,
            List<BattleAction> actions)
        {
            int numHits = _randomProvider.Next(multiHitEffect.MinHits, multiHitEffect.MaxHits + 1);
            int totalDamage = 0;

            for (int i = 0; i < numHits; i++)
            {
                // Each hit has independent accuracy and damage calculation
                var context = _damagePipeline.Calculate(user, target, move, field);

                if (context.FinalDamage > 0)
                {
                    var damageAction = new DamageAction(user, target, context);
                    actions.Add(damageAction);
                    totalDamage += context.FinalDamage; // Accumulate total damage
                }
            }

            return totalDamage;
        }

        /// <summary>
        /// Procesa el daño de un movimiento de un solo objetivo o de dispersión.
        /// </summary>
        private int ProcessSingleOrSpreadDamage(
            BattleSlot user,
            BattleSlot target,
            MoveData move,
            BattleField field,
            List<BattleAction> actions)
        {
            // Check if this is a spread move (hits multiple targets) using Move State Checker
            var validTargets = _targetResolver.GetValidTargets(user, move, field);
            var moveStateChecker = _behaviorRegistry.GetMoveStateChecker();
            bool isSpreadMove = moveStateChecker.IsSpreadMove(move, field, validTargets.Count);

            if (isSpreadMove && validTargets.Count > 1)
            {
                // Spread move: hit all valid targets with 75% damage in doubles/triples
                bool isMultiTargetFormat = field.Rules.PlayerSlots > 1 || field.Rules.EnemySlots > 1;
                float spreadMultiplier = isMultiTargetFormat ? 0.75f : 1.0f;

                int totalDamage = 0;
                foreach (var spreadTarget in validTargets)
                {
                    if (!spreadTarget.IsActive())
                        continue;

                    var context = _damagePipeline.Calculate(user, spreadTarget, move, field);

                    // Apply spread move damage reduction
                    if (spreadMultiplier < 1.0f)
                    {
                        context.Multiplier *= spreadMultiplier;
                    }

                    if (context.FinalDamage > 0)
                    {
                        var damageAction = new DamageAction(user, spreadTarget, context);
                        actions.Add(damageAction);
                        totalDamage += context.FinalDamage;
                    }
                }
                return totalDamage;
            }
            else
            {
                // Single-target move: normal damage calculation
                var context = _damagePipeline.Calculate(user, target, move, field);

                if (context.FinalDamage > 0)
                {
                    var damageAction = new DamageAction(user, target, context);
                    actions.Add(damageAction);
                    return context.FinalDamage;
                }

                return 0;
            }
        }
    }
}
