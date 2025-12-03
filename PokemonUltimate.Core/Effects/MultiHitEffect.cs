using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// Hits multiple times in a single turn.
    /// Used by moves like Fury Attack (2-5), Double Kick (2), Bullet Seed (2-5).
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.2: Move Data
    /// **Documentation**: See `docs/features/1-game-data/1.2-move-data/architecture.md`
    /// </remarks>
    public class MultiHitEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.MultiHit;
        public string Description => MinHits == MaxHits 
            ? string.Format(GameMessages.HitsExactly, MinHits)
            : string.Format(GameMessages.HitsRange, MinHits, MaxHits);
        
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
