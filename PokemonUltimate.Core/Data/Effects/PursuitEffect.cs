using PokemonUltimate.Core.Data.Effects.Definition;
using PokemonUltimate.Core.Data.Enums;


namespace PokemonUltimate.Core.Data.Effects
{
    /// <summary>
    /// Doubles power if target switches out this turn.
    /// Hits before the switch completes.
    /// Used by Pursuit.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.15: Advanced Move Mechanics
    /// **Documentation**: See `docs/features/2-combat-system/2.15-advanced-move-mechanics/README.md`
    /// </remarks>
    public class PursuitEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.PriorityModifier;
        public string Description => "Doubles power if target switches out. Hits before switch completes.";
    }
}

