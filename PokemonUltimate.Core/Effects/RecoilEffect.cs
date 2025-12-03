using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// User takes a percentage of the damage dealt as recoil.
    /// Used by moves like Double-Edge (33%), Brave Bird (33%), Take Down (25%).
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.2: Move Data
    /// **Documentation**: See `docs/features/1-game-data/1.2-move-data/architecture.md`
    /// </remarks>
    public class RecoilEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.Recoil;
        public string Description => $"User takes {RecoilPercent}% of damage dealt as recoil.";
        
        /// <summary>Percentage of damage dealt that user takes (0-100).</summary>
        public int RecoilPercent { get; set; }
        
        /// <summary>Alias for RecoilPercent for API consistency.</summary>
        public int Percent
        {
            get => RecoilPercent;
            set => RecoilPercent = value;
        }
        
        public RecoilEffect() { }
        
        public RecoilEffect(int recoilPercent)
        {
            RecoilPercent = recoilPercent;
        }
    }
}
