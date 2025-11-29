using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// User takes a percentage of the damage dealt as recoil.
    /// Used by moves like Double-Edge (33%), Brave Bird (33%), Take Down (25%).
    /// </summary>
    public class RecoilEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.Recoil;
        public string Description => $"User takes {RecoilPercent}% of damage dealt as recoil.";
        
        /// <summary>Percentage of damage dealt that user takes (0-100).</summary>
        public int RecoilPercent { get; set; }
        
        public RecoilEffect() { }
        
        public RecoilEffect(int recoilPercent)
        {
            RecoilPercent = recoilPercent;
        }
    }
}
