using NUnit.Framework;
using PokemonUltimate.Content.Catalogs.Field;
using PokemonUltimate.Content.Builders;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Blueprints
{
    /// <summary>
    /// Tests for SideConditionData and SideConditionBuilder.
    /// </summary>
    [TestFixture]
    public class SideConditionDataTests
    {
        #region Builder Tests

        [Test]
        public void Define_WithName_SetsNameAndId()
        {
            var condition = Screen.Define("Test Screen").Build();

            Assert.That(condition.Name, Is.EqualTo("Test Screen"));
            Assert.That(condition.Id, Is.EqualTo("test-screen"));
        }

        [Test]
        public void Duration_SetsCorrectly()
        {
            var condition = Screen.Define("Test")
                .Duration(5)
                .Build();

            Assert.That(condition.DefaultDuration, Is.EqualTo(5));
            Assert.That(condition.IsSingleTurn, Is.False);
        }

        [Test]
        public void SingleTurn_SetsCorrectly()
        {
            var condition = Screen.Define("Test")
                .SingleTurn()
                .Build();

            Assert.That(condition.DefaultDuration, Is.EqualTo(1));
            Assert.That(condition.IsSingleTurn, Is.True);
        }

        [Test]
        public void ExtendedBy_SetsCorrectly()
        {
            var condition = Screen.Define("Test")
                .Duration(5)
                .ExtendedBy("Light Clay", 8)
                .Build();

            Assert.That(condition.GetDuration(null), Is.EqualTo(5));
            Assert.That(condition.GetDuration("Light Clay"), Is.EqualTo(8));
        }

        [Test]
        public void ReducesPhysicalDamage_SetsCorrectly()
        {
            var condition = Screen.Define("Test")
                .ReducesPhysicalDamage(0.5f, 0.66f)
                .Build();

            Assert.That(condition.ReducesDamage, Is.True);
            Assert.That(condition.ReducesDamageFrom, Is.EqualTo(MoveCategory.Physical));
            Assert.That(condition.DamageMultiplierSingles, Is.EqualTo(0.5f));
            Assert.That(condition.DamageMultiplierDoubles, Is.EqualTo(0.66f));
        }

        [Test]
        public void ReducesSpecialDamage_SetsCorrectly()
        {
            var condition = Screen.Define("Test")
                .ReducesSpecialDamage()
                .Build();

            Assert.That(condition.ReducesDamageFrom, Is.EqualTo(MoveCategory.Special));
        }

        [Test]
        public void ReducesAllDamage_SetsCorrectly()
        {
            var condition = Screen.Define("Test")
                .ReducesAllDamage()
                .Build();

            Assert.That(condition.ReducesDamageFrom, Is.Null);
            Assert.That(condition.DamageMultiplierSingles, Is.EqualTo(0.5f));
        }

        [Test]
        public void DoublesSpeed_SetsCorrectly()
        {
            var condition = Screen.Define("Test")
                .DoublesSpeed()
                .Build();

            Assert.That(condition.SpeedMultiplier, Is.EqualTo(2.0f));
            Assert.That(condition.ModifiesSpeed, Is.True);
        }

        [Test]
        public void PreventsStatus_SetsCorrectly()
        {
            var condition = Screen.Define("Test")
                .PreventsStatus()
                .Build();

            Assert.That(condition.PreventsStatus, Is.True);
        }

        [Test]
        public void BlocksSpreadMoves_SetsCorrectly()
        {
            var condition = Screen.Define("Test")
                .BlocksSpreadMoves()
                .Build();

            Assert.That(condition.BlocksSpreadMoves, Is.True);
        }

        [Test]
        public void RequiresWeather_SetsCorrectly()
        {
            var condition = Screen.Define("Test")
                .RequiresWeather(Weather.Hail, Weather.Snow)
                .Build();

            Assert.That(condition.RequiredWeather, Is.EqualTo(Weather.Hail));
            Assert.That(condition.AlternateWeather, Is.EqualTo(Weather.Snow));
        }

        #endregion

        #region Helper Methods Tests

        [Test]
        public void GetDamageMultiplier_Singles_ReturnsCorrect()
        {
            var condition = Screen.Define("Test")
                .ReducesPhysicalDamage(0.5f, 0.66f)
                .Build();

            Assert.That(condition.GetDamageMultiplier(isDoubles: false), Is.EqualTo(0.5f));
            Assert.That(condition.GetDamageMultiplier(isDoubles: true), Is.EqualTo(0.66f));
        }

        [Test]
        public void CanBeSetInWeather_WithRequirement_ChecksCorrectly()
        {
            var condition = Screen.Define("Test")
                .RequiresWeather(Weather.Hail, Weather.Snow)
                .Build();

            Assert.That(condition.CanBeSetInWeather(Weather.Hail), Is.True);
            Assert.That(condition.CanBeSetInWeather(Weather.Snow), Is.True);
            Assert.That(condition.CanBeSetInWeather(Weather.Rain), Is.False);
            Assert.That(condition.CanBeSetInWeather(Weather.None), Is.False);
        }

        [Test]
        public void CanBeSetInWeather_NoRequirement_ReturnsTrue()
        {
            var condition = Screen.Define("Test").Build();

            Assert.That(condition.CanBeSetInWeather(Weather.None), Is.True);
            Assert.That(condition.CanBeSetInWeather(Weather.Rain), Is.True);
        }

        [Test]
        public void ReducesMoveCategory_ChecksCorrectly()
        {
            var condition = Screen.Define("Test")
                .ReducesPhysicalDamage()
                .Build();

            Assert.That(condition.ReducesMoveCategory(MoveCategory.Physical), Is.True);
            Assert.That(condition.ReducesMoveCategory(MoveCategory.Special), Is.False);
        }

        #endregion

        #region Catalog Tests

        [Test]
        public void Catalog_Reflect_HasCorrectProperties()
        {
            var reflect = SideConditionCatalog.Reflect;

            Assert.That(reflect.Type, Is.EqualTo(SideCondition.Reflect));
            Assert.That(reflect.DefaultDuration, Is.EqualTo(5));
            Assert.That(reflect.ExtendedDuration, Is.EqualTo(8));
            Assert.That(reflect.ReducesDamageFrom, Is.EqualTo(MoveCategory.Physical));
            Assert.That(reflect.DamageMultiplierSingles, Is.EqualTo(0.5f));
        }

        [Test]
        public void Catalog_LightScreen_HasCorrectProperties()
        {
            var ls = SideConditionCatalog.LightScreen;

            Assert.That(ls.Type, Is.EqualTo(SideCondition.LightScreen));
            Assert.That(ls.ReducesDamageFrom, Is.EqualTo(MoveCategory.Special));
        }

        [Test]
        public void Catalog_AuroraVeil_RequiresHailOrSnow()
        {
            var av = SideConditionCatalog.AuroraVeil;

            Assert.That(av.Type, Is.EqualTo(SideCondition.AuroraVeil));
            Assert.That(av.RequiredWeather, Is.EqualTo(Weather.Hail));
            Assert.That(av.AlternateWeather, Is.EqualTo(Weather.Snow));
            Assert.That(av.ReducesDamageFrom, Is.Null); // Both
        }

        [Test]
        public void Catalog_Tailwind_DoublesSpeed()
        {
            var tailwind = SideConditionCatalog.Tailwind;

            Assert.That(tailwind.Type, Is.EqualTo(SideCondition.Tailwind));
            Assert.That(tailwind.SpeedMultiplier, Is.EqualTo(2.0f));
            Assert.That(tailwind.DefaultDuration, Is.EqualTo(4));
        }

        [Test]
        public void Catalog_Safeguard_PreventsStatus()
        {
            var safeguard = SideConditionCatalog.Safeguard;

            Assert.That(safeguard.PreventsStatus, Is.True);
        }

        [Test]
        public void Catalog_WideGuard_IsSingleTurn()
        {
            var wg = SideConditionCatalog.WideGuard;

            Assert.That(wg.IsSingleTurn, Is.True);
            Assert.That(wg.BlocksSpreadMoves, Is.True);
        }

        [Test]
        public void Catalog_All_Contains10Conditions()
        {
            Assert.That(SideConditionCatalog.All.Count, Is.EqualTo(10));
        }

        [Test]
        public void Catalog_GetByType_ReturnsCorrectCondition()
        {
            Assert.That(SideConditionCatalog.GetByType(SideCondition.Reflect), Is.EqualTo(SideConditionCatalog.Reflect));
            Assert.That(SideConditionCatalog.GetByType(SideCondition.None), Is.Null);
        }

        [Test]
        public void Catalog_GetScreens_ReturnsOnlyDamageReducers()
        {
            var screens = new System.Collections.Generic.List<Core.Blueprints.SideConditionData>(
                SideConditionCatalog.GetScreens());

            Assert.That(screens, Contains.Item(SideConditionCatalog.Reflect));
            Assert.That(screens, Contains.Item(SideConditionCatalog.LightScreen));
            Assert.That(screens, Contains.Item(SideConditionCatalog.AuroraVeil));
            Assert.That(screens, Does.Not.Contain(SideConditionCatalog.Tailwind));
        }

        #endregion
    }
}

