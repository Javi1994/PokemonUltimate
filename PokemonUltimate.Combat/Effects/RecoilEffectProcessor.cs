using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Effects;

namespace PokemonUltimate.Combat.Effects
{
    /// <summary>
    /// Processes RecoilEffect (applies recoil damage to user).
    /// </summary>
    public class RecoilEffectProcessor : IMoveEffectProcessor
    {
        private readonly DamageContextFactory _damageContextFactory;

        /// <summary>
        /// Creates a new RecoilEffectProcessor.
        /// </summary>
        /// <param name="damageContextFactory">The factory for creating damage contexts. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If damageContextFactory is null.</exception>
        public RecoilEffectProcessor(DamageContextFactory damageContextFactory)
        {
            _damageContextFactory = damageContextFactory ?? throw new ArgumentNullException(nameof(damageContextFactory));
        }

        public void Process(
            Core.Effects.IMoveEffect effect,
            BattleSlot user,
            BattleSlot target,
            Core.Blueprints.MoveData move,
            BattleField field,
            int damageDealt,
            List<BattleAction> actions)
        {
            if (effect is RecoilEffect recoilEffect)
            {
                // Apply recoil damage to user (if damage was dealt)
                // Recoil always deals at least 1 HP if damage was dealt
                if (damageDealt > 0)
                {
                    int recoilDamage = (int)(damageDealt * recoilEffect.RecoilPercent / 100f);
                    recoilDamage = System.Math.Max(1, recoilDamage); // At least 1 HP

                    // Create a simple damage context for recoil
                    var recoilContext = _damageContextFactory.CreateForRecoil(user, recoilDamage, move, field);
                    actions.Add(new DamageAction(user, user, recoilContext));
                }
            }
        }
    }
}
