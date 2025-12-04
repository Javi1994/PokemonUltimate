using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Field;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Field
{
    /// <summary>
    /// Tests for Side Condition tracking in BattleSide.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.16: Field Conditions
    /// **Documentation**: See `docs/features/2-combat-system/2.16-field-conditions/README.md`
    /// </remarks>
    [TestFixture]
    public class SideConditionTrackingTests
    {
        private BattleSide _side;
        private BattleRules _rules;

        [SetUp]
        public void SetUp()
        {
            _rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            _side = new BattleSide(1, isPlayer: true);
            var party = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            _side.SetParty(party);
        }

        #region Add Side Condition Tests

        [Test]
        public void AddSideCondition_Reflect_SetsCondition()
        {
            var conditionData = SideConditionCatalog.GetByType(SideCondition.Reflect);

            _side.AddSideCondition(conditionData, 5);

            Assert.That(_side.HasSideCondition(SideCondition.Reflect), Is.True);
            Assert.That(_side.GetSideConditionDuration(SideCondition.Reflect), Is.EqualTo(5));
        }

        [Test]
        public void AddSideCondition_Tailwind_SetsCondition()
        {
            var conditionData = SideConditionCatalog.GetByType(SideCondition.Tailwind);

            _side.AddSideCondition(conditionData, 4);

            Assert.That(_side.HasSideCondition(SideCondition.Tailwind), Is.True);
            Assert.That(_side.GetSideConditionDuration(SideCondition.Tailwind), Is.EqualTo(4));
        }

        [Test]
        public void AddSideCondition_Safeguard_SetsCondition()
        {
            var conditionData = SideConditionCatalog.GetByType(SideCondition.Safeguard);

            _side.AddSideCondition(conditionData, 5);

            Assert.That(_side.HasSideCondition(SideCondition.Safeguard), Is.True);
            Assert.That(_side.GetSideConditionDuration(SideCondition.Safeguard), Is.EqualTo(5));
        }

        [Test]
        public void AddSideCondition_ReplacesExisting()
        {
            var conditionData = SideConditionCatalog.GetByType(SideCondition.Reflect);
            _side.AddSideCondition(conditionData, 3);

            // Add again with different duration
            _side.AddSideCondition(conditionData, 5);

            Assert.That(_side.GetSideConditionDuration(SideCondition.Reflect), Is.EqualTo(5));
        }

        #endregion

        #region Remove Side Condition Tests

        [Test]
        public void RemoveSideCondition_Reflect_RemovesCondition()
        {
            var conditionData = SideConditionCatalog.GetByType(SideCondition.Reflect);
            _side.AddSideCondition(conditionData, 5);

            _side.RemoveSideCondition(SideCondition.Reflect);

            Assert.That(_side.HasSideCondition(SideCondition.Reflect), Is.False);
            Assert.That(_side.GetSideConditionDuration(SideCondition.Reflect), Is.EqualTo(0));
        }

        [Test]
        public void RemoveSideCondition_AllConditions_RemovesAll()
        {
            var reflectData = SideConditionCatalog.GetByType(SideCondition.Reflect);
            var tailwindData = SideConditionCatalog.GetByType(SideCondition.Tailwind);
            var safeguardData = SideConditionCatalog.GetByType(SideCondition.Safeguard);

            _side.AddSideCondition(reflectData, 5);
            _side.AddSideCondition(tailwindData, 4);
            _side.AddSideCondition(safeguardData, 5);

            _side.RemoveAllSideConditions();

            Assert.That(_side.HasSideCondition(SideCondition.Reflect), Is.False);
            Assert.That(_side.HasSideCondition(SideCondition.Tailwind), Is.False);
            Assert.That(_side.HasSideCondition(SideCondition.Safeguard), Is.False);
        }

        [Test]
        public void RemoveSideCondition_NoCondition_DoesNothing()
        {
            // No conditions added

            _side.RemoveSideCondition(SideCondition.Reflect);

            Assert.That(_side.HasSideCondition(SideCondition.Reflect), Is.False);
        }

        #endregion

        #region Has Side Condition Tests

        [Test]
        public void HasSideCondition_NoConditions_ReturnsFalse()
        {
            Assert.That(_side.HasSideCondition(SideCondition.Reflect), Is.False);
            Assert.That(_side.HasSideCondition(SideCondition.Tailwind), Is.False);
            Assert.That(_side.HasSideCondition(SideCondition.Safeguard), Is.False);
        }

        [Test]
        public void HasSideCondition_WithCondition_ReturnsTrue()
        {
            var conditionData = SideConditionCatalog.GetByType(SideCondition.Reflect);
            _side.AddSideCondition(conditionData, 5);

            Assert.That(_side.HasSideCondition(SideCondition.Reflect), Is.True);
        }

        #endregion

        #region Get Side Condition Duration Tests

        [Test]
        public void GetSideConditionDuration_NoCondition_ReturnsZero()
        {
            Assert.That(_side.GetSideConditionDuration(SideCondition.Reflect), Is.EqualTo(0));
        }

        [Test]
        public void GetSideConditionDuration_WithCondition_ReturnsCorrectDuration()
        {
            var conditionData = SideConditionCatalog.GetByType(SideCondition.Reflect);
            _side.AddSideCondition(conditionData, 5);

            Assert.That(_side.GetSideConditionDuration(SideCondition.Reflect), Is.EqualTo(5));
        }

        #endregion

        #region Decrement Duration Tests

        [Test]
        public void DecrementSideConditionDuration_DecrementsCorrectly()
        {
            var conditionData = SideConditionCatalog.GetByType(SideCondition.Reflect);
            _side.AddSideCondition(conditionData, 5);

            _side.DecrementSideConditionDuration(SideCondition.Reflect);

            Assert.That(_side.GetSideConditionDuration(SideCondition.Reflect), Is.EqualTo(4));
        }

        [Test]
        public void DecrementSideConditionDuration_ReachesZero_RemovesCondition()
        {
            var conditionData = SideConditionCatalog.GetByType(SideCondition.Reflect);
            _side.AddSideCondition(conditionData, 1);

            _side.DecrementSideConditionDuration(SideCondition.Reflect);

            Assert.That(_side.HasSideCondition(SideCondition.Reflect), Is.False);
            Assert.That(_side.GetSideConditionDuration(SideCondition.Reflect), Is.EqualTo(0));
        }

        [Test]
        public void DecrementSideConditionDuration_NoCondition_DoesNothing()
        {
            // No condition added

            _side.DecrementSideConditionDuration(SideCondition.Reflect);

            Assert.That(_side.HasSideCondition(SideCondition.Reflect), Is.False);
        }

        #endregion

        #region Get Side Condition Data Tests

        [Test]
        public void GetSideConditionData_WithCondition_ReturnsData()
        {
            var conditionData = SideConditionCatalog.GetByType(SideCondition.Reflect);
            _side.AddSideCondition(conditionData, 5);

            var retrievedData = _side.GetSideConditionData(SideCondition.Reflect);

            Assert.That(retrievedData, Is.Not.Null);
            Assert.That(retrievedData.Type, Is.EqualTo(SideCondition.Reflect));
        }

        [Test]
        public void GetSideConditionData_NoCondition_ReturnsNull()
        {
            var data = _side.GetSideConditionData(SideCondition.Reflect);

            Assert.That(data, Is.Null);
        }

        #endregion
    }
}

