using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// Heals the user by a percentage of their max HP.
    /// Used by moves like Recover (50%), Roost (50%), Moonlight (varies by weather).
    /// </summary>
    public class HealEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.Heal;
        public string Description => $"Heals user for {HealPercent}% of max HP.";
        
        /// <summary>Percentage of max HP to heal (0-100).</summary>
        public int HealPercent { get; set; }
        
        public HealEffect() { }
        
        public HealEffect(int healPercent)
        {
            HealPercent = healPercent;
        }
    }
}
