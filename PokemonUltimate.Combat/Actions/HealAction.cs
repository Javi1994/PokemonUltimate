using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions.Validation;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Combat.View.Definition;
using PokemonUltimate.Core.Data.Constants;

namespace PokemonUltimate.Combat.Actions
{
    /// <summary>
    /// Restores HP to a target Pokemon.
    /// Prevents overhealing (HP cannot exceed MaxHP).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class HealAction : BattleAction
    {
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        /// <summary>
        /// The slot receiving healing.
        /// </summary>
        public BattleSlot Target { get; }

        /// <summary>
        /// The amount of HP to restore.
        /// </summary>
        public int Amount { get; }

        /// <summary>
        /// Creates a new heal action.
        /// </summary>
        /// <param name="user">The slot that initiated this healing. Can be null for system actions.</param>
        /// <param name="target">The slot receiving healing. Cannot be null.</param>
        /// <param name="amount">The amount of HP to restore. Must be non-negative.</param>
        /// <param name="handlerRegistry">The handler registry. If null, creates and initializes a default one.</param>
        /// <exception cref="ArgumentNullException">If target is null.</exception>
        /// <exception cref="ArgumentException">If amount is negative.</exception>
        public HealAction(BattleSlot user, BattleSlot target, int amount, CombatEffectHandlerRegistry handlerRegistry = null) : base(user)
        {
            ActionValidators.ValidateTargetNotNull(target, nameof(target));

            if (amount < 0)
                throw new ArgumentException(ErrorMessages.AmountCannotBeNegative, nameof(amount));

            Target = target;
            Amount = amount;
            _handlerRegistry = handlerRegistry ?? CombatEffectHandlerRegistry.CreateDefault();
        }

        /// <summary>
        /// Restores HP to the target Pokemon.
        /// Prevents overhealing.
        /// </summary>
        public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            if (!ActionValidators.ShouldExecute(field, Target))
                return Enumerable.Empty<BattleAction>();

            // Use Healing Application Handler to validate and calculate effective healing
            var healingHandler = _handlerRegistry.GetHealingApplicationHandler();

            if (!healingHandler.CanHeal(Target.Pokemon, Amount))
                return Enumerable.Empty<BattleAction>();

            // Calculate effective healing (prevents overhealing)
            int effectiveHealing = healingHandler.CalculateEffectiveHealing(Target.Pokemon, Amount);

            if (effectiveHealing > 0)
            {
                Target.Pokemon.Heal(effectiveHealing);
            }

            return Enumerable.Empty<BattleAction>();
        }

        /// <summary>
        /// Updates the HP bar to show the healing.
        /// </summary>
        public override Task ExecuteVisual(IBattleView view)
        {
            ActionValidators.ValidateView(view);

            if (!ActionValidators.ValidateTarget(Target))
                return Task.CompletedTask;

            // Use Healing Application Handler to check if healing would be effective
            var healingHandler = _handlerRegistry.GetHealingApplicationHandler();

            if (!healingHandler.CanHeal(Target.Pokemon, Amount))
                return Task.CompletedTask;

            return view.UpdateHPBar(Target);
        }
    }
}

