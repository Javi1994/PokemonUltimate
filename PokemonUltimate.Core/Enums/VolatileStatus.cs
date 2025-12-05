using System;

namespace PokemonUltimate.Core.Enums
{
    /// <summary>
    /// Temporary status conditions that are removed when Pokemon switches out.
    /// Multiple can be active simultaneously (flags).
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    [Flags]
    public enum VolatileStatus
    {
        /// <summary>No volatile status.</summary>
        None = 0,

        /// <summary>May hurt itself in confusion (1-4 turns).</summary>
        Confusion = 1,

        /// <summary>Cannot act this turn if hit by a flinching move.</summary>
        Flinch = 2,

        /// <summary>Drains HP each turn, healing the opponent.</summary>
        LeechSeed = 4,

        /// <summary>May be immobilized by love (Attract).</summary>
        Attract = 8,

        /// <summary>Takes damage each turn (Ghost-type Curse).</summary>
        Curse = 16,

        /// <summary>Forced to use the same move.</summary>
        Encore = 32,

        /// <summary>Cannot use status moves.</summary>
        Taunt = 64,

        /// <summary>Cannot use the same move twice in a row.</summary>
        Torment = 128,

        /// <summary>One move is disabled and cannot be used.</summary>
        Disable = 256,

        /// <summary>Semi-invulnerable states (Fly, Dig, Dive).</summary>
        SemiInvulnerable = 512,

        /// <summary>Charging states (Solar Beam, Skull Bash).</summary>
        Charging = 1024,

        /// <summary>Protected from most attacks (Protect, Detect).</summary>
        Protected = 2048,

        /// <summary>Pokemon is switching out this turn (for Pursuit detection).</summary>
        SwitchingOut = 4096,

        /// <summary>Pokemon is tightening focus for Focus Punch.</summary>
        Focusing = 8192
    }
}
