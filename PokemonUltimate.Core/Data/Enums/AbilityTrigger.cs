namespace PokemonUltimate.Core.Data.Enums
{
    /// <summary>
    /// Defines when an ability activates during battle.
    /// Abilities can listen to one or more triggers.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    [System.Flags]
    public enum AbilityTrigger
    {
        None = 0,

        // === SWITCH EVENTS ===
        /// <summary>When this Pokemon switches into battle.</summary>
        OnSwitchIn = 1 << 0,
        /// <summary>When this Pokemon switches out of battle.</summary>
        OnSwitchOut = 1 << 1,

        // === TURN EVENTS ===
        /// <summary>At the start of each turn.</summary>
        OnTurnStart = 1 << 2,
        /// <summary>At the end of each turn.</summary>
        OnTurnEnd = 1 << 3,

        // === MOVE EVENTS ===
        /// <summary>Before this Pokemon uses a move.</summary>
        OnBeforeMove = 1 << 4,
        /// <summary>After this Pokemon uses a move.</summary>
        OnAfterMove = 1 << 5,
        /// <summary>When this Pokemon's move hits.</summary>
        OnMoveHit = 1 << 6,
        /// <summary>When this Pokemon's move misses.</summary>
        OnMoveMiss = 1 << 7,

        // === DAMAGE EVENTS ===
        /// <summary>When this Pokemon takes damage.</summary>
        OnDamageTaken = 1 << 8,
        /// <summary>When this Pokemon deals damage.</summary>
        OnDamageDealt = 1 << 9,
        /// <summary>When this Pokemon would be knocked out.</summary>
        OnWouldFaint = 1 << 10,

        // === CONTACT EVENTS ===
        /// <summary>When this Pokemon is hit by a contact move.</summary>
        OnContactReceived = 1 << 11,
        /// <summary>When this Pokemon makes contact with target.</summary>
        OnContactMade = 1 << 12,

        // === TYPE EVENTS ===
        /// <summary>When hit by a super effective move.</summary>
        OnSuperEffectiveHit = 1 << 13,
        /// <summary>When this Pokemon lands a critical hit.</summary>
        OnCriticalHit = 1 << 14,

        // === STATUS EVENTS ===
        /// <summary>When a status is about to be applied.</summary>
        OnStatusAttempt = 1 << 15,
        /// <summary>When a status is applied to this Pokemon.</summary>
        OnStatusApplied = 1 << 16,

        // === STAT EVENTS ===
        /// <summary>When stats are about to change.</summary>
        OnStatChangeAttempt = 1 << 17,
        /// <summary>When stats have changed.</summary>
        OnStatChanged = 1 << 18,

        // === WEATHER EVENTS ===
        /// <summary>When weather changes.</summary>
        OnWeatherChange = 1 << 19,
        /// <summary>Each turn during weather.</summary>
        OnWeatherTick = 1 << 20,

        // === TERRAIN EVENTS ===
        /// <summary>When terrain changes.</summary>
        OnTerrainChange = 1 << 21,
        /// <summary>Each turn during terrain.</summary>
        OnTerrainTick = 1 << 22,

        // === PASSIVE (always active, no trigger needed) ===
        /// <summary>Ability provides passive stat modification.</summary>
        Passive = 1 << 30,
    }
}

