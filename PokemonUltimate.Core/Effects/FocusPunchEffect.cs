using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// User "tightens focus" at start of turn.
    /// If hit before moving, the move fails.
    /// PP is still deducted even if fails.
    /// Used by Focus Punch.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.15: Advanced Move Mechanics
    /// **Documentation**: See `docs/features/2-combat-system/2.15-advanced-move-mechanics/README.md`
    /// </remarks>
    public class FocusPunchEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.PowerUp;
        public string Description => "User tightens focus. Fails if hit before moving.";
    }
}

