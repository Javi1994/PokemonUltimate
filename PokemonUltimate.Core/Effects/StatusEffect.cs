using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// Applies a persistent status condition to the target.
    /// Used by moves like Thunder Wave (Paralysis), Will-O-Wisp (Burn), etc.
    /// </summary>
    public class StatusEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.Status;
        public string Description => $"{ChancePercent}% chance to apply {Status}.";
        
        /// <summary>The status condition to apply.</summary>
        public PersistentStatus Status { get; set; }
        
        /// <summary>Chance to apply (0-100, where 100 = guaranteed).</summary>
        public int ChancePercent { get; set; } = 100;
        
        /// <summary>If true, applies to the user instead of target (e.g., Rest applies Sleep to self).</summary>
        public bool TargetSelf { get; set; } = false;
        
        public StatusEffect() { }
        
        public StatusEffect(PersistentStatus status, int chancePercent = 100)
        {
            Status = status;
            ChancePercent = chancePercent;
        }
    }
}
