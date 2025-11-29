using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// Causes the target to flinch (skip their action) if they haven't acted yet.
    /// Used by moves like Fake Out (100%), Air Slash (30%), Iron Head (30%).
    /// </summary>
    public class FlinchEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.Flinch;
        public string Description => $"{ChancePercent}% chance to cause flinch.";
        
        /// <summary>Chance to cause flinch (0-100).</summary>
        public int ChancePercent { get; set; }
        
        public FlinchEffect() { }
        
        public FlinchEffect(int chancePercent)
        {
            ChancePercent = chancePercent;
        }
    }
}
