using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions.Validation;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Combat.View.Definition;

namespace PokemonUltimate.Combat.Actions
{
    /// <summary>
    /// Applies damage to a target Pokemon slot.
    /// Uses DamageContext to determine the final damage amount.
    /// Automatically triggers FaintAction if HP reaches zero.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class DamageAction : BattleAction
    {
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

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
        /// <param name="handlerRegistry">The handler registry. If null, creates and initializes a default one.</param>
        /// <exception cref="ArgumentNullException">If target or context is null.</exception>
        public DamageAction(BattleSlot user, BattleSlot target, DamageContext context, CombatEffectHandlerRegistry handlerRegistry = null) : base(user)
        {
            ActionValidators.ValidateTargetNotNull(target, nameof(target));
            ActionValidators.ValidateDamageContext(context, nameof(context));
            Target = target;
            Context = context;
            _handlerRegistry = handlerRegistry ?? CombatEffectHandlerRegistry.CreateDefault();
        }


        /// <summary>
        /// Applies damage to the target Pokemon.
        /// Returns FaintAction if HP reaches zero.
        /// </summary>
        public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            if (!ActionValidators.ShouldExecute(field, Target, checkActive: true))
                return Enumerable.Empty<BattleAction>();

            // Use Damage Application Handler to process all damage logic (OHKO prevention, messages, etc.)
            var damageHandler = _handlerRegistry.GetDamageApplicationHandler();
            var damageResult = damageHandler.ProcessDamageApplication(Target, Context.FinalDamage, field);

            // No damage to apply (immune or status move)
            if (damageResult.ModifiedDamage == 0)
                return Enumerable.Empty<BattleAction>();

            var reactions = new List<BattleAction>(damageResult.Reactions);

            // Apply damage (may have been modified by Focus Sash/Sturdy)
            int actualDamage = Target.Pokemon.TakeDamage(damageResult.ModifiedDamage);

            // Record damage taken for Counter/Mirror Coat and Focus Punch (using handler)
            // Note: Context.Move is never null (validated in DamageContext constructor)
            damageHandler.RecordDamageTaken(Target, actualDamage, Context.Move.Category);

            // Note: Damage-taken and contact-received effects are processed by DamageTakenEffectsStep and ContactReceivedEffectsStep
            // This keeps actions simple and decoupled from reactive effect processing

            // Check if Pokemon fainted (after OnWouldFaint triggers may have prevented it)
            if (Target.Pokemon.IsFainted)
            {
                reactions.Add(new FaintAction(User, Target));
            }

            return reactions;
        }

        /// <summary>
        /// Plays damage animation and updates HP bar.
        /// </summary>
        public override Task ExecuteVisual(IBattleView view)
        {
            ActionValidators.ValidateView(view);

            if (!ActionValidators.ValidateActiveTarget(Target) || Context.FinalDamage == 0)
                return Task.CompletedTask;

            return Task.WhenAll(
                view.PlayDamageAnimation(Target),
                view.UpdateHPBar(Target)
            );
        }
    }
}

