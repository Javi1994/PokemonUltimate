using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Infrastructure.Builders;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Content.Catalogs.Items
{
    /// <summary>
    /// Held items that provide passive or triggered effects.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.3: Item Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.3-item-expansion/README.md`
    /// </remarks>
    public static partial class ItemCatalog
    {
        // ===== RECOVERY ITEMS =====

        /// <summary>
        /// Restores 1/16 max HP at end of each turn.
        /// </summary>
        public static readonly ItemData Leftovers = Item.Define("Leftovers")
            .Description("Restores HP each turn.")
            .Category(ItemCategory.HeldItem)
            .Price(200)
            .HealsPercentEachTurn(0.0625f) // 1/16 = 6.25%
            .Build();

        /// <summary>
        /// Restores 1/16 max HP if Poison type, damages otherwise.
        /// </summary>
        public static readonly ItemData BlackSludge = Item.Define("Black Sludge")
            .Description("Restores HP for Poison types; damages others.")
            .Category(ItemCategory.HeldItem)
            .Price(200)
            .HealsPercentEachTurn(0.0625f)
            .Build();

        // ===== CHOICE ITEMS =====

        /// <summary>
        /// Boosts Attack by 50% but locks to one move.
        /// </summary>
        public static readonly ItemData ChoiceBand = Item.Define("Choice Band")
            .Description("Boosts Attack but locks to one move.")
            .Category(ItemCategory.HeldItem)
            .Price(100)
            .ChoiceItem(Stat.Attack)
            .Build();

        /// <summary>
        /// Boosts Special Attack by 50% but locks to one move.
        /// </summary>
        public static readonly ItemData ChoiceSpecs = Item.Define("Choice Specs")
            .Description("Boosts Sp. Attack but locks to one move.")
            .Category(ItemCategory.HeldItem)
            .Price(100)
            .ChoiceItem(Stat.SpAttack)
            .Build();

        /// <summary>
        /// Boosts Speed by 50% but locks to one move.
        /// </summary>
        public static readonly ItemData ChoiceScarf = Item.Define("Choice Scarf")
            .Description("Boosts Speed but locks to one move.")
            .Category(ItemCategory.HeldItem)
            .Price(100)
            .ChoiceItem(Stat.Speed)
            .Build();

        // ===== DAMAGE BOOSTING ITEMS =====

        /// <summary>
        /// Boosts damage by 30% but causes 10% recoil.
        /// </summary>
        public static readonly ItemData LifeOrb = Item.Define("Life Orb")
            .Description("Boosts damage but causes recoil.")
            .Category(ItemCategory.HeldItem)
            .Price(200)
            .BoostsDamageWithRecoil(1.3f, 0.1f)
            .Build();

        /// <summary>
        /// Boosts super effective moves by 20%.
        /// </summary>
        public static readonly ItemData ExpertBelt = Item.Define("Expert Belt")
            .Description("Boosts super effective moves.")
            .Category(ItemCategory.HeldItem)
            .Price(200)
            .OnTrigger(ItemTrigger.Passive)
            .Build();

        // ===== TYPE-BOOSTING ITEMS =====

        /// <summary>
        /// Boosts Fire moves by 20%.
        /// </summary>
        public static readonly ItemData Charcoal = Item.Define("Charcoal")
            .Description("Boosts Fire-type moves.")
            .Category(ItemCategory.HeldItem)
            .Price(100)
            .BoostsType(PokemonType.Fire, 1.2f)
            .Build();

        /// <summary>
        /// Boosts Water moves by 20%.
        /// </summary>
        public static readonly ItemData MysticWater = Item.Define("Mystic Water")
            .Description("Boosts Water-type moves.")
            .Category(ItemCategory.HeldItem)
            .Price(100)
            .BoostsType(PokemonType.Water, 1.2f)
            .Build();

        /// <summary>
        /// Boosts Electric moves by 20%.
        /// </summary>
        public static readonly ItemData Magnet = Item.Define("Magnet")
            .Description("Boosts Electric-type moves.")
            .Category(ItemCategory.HeldItem)
            .Price(100)
            .BoostsType(PokemonType.Electric, 1.2f)
            .Build();

        /// <summary>
        /// Boosts Grass moves by 20%.
        /// </summary>
        public static readonly ItemData MiracleSeed = Item.Define("Miracle Seed")
            .Description("Boosts Grass-type moves.")
            .Category(ItemCategory.HeldItem)
            .Price(100)
            .BoostsType(PokemonType.Grass, 1.2f)
            .Build();

        // ===== SURVIVAL ITEMS =====

        /// <summary>
        /// Survives a fatal hit at full HP. Consumed on use.
        /// </summary>
        public static readonly ItemData FocusSash = Item.Define("Focus Sash")
            .Description("Survives one fatal hit at full HP.")
            .Category(ItemCategory.HeldItem)
            .Price(200)
            .SurvivesFatalHit()
            .Build();

        // ===== DEFENSIVE ITEMS =====

        /// <summary>
        /// Boosts Defense and Special Defense by 50% if holder can still evolve.
        /// </summary>
        public static readonly ItemData Eviolite = Item.Define("Eviolite")
            .Description("Boosts defenses of unevolved Pokémon.")
            .Category(ItemCategory.HeldItem)
            .Price(200)
            .Passive()
            .Build();

        /// <summary>
        /// Boosts Special Defense by 50% but prevents status moves.
        /// </summary>
        public static readonly ItemData AssaultVest = Item.Define("Assault Vest")
            .Description("Boosts Sp. Def but prevents status moves.")
            .Category(ItemCategory.HeldItem)
            .Price(200)
            .BoostsStat(Stat.SpDefense, 1.5f)
            .Build();

        /// <summary>
        /// Damages physical attackers on contact.
        /// </summary>
        public static readonly ItemData RockyHelmet = Item.Define("Rocky Helmet")
            .Description("Damages attackers on contact.")
            .Category(ItemCategory.HeldItem)
            .Price(200)
            .OnTrigger(ItemTrigger.OnContactReceived)
            .Build();

        // ===== REGISTRATION =====

        static partial void RegisterHeldItems()
        {
            _all.Add(Leftovers);
            _all.Add(BlackSludge);
            _all.Add(ChoiceBand);
            _all.Add(ChoiceSpecs);
            _all.Add(ChoiceScarf);
            _all.Add(LifeOrb);
            _all.Add(ExpertBelt);
            _all.Add(Charcoal);
            _all.Add(MysticWater);
            _all.Add(Magnet);
            _all.Add(MiracleSeed);
            _all.Add(FocusSash);
            _all.Add(Eviolite);
            _all.Add(AssaultVest);
            _all.Add(RockyHelmet);
        }
    }
}

