using System;
using PokemonUltimate.Core.Data.Enums;


namespace PokemonUltimate.Core.Data.Blueprints
{
    /// <summary>
    /// Immutable data defining an item.
    /// Contains all the information needed to determine how an item behaves.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.4: Item Data
    /// **Documentation**: See `docs/features/1-game-data/1.4-item-data/README.md`
    /// </remarks>
    public sealed class ItemData
    {
        #region Core Properties

        /// <summary>
        /// Internal identifier for the item.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Display name of the item.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Description of what the item does.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The category of this item.
        /// </summary>
        public ItemCategory Category { get; }

        /// <summary>
        /// Price in PokeDollars (0 if not sellable).
        /// </summary>
        public int Price { get; }

        /// <summary>
        /// Whether this item can be held by Pokemon.
        /// </summary>
        public bool IsHoldable { get; }

        /// <summary>
        /// Whether this item is consumed when used/activated.
        /// </summary>
        public bool IsConsumable { get; }

        #endregion

        #region Battle Properties

        /// <summary>
        /// When this item triggers in battle (for held items).
        /// </summary>
        public ItemTrigger Triggers { get; }

        /// <summary>
        /// Whether this item provides passive stat modifications.
        /// </summary>
        public bool HasPassiveEffect => (Triggers & ItemTrigger.Passive) != 0;

        #endregion

        #region Effect Parameters

        /// <summary>
        /// Stat this item affects.
        /// </summary>
        public Stat? TargetStat { get; }

        /// <summary>
        /// Multiplier for stat boosts (Choice Band = 1.5).
        /// </summary>
        public float StatMultiplier { get; }

        /// <summary>
        /// HP heal amount or percentage.
        /// </summary>
        public int HealAmount { get; }

        /// <summary>
        /// HP threshold to trigger (as percentage 0-1).
        /// </summary>
        public float HPThreshold { get; }

        /// <summary>
        /// Status this item cures.
        /// </summary>
        public PersistentStatus? CuresStatus { get; }

        /// <summary>
        /// Type this item boosts (Type-enhancing items).
        /// </summary>
        public PokemonType? BoostsType { get; }

        /// <summary>
        /// Damage multiplier (Life Orb = 1.3).
        /// </summary>
        public float DamageMultiplier { get; }

        /// <summary>
        /// Recoil damage as percentage of damage dealt (Life Orb = 0.1).
        /// </summary>
        public float RecoilPercent { get; }

        /// <summary>
        /// Whether this item locks the user to one move (Choice items).
        /// </summary>
        public bool LocksMove { get; }

        #endregion

        #region Berry Properties

        /// <summary>
        /// Whether this is a berry.
        /// </summary>
        public bool IsBerry => Category == ItemCategory.Berry;

        /// <summary>
        /// Berry flavor for confusion calculation.
        /// </summary>
        public Stat? BerryFlavor { get; }

        #endregion

        #region Constructor

        internal ItemData(
            string id,
            string name,
            string description,
            ItemCategory category,
            int price,
            bool isHoldable,
            bool isConsumable,
            ItemTrigger triggers,
            Stat? targetStat,
            float statMultiplier,
            int healAmount,
            float hpThreshold,
            PersistentStatus? curesStatus,
            PokemonType? boostsType,
            float damageMultiplier,
            float recoilPercent,
            bool locksMove,
            Stat? berryFlavor)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? string.Empty;
            Category = category;
            Price = price;
            IsHoldable = isHoldable;
            IsConsumable = isConsumable;
            Triggers = triggers;
            TargetStat = targetStat;
            StatMultiplier = statMultiplier;
            HealAmount = healAmount;
            HPThreshold = hpThreshold;
            CuresStatus = curesStatus;
            BoostsType = boostsType;
            DamageMultiplier = damageMultiplier;
            RecoilPercent = recoilPercent;
            LocksMove = locksMove;
            BerryFlavor = berryFlavor;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Checks if this item listens to a specific trigger.
        /// </summary>
        public bool ListensTo(ItemTrigger trigger)
        {
            return (Triggers & trigger) != 0;
        }

        public override string ToString() => Name;

        #endregion
    }
}

