using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Effects;

namespace PokemonUltimate.Combat.Effects
{
    /// <summary>
    /// Processes HealEffect (heals the user by percentage of max HP).
    /// </summary>
    public class HealEffectProcessor : IMoveEffectProcessor
    {
        public void Process(
            Core.Effects.IMoveEffect effect,
            BattleSlot user,
            BattleSlot target,
            Core.Blueprints.MoveData move,
            BattleField field,
            int damageDealt,
            List<BattleAction> actions)
        {
            if (effect is HealEffect healEffect)
            {
                // Heal user by percentage of max HP
                var healAmount = (int)(user.Pokemon.MaxHP * healEffect.HealPercent / 100f);
                actions.Add(new HealAction(user, user, healAmount));
            }
        }
    }
}
