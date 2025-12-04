using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Combat.Actions
{
    /// <summary>
    /// Applies a persistent status condition to a Pokemon.
    /// Can also clear status by applying PersistentStatus.None.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class ApplyStatusAction : BattleAction
    {
        /// <summary>
        /// The slot receiving the status condition.
        /// </summary>
        public BattleSlot Target { get; }

        /// <summary>
        /// The status condition to apply.
        /// </summary>
        public PersistentStatus Status { get; }

        /// <summary>
        /// Creates a new apply status action.
        /// </summary>
        /// <param name="user">The slot that initiated this status application. Can be null for system actions.</param>
        /// <param name="target">The slot receiving the status. Cannot be null.</param>
        /// <param name="status">The status condition to apply.</param>
        /// <exception cref="ArgumentNullException">If target is null.</exception>
        public ApplyStatusAction(BattleSlot user, BattleSlot target, PersistentStatus status) : base(user)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target), ErrorMessages.PokemonCannotBeNull);
            Status = status;
        }

        /// <summary>
        /// Applies the status condition to the target Pokemon.
        /// If status is None, clears any existing status.
        /// Safeguard prevents status application (except for clearing status).
        /// </summary>
        public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (Target.IsEmpty)
                return Enumerable.Empty<BattleAction>();

            // Safeguard prevents status application (but allows clearing status)
            if (Status != PersistentStatus.None && Target.Side.HasSideCondition(SideCondition.Safeguard))
            {
                var safeguardData = Target.Side.GetSideConditionData(SideCondition.Safeguard);
                if (safeguardData != null && safeguardData.PreventsStatus)
                {
                    // Status is blocked by Safeguard
                    return Enumerable.Empty<BattleAction>();
                }
            }

            Target.Pokemon.Status = Status;

            return Enumerable.Empty<BattleAction>();
        }

        /// <summary>
        /// Plays the status application animation.
        /// Skips animation if status is None (clearing status).
        /// </summary>
        public override Task ExecuteVisual(IBattleView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            if (Target.IsEmpty || Status == PersistentStatus.None)
                return Task.CompletedTask;

            string statusName = Status.ToString();
            return view.PlayStatusAnimation(Target, statusName);
        }
    }
}

