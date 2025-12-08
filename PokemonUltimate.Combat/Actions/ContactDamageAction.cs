using System.Collections.Generic;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Extensions;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Combat.Integration.View;
using PokemonUltimate.Combat.Integration.View.Definition;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Localization.Services;
using PokemonUltimate.Core.Services;
using PokemonUltimate.Localization.Constants;

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
            Target = target ?? throw new System.ArgumentNullException(nameof(target), ErrorMessages.SlotCannotBeNull);
            Damage = damage;
            Source = source ?? "contact damage";
        }

        /// <summary>
        /// Applies contact damage to the target Pokemon.
        /// </summary>
        public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            if (field == null)
                throw new System.ArgumentNullException(nameof(field));

            // Check if target is valid (not empty and not fainted)
            // Note: Target is the attacker who made contact, and they receive the contact damage
            if (Target == null || Target.IsEmpty || Target.HasFainted)
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

