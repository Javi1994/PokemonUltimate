namespace PokemonUltimate.Core.Enums
{
    /// <summary>
    /// Defines when a held item activates during battle.
    /// </summary>
    [System.Flags]
    public enum ItemTrigger
    {
        None = 0,
        
        // === TURN EVENTS ===
        /// <summary>At the end of each turn (Leftovers, Black Sludge).</summary>
        OnTurnEnd = 1 << 0,
        
        // === HP EVENTS ===
        /// <summary>When HP drops below threshold (Berries, Focus Sash).</summary>
        OnLowHP = 1 << 1,
        /// <summary>When would be knocked out (Focus Sash).</summary>
        OnWouldFaint = 1 << 2,
        
        // === STATUS EVENTS ===
        /// <summary>When status is applied (Lum Berry, Chesto Berry).</summary>
        OnStatusApplied = 1 << 3,
        /// <summary>When confused (Persim Berry).</summary>
        OnConfused = 1 << 4,
        
        // === MOVE EVENTS ===
        /// <summary>When using a move (Choice items lock).</summary>
        OnMoveUsed = 1 << 5,
        /// <summary>After dealing damage (Life Orb recoil).</summary>
        OnDamageDealt = 1 << 6,
        /// <summary>When hit by super effective move (Weakness Policy).</summary>
        OnSuperEffectiveHit = 1 << 7,
        
        // === STAT EVENTS ===
        /// <summary>When stats are lowered (White Herb).</summary>
        OnStatLowered = 1 << 8,
        
        // === CONTACT EVENTS ===
        /// <summary>When hit by contact move (Rocky Helmet).</summary>
        OnContactReceived = 1 << 9,
        
        // === PASSIVE (always active) ===
        /// <summary>Item provides passive stat boost (Choice Band).</summary>
        Passive = 1 << 30,
    }
}

