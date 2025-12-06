using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Constants;
using PokemonUltimate.Combat.Extensions;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;
using ContactDamageAction = PokemonUltimate.Combat.Actions.ContactDamageAction;

namespace PokemonUltimate.Combat.Events
{
    /// <summary>
    /// Adapts ItemData to IBattleListener interface.
    /// Converts item triggers to battle actions based on item configuration.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.9: Abilities & Items
    /// **Documentation**: See `docs/features/2-combat-system/2.9-abilities-items/architecture.md`
    /// </remarks>
    public class ItemListener : IBattleListener
    {
        private readonly ItemData _itemData;

        // Dictionary mapping BattleTrigger to ItemTrigger for efficient lookup
        private static readonly Dictionary<BattleTrigger, Core.Enums.ItemTrigger?> TriggerMapping = new Dictionary<BattleTrigger, Core.Enums.ItemTrigger?>
        {
            { BattleTrigger.OnTurnEnd, Core.Enums.ItemTrigger.OnTurnEnd },
            { BattleTrigger.OnDamageTaken, Core.Enums.ItemTrigger.OnContactReceived },
            { BattleTrigger.OnAfterMove, Core.Enums.ItemTrigger.OnDamageDealt },
            { BattleTrigger.OnWeatherChange, null }, // Items don't respond to weather changes
            { BattleTrigger.OnSwitchIn, null }, // Items don't trigger on switch-in
            { BattleTrigger.OnBeforeMove, null }, // Items don't trigger before move (except Choice items, deferred)
            { BattleTrigger.OnContactReceived, Core.Enums.ItemTrigger.OnContactReceived },
            { BattleTrigger.OnWouldFaint, Core.Enums.ItemTrigger.OnWouldFaint }
        };

        /// <summary>
        /// Creates a new item listener for the given item data.
        /// </summary>
        /// <param name="itemData">The item data to listen with. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If itemData is null.</exception>
        public ItemListener(ItemData itemData)
        {
            _itemData = itemData ?? throw new ArgumentNullException(nameof(itemData), ErrorMessages.ItemCannotBeNull);
        }

        /// <summary>
        /// Called when a battle trigger occurs.
        /// Returns actions to be enqueued and processed.
        /// </summary>
        public IEnumerable<BattleAction> OnTrigger(BattleTrigger trigger, BattleSlot holder, BattleField field)
        {
            return OnTrigger(trigger, holder, field, attacker: null);
        }

        /// <summary>
        /// Called when a battle trigger occurs with additional context (e.g., attacker for contact moves).
        /// Returns actions to be enqueued and processed.
        /// </summary>
        public IEnumerable<BattleAction> OnTrigger(BattleTrigger trigger, BattleSlot holder, BattleField field, BattleSlot attacker)
        {
            if (holder == null)
                throw new ArgumentNullException(nameof(holder), ErrorMessages.SlotCannotBeNull);
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            // Check if this item listens to this trigger
            if (!ShouldRespondToTrigger(trigger))
                yield break;

            // Process based on item type
            foreach (var action in ProcessItemEffect(trigger, holder, field, attacker))
            {
                yield return action;
            }
        }

        /// <summary>
        /// Checks if this item should respond to the given trigger.
        /// Uses dictionary lookup for efficient trigger mapping.
        /// </summary>
        private bool ShouldRespondToTrigger(BattleTrigger trigger)
        {
            if (TriggerMapping.TryGetValue(trigger, out var itemTrigger))
            {
                // If mapping is null, item doesn't respond to this trigger
                if (itemTrigger == null)
                    return false;

                return _itemData.ListensTo(itemTrigger.Value);
            }

            return false;
        }

