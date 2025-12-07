using PokemonUltimate.Core.Data.Effects.Definition;
using PokemonUltimate.Core.Data.Enums;


namespace PokemonUltimate.Core.Data.Effects
{
    /// <summary>
    /// Move hits multiple times (2-5 hits).
    /// Each hit has independent accuracy and critical hit chance.
    /// Used by Double Slap, Fury Attack, Bullet Seed, etc.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.15: Advanced Move Mechanics
    /// **Documentation**: See `docs/features/2-combat-system/2.15-advanced-move-mechanics/README.md`
    /// </remarks>
    public class MultiHitEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.MultiHit;
        public string Description => "Move hits multiple times (2-5 hits).";

        /// <summary>
        /// Minimum number of hits (usually 2).
        /// </summary>
        public int MinHits { get; set; } = 2;

        /// <summary>
        /// Maximum number of hits (usually 5).
        /// </summary>
        public int MaxHits { get; set; } = 5;

        /// <summary>
        /// Creates a new MultiHitEffect with default values (2-5 hits).
        /// </summary>
        public MultiHitEffect()
        {
        }

        /// <summary>
        /// Creates a new MultiHitEffect with specified min and max hits.
        /// </summary>
        public MultiHitEffect(int minHits, int maxHits)
        {
            MinHits = minHits;
            MaxHits = maxHits;
        }

        /// <summary>
        /// Creates a new MultiHitEffect with specified fixed number of hits (min = max).
        /// </summary>
        public MultiHitEffect(int fixedHits)
        {
            MinHits = fixedHits;
            MaxHits = fixedHits;
        }
    }
}
