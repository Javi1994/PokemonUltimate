using PokemonUltimate.Core.Data.Effects.Definition;
using PokemonUltimate.Core.Data.Enums;


namespace PokemonUltimate.Core.Data.Effects
{
    /// <summary>
    /// Applies a volatile status condition to the target.
    /// Volatile statuses are cleared on switch-out.
    /// Used by moves like Confuse Ray (Confusion), Attract (Infatuation), etc.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.2: Move Data
    /// **Documentation**: See `docs/features/1-game-data/1.2-move-data/architecture.md`
    /// </remarks>
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

