using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Effects;

namespace PokemonUltimate.Combat.Effects
{
    /// <summary>
    /// Processes DrainEffect (heals user by percentage of damage dealt).
    /// </summary>
    public class DrainEffectProcessor : IMoveEffectProcessor
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
