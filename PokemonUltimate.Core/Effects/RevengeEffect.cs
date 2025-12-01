using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// Returns damage based on damage taken that turn.
    /// Used by Counter, Mirror Coat, Metal Burst, Bide.
    /// </summary>
    public class RevengeEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.Revenge;
        public string Description => $"Returns {DamageMultiplier}x {(CountersCategory.HasValue ? CountersCategory.Value.ToString() : "any")} damage taken.";
        
        /// <summary>Move category that this counters (Physical for Counter, Special for Mirror Coat, null for both).</summary>
        public MoveCategory? CountersCategory { get; set; }
        
        /// <summary>Damage multiplier (2.0 for Counter/Mirror Coat, 1.5 for Metal Burst).</summary>
        public float DamageMultiplier { get; set; } = 2.0f;
        
        /// <summary>Whether this move has negative priority (Counter/Mirror Coat are -5).</summary>
        public int Priority { get; set; } = 0;
        
        /// <summary>Whether this only works if hit that turn.</summary>
        public bool RequiresHit { get; set; } = true;
        
        /// <summary>For Bide: accumulates damage over multiple turns.</summary>
        public int AccumulationTurns { get; set; } = 0;
        
        public RevengeEffect() { }
        
        public RevengeEffect(MoveCategory? counters, float multiplier = 2.0f)
        {
            CountersCategory = counters;
            DamageMultiplier = multiplier;
        }
    }
}

