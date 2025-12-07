using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Combat.Integration.View;
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
        /// <exception cref="ArgumentNullException">If target is null.</exception>
        /// <exception cref="ArgumentException">If amount is negative.</exception>
        public HealAction(BattleSlot user, BattleSlot target, int amount) : base(user)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target), ErrorMessages.PokemonCannotBeNull);

            if (amount < 0)
                throw new ArgumentException(ErrorMessages.AmountCannotBeNegative, nameof(amount));

            Amount = amount;
        }

        /// <summary>
        /// Restores HP to the target Pokemon.
        /// Prevents overhealing.
        /// </summary>
        public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (Target.IsEmpty || Amount == 0)
                return Enumerable.Empty<BattleAction>();

            Target.Pokemon.Heal(Amount);

            return Enumerable.Empty<BattleAction>();
        }

        /// <summary>
        /// Updates the HP bar to show the healing.
        /// </summary>
        public override Task ExecuteVisual(IBattleView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            if (Target.IsEmpty || Amount == 0)
                return Task.CompletedTask;

            return view.UpdateHPBar(Target);
        }
    }
}