        /// <summary>
        /// Processes item effects and returns actions.
        /// </summary>
        private IEnumerable<BattleAction> ProcessItemEffect(BattleTrigger trigger, BattleSlot holder, BattleField field, BattleSlot attacker = null)
        {
            // End-of-turn healing (Leftovers, Black Sludge)
            if (trigger == BattleTrigger.OnTurnEnd && _itemData.ListensTo(ItemTrigger.OnTurnEnd))
            {
                foreach (var action in ProcessEndOfTurnHealing(holder))
                    yield return action;
            }

            // Contact damage (Rocky Helmet)
            if (trigger == BattleTrigger.OnContactReceived && attacker != null && _itemData.ListensTo(ItemTrigger.OnContactReceived))
            {
                foreach (var action in ProcessContactDamage(holder, attacker, field))
                    yield return action;
            }

            // Life Orb recoil (OnAfterMove when damage was dealt)
            // Life Orb only causes recoil if damage was actually dealt
            // We check by verifying the move used was a damaging move (not status-only)
            if (trigger == BattleTrigger.OnAfterMove && _itemData.ListensTo(ItemTrigger.OnDamageDealt))
            {
                // Check if holder's last action was a damaging move
                // For now, we'll process Life Orb - the actual damage check happens in ProcessLifeOrbRecoil
                // In a real implementation, we'd track damage dealt per turn
                foreach (var action in ProcessLifeOrbRecoil(holder, field))
                    yield return action;
            }

            // Focus Sash (OnWouldFaint)
            if (trigger == BattleTrigger.OnWouldFaint && _itemData.ListensTo(ItemTrigger.OnWouldFaint))
            {
                foreach (var action in ProcessFocusSash(holder, field))
                    yield return action;
            }
        }

        /// <summary>
        /// Processes end-of-turn healing items (Leftovers, Black Sludge).
        /// </summary>
        private IEnumerable<BattleAction> ProcessEndOfTurnHealing(BattleSlot holder)
        {
            var pokemon = holder.Pokemon;

            // Special handling for Black Sludge
            bool isBlackSludge = _itemData.Name.Equals("Black Sludge", StringComparison.OrdinalIgnoreCase);
            bool isPoisonType = pokemon.Species.PrimaryType == PokemonType.Poison || 
                               pokemon.Species.SecondaryType == PokemonType.Poison;

            if (isBlackSludge)
            {
                // Black Sludge: heals Poison types, damages others
                if (isPoisonType)
                {
                    // Heal Poison types (same as Leftovers)
                    if (pokemon.CurrentHP >= pokemon.MaxHP)
                        yield break;

                    int blackSludgeHeal = pokemon.MaxHP / ItemConstants.LeftoversHealDivisor;
                    if (blackSludgeHeal < 1)
                        blackSludgeHeal = 1;

                    yield return new MessageAction(string.Format(GameMessages.ItemActivated, pokemon.DisplayName, _itemData.Name));
                    yield return new HealAction(holder, holder, blackSludgeHeal);
                }
                else
                {
                    // Damage non-Poison types
                    int damageAmount = pokemon.MaxHP / ItemConstants.LeftoversHealDivisor;
                    if (damageAmount < 1)
                        damageAmount = 1;

                    yield return new MessageAction(string.Format(GameMessages.ItemActivated, pokemon.DisplayName, _itemData.Name));
                    
                    // Directly damage the Pokemon
                    pokemon.TakeDamage(damageAmount);
                    yield return new MessageAction(string.Format("{0} was hurt by {1}!", pokemon.DisplayName, _itemData.Name));
                }
                yield break;
            }

            // Standard healing items (Leftovers, etc.)
            // Don't heal if already at full HP
            if (pokemon.CurrentHP >= pokemon.MaxHP)
                yield break;

            // Calculate heal amount
            int standardHealAmount;
            if (_itemData.HealAmount > 0)
            {
                // Fixed amount healing
                standardHealAmount = _itemData.HealAmount;
            }
            else if (_itemData.HealAmount < 0)
            {
                // Percentage-based healing (stored as negative value)
                // HealAmount = -(percent * 100), so percent = -HealAmount / 100
                float percent = -_itemData.HealAmount / 100f;
                standardHealAmount = (int)(pokemon.MaxHP * percent);
            }
            else
            {
                // Default: Leftovers-style healing (1/16)
                standardHealAmount = pokemon.MaxHP / ItemConstants.LeftoversHealDivisor;
            }

            // Minimum 1 HP healed
            if (standardHealAmount < 1)
                standardHealAmount = 1;

            // Message
            yield return new MessageAction(string.Format(GameMessages.ItemActivated, pokemon.DisplayName, _itemData.Name));

            // Healing action
            yield return new HealAction(holder, holder, standardHealAmount);
        }

