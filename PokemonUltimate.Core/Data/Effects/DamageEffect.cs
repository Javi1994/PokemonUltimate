using PokemonUltimate.Core.Data.Effects.Definition;
using PokemonUltimate.Core.Data.Enums;


namespace PokemonUltimate.Core.Data.Effects
{
    /// <summary>
    /// Standard damage effect using the Pokemon damage formula.
    /// Most damaging moves have this effect.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.2: Move Data
    /// **Documentation**: See `docs/features/1-game-data/1.2-move-data/architecture.md`
    /// </remarks>
    public class DamageEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.Damage;
        public string Description => "Deals damage to the target using the standard damage formula.";

        /// <summary>Multiplier applied to final damage (default 1.0 = normal damage).</summary>
        public float DamageMultiplier { get; set; } = 1.0f;

        /// <summary>Whether this move can score critical hits.</summary>
        public bool CanCrit { get; set; } = true;

        /// <summary>Additional crit stages (0 = normal, 1 = high crit, 2+ = very high).</summary>
        public int CritStages { get; set; } = 0;
    }
}
