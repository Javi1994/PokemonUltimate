using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions.Validation;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Combat.View.Definition;
using PokemonUltimate.Core.Data.Enums;

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
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

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
        /// <param name="handlerRegistry">The handler registry. If null, creates and initializes a default one.</param>
        /// <exception cref="ArgumentNullException">If target is null.</exception>
        public ApplyStatusAction(BattleSlot user, BattleSlot target, PersistentStatus status, CombatEffectHandlerRegistry handlerRegistry = null) : base(user)
        {
            ActionValidators.ValidateTargetNotNull(target, nameof(target));
            Target = target;
            Status = status;
            _handlerRegistry = handlerRegistry ?? CombatEffectHandlerRegistry.CreateDefault();
        }

        /// <summary>
        /// Applies the status condition to the target Pokemon.
        /// If status is None, clears any existing status.
        /// Safeguard prevents status application (except for clearing status).
        /// </summary>
        public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            if (!ActionValidators.ShouldExecute(field, Target))
                return Enumerable.Empty<BattleAction>();

            // Use Status Application Handler to validate and apply status (eliminates complex validation logic)
            var statusHandler = _handlerRegistry.GetStatusApplicationHandler();
            var result = statusHandler.CanApplyStatus(Target, Status, field);

            if (!result.CanApply)
            {
                // Status cannot be applied (already has status, immune, Safeguard, etc.)
                return Enumerable.Empty<BattleAction>();
            }

            // Apply the status
            Target.Pokemon.Status = Status;

            return Enumerable.Empty<BattleAction>();
        }

        /// <summary>
        /// Plays the status application animation.
        /// Skips animation if status is None (clearing status).
        /// </summary>
        public override Task ExecuteVisual(IBattleView view)
        {
            ActionValidators.ValidateView(view);

            if (!ActionValidators.ValidateTarget(Target) || Status == PersistentStatus.None)
                return Task.CompletedTask;

            string statusName = Status.ToString();
            return view.PlayStatusAnimation(Target, statusName);
        }
    }
}

