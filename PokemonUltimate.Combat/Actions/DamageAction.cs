using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Extensions;
using PokemonUltimate.Content.Catalogs.Abilities;
using PokemonUltimate.Content.Catalogs.Items;
using PokemonUltimate.Content.Extensions;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Extensions;
using PokemonUltimate.Core.Localization;

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

            if (!Target.IsActive())
                return Enumerable.Empty<BattleAction>();

            int damage = Context.FinalDamage;

            // No damage to apply (immune or status move)
            if (damage == 0)
                return Enumerable.Empty<BattleAction>();

            var reactions = new List<BattleAction>();

            // Check if damage would cause fainting BEFORE applying damage (for Focus Sash, Sturdy)
            bool wouldFaint = Target.Pokemon.CurrentHP <= damage;
            bool wasAtFullHP = Target.Pokemon.CurrentHP >= Target.Pokemon.MaxHP;

            // Trigger OnWouldFaint BEFORE applying damage if damage would be fatal
            // This allows items/abilities to prevent fainting (Focus Sash, Sturdy)
            // Items/abilities can modify the damage amount by checking conditions
            if (wouldFaint && wasAtFullHP)
            {
                // Check for Focus Sash or Sturdy - use catalog lookup instead of hardcoded strings
                var focusSashItem = PokemonUltimate.Content.Catalogs.Items.ItemCatalog.GetByName("Focus Sash");
                bool hasFocusSash = Target.Pokemon.HeldItem != null && focusSashItem != null &&
                                    Target.Pokemon.HeldItem.Name.Equals(focusSashItem.Name, StringComparison.OrdinalIgnoreCase);
                var sturdyAbility = PokemonUltimate.Content.Catalogs.Abilities.AbilityCatalog.GetByName("Sturdy");
                bool hasSturdy = Target.Pokemon.Ability != null && sturdyAbility != null &&
                                Target.Pokemon.Ability.Name.Equals(sturdyAbility.Name, StringComparison.OrdinalIgnoreCase);

                if (hasFocusSash || hasSturdy)
                {
                    // Reduce damage to leave Pokemon at 1 HP
                    int newDamage = Target.Pokemon.CurrentHP - 1;
                    if (newDamage < 0)
                        newDamage = 0;

                    damage = newDamage;

                    // Consume Focus Sash (Sturdy is an ability, so it doesn't get consumed)
                    if (hasFocusSash)
                    {
                        Target.Pokemon.HeldItem = null; // Consume item
                        var provider = PokemonUltimate.Core.Localization.LocalizationManager.Instance;
                        var itemName = focusSashItem?.GetLocalizedName(provider) ?? "Focus Sash";
                        reactions.Add(new MessageAction(provider.GetString(LocalizationKey.ItemActivated, Target.Pokemon.DisplayName, itemName)));
                        reactions.Add(new MessageAction(provider.GetString(LocalizationKey.HeldOnUsingItem, Target.Pokemon.DisplayName, itemName)));
                    }
                    else if (hasSturdy)
                    {
                        var provider = PokemonUltimate.Core.Localization.LocalizationManager.Instance;
                        var abilityName = sturdyAbility?.GetDisplayName(provider) ?? "Sturdy";
                        reactions.Add(new MessageAction(provider.GetString(LocalizationKey.AbilityActivated, Target.Pokemon.DisplayName, abilityName)));
                        reactions.Add(new MessageAction(provider.GetString(LocalizationKey.EnduredHit, Target.Pokemon.DisplayName)));
                    }
                }
            }

            // Apply damage (may have been modified by Focus Sash/Sturdy)
            int actualDamage = Target.Pokemon.TakeDamage(damage);

            // Record damage taken for Counter/Mirror Coat
            // Also mark if target was hit while focusing (for Focus Punch)
            // Note: Context.Move is never null (validated in DamageContext constructor),
            // but we check actualDamage > 0 to avoid recording zero damage
            if (actualDamage > 0)
            {
                if (Context.Move.Category == MoveCategory.Physical)
                {
                    Target.RecordPhysicalDamage(actualDamage);
                }
                else if (Context.Move.Category == MoveCategory.Special)
                {
                    Target.RecordSpecialDamage(actualDamage);
                }

                // Mark if target was hit while focusing
                Target.MarkHitWhileFocusing();
            }

            // Note: Damage-taken and contact-received effects are processed by ActionProcessorObserver
            // This keeps actions simple and decoupled from processors

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
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            if (!Target.IsActive() || Context.FinalDamage == 0)
                return Task.CompletedTask;

            return Task.WhenAll(
                view.PlayDamageAnimation(Target),
                view.UpdateHPBar(Target)
            );
        }
    }
}

