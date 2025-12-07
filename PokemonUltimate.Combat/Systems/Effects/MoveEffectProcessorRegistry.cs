using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Combat.Infrastructure.Factories;
using PokemonUltimate.Combat.Infrastructure.Providers;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Effects;
using PokemonUltimate.Core.Data.Effects.Definition;

namespace PokemonUltimate.Combat.Systems.Effects
{
    /// <summary>
    /// Registry for move effect processors.
    /// Maps effect types to their corresponding processors using Strategy Pattern.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class MoveEffectProcessorRegistry
    {
        private readonly Dictionary<Type, IMoveEffectProcessor> _processors;

        /// <summary>
        /// Creates a new registry with default processors.
        /// </summary>
        /// <param name="randomProvider">The random provider for chance-based effects. Cannot be null.</param>
        /// <param name="damageContextFactory">The factory for creating damage contexts. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If randomProvider or damageContextFactory is null.</exception>
        public MoveEffectProcessorRegistry(
            IRandomProvider randomProvider,
            DamageContextFactory damageContextFactory)
        {
            if (randomProvider == null)
                throw new ArgumentNullException(nameof(randomProvider));
            if (damageContextFactory == null)
                throw new ArgumentNullException(nameof(damageContextFactory));

            _processors = new Dictionary<Type, IMoveEffectProcessor>
            {
                { typeof(StatusEffect), new StatusEffectProcessor(randomProvider) },
                { typeof(StatChangeEffect), new StatChangeEffectProcessor(randomProvider) },
                { typeof(RecoilEffect), new RecoilEffectProcessor(damageContextFactory) },
                { typeof(DrainEffect), new DrainEffectProcessor() },
                { typeof(FlinchEffect), new FlinchEffectProcessor(randomProvider) },
                { typeof(ProtectEffect), new ProtectEffectProcessor(randomProvider) },
                { typeof(CounterEffect), new CounterEffectProcessor(damageContextFactory) },
                { typeof(HealEffect), new HealEffectProcessor() }
            };
        }

        /// <summary>
        /// Processes a move effect using the appropriate processor.
        /// </summary>
        /// <param name="effect">The effect to process. Cannot be null.</param>
        /// <param name="user">The slot using the move. Cannot be null.</param>
        /// <param name="target">The target slot. Cannot be null.</param>
        /// <param name="move">The move data. Cannot be null.</param>
        /// <param name="field">The battlefield. Cannot be null.</param>
        /// <param name="damageDealt">The damage dealt by the move.</param>
        /// <param name="actions">The list of actions to add to. Cannot be null.</param>
        /// <returns>True if the effect was processed, false if no processor was found.</returns>
        public bool Process(
            IMoveEffect effect,
            BattleSlot user,
            BattleSlot target,
            MoveData move,
            BattleField field,
            int damageDealt,
            List<BattleAction> actions)
        {
            if (effect == null)
                throw new ArgumentNullException(nameof(effect));
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (move == null)
                throw new ArgumentNullException(nameof(move));
            if (field == null)
                throw new ArgumentNullException(nameof(field));
            if (actions == null)
                throw new ArgumentNullException(nameof(actions));

            var effectType = effect.GetType();
            if (_processors.TryGetValue(effectType, out var processor))
            {
                processor.Process(effect, user, target, move, field, damageDealt, actions);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Registers a custom processor for an effect type.
        /// </summary>
        /// <param name="effectType">The type of effect. Cannot be null.</param>
        /// <param name="processor">The processor for this effect type. Cannot be null.</param>
        public void Register(Type effectType, IMoveEffectProcessor processor)
        {
            if (effectType == null)
                throw new ArgumentNullException(nameof(effectType));
            if (processor == null)
                throw new ArgumentNullException(nameof(processor));

            _processors[effectType] = processor;
        }
    }
}
