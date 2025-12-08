using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Effects.Definition;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Effects;
using PokemonUltimate.Core.Data.Effects.Definition;

namespace PokemonUltimate.Combat.Effects
{
    /// <summary>
    /// Processes HealEffect (heals the user by percentage of max HP).
    /// </summary>
    public class HealEffectProcessor : IMoveEffectProcessor
    {
        public void Process(
            IMoveEffect effect,
            BattleSlot user,
            BattleSlot target,
            MoveData move,
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
