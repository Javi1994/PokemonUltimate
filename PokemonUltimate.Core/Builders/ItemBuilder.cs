using System;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Builders
{
    /// <summary>
    /// Fluent builder for creating ItemData.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.9: Builders
    /// **Documentation**: See `docs/features/3-content-expansion/3.9-builders/README.md`
    /// </remarks>
    public class ItemBuilder
    {
        private readonly string _id;
        private readonly string _name;
        private string _description = string.Empty;
        private ItemCategory _category = ItemCategory.HeldItem;
        private int _price;
        private bool _isHoldable = true;
        private bool _isConsumable;
        private ItemTrigger _triggers = ItemTrigger.None;
        private Stat? _targetStat;
        private float _statMultiplier = 1.0f;
        private int _healAmount;
        private float _hpThreshold;
        private PersistentStatus? _curesStatus;
        private PokemonType? _boostsType;
        private float _damageMultiplier = 1.0f;
        private float _recoilPercent;
        private bool _locksMove;
        private Stat? _berryFlavor;

        private ItemBuilder(string name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _id = name.ToLowerInvariant().Replace(" ", "-");
        }

        /// <summary>
        /// Start defining a new item.
        /// </summary>
        public static ItemBuilder Define(string name) => new ItemBuilder(name);

        #region Core Properties

        public ItemBuilder Description(string description)
        {
            _description = description;
            return this;
        }

        public ItemBuilder Category(ItemCategory category)
        {
            _category = category;
            return this;
        }

        public ItemBuilder Price(int price)
        {
            _price = price;
            return this;
        }

        public ItemBuilder Holdable(bool isHoldable = true)
        {
            _isHoldable = isHoldable;
            return this;
        }

        public ItemBuilder NotHoldable()
        {
            _isHoldable = false;
            return this;
        }

        public ItemBuilder Consumable(bool isConsumable = true)
        {
            _isConsumable = isConsumable;
            return this;
        }

        #endregion

        #region Triggers

        public ItemBuilder OnTrigger(ItemTrigger trigger)
        {
            _triggers |= trigger;
            return this;
        }

        public ItemBuilder OnTurnEnd()
        {
            _triggers |= ItemTrigger.OnTurnEnd;
            return this;
        }

        public ItemBuilder OnLowHP()
        {
            _triggers |= ItemTrigger.OnLowHP;
            return this;
        }

        public ItemBuilder OnWouldFaint()
        {
            _triggers |= ItemTrigger.OnWouldFaint;
            return this;
        }

        public ItemBuilder Passive()
        {
            _triggers |= ItemTrigger.Passive;
            return this;
        }

        #endregion

        #region Effects - Stat Boosts

        /// <summary>
        /// Boosts a stat by multiplier (Choice Band = Attack 1.5x).
        /// </summary>
        public ItemBuilder BoostsStat(Stat stat, float multiplier)
        {
            _targetStat = stat;
            _statMultiplier = multiplier;
            _triggers |= ItemTrigger.Passive;
            return this;
        }

        /// <summary>
        /// Choice item: boosts stat but locks to one move.
        /// </summary>
        public ItemBuilder ChoiceItem(Stat stat)
        {
            _targetStat = stat;
            _statMultiplier = 1.5f;
            _locksMove = true;
            _triggers |= ItemTrigger.Passive | ItemTrigger.OnMoveUsed;
            return this;
        }

        #endregion

        #region Effects - Damage

        /// <summary>
        /// Boosts damage but causes recoil (Life Orb).
        /// </summary>
        public ItemBuilder BoostsDamageWithRecoil(float damageMultiplier, float recoilPercent)
        {
            _damageMultiplier = damageMultiplier;
            _recoilPercent = recoilPercent;
            _triggers |= ItemTrigger.OnDamageDealt;
            return this;
        }

        /// <summary>
        /// Boosts moves of a specific type.
        /// </summary>
        public ItemBuilder BoostsType(PokemonType type, float multiplier = 1.2f)
        {
            _boostsType = type;
            _damageMultiplier = multiplier;
            _triggers |= ItemTrigger.Passive;
            return this;
        }

        #endregion

        #region Effects - Healing

        /// <summary>
        /// Heals at end of turn (Leftovers).
        /// </summary>
        public ItemBuilder HealsEachTurn(int amount)
        {
            _healAmount = amount;
            _triggers |= ItemTrigger.OnTurnEnd;
            return this;
        }

        /// <summary>
        /// Heals percentage of max HP each turn.
        /// </summary>
        public ItemBuilder HealsPercentEachTurn(float percent)
        {
            // Store as negative to indicate percentage
            _healAmount = -(int)(percent * 100);
            _triggers |= ItemTrigger.OnTurnEnd;
            return this;
        }

        /// <summary>
        /// Heals when HP is low (Sitrus Berry).
        /// </summary>
        public ItemBuilder HealsAtLowHP(int amount, float threshold = 0.5f)
        {
            _healAmount = amount;
            _hpThreshold = threshold;
            _isConsumable = true;
            _triggers |= ItemTrigger.OnLowHP;
            return this;
        }

        /// <summary>
        /// Heals percentage when HP is low.
        /// </summary>
        public ItemBuilder HealsPercentAtLowHP(float healPercent, float threshold = 0.25f)
        {
            _healAmount = -(int)(healPercent * 100);
            _hpThreshold = threshold;
            _isConsumable = true;
            _triggers |= ItemTrigger.OnLowHP;
            return this;
        }

        #endregion

        #region Effects - Survival

        /// <summary>
        /// Survives fatal hit at full HP (Focus Sash).
        /// </summary>
        public ItemBuilder SurvivesFatalHit()
        {
            _hpThreshold = 1.0f;
            _isConsumable = true;
            _triggers |= ItemTrigger.OnWouldFaint;
            return this;
        }

        #endregion

        #region Effects - Status

        /// <summary>
        /// Cures a specific status (Chesto Berry cures Sleep).
        /// </summary>
        public ItemBuilder CuresStatus(PersistentStatus status)
        {
            _curesStatus = status;
            _isConsumable = true;
            _triggers |= ItemTrigger.OnStatusApplied;
            return this;
        }

        /// <summary>
        /// Cures all statuses (Lum Berry).
        /// </summary>
        public ItemBuilder CuresAllStatuses()
        {
            // Use None to indicate "all"
            _curesStatus = PersistentStatus.None;
            _isConsumable = true;
            _triggers |= ItemTrigger.OnStatusApplied;
            return this;
        }

        #endregion

        #region Berry Configuration

        /// <summary>
        /// Marks this as a berry with a flavor (for confusion).
        /// </summary>
        public ItemBuilder Berry(Stat? flavorStat = null)
        {
            _category = ItemCategory.Berry;
            _isConsumable = true;
            _berryFlavor = flavorStat;
            return this;
        }

        #endregion

        #region Medicine Configuration

        /// <summary>
        /// Medicine that heals HP.
        /// </summary>
        public ItemBuilder Medicine(int healAmount)
        {
            _category = ItemCategory.Medicine;
            _isHoldable = false;
            _isConsumable = true;
            _healAmount = healAmount;
            return this;
        }

        /// <summary>
        /// Medicine that cures status.
        /// </summary>
        public ItemBuilder StatusMedicine(PersistentStatus cures)
        {
            _category = ItemCategory.StatusHeal;
            _isHoldable = false;
            _isConsumable = true;
            _curesStatus = cures;
            return this;
        }

        #endregion

        #region Build

        public ItemData Build()
        {
            return new ItemData(
                _id,
                _name,
                _description,
                _category,
                _price,
                _isHoldable,
                _isConsumable,
                _triggers,
                _targetStat,
                _statMultiplier,
                _healAmount,
                _hpThreshold,
                _curesStatus,
                _boostsType,
                _damageMultiplier,
                _recoilPercent,
                _locksMove,
                _berryFlavor);
        }

        #endregion
    }

    /// <summary>
    /// Alias for ItemBuilder for cleaner syntax in catalogs.
    /// </summary>
    public static class Item
    {
        public static ItemBuilder Define(string name) => ItemBuilder.Define(name);
    }
}

