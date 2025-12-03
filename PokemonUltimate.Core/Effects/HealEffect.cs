using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// Heals the user by a percentage of their max HP.
    /// Used by moves like Recover (50%), Roost (50%), Moonlight (varies by weather).
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.2: Move Data
    /// **Documentation**: See `docs/features/1-game-data/1.2-move-data/architecture.md`
    /// </remarks>
    public class HealEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.Heal;
        public string Description => $"Heals user for {HealPercent}% of max HP.";
        
        /// <summary>Percentage of max HP to heal (0-100).</summary>
        public int HealPercent { get; set; }
        
        /// <summary>Alias for HealPercent for API consistency.</summary>
        public int Percent
        {
            get => HealPercent;
            set => HealPercent = value;
        }
        
        public HealEffect() { }
        
        public HealEffect(int healPercent)
        {
            HealPercent = healPercent;
        }
    }
}
