namespace PokemonUltimate.Combat.Engine
{
    /// <summary>
    /// Enumeration of all battle phases/moments where processors can execute.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public enum BattlePhase
    {
        /// <summary>When the battle starts.</summary>
        BattleStart,

        /// <summary>When a turn starts.</summary>
        TurnStart,

        /// <summary>When collecting actions from all slots.</summary>
        ActionCollection,

        /// <summary>When sorting actions by priority and speed.</summary>
        ActionSorting,

        /// <summary>When executing actions (handled by BattleQueue).</summary>
        ActionExecution,

        /// <summary>When a Pokemon switches into battle.</summary>
        SwitchIn,

        /// <summary>Before a Pokemon uses a move.</summary>
        BeforeMove,

        /// <summary>After a Pokemon uses a move.</summary>
        AfterMove,

        /// <summary>When a Pokemon receives contact from a move.</summary>
        ContactReceived,

        /// <summary>When a Pokemon takes damage.</summary>
        DamageTaken,

        /// <summary>When weather changes.</summary>
        WeatherChange,

        /// <summary>When handling automatic switching for fainted Pokemon.</summary>
        FaintedPokemonSwitching,

        /// <summary>When processing end-of-turn effects (status damage, weather, etc.).</summary>
        EndOfTurnEffects,

        /// <summary>When decrementing durations (weather, terrain, side conditions).</summary>
        DurationDecrement,

        /// <summary>When processing end-of-turn triggers (abilities and items).</summary>
        TurnEnd,

        /// <summary>When the turn ends (validation, events).</summary>
        TurnEndValidation,

        /// <summary>When the battle ends.</summary>
        BattleEnd
    }
}
