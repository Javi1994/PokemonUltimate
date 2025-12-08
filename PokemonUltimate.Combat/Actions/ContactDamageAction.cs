using System.Collections.Generic;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions.Validation;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.View.Definition;
using PokemonUltimate.Localization.Constants;
using PokemonUltimate.Localization.Services;

namespace PokemonUltimate.Combat.Actions
{
    /// <summary>
    /// Applies contact damage directly to a Pokemon (for abilities like Rough Skin, Iron Barbs, Rocky Helmet).
    /// This bypasses the normal damage pipeline.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.17: Advanced Abilities / 2.18: Advanced Items
    /// **Documentation**: See `docs/features/2-combat-system/2.17-advanced-abilities/README.md`
    /// </remarks>
    public class ContactDamageAction : BattleAction
    {
        /// <summary>
        /// The slot receiving contact damage.
        /// </summary>
        public BattleSlot Target { get; }

        /// <summary>
        /// The amount of damage to apply.
        /// </summary>
        public int Damage { get; }

        /// <summary>
        /// The source of the damage (ability/item name for messages).
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Creates a new contact damage action.
        /// </summary>
        /// <param name="target">The slot receiving damage. Cannot be null.</param>
        /// <param name="damage">The amount of damage to apply.</param>
        /// <param name="source">The source of the damage (ability/item name).</param>
        public ContactDamageAction(BattleSlot target, int damage, string source) : base(null)
        {
            ActionValidators.ValidateSlot(target, nameof(target));
            Target = target;
            Damage = damage;
            Source = source ?? "contact damage";
        }

        /// <summary>
        /// Applies contact damage to the target Pokemon.
        /// </summary>
        public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            ActionValidators.ValidateField(field);

            // Check if target is valid (not empty and not fainted)
            // Note: Target is the attacker who made contact, and they receive the contact damage
            if (!ActionValidators.ValidateTarget(Target) || Target.HasFainted)
                yield break;

            // Apply damage directly
            Target.Pokemon.TakeDamage(Damage);

            // Return message about damage dealt
            var provider = LocalizationService.Instance;
            yield return new MessageAction(provider.GetString(LocalizationKey.HurtByContact, Target.Pokemon.DisplayName, Source));
        }

        /// <summary>
        /// Visual execution (no-op for contact damage).
        /// </summary>
        public override Task ExecuteVisual(IBattleView view)
        {
            return Task.CompletedTask;
        }
    }
}

