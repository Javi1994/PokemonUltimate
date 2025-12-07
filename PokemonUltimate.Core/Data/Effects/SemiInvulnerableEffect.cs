using PokemonUltimate.Core.Data.Effects.Definition;
using PokemonUltimate.Core.Data.Enums;


namespace PokemonUltimate.Core.Data.Effects
{
    /// <summary>
    /// User becomes semi-invulnerable for 2 turns (charge turn, attack turn).
    /// Most moves miss, but specific moves can hit (Earthquake hits Dig, Surf hits Dive, etc.).
    /// Used by Fly, Dig, Dive, Bounce, Shadow Force, Phantom Force.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.15: Advanced Move Mechanics
    /// **Documentation**: See `docs/features/2-combat-system/2.15-advanced-move-mechanics/README.md`
    /// </remarks>
    public class SemiInvulnerableEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.Charging;
        public string Description => "User becomes semi-invulnerable for 2 turns. Most moves miss.";

        /// <summary>
        /// Moves that can hit this semi-invulnerable state (e.g., "Earthquake" for Dig, "Surf" for Dive).
        /// </summary>
        public string[] CanHitMoveIds { get; set; } = new string[0];
    }
}

