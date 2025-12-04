using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Field;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Actions
{
    /// <summary>
    /// Tests for Safeguard status protection in ApplyStatusAction.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.16: Field Conditions
    /// **Documentation**: See `docs/features/2-combat-system/2.16-field-conditions/README.md`
    /// </remarks>
    [TestFixture]
    public class SafeguardProtectionTests
    {
        private BattleField _field;
        private BattleSlot _targetSlot;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            var playerParty = new List<PokemonInstance> { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new List<PokemonInstance> { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) };
            _field.Initialize(rules, playerParty, enemyParty);
            _targetSlot = _field.PlayerSide.Slots[0];
        }

        [Test]
        public void ExecuteLogic_Safeguard_PreventsStatus()
        {
            var safeguardData = SideConditionCatalog.GetByType(SideCondition.Safeguard);
            _targetSlot.Side.AddSideCondition(safeguardData, 5);

            var statusAction = new ApplyStatusAction(null, _targetSlot, PersistentStatus.Poison);
            var actions = statusAction.ExecuteLogic(_field).ToList();

            // Safeguard should prevent status application
            // Status should remain None
            Assert.That(_targetSlot.Pokemon.Status, Is.EqualTo(PersistentStatus.None));
        }

        [Test]
        public void ExecuteLogic_NoSafeguard_AllowsStatus()
        {
            // No Safeguard added

            var statusAction = new ApplyStatusAction(null, _targetSlot, PersistentStatus.Poison);
            statusAction.ExecuteLogic(_field);

            // Status should be applied
            Assert.That(_targetSlot.Pokemon.Status, Is.EqualTo(PersistentStatus.Poison));
        }

        [Test]
        public void ExecuteLogic_SafeguardOnOpposingSide_NoProtection()
        {
            var safeguardData = SideConditionCatalog.GetByType(SideCondition.Safeguard);
            _field.EnemySide.AddSideCondition(safeguardData, 5); // Safeguard on enemy side

            var statusAction = new ApplyStatusAction(null, _targetSlot, PersistentStatus.Poison);
            statusAction.ExecuteLogic(_field);

            // Safeguard on enemy side should not protect player side
            Assert.That(_targetSlot.Pokemon.Status, Is.EqualTo(PersistentStatus.Poison));
        }

        [Test]
        public void ExecuteLogic_Safeguard_PreventsAllStatusTypes()
        {
            var safeguardData = SideConditionCatalog.GetByType(SideCondition.Safeguard);
            _targetSlot.Side.AddSideCondition(safeguardData, 5);

            // Test Poison
            var poisonAction = new ApplyStatusAction(null, _targetSlot, PersistentStatus.Poison);
            poisonAction.ExecuteLogic(_field);
            Assert.That(_targetSlot.Pokemon.Status, Is.EqualTo(PersistentStatus.None));

            // Test Burn
            var burnAction = new ApplyStatusAction(null, _targetSlot, PersistentStatus.Burn);
            burnAction.ExecuteLogic(_field);
            Assert.That(_targetSlot.Pokemon.Status, Is.EqualTo(PersistentStatus.None));

            // Test Paralysis
            var paralysisAction = new ApplyStatusAction(null, _targetSlot, PersistentStatus.Paralysis);
            paralysisAction.ExecuteLogic(_field);
            Assert.That(_targetSlot.Pokemon.Status, Is.EqualTo(PersistentStatus.None));
        }
    }
}