        /// <summary>
        /// Processes contact damage items (Rocky Helmet).
        /// </summary>
        private IEnumerable<BattleAction> ProcessContactDamage(BattleSlot holder, BattleSlot attacker, BattleField field)
        {
            if (attacker == null || attacker.IsEmpty || attacker.HasFainted)
                yield break;

            // Rocky Helmet deals 1/6 of attacker's max HP
            int damage = attacker.Pokemon.MaxHP / 6;
            if (damage < 1)
                damage = 1;

            // Message for item activation
            yield return new MessageAction(string.Format(GameMessages.ItemActivated, holder.Pokemon.DisplayName, _itemData.Name));

            // Apply damage using ContactDamageAction (executes when action is executed, not when it's created)
            yield return new ContactDamageAction(attacker, damage, _itemData.Name);
        }

        /// <summary>
        /// Processes Life Orb recoil (10% of max HP after dealing damage).
        /// Life Orb only causes recoil if damage was actually dealt this turn.
        /// Note: This is called from OnAfterMove, which happens after ProcessEffects in UseMoveAction.
        /// At this point, DamageActions have been created but not yet executed.
        /// We check if any opponent received damage by checking their damage trackers.
        /// </summary>
        private IEnumerable<BattleAction> ProcessLifeOrbRecoil(BattleSlot holder, BattleField field)
        {
            var pokemon = holder.Pokemon;

            // Check if any opponent received damage this turn
            // Life Orb only causes recoil if damage was dealt
            var opposingSide = field.GetOppositeSide(holder.Side);
            bool damageWasDealt = false;
            
            foreach (var enemySlot in opposingSide.GetActiveSlots())
            {
                if (!enemySlot.IsEmpty && (enemySlot.PhysicalDamageTakenThisTurn > 0 || enemySlot.SpecialDamageTakenThisTurn > 0))
                {
                    damageWasDealt = true;
                    break;
                }
            }
            
            // If no damage was dealt, don't cause recoil
            // This handles status moves and moves that missed/immune
            if (!damageWasDealt)
                yield break;
            
            // Life Orb recoil is 10% of max HP
            int recoilDamage = pokemon.MaxHP / 10;
            if (recoilDamage < 1)
                recoilDamage = 1;

            // Message for item activation
            yield return new MessageAction(string.Format(GameMessages.ItemActivated, pokemon.DisplayName, _itemData.Name));

            // Directly damage the holder (recoil)
            pokemon.TakeDamage(recoilDamage);
            
            // Return message about recoil damage
            yield return new MessageAction(string.Format("{0} was hurt by recoil!", pokemon.DisplayName));
        }

        /// <summary>
        /// Processes Focus Sash (prevents fainting if at full HP, leaves at 1 HP).
        /// </summary>
        private IEnumerable<BattleAction> ProcessFocusSash(BattleSlot holder, BattleField field)
        {
            var pokemon = holder.Pokemon;

            // Focus Sash only works if Pokemon was at full HP
            // Check if Pokemon is about to faint (HP <= damage that would be fatal)
            // This is called BEFORE damage is applied, so we need to check current HP
            // For now, we'll check if HP is at max (was at full HP before damage)
            // TODO: This needs to be called BEFORE damage is applied, which requires refactoring DamageAction
            
            // Focus Sash prevents fainting and leaves Pokemon at 1 HP
            // We need to modify the damage before it's applied
            // For now, this is a placeholder - the actual implementation requires modifying DamageAction
            // to allow items/abilities to intercept and modify damage before application
            
            yield break; // Placeholder - requires damage interception system
        }
    }
}

