using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Core.Constants;

namespace PokemonUltimate.Combat.Actions
{
    /// <summary>
    /// Applies damage to a target Pokemon slot.
    /// Uses DamageContext to determine the final damage amount.
    /// Automatically triggers FaintAction if HP reaches zero.
    /// </summary>
    public class DamageAction : BattleAction
    {
        /// <summary>
        /// The slot receiving damage.
        /// </summary>
        public BattleSlot Target { get; }

        /// <summary>
        /// The damage context containing calculated damage.
        /// </summary>
        public DamageContext Context { get; }

        /// <summary>
        /// Creates a new damage action.
        /// </summary>
        /// <param name="user">The slot that initiated this damage (attacker).</param>
        /// <param name="target">The slot receiving damage. Cannot be null.</param>
        /// <param name="context">The damage context with calculated damage. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If target or context is null.</exception>
        public DamageAction(BattleSlot user, BattleSlot target, DamageContext context) : base(user)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target), ErrorMessages.PokemonCannotBeNull);
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Applies damage to the target Pokemon.
        /// Returns FaintAction if HP reaches zero.
        /// </summary>
        public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (Target.IsEmpty || Target.HasFainted)
                return Enumerable.Empty<BattleAction>();

            int damage = Context.FinalDamage;

            // No damage to apply (immune or status move)
            if (damage == 0)
                return Enumerable.Empty<BattleAction>();

            // Apply damage
            int actualDamage = Target.Pokemon.TakeDamage(damage);

            // Check if Pokemon fainted
            if (Target.Pokemon.IsFainted)
            {
                return new[] { new FaintAction(User, Target) };
            }

            return Enumerable.Empty<BattleAction>();
        }

        /// <summary>
        /// Plays damage animation and updates HP bar.
        /// </summary>
        public override Task ExecuteVisual(IBattleView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            if (Target.IsEmpty || Target.HasFainted || Context.FinalDamage == 0)
                return Task.CompletedTask;

            return Task.WhenAll(
                view.PlayDamageAnimation(Target),
                view.UpdateHPBar(Target)
            );
        }
    }
}

