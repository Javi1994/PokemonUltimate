using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// Applies a volatile status condition to the target.
    /// Volatile statuses are cleared on switch-out.
    /// Used by moves like Confuse Ray (Confusion), Attract (Infatuation), etc.
    /// </summary>
    public class VolatileStatusEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.VolatileStatus;
        public string Description => $"{ChancePercent}% chance to apply {Status}.";
        
        /// <summary>The volatile status to apply.</summary>
        public VolatileStatus Status { get; set; }
        
        /// <summary>Chance to apply (0-100, where 100 = guaranteed).</summary>
        public int ChancePercent { get; set; } = 100;
        
        /// <summary>If true, applies to the user instead of target.</summary>
        public bool TargetSelf { get; set; } = false;
        
        /// <summary>Duration in turns (0 = until cured/switched).</summary>
        public int Duration { get; set; } = 0;
        
        public VolatileStatusEffect() { }
        
        public VolatileStatusEffect(VolatileStatus status, int chancePercent = 100)
        {
            Status = status;
            ChancePercent = chancePercent;
        }
    }
}

