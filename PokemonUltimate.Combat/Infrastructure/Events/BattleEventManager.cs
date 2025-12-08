using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Constants;

namespace PokemonUltimate.Combat.Infrastructure.Events
{
    /// <summary>
    /// Centralized manager for battle events.
    /// Provides easy integration and separates production events from debug events.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public static class BattleEventManager
    {
        private static bool _isDebugMode = false;

        /// <summary>
        /// Gets or sets whether debug events should be raised.
        /// When false, only production events are raised.
        /// </summary>
        public static bool IsDebugMode
        {
            get => _isDebugMode;
            set => _isDebugMode = value;
        }

        #region Production Events (Always Available)

        /// <summary>
        /// Event raised when a battle starts.
        /// </summary>
        public static event EventHandler<BattleStartEventArgs> BattleStart;

        /// <summary>
        /// Event raised when a battle ends.
        /// </summary>
        public static event EventHandler<BattleEndEventArgs> BattleEnd;

        /// <summary>
        /// Event raised when a turn starts.
        /// </summary>
        public static event EventHandler<TurnEventArgs> TurnStart;

        /// <summary>
        /// Event raised when a turn ends.
        /// </summary>
        public static event EventHandler<TurnEventArgs> TurnEnd;

        /// <summary>
        /// Event raised when an action is executed.
        /// </summary>
        public static event EventHandler<ActionExecutedEventArgs> ActionExecuted;

        #endregion

        #region Debug Events (Only in Debug Mode)

        /// <summary>
        /// Event raised when a step executes (debug only).
        /// </summary>
        public static event EventHandler<StepExecutedEventArgs> StepExecuted;

        /// <summary>
        /// Event raised when a step starts executing (debug only).
        /// </summary>
        public static event EventHandler<StepStartedEventArgs> StepStarted;

        /// <summary>
        /// Event raised when a step finishes executing (debug only).
        /// </summary>
        public static event EventHandler<StepFinishedEventArgs> StepFinished;

        /// <summary>
        /// Event raised when battle state changes (debug only).
        /// </summary>
        public static event EventHandler<BattleStateChangedEventArgs> BattleStateChanged;

        #endregion

        #region Production Event Raisers

        /// <summary>
        /// Raises the BattleStart event.
        /// </summary>
        public static void RaiseBattleStart(BattleField field)
        {
            // Optimize: only create EventArgs if there are subscribers
            if (BattleStart != null)
            {
                BattleStart(null, new BattleStartEventArgs { Field = field });
            }
        }

        /// <summary>
        /// Raises the BattleEnd event.
        /// </summary>
        public static void RaiseBattleEnd(BattleOutcome outcome, BattleField field)
        {
            // Optimize: only create EventArgs if there are subscribers
            if (BattleEnd != null)
            {
                BattleEnd(null, new BattleEndEventArgs { Outcome = outcome, Field = field });
            }
        }

        /// <summary>
        /// Raises the TurnStart event.
        /// </summary>
        public static void RaiseTurnStart(int turnNumber, BattleField field)
        {
            // Optimize: only create EventArgs if there are subscribers
            if (TurnStart != null)
            {
                TurnStart(null, new TurnEventArgs { TurnNumber = turnNumber, Field = field });
            }
        }

        /// <summary>
        /// Raises the TurnEnd event.
        /// </summary>
        public static void RaiseTurnEnd(int turnNumber, BattleField field)
        {
            // Optimize: only create EventArgs if there are subscribers
            if (TurnEnd != null)
            {
                TurnEnd(null, new TurnEventArgs { TurnNumber = turnNumber, Field = field });
            }
        }

        /// <summary>
        /// Raises the ActionExecuted event.
        /// </summary>
        public static void RaiseActionExecuted(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions)
        {
            // Optimize: only create EventArgs if there are subscribers
            if (ActionExecuted != null)
            {
                ActionExecuted(null, new ActionExecutedEventArgs
                {
                    Action = action,
                    Field = field,
                    Reactions = reactions
                });
            }
        }

        #endregion

        #region Debug Event Raisers (Only if Debug Mode)

        /// <summary>
        /// Raises the StepExecuted event (only if debug mode is enabled).
        /// </summary>
        public static void RaiseStepExecuted(string stepName, StepType stepType, BattleField field, TimeSpan duration)
        {
            if (!_isDebugMode || StepExecuted == null) return;

            StepExecuted(null, new StepExecutedEventArgs
            {
                StepName = stepName,
                StepType = stepType,
                Field = field,
                Duration = duration
            });
        }

        /// <summary>
        /// Raises the StepStarted event (only if debug mode is enabled).
        /// </summary>
        public static void RaiseStepStarted(string stepName, StepType stepType, BattleField field)
        {
            if (!_isDebugMode || StepStarted == null) return;

            StepStarted(null, new StepStartedEventArgs
            {
                StepName = stepName,
                StepType = stepType,
                Field = field
            });
        }

        /// <summary>
        /// Raises the StepFinished event (only if debug mode is enabled).
        /// </summary>
        public static void RaiseStepFinished(string stepName, StepType stepType, BattleField field, TimeSpan duration, bool shouldContinue)
        {
            if (!_isDebugMode || StepFinished == null) return;

            StepFinished(null, new StepFinishedEventArgs
            {
                StepName = stepName,
                StepType = stepType,
                Field = field,
                Duration = duration,
                ShouldContinue = shouldContinue
            });
        }

        /// <summary>
        /// Raises the BattleStateChanged event (only if debug mode is enabled).
        /// </summary>
        public static void RaiseBattleStateChanged(BattleField field, string changeDescription)
        {
            if (!_isDebugMode) return;

            BattleStateChanged?.Invoke(null, new BattleStateChangedEventArgs
            {
                Field = field,
                ChangeDescription = changeDescription
            });
        }

        #endregion

        /// <summary>
        /// Clears all event subscriptions.
        /// Useful for testing or cleanup.
        /// </summary>
        public static void ClearAll()
        {
            BattleStart = null;
            BattleEnd = null;
            TurnStart = null;
            TurnEnd = null;
            ActionExecuted = null;
            StepExecuted = null;
            StepStarted = null;
            StepFinished = null;
            BattleStateChanged = null;
        }
    }

    #region Additional Event Args

    /// <summary>
    /// Event arguments for step started.
    /// </summary>
    public class StepStartedEventArgs : BattleEventArgs
    {
        public string StepName { get; set; }
        public StepType StepType { get; set; }
    }

    /// <summary>
    /// Event arguments for step finished.
    /// </summary>
    public class StepFinishedEventArgs : BattleEventArgs
    {
        public string StepName { get; set; }
        public StepType StepType { get; set; }
        public TimeSpan Duration { get; set; }
        public bool ShouldContinue { get; set; }
    }

    /// <summary>
    /// Event arguments for battle state changes.
    /// </summary>
    public class BattleStateChangedEventArgs : BattleEventArgs
    {
        public string ChangeDescription { get; set; }
    }

    #endregion
}
