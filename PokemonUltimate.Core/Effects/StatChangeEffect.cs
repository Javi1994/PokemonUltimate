using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// Modifies a stat by a number of stages (-6 to +6).
    /// Used by moves like Swords Dance (+2 Atk), Growl (-1 Atk), etc.
    /// </summary>
    public class StatChangeEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.StatChange;
        public string Description => $"{(Stages > 0 ? "+" : "")}{Stages} {TargetStat} to {(TargetSelf ? "user" : "target")}.";
        
        /// <summary>Which stat to modify.</summary>
        public Stat TargetStat { get; set; }
        
        /// <summary>Number of stages to change (-6 to +6).</summary>
        public int Stages { get; set; }
        
        /// <summary>Chance to apply (0-100, where 100 = guaranteed).</summary>
        public int ChancePercent { get; set; } = 100;
        
        /// <summary>If true, applies to the user; if false, applies to target.</summary>
        public bool TargetSelf { get; set; } = false;
        
        public StatChangeEffect() { }
        
        public StatChangeEffect(Stat stat, int stages, bool targetSelf = false, int chancePercent = 100)
        {
            TargetStat = stat;
            Stages = stages;
            TargetSelf = targetSelf;
            ChancePercent = chancePercent;
        }
    }
}
