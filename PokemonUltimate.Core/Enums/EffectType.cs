namespace PokemonUltimate.Core.Enums
{
    /// <summary>
    /// Defines the types of effects that moves can have.
    /// Each effect type corresponds to a concrete IMoveEffect implementation.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    public enum EffectType
    {
        // ===== CORE DAMAGE EFFECTS =====
        
        /// <summary>Standard damage based on move power and stats.</summary>
        Damage,
        
        /// <summary>Fixed amount of damage, ignores stats (Dragon Rage, Seismic Toss).</summary>
        FixedDamage,
        
        /// <summary>User takes recoil damage based on damage dealt (Brave Bird, Flare Blitz).</summary>
        Recoil,
        
        /// <summary>User heals based on damage dealt (Giga Drain, Drain Punch).</summary>
        Drain,
        
        /// <summary>Hits multiple times in one turn (Bullet Seed, Fury Attack).</summary>
        MultiHit,
        
        // ===== STATUS EFFECTS =====
        
        /// <summary>Applies a persistent status condition (Thunder Wave, Will-O-Wisp).</summary>
        Status,
        
        /// <summary>Applies a volatile status (Confuse Ray, Attract, Leech Seed).</summary>
        VolatileStatus,
        
        /// <summary>May cause target to flinch (Iron Head, Air Slash).</summary>
        Flinch,
        
        // ===== STAT EFFECTS =====
        
        /// <summary>Modifies stat stages -6 to +6 (Swords Dance, Growl).</summary>
        StatChange,
        
        // ===== HEALING EFFECTS =====
        
        /// <summary>Direct HP recovery (Recover, Roost, Moonlight).</summary>
        Heal,
        
        // ===== TWO-TURN EFFECTS =====
        
        /// <summary>Two-turn move with charging phase (Fly, Dig, Solar Beam).</summary>
        Charging,
        
        /// <summary>Damage occurs after a delay (Future Sight, Doom Desire, Wish).</summary>
        DelayedDamage,
        
        // ===== PROTECTION EFFECTS =====
        
        /// <summary>Blocks incoming attacks (Protect, Detect, King's Shield).</summary>
        Protection,
        
        /// <summary>Creates a substitute that absorbs damage.</summary>
        Substitute,
        
        // ===== MOVE RESTRICTION EFFECTS =====
        
        /// <summary>Restricts target's move usage (Encore, Disable, Taunt).</summary>
        MoveRestriction,
        
        /// <summary>Traps and damages over multiple turns (Wrap, Fire Spin).</summary>
        Binding,
        
        // ===== SWITCHING EFFECTS =====
        
        /// <summary>Forces target to switch out (Roar, Dragon Tail).</summary>
        ForceSwitch,
        
        /// <summary>Switches user out after attacking (U-turn, Volt Switch).</summary>
        SwitchAfterAttack,
        
        /// <summary>Switch that transfers stat stages and volatile status.</summary>
        BatonPass,
        
        // ===== FIELD EFFECTS =====
        
        /// <summary>Affects the battle field (Weather, Terrain, Hazards, Screens).</summary>
        FieldCondition,
        
        // ===== SPECIAL DAMAGE EFFECTS =====
        
        /// <summary>User faints after dealing damage (Explosion, Memento).</summary>
        SelfDestruct,
        
        /// <summary>Revenge damage based on damage taken (Counter, Mirror Coat).</summary>
        Revenge,
        
        // ===== PRIORITY EFFECTS =====
        
        /// <summary>Modifies priority conditionally (Grassy Glide in terrain).</summary>
        PriorityModifier,
        
        // ===== UTILITY EFFECTS =====
        
        /// <summary>Boosts next attack's power (Focus Energy, Charge).</summary>
        PowerUp,
        
        /// <summary>Calls another move (Metronome, Mirror Move, Copycat).</summary>
        CallMove,
        
        /// <summary>Copies the target's species, stats, and moves (Transform).</summary>
        Transform,
        
        // ===== PLANNED (Not yet implemented) =====
        
        /// <summary>Absorbs status moves (Magic Coat, Magic Bounce).</summary>
        MagicBounce,
        
        /// <summary>Transfers item between user and target (Trick, Switcheroo).</summary>
        ItemSwap,
        
        /// <summary>Copies target's ability (Role Play, Skill Swap).</summary>
        AbilityCopy,
        
        /// <summary>Removes target's ability (Gastro Acid).</summary>
        AbilitySuppress,
        
        /// <summary>Changes target's type (Soak, Forest's Curse).</summary>
        TypeChange,
        
        /// <summary>Sets a room effect (Trick Room, Wonder Room).</summary>
        RoomEffect,
    }
}
