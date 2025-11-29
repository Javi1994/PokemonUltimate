using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// Deals a fixed amount of damage, ignoring stats and type effectiveness.
    /// Used by moves like Dragon Rage (40), Sonic Boom (20), Seismic Toss (level-based).
    /// </summary>
    public class FixedDamageEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.FixedDamage;
        public string Description => $"Deals exactly {Amount} damage.";
        
        /// <summary>The exact amount of damage to deal.</summary>
        public int Amount { get; set; }
        
        /// <summary>If true, damage equals user's level (Seismic Toss, Night Shade).</summary>
        public bool UseLevelAsDamage { get; set; } = false;
        
        public FixedDamageEffect() { }
        
        public FixedDamageEffect(int amount)
        {
            Amount = amount;
        }
    }
}
