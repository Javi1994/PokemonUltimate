using PokemonUltimate.Core.Data.Effects.Definition;
using PokemonUltimate.Core.Data.Enums;


namespace PokemonUltimate.Core.Data.Effects
{
    /// <summary>
    /// Protects the user from most attacks for one turn.
    /// Success rate halves with consecutive use (100%, 50%, 25%, 12.5%...).
    /// Used by Protect, Detect, King's Shield, etc.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.15: Advanced Move Mechanics
    /// **Documentation**: See `docs/features/2-combat-system/2.15-advanced-move-mechanics/README.md`
    /// </remarks>
    public class ProtectEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.Protection;
        public string Description => "Protects the user from most attacks. Success rate halves with consecutive use.";

        /// <summary>
        /// Moves that bypass Protect (e.g., Feint, Shadow Force, Phantom Force).
        /// </summary>
        public string[] BypassMoveIds { get; set; } = new string[0];
    }
}

