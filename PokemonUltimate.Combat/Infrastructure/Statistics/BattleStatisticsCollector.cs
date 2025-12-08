using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Constants;
using PokemonUltimate.Combat.Infrastructure.Events;
using PokemonUltimate.Combat.Infrastructure.Statistics.Definition;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Infrastructure.Statistics
{
    /// <summary>
    /// Collects battle statistics by subscribing to battle events.
    /// Automatically tracks statistics when events are raised.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class BattleStatisticsCollector : IBattleStatisticsCollector
    {
        private readonly BattleStatistics _statistics = new BattleStatistics();
        private int _currentTurn = 0;
        private bool _isSubscribed = false;
        private bool _autoResetOnBattleStart = true; // Default: reset on battle start

        /// <summary>
        /// Subscribes this collector to battle events.
        /// Uses the BattleEventManager system.
        /// Idempotent: can be called multiple times safely.
        /// </summary>
        public void Subscribe()
        {
            if (_isSubscribed)
                return; // Already subscribed

            BattleEventManager.BattleStart += OnBattleStart;
            BattleEventManager.BattleEnd += OnBattleEnd;
            BattleEventManager.TurnStart += OnTurnStart;
            BattleEventManager.TurnEnd += OnTurnEnd;
            BattleEventManager.ActionExecuted += OnActionExecuted;
            BattleEventManager.StepExecuted += OnStepExecuted; // Debug event

            _isSubscribed = true;
        }

        /// <summary>
        /// Unsubscribes this collector from battle events.
        /// </summary>
        public void Unsubscribe()
        {
            if (!_isSubscribed)
                return; // Not subscribed

            BattleEventManager.BattleStart -= OnBattleStart;
            BattleEventManager.BattleEnd -= OnBattleEnd;
            BattleEventManager.TurnStart -= OnTurnStart;
            BattleEventManager.TurnEnd -= OnTurnEnd;
            BattleEventManager.ActionExecuted -= OnActionExecuted;
            BattleEventManager.StepExecuted -= OnStepExecuted;

            _isSubscribed = false;
        }


        /// <summary>
        /// Gets or sets whether statistics should be automatically reset when a battle starts.
        /// Default is true. Set to false when accumulating statistics across multiple battles/tests.
        /// </summary>
        public bool AutoResetOnBattleStart
        {
            get => _autoResetOnBattleStart;
            set => _autoResetOnBattleStart = value;
        }

        /// <summary>
        /// Gets the collected statistics.
        /// </summary>
        public BattleStatistics GetStatistics()
        {
            return _statistics;
        }

        /// <summary>
        /// Resets all statistics.
        /// </summary>
        public void Reset()
        {
            _statistics.TotalTurns = 0;
            _statistics.TotalActions = 0;
            _statistics.ActionsByType.Clear();
            _statistics.PlayerDamageDealt = 0;
            _statistics.EnemyDamageDealt = 0;
            _statistics.PlayerHealing = 0;
            _statistics.EnemyHealing = 0;
            _statistics.PlayerMoveUsage.Clear();
            _statistics.EnemyMoveUsage.Clear();
            _statistics.MoveUsageByPokemon.Clear();
            _statistics.DamageByMove.Clear();
            _statistics.DamageValuesByMove.Clear();
            _statistics.ActionsPerTurn.Clear();
            _statistics.PokemonSwitches.Clear();
            _statistics.PlayerFainted.Clear();
            _statistics.EnemyFainted.Clear();
            _statistics.StatusEffectsApplied.Clear();
            _statistics.StatChanges.Clear();
            _statistics.WeatherChanges.Clear();
            _statistics.TerrainChanges.Clear();
            _statistics.StepExecutionTimes.Clear();
            _statistics.ActionsByPokemon.Clear();
            _statistics.PlayerActionsByType.Clear();
            _statistics.EnemyActionsByType.Clear();
            _statistics.CriticalHits = 0;
            _statistics.MissedMoves = 0;
            _currentTurn = 0;
            // Note: _isSubscribed is NOT reset - subscription persists across battles
        }

        private void OnBattleStart(object sender, BattleStartEventArgs e)
        {
            // Only reset if auto-reset is enabled
            // This allows accumulating statistics across multiple battles/tests
            if (_autoResetOnBattleStart)
            {
                Reset();
            }
        }

        private void OnBattleEnd(object sender, BattleEndEventArgs e)
        {
            _statistics.Outcome = e.Outcome;
            _statistics.FinalField = e.Field;
        }

        private void OnTurnStart(object sender, TurnEventArgs e)
        {
            _currentTurn = e.TurnNumber;
            _statistics.TotalTurns = Math.Max(_statistics.TotalTurns, e.TurnNumber);
            _statistics.ActionsPerTurn[_currentTurn] = 0; // Initialize action count for this turn
        }

        private void OnTurnEnd(object sender, TurnEventArgs e)
        {
            // Turn end statistics can be added here if needed
        }

        private void OnStepExecuted(object sender, StepExecutedEventArgs e)
        {
            if (!_statistics.StepExecutionTimes.ContainsKey(e.StepName))
            {
                _statistics.StepExecutionTimes[e.StepName] = new List<TimeSpan>();
            }
            _statistics.StepExecutionTimes[e.StepName].Add(e.Duration);
        }

        private void OnActionExecuted(object sender, ActionExecutedEventArgs e)
        {
            var action = e.Action;
            var field = e.Field;

            _statistics.TotalActions++;

            // Track actions per turn (optimized: use TryGetValue)
            if (!_statistics.ActionsPerTurn.TryGetValue(_currentTurn, out var turnActions))
            {
                _statistics.ActionsPerTurn[_currentTurn] = 1;
            }
            else
            {
                _statistics.ActionsPerTurn[_currentTurn] = turnActions + 1;
            }

            // Track action type (optimized: use TryGetValue)
            var actionType = action.GetType().Name;
            if (!_statistics.ActionsByType.TryGetValue(actionType, out var actionCount))
            {
                _statistics.ActionsByType[actionType] = 1;
            }
            else
            {
                _statistics.ActionsByType[actionType] = actionCount + 1;
            }

            // Track actions by Pokemon and by team (optimized: cache isPlayer check)
            if (action.User?.Pokemon != null)
            {
                var pokemonName = action.User.Pokemon.DisplayName;
                var isPlayer = IsPlayerSlot(action.User, field);

                // Track by Pokemon (optimized: use TryGetValue)
                if (!_statistics.ActionsByPokemon.TryGetValue(pokemonName, out var pokemonActions))
                {
                    _statistics.ActionsByPokemon[pokemonName] = new Dictionary<string, int> { [actionType] = 1 };
                }
                else
                {
                    if (!pokemonActions.TryGetValue(actionType, out var pokemonActionCount))
                    {
                        pokemonActions[actionType] = 1;
                    }
                    else
                    {
                        pokemonActions[actionType] = pokemonActionCount + 1;
                    }
                }

                // Track by team (optimized: use TryGetValue)
                var teamActions = isPlayer ? _statistics.PlayerActionsByType : _statistics.EnemyActionsByType;
                if (!teamActions.TryGetValue(actionType, out var teamActionCount))
                {
                    teamActions[actionType] = 1;
                }
                else
                {
                    teamActions[actionType] = teamActionCount + 1;
                }
            }

            // Track specific action types
            switch (action)
            {
                case DamageAction damageAction:
                    TrackDamage(damageAction, field);
                    break;

                case HealAction healAction:
                    TrackHealing(healAction, field);
                    break;

                case UseMoveAction moveAction:
                    TrackMoveUsage(moveAction, field);
                    break;

                case FaintAction faintAction:
                    TrackFaint(faintAction, field);
                    break;

                case ApplyStatusAction statusAction:
                    TrackStatusEffect(statusAction, field);
                    break;

                case StatChangeAction statAction:
                    TrackStatChange(statAction, field);
                    break;

                case SetWeatherAction weatherAction:
                    TrackWeatherChange(weatherAction);
                    break;

                case SetTerrainAction terrainAction:
                    TrackTerrainChange(terrainAction);
                    break;

                case SwitchAction switchAction:
                    TrackSwitch(switchAction, field);
                    break;
            }
        }

        private void TrackDamage(DamageAction action, BattleField field)
        {
            if (action.User == null || action.Context == null) return;

            var isPlayer = IsPlayerSlot(action.User, field);
            var damage = action.Context.FinalDamage;
            var move = action.Context.Move;

            // Track total damage
            if (isPlayer)
            {
                _statistics.PlayerDamageDealt += damage;
            }
            else
            {
                _statistics.EnemyDamageDealt += damage;
            }

            // Track damage by move (optimized: use TryGetValue)
            if (move != null)
            {
                var moveName = move.Name;
                if (!_statistics.DamageByMove.TryGetValue(moveName, out var currentDamage))
                {
                    _statistics.DamageByMove[moveName] = damage;
                }
                else
                {
                    _statistics.DamageByMove[moveName] = currentDamage + damage;
                }

                // Track individual damage values for statistics
                if (!_statistics.DamageValuesByMove.TryGetValue(moveName, out var damageList))
                {
                    _statistics.DamageValuesByMove[moveName] = new List<int> { damage };
                }
                else
                {
                    damageList.Add(damage);
                }
            }

            // Track critical hits
            if (action.Context.IsCritical)
            {
                _statistics.CriticalHits++;
            }
        }

        private void TrackHealing(HealAction action, BattleField field)
        {
            if (action.User == null) return;

            var isPlayer = IsPlayerSlot(action.User, field);
            var healing = action.Amount;

            if (isPlayer)
            {
                _statistics.PlayerHealing += healing;
            }
            else
            {
                _statistics.EnemyHealing += healing;
            }
        }

        private void TrackMoveUsage(UseMoveAction action, BattleField field)
        {
            if (action.User == null || action.MoveInstance == null || action.MoveInstance.Move == null) return;

            var isPlayer = IsPlayerSlot(action.User, field);
            var moveName = action.MoveInstance.Move.Name;
            var pokemonName = action.User.Pokemon?.DisplayName ?? "Unknown";

            // Track move usage by side (optimized: use TryGetValue)
            var moveUsage = isPlayer ? _statistics.PlayerMoveUsage : _statistics.EnemyMoveUsage;
            if (!moveUsage.TryGetValue(moveName, out var moveCount))
            {
                moveUsage[moveName] = 1;
            }
            else
            {
                moveUsage[moveName] = moveCount + 1;
            }

            // Track move usage by Pokemon (optimized: use TryGetValue)
            if (!_statistics.MoveUsageByPokemon.TryGetValue(pokemonName, out var pokemonMoveUsage))
            {
                _statistics.MoveUsageByPokemon[pokemonName] = new Dictionary<string, int> { [moveName] = 1 };
            }
            else
            {
                if (!pokemonMoveUsage.TryGetValue(moveName, out var pokemonMoveCount))
                {
                    pokemonMoveUsage[moveName] = 1;
                }
                else
                {
                    pokemonMoveUsage[moveName] = pokemonMoveCount + 1;
                }
            }
        }

        private void TrackFaint(FaintAction action, BattleField field)
        {
            // Track the Pokemon that fainted (Target is the fainted Pokemon)
            if (action.Target?.Pokemon == null) return;

            var isPlayer = IsPlayerSlot(action.Target, field);
            var pokemonName = action.Target.Pokemon.DisplayName;

            if (isPlayer)
            {
                _statistics.PlayerFainted.Add(pokemonName);
            }
            else
            {
                _statistics.EnemyFainted.Add(pokemonName);
            }
        }

        private void TrackSwitch(SwitchAction action, BattleField field)
        {
            if (action.NewPokemon == null) return;

            var pokemonName = action.NewPokemon.DisplayName;
            if (!_statistics.PokemonSwitches.TryGetValue(pokemonName, out var switchCount))
            {
                _statistics.PokemonSwitches[pokemonName] = 1;
            }
            else
            {
                _statistics.PokemonSwitches[pokemonName] = switchCount + 1;
            }
        }

        private void TrackStatusEffect(ApplyStatusAction action, BattleField field)
        {
            if (action.Status == PersistentStatus.None) return;

            var statusName = action.Status.ToString();
            if (!_statistics.StatusEffectsApplied.TryGetValue(statusName, out var statusCount))
            {
                _statistics.StatusEffectsApplied[statusName] = 1;
            }
            else
            {
                _statistics.StatusEffectsApplied[statusName] = statusCount + 1;
            }
        }

        private void TrackStatChange(StatChangeAction action, BattleField field)
        {
            if (action.Target?.Pokemon == null) return;

            var pokemonName = action.Target.Pokemon.DisplayName;
            var statName = action.Stat.ToString();
            var change = action.Change;

            // Optimized: use TryGetValue
            if (!_statistics.StatChanges.TryGetValue(pokemonName, out var pokemonStatChanges))
            {
                _statistics.StatChanges[pokemonName] = new Dictionary<string, int> { [statName] = change };
            }
            else
            {
                if (!pokemonStatChanges.TryGetValue(statName, out var currentChange))
                {
                    pokemonStatChanges[statName] = change;
                }
                else
                {
                    pokemonStatChanges[statName] = currentChange + change;
                }
            }
        }

        private void TrackWeatherChange(SetWeatherAction action)
        {
            if (action.WeatherData != null && !string.IsNullOrEmpty(action.WeatherData.Name))
            {
                _statistics.WeatherChanges.Add(action.WeatherData.Name);
            }
            else
            {
                _statistics.WeatherChanges.Add(action.Weather.ToString());
            }
        }

        private void TrackTerrainChange(SetTerrainAction action)
        {
            if (action.TerrainData != null && !string.IsNullOrEmpty(action.TerrainData.Name))
            {
                _statistics.TerrainChanges.Add(action.TerrainData.Name);
            }
            else
            {
                _statistics.TerrainChanges.Add(action.Terrain.ToString());
            }
        }

        /// <summary>
        /// Optimized helper to check if a slot belongs to the player side.
        /// Caches the result to avoid repeated Contains() calls.
        /// </summary>
        private bool IsPlayerSlot(BattleSlot slot, BattleField field)
        {
            // Optimized: Check if slot is in player side slots (O(1) if slots are indexed, O(n) otherwise)
            // For singles/doubles, this is typically O(1) or O(2), which is acceptable
            return field.PlayerSide.Slots.Contains(slot);
        }
    }
}
