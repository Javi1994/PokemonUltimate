using PokemonUltimate.Core.Data.Effects.Definition;
using PokemonUltimate.Core.Data.Enums;


namespace PokemonUltimate.Core.Data.Effects
{
    /// <summary>
    /// Causes the target to flinch (skip their action) if they haven't acted yet.
    /// Used by moves like Fake Out (100%), Air Slash (30%), Iron Head (30%).
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.2: Move Data
    /// **Documentation**: See `docs/features/1-game-data/1.2-move-data/architecture.md`
    /// </remarks>
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
