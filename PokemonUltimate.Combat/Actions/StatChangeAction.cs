using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions.Registry;
using PokemonUltimate.Combat.Actions.Validation;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.View.Definition;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Actions
{
    /// <summary>
    /// Modifies a stat stage of a Pokemon (-6 to +6).
    /// Stages are clamped to the valid range automatically.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class StatChangeAction : BattleAction
    {
        private readonly BehaviorCheckerRegistry _behaviorRegistry;

        /// <summary>
        /// The slot whose stat is being modified.
        /// </summary>
        public BattleSlot Target { get; }

        /// <summary>
        /// The stat being modified.
        /// </summary>
        public Stat Stat { get; }

        /// <summary>
        /// The amount to change the stat stage by (+/-).
        /// </summary>
        public int Change { get; }

        /// <summary>
        /// Creates a new stat change action.
        /// </summary>
        /// <param name="user">The slot that initiated this change. Can be null for system actions.</param>
        /// <param name="target">The slot whose stat is being modified. Cannot be null.</param>
        /// <param name="stat">The stat to modify. Cannot be HP.</param>
        /// <param name="change">The amount to change (+/-).</param>
        /// <param name="behaviorRegistry">The behavior checker registry. If null, creates a default one.</param>
        /// <exception cref="ArgumentNullException">If target is null.</exception>
        /// <exception cref="ArgumentException">If stat is HP.</exception>
        public StatChangeAction(BattleSlot user, BattleSlot target, Stat stat, int change, BehaviorCheckerRegistry behaviorRegistry = null) : base(user)
        {
            ActionValidators.ValidateTargetNotNull(target, nameof(target));

            if (stat == Stat.HP)
                throw new ArgumentException(ErrorMessages.CannotModifyHPStatStage, nameof(stat));

            Target = target;
            Stat = stat;
            Change = change;
            _behaviorRegistry = behaviorRegistry ?? new BehaviorCheckerRegistry();
        }

        /// <summary>
        /// Modifies the stat stage of the target Pokemon.
        /// Stages are automatically clamped to -6/+6.
        /// Mist prevents stat reductions from opponents.
        /// </summary>
        public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            if (!ActionValidators.ShouldExecute(field, Target) || Change == 0)
                return Enumerable.Empty<BattleAction>();

            // Use Stat Change Application Checker to validate if change can be applied (eliminates complex Mist logic)
            var statChangeChecker = _behaviorRegistry.GetStatChangeApplicationChecker();
            if (!statChangeChecker.CanApplyStatChange(Target, User, Change, field))
            {
                // Stat change blocked (e.g., by Mist)
                return Enumerable.Empty<BattleAction>();
            }

            Target.ModifyStatStage(Stat, Change);

            return Enumerable.Empty<BattleAction>();
        }

        /// <summary>
        /// Shows the stat change indicator to the player.
        /// </summary>
        public override Task ExecuteVisual(IBattleView view)
        {
            ActionValidators.ValidateView(view);

            if (!ActionValidators.ValidateTarget(Target) || Change == 0)
                return Task.CompletedTask;

            string statName = Stat.ToString();
            return view.ShowStatChange(Target, statName, Change);
        }
    }
}

