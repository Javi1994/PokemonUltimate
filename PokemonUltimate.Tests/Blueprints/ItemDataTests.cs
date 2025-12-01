using NUnit.Framework;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Builders;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Content.Catalogs.Items;

namespace PokemonUltimate.Tests.Blueprints
{
    /// <summary>
    /// Tests for ItemData and ItemBuilder.
    /// </summary>
    [TestFixture]
    public class ItemDataTests
    {
        #region Builder Tests

        [Test]
        public void Define_CreatesItemWithName()
        {
            var item = Item.Define("Test Item").Build();

            Assert.That(item.Name, Is.EqualTo("Test Item"));
            Assert.That(item.Id, Is.EqualTo("test-item"));
        }

        [Test]
        public void Description_SetsDescription()
        {
            var item = Item.Define("Test")
                .Description("A test item")
                .Build();

            Assert.That(item.Description, Is.EqualTo("A test item"));
        }

        [Test]
        public void Category_SetsCategory()
        {
            var item = Item.Define("Test")
                .Category(ItemCategory.Berry)
                .Build();

            Assert.That(item.Category, Is.EqualTo(ItemCategory.Berry));
        }

        [Test]
        public void Price_SetsPrice()
        {
            var item = Item.Define("Test")
                .Price(500)
                .Build();

            Assert.That(item.Price, Is.EqualTo(500));
        }

        [Test]
        public void Consumable_SetsConsumable()
        {
            var item = Item.Define("Test")
                .Consumable()
                .Build();

            Assert.That(item.IsConsumable, Is.True);
        }

        [Test]
        public void ChoiceItem_ConfiguresCorrectly()
        {
            var item = Item.Define("Choice Band Clone")
                .ChoiceItem(Stat.Attack)
                .Build();

            Assert.That(item.TargetStat, Is.EqualTo(Stat.Attack));
            Assert.That(item.StatMultiplier, Is.EqualTo(1.5f));
            Assert.That(item.LocksMove, Is.True);
            Assert.That(item.HasPassiveEffect, Is.True);
        }

        [Test]
        public void BoostsDamageWithRecoil_ConfiguresCorrectly()
        {
            var item = Item.Define("Life Orb Clone")
                .BoostsDamageWithRecoil(1.3f, 0.1f)
                .Build();

            Assert.That(item.DamageMultiplier, Is.EqualTo(1.3f));
            Assert.That(item.RecoilPercent, Is.EqualTo(0.1f));
            Assert.That(item.ListensTo(ItemTrigger.OnDamageDealt), Is.True);
        }

        [Test]
        public void HealsPercentEachTurn_ConfiguresCorrectly()
        {
            var item = Item.Define("Leftovers Clone")
                .HealsPercentEachTurn(0.0625f)
                .Build();

            Assert.That(item.ListensTo(ItemTrigger.OnTurnEnd), Is.True);
            // Heal amount stored as negative percentage
            Assert.That(item.HealAmount, Is.EqualTo(-6)); // -6.25%
        }

        [Test]
        public void SurvivesFatalHit_ConfiguresCorrectly()
        {
            var item = Item.Define("Focus Sash Clone")
                .SurvivesFatalHit()
                .Build();

            Assert.That(item.HPThreshold, Is.EqualTo(1.0f));
            Assert.That(item.IsConsumable, Is.True);
            Assert.That(item.ListensTo(ItemTrigger.OnWouldFaint), Is.True);
        }

        [Test]
        public void Berry_SetsCategory()
        {
            var item = Item.Define("Test Berry")
                .Berry()
                .Build();

            Assert.That(item.Category, Is.EqualTo(ItemCategory.Berry));
            Assert.That(item.IsBerry, Is.True);
            Assert.That(item.IsConsumable, Is.True);
        }

        [Test]
        public void CuresStatus_ConfiguresCorrectly()
        {
            var item = Item.Define("Cheri Berry Clone")
                .Berry()
                .CuresStatus(PersistentStatus.Paralysis)
                .Build();

            Assert.That(item.CuresStatus, Is.EqualTo(PersistentStatus.Paralysis));
            Assert.That(item.IsConsumable, Is.True);
        }

        #endregion

        #region Catalog Tests

        [Test]
        public void Catalog_ContainsItems()
        {
            Assert.That(ItemCatalog.All.Count, Is.GreaterThan(0));
        }

        [Test]
        public void Catalog_Leftovers_HasCorrectConfiguration()
        {
            var leftovers = ItemCatalog.Leftovers;

            Assert.That(leftovers.Name, Is.EqualTo("Leftovers"));
            Assert.That(leftovers.Category, Is.EqualTo(ItemCategory.HeldItem));
            Assert.That(leftovers.IsHoldable, Is.True);
            Assert.That(leftovers.ListensTo(ItemTrigger.OnTurnEnd), Is.True);
        }

        [Test]
        public void Catalog_ChoiceBand_HasCorrectConfiguration()
        {
            var choiceBand = ItemCatalog.ChoiceBand;

            Assert.That(choiceBand.Name, Is.EqualTo("Choice Band"));
            Assert.That(choiceBand.TargetStat, Is.EqualTo(Stat.Attack));
            Assert.That(choiceBand.StatMultiplier, Is.EqualTo(1.5f));
            Assert.That(choiceBand.LocksMove, Is.True);
        }

