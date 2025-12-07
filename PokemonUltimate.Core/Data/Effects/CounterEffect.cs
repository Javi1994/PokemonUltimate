using PokemonUltimate.Core.Data.Effects.Definition;
using PokemonUltimate.Core.Data.Enums;


namespace PokemonUltimate.Core.Data.Effects
{
    /// <summary>
    /// Returns damage based on damage taken this turn.
    /// Counter returns 2x physical damage, Mirror Coat returns 2x special damage.
    /// Used by Counter, Mirror Coat, etc.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.15: Advanced Move Mechanics
    /// **Documentation**: See `docs/features/2-combat-system/2.15-advanced-move-mechanics/README.md`
    /// </remarks>
    public class CounterEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.Revenge;
        public string Description => IsPhysicalCounter
            ? "Returns 2x physical damage taken."
            : "Returns 2x special damage taken.";

        /// <summary>
        /// True if this counters physical damage (Counter), false if special (Mirror Coat).
        /// </summary>
        public bool IsPhysicalCounter { get; set; } = true;
    }
}

