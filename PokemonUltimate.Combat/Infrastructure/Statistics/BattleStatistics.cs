using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Constants;

namespace PokemonUltimate.Combat.Infrastructure.Statistics
{
    /// <summary>
    /// Collected statistics about a battle.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class BattleStatistics
    {
        /// <summary>
        /// Total number of turns.
        /// </summary>
        public int TotalTurns { get; set; }

        /// <summary>
        /// Total number of actions executed.
        /// </summary>
        public int TotalActions { get; set; }

        /// <summary>
        /// Actions executed by type.
        /// </summary>
        public Dictionary<string, int> ActionsByType { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Actions executed by Pokemon (Pokemon name -> Action type -> count).
        /// </summary>
        public Dictionary<string, Dictionary<string, int>> ActionsByPokemon { get; set; } = new Dictionary<string, Dictionary<string, int>>();

        /// <summary>
        /// Actions executed by player team (Action type -> count).
        /// </summary>
        public Dictionary<string, int> PlayerActionsByType { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Actions executed by enemy team (Action type -> count).
        /// </summary>
        public Dictionary<string, int> EnemyActionsByType { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Total damage dealt by player.
        /// </summary>
        public int PlayerDamageDealt { get; set; }

        /// <summary>
        /// Total damage dealt by enemy.
        /// </summary>
        public int EnemyDamageDealt { get; set; }

        /// <summary>
        /// Total healing by player.
        /// </summary>
        public int PlayerHealing { get; set; }

        /// <summary>
        /// Total healing by enemy.
        /// </summary>
        public int EnemyHealing { get; set; }

        /// <summary>
        /// Moves used by player Pokemon (move name -> count).
        /// </summary>
        public Dictionary<string, int> PlayerMoveUsage { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Moves used by enemy Pokemon (move name -> count).
        /// </summary>
        public Dictionary<string, int> EnemyMoveUsage { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Detailed move usage by Pokemon (Pokemon name -> Move name -> count).
        /// </summary>
        public Dictionary<string, Dictionary<string, int>> MoveUsageByPokemon { get; set; } = new Dictionary<string, Dictionary<string, int>>();

        /// <summary>
        /// Damage dealt by each move (Move name -> total damage).
        /// </summary>
        public Dictionary<string, int> DamageByMove { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Individual damage values per move (Move name -> list of damage values).
        /// Used for statistics like min, max, median, average.
        /// </summary>
        public Dictionary<string, List<int>> DamageValuesByMove { get; set; } = new Dictionary<string, List<int>>();

        /// <summary>
        /// Actions executed per turn.
        /// </summary>
        public Dictionary<int, int> ActionsPerTurn { get; set; } = new Dictionary<int, int>();

        /// <summary>
        /// Pokemon switches (Pokemon name -> count).
        /// </summary>
        public Dictionary<string, int> PokemonSwitches { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Critical hits count.
        /// </summary>
        public int CriticalHits { get; set; }

        /// <summary>
        /// Missed moves count.
        /// </summary>
        public int MissedMoves { get; set; }

        /// <summary>
        /// Pokemon that fainted (player side).
        /// </summary>
        public List<string> PlayerFainted { get; set; } = new List<string>();

        /// <summary>
        /// Pokemon that fainted (enemy side).
        /// </summary>
        public List<string> EnemyFainted { get; set; } = new List<string>();

        /// <summary>
        /// Status effects applied.
        /// </summary>
        public Dictionary<string, int> StatusEffectsApplied { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Stat changes applied.
        /// </summary>
        public Dictionary<string, Dictionary<string, int>> StatChanges { get; set; } = new Dictionary<string, Dictionary<string, int>>();

        /// <summary>
        /// Weather changes.
        /// </summary>
        public List<string> WeatherChanges { get; set; } = new List<string>();

        /// <summary>
        /// Terrain changes.
        /// </summary>
        public List<string> TerrainChanges { get; set; } = new List<string>();

        /// <summary>
        /// Step execution times.
        /// </summary>
        public Dictionary<string, List<System.TimeSpan>> StepExecutionTimes { get; set; } = new Dictionary<string, List<System.TimeSpan>>();

        /// <summary>
        /// Battle outcome.
        /// </summary>
        public BattleOutcome Outcome { get; set; }

        /// <summary>
        /// Final battlefield state.
        /// </summary>
        public BattleField FinalField { get; set; }
    }
}