        [Test]
        public void Catalog_ChoiceSpecs_HasCorrectConfiguration()
        {
            var choiceSpecs = ItemCatalog.ChoiceSpecs;

            Assert.That(choiceSpecs.Name, Is.EqualTo("Choice Specs"));
            Assert.That(choiceSpecs.TargetStat, Is.EqualTo(Stat.SpAttack));
            Assert.That(choiceSpecs.StatMultiplier, Is.EqualTo(1.5f));
        }

        [Test]
        public void Catalog_ChoiceScarf_HasCorrectConfiguration()
        {
            var choiceScarf = ItemCatalog.ChoiceScarf;

            Assert.That(choiceScarf.Name, Is.EqualTo("Choice Scarf"));
            Assert.That(choiceScarf.TargetStat, Is.EqualTo(Stat.Speed));
            Assert.That(choiceScarf.StatMultiplier, Is.EqualTo(1.5f));
        }

        [Test]
        public void Catalog_LifeOrb_HasCorrectConfiguration()
        {
            var lifeOrb = ItemCatalog.LifeOrb;

            Assert.That(lifeOrb.Name, Is.EqualTo("Life Orb"));
            Assert.That(lifeOrb.DamageMultiplier, Is.EqualTo(1.3f));
            Assert.That(lifeOrb.RecoilPercent, Is.EqualTo(0.1f));
        }

        [Test]
        public void Catalog_FocusSash_HasCorrectConfiguration()
        {
            var focusSash = ItemCatalog.FocusSash;

            Assert.That(focusSash.Name, Is.EqualTo("Focus Sash"));
            Assert.That(focusSash.HPThreshold, Is.EqualTo(1.0f));
            Assert.That(focusSash.IsConsumable, Is.True);
        }

        [Test]
        public void Catalog_SitrusBerry_HasCorrectConfiguration()
        {
            var sitrus = ItemCatalog.SitrusBerry;

            Assert.That(sitrus.Name, Is.EqualTo("Sitrus Berry"));
            Assert.That(sitrus.IsBerry, Is.True);
            Assert.That(sitrus.IsConsumable, Is.True);
            Assert.That(sitrus.HPThreshold, Is.EqualTo(0.25f));
        }

        [Test]
        public void Catalog_LumBerry_HasCorrectConfiguration()
        {
            var lumBerry = ItemCatalog.LumBerry;

            Assert.That(lumBerry.Name, Is.EqualTo("Lum Berry"));
            Assert.That(lumBerry.IsBerry, Is.True);
            // CuresStatus = None indicates "all statuses"
            Assert.That(lumBerry.CuresStatus, Is.EqualTo(PersistentStatus.None));
        }

        [Test]
        public void Catalog_GetByName_ReturnsCorrectItem()
        {
            var item = ItemCatalog.GetByName("Leftovers");

            Assert.That(item, Is.Not.Null);
            Assert.That(item.Name, Is.EqualTo("Leftovers"));
        }

        [Test]
        public void Catalog_GetByName_NotFound_ReturnsNull()
        {
            var item = ItemCatalog.GetByName("Nonexistent");

            Assert.That(item, Is.Null);
        }

        [Test]
        public void Catalog_GetById_ReturnsCorrectItem()
        {
            var item = ItemCatalog.GetById("leftovers");

            Assert.That(item, Is.Not.Null);
            Assert.That(item.Name, Is.EqualTo("Leftovers"));
        }

        #endregion

        #region ListensTo Tests

        [Test]
        public void ListensTo_MultipleTriggers_WorksCorrectly()
        {
            var item = Item.Define("Multi")
                .OnTurnEnd()
                .OnLowHP()
                .Build();

            Assert.That(item.ListensTo(ItemTrigger.OnTurnEnd), Is.True);
            Assert.That(item.ListensTo(ItemTrigger.OnLowHP), Is.True);
            Assert.That(item.ListensTo(ItemTrigger.OnWouldFaint), Is.False);
        }

        [Test]
        public void HasPassiveEffect_WhenPassive_ReturnsTrue()
        {
            var item = Item.Define("Passive")
                .Passive()
                .Build();

            Assert.That(item.HasPassiveEffect, Is.True);
        }

        [Test]
        public void HasPassiveEffect_WhenNotPassive_ReturnsFalse()
        {
            var item = Item.Define("Active")
                .OnTurnEnd()
                .Build();

            Assert.That(item.HasPassiveEffect, Is.False);
        }

        #endregion

        #region Type Boost Items Tests

        [Test]
        public void Catalog_Charcoal_BoostsFire()
        {
            var charcoal = ItemCatalog.Charcoal;

            Assert.That(charcoal.BoostsType, Is.EqualTo(PokemonType.Fire));
            Assert.That(charcoal.DamageMultiplier, Is.EqualTo(1.2f));
        }

        [Test]
        public void Catalog_MysticWater_BoostsWater()
        {
            var mysticWater = ItemCatalog.MysticWater;

            Assert.That(mysticWater.BoostsType, Is.EqualTo(PokemonType.Water));
            Assert.That(mysticWater.DamageMultiplier, Is.EqualTo(1.2f));
        }

        [Test]
        public void Catalog_Magnet_BoostsElectric()
        {
            var magnet = ItemCatalog.Magnet;

            Assert.That(magnet.BoostsType, Is.EqualTo(PokemonType.Electric));
            Assert.That(magnet.DamageMultiplier, Is.EqualTo(1.2f));
        }

        #endregion
    }
}

