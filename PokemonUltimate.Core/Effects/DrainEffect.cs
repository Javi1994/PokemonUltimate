using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// User heals a percentage of the damage dealt.
    /// Used by moves like Giga Drain (50%), Drain Punch (50%), Absorb (50%).
    /// </summary>
    public class DrainEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.Drain;
        public string Description => $"User heals {DrainPercent}% of damage dealt.";
        
        /// <summary>Percentage of damage dealt that user heals (0-100).</summary>
        public int DrainPercent { get; set; }
        
        /// <summary>Alias for DrainPercent for API consistency.</summary>
        public int Percent
        {
            get => DrainPercent;
            set => DrainPercent = value;
        }
        
        public DrainEffect() { }
        
        public DrainEffect(int drainPercent)
        {
            DrainPercent = drainPercent;
        }
    }
}
