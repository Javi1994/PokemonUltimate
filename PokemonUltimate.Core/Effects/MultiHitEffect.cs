using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// Hits multiple times in a single turn.
    /// Used by moves like Fury Attack (2-5), Double Kick (2), Bullet Seed (2-5).
    /// </summary>
    public class MultiHitEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.MultiHit;
        public string Description => MinHits == MaxHits 
            ? $"Hits {MinHits} times." 
            : $"Hits {MinHits}-{MaxHits} times.";
        
        /// <summary>Minimum number of hits.</summary>
        public int MinHits { get; set; }
        
        /// <summary>Maximum number of hits.</summary>
        public int MaxHits { get; set; }
        
        public MultiHitEffect() { }
        
        public MultiHitEffect(int minHits, int maxHits)
        {
            MinHits = minHits;
            MaxHits = maxHits;
        }
        
        /// <summary>Convenience constructor for fixed hit count.</summary>
        public MultiHitEffect(int hits) : this(hits, hits) { }
    }
}
