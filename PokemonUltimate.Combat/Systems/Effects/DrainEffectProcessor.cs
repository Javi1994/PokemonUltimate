using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Combat.Systems.Effects.Definition;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Effects;
using PokemonUltimate.Core.Data.Effects.Definition;

namespace PokemonUltimate.Combat.Systems.Effects
{
    /// <summary>
    /// Processes DrainEffect (heals user by percentage of damage dealt).
    /// </summary>
    public class DrainEffectProcessor : IMoveEffectProcessor
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
            if (effect is DrainEffect drainEffect)
            {
                // Heal user by percentage of damage dealt (if damage was dealt)
                // Drain always heals at least 1 HP if damage was dealt
                if (damageDealt > 0)
                {
                    int drainHealAmount = (int)(damageDealt * drainEffect.DrainPercent / 100f);
                    drainHealAmount = System.Math.Max(1, drainHealAmount); // At least 1 HP

                    actions.Add(new HealAction(user, user, drainHealAmount));
                }
            }
        }
    }
}
