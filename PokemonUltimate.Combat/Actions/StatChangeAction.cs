using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        /// <exception cref="ArgumentNullException">If target is null.</exception>
        /// <exception cref="ArgumentException">If stat is HP.</exception>
        public StatChangeAction(BattleSlot user, BattleSlot target, Stat stat, int change) : base(user)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target), ErrorMessages.PokemonCannotBeNull);

            if (stat == Stat.HP)
                throw new ArgumentException(ErrorMessages.CannotModifyHPStatStage, nameof(stat));

            Stat = stat;
            Change = change;
        }

        /// <summary>
        /// Modifies the stat stage of the target Pokemon.
        /// Stages are automatically clamped to -6/+6.
        /// Mist prevents stat reductions from opponents.
        /// </summary>
        public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (Target.IsEmpty || Change == 0)
                return Enumerable.Empty<BattleAction>();

            // Mist prevents stat reductions from opponents (but allows stat increases)
            if (Change < 0 && Target.Side.HasSideCondition(SideCondition.Mist))
            {
                var mistData = Target.Side.GetSideConditionData(SideCondition.Mist);
                if (mistData != null && mistData.PreventsStatReduction)
                {
                    // Check if the stat reduction is from an opponent
                    // If User is null or User.Side != Target.Side, it's from an opponent
                    if (User == null || User.Side != Target.Side)
                    {
                        // Stat reduction is blocked by Mist
                        return Enumerable.Empty<BattleAction>();
                    }
                }
            }

            Target.ModifyStatStage(Stat, Change);

            return Enumerable.Empty<BattleAction>();
        }

        /// <summary>
        /// Shows the stat change indicator to the player.
        /// </summary>
        public override Task ExecuteVisual(IBattleView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            if (Target.IsEmpty || Change == 0)
                return Task.CompletedTask;

            string statName = Stat.ToString();
            return view.ShowStatChange(Target, statName, Change);
        }
    }
}

