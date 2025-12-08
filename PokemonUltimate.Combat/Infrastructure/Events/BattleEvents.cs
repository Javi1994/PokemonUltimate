using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Constants;

namespace PokemonUltimate.Combat.Infrastructure.Events
{
    /// <summary>
    /// Event arguments for battle events.
    /// Provides data about what happened in the battle.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class BattleEventArgs : EventArgs
    {
        public BattleField Field { get; set; }
    }

    /// <summary>
    /// Event arguments for battle start.
    /// </summary>
    public class BattleStartEventArgs : BattleEventArgs
    {
    }

    /// <summary>
    /// Event arguments for battle end.
    /// </summary>
    public class BattleEndEventArgs : BattleEventArgs
    {
        public BattleOutcome Outcome { get; set; }
    }

    /// <summary>
    /// Event arguments for turn start/end.
    /// </summary>
    public class TurnEventArgs : BattleEventArgs
    {
        public int TurnNumber { get; set; }
    }

    /// <summary>
    /// Event arguments for step execution.
    /// </summary>
    public class StepExecutedEventArgs : BattleEventArgs
    {
        public string StepName { get; set; }
        public StepType StepType { get; set; }
        public TimeSpan Duration { get; set; }
    }

    /// <summary>
    /// Event arguments for action execution.
    /// </summary>
    public class ActionExecutedEventArgs : BattleEventArgs
    {
        public BattleAction Action { get; set; }
        public IEnumerable<BattleAction> Reactions { get; set; }
    }
}
