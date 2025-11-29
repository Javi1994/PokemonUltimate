namespace PokemonUltimate.Core.Enums
{
    /// <summary>
    /// Defines the types of effects that moves can have.
    /// Each effect type corresponds to a concrete IMoveEffect implementation.
    /// </summary>
    public enum EffectType
    {
        // ===== IMPLEMENTED =====
        
        /// <summary>Standard damage based on move power and stats.</summary>
        Damage,
        
        /// <summary>Fixed amount of damage, ignores stats.</summary>
        FixedDamage,
        
        /// <summary>Applies a persistent status condition (Burn, Paralysis, etc.).</summary>
        Status,
        
        /// <summary>Modifies stat stages (-6 to +6).</summary>
        StatChange,
        
        /// <summary>User takes recoil damage based on damage dealt.</summary>
        Recoil,
        
        /// <summary>User heals based on damage dealt.</summary>
        Drain,
        
        /// <summary>Direct HP recovery.</summary>
        Heal,
        
        /// <summary>May cause target to flinch.</summary>
        Flinch,
        
        /// <summary>Hits multiple times in one turn.</summary>
        MultiHit,
        
        // ===== PLANNED (Not yet implemented) =====
        
        /// <summary>Applies a volatile status (Confusion, LeechSeed, Attract, etc.).</summary>
        VolatileStatus,
        
        /// <summary>Two-turn move with charging phase (Fly, Dig, Solar Beam).</summary>
        Charging,
        
        /// <summary>Damage occurs after a delay (Future Sight, Doom Desire).</summary>
        DelayedDamage,
        
        /// <summary>Blocks incoming attacks (Protect, Detect, Wide Guard).</summary>
        Protection,
        
        /// <summary>Creates a substitute that absorbs damage.</summary>
        Substitute,
        
        /// <summary>Restricts target's move usage (Encore, Disable, Taunt, Torment).</summary>
        MoveRestriction,
        
        /// <summary>Switch that transfers stat stages and volatile status.</summary>
        BatonPass,
        
        /// <summary>Affects the battle field (Weather, Terrain, Trick Room).</summary>
        FieldCondition,
        
        /// <summary>Calls another move (Metronome, Mirror Move, Copycat).</summary>
        CallMove,
        
        /// <summary>Copies the target's species, stats, and moves.</summary>
        Transform,
        
        /// <summary>User faints after dealing damage (Explosion, Self-Destruct).</summary>
        SelfDestruct,
        
        /// <summary>Forces target to switch out (Roar, Whirlwind, Dragon Tail).</summary>
        ForceSwitch,
        
        /// <summary>Traps and damages over multiple turns (Wrap, Bind, Fire Spin).</summary>
        Binding,
        
        /// <summary>Switches user out after attacking (U-turn, Volt Switch).</summary>
        SwitchAfterAttack,
        
        /// <summary>Boosts next attack's power (Focus Energy crit, Charge for Electric).</summary>
        PowerUp,
        
        /// <summary>Revenge damage based on damage taken (Counter, Mirror Coat).</summary>
        Revenge
    }
}
