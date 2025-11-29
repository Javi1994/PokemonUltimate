using System;

namespace PokemonUltimate.Core.Enums
{
    // Temporary status conditions that are removed when Pokemon switches out.
    // Multiple can be active simultaneously (flags).
    [Flags]
    public enum VolatileStatus
    {
        None = 0,
        Confusion = 1,
        Flinch = 2,
        LeechSeed = 4,
        Attract = 8,
        Curse = 16,
        Encore = 32,
        Taunt = 64,
        Torment = 128,
        Disable = 256,
        
        // Semi-invulnerable states (Fly, Dig, Dive)
        SemiInvulnerable = 512,
        
        // Charging states (Solar Beam, Skull Bash)
        Charging = 1024
    }
}

