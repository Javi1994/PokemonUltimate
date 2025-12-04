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
    /// Tests for SetSideConditionAction.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.16: Field Conditions
    /// **Documentation**: See `docs/features/2-combat-system/2.16-field-conditions/README.md`
    /// </remarks>
    [TestFixture]
    public class SetSideConditionActionTests
    {
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSide _targetSide;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            var playerParty = new List<PokemonInstance> { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new List<PokemonInstance> { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) };
            _field.Initialize(rules, playerParty, enemyParty);
            _userSlot = _field.PlayerSide.Slots[0];
            _targetSide = _field.PlayerSide;
        }

        [Test]
        public void ExecuteLogic_Reflect_SetsCondition()
        {
            var conditionData = SideConditionCatalog.GetByType(SideCondition.Reflect);
            var action = new SetSideConditionAction(_userSlot, _targetSide, SideCondition.Reflect, 5, conditionData);

            action.ExecuteLogic(_field);

            Assert.That(_targetSide.HasSideCondition(SideCondition.Reflect), Is.True);
            Assert.That(_targetSide.GetSideConditionDuration(SideCondition.Reflect), Is.EqualTo(5));
        }

        [Test]
        public void ExecuteLogic_Tailwind_SetsCondition()
        {
            var conditionData = SideConditionCatalog.GetByType(SideCondition.Tailwind);
            var action = new SetSideConditionAction(_userSlot, _targetSide, SideCondition.Tailwind, 4, conditionData);

            action.ExecuteLogic(_field);

            Assert.That(_targetSide.HasSideCondition(SideCondition.Tailwind), Is.True);
            Assert.That(_targetSide.GetSideConditionDuration(SideCondition.Tailwind), Is.EqualTo(4));
        }

        [Test]
        public void ExecuteLogic_Safeguard_SetsCondition()
        {
            var conditionData = SideConditionCatalog.GetByType(SideCondition.Safeguard);
            var action = new SetSideConditionAction(_userSlot, _targetSide, SideCondition.Safeguard, 5, conditionData);

            action.ExecuteLogic(_field);

            Assert.That(_targetSide.HasSideCondition(SideCondition.Safeguard), Is.True);
            Assert.That(_targetSide.GetSideConditionDuration(SideCondition.Safeguard), Is.EqualTo(5));
        }

        [Test]
        public void ExecuteLogic_None_ClearsAllConditions()
        {
            var reflectData = SideConditionCatalog.GetByType(SideCondition.Reflect);
            var tailwindData = SideConditionCatalog.GetByType(SideCondition.Tailwind);
            _targetSide.AddSideCondition(reflectData, 5);
            _targetSide.AddSideCondition(tailwindData, 4);

            var action = new SetSideConditionAction(_userSlot, _targetSide, SideCondition.None, 0);
            action.ExecuteLogic(_field);

            Assert.That(_targetSide.HasSideCondition(SideCondition.Reflect), Is.False);
            Assert.That(_targetSide.HasSideCondition(SideCondition.Tailwind), Is.False);
        }

        [Test]
        public void ExecuteLogic_AuroraVeil_RequiresHail()
        {
            var conditionData = SideConditionCatalog.GetByType(SideCondition.AuroraVeil);
            
            // Set Hail weather
            _field.SetWeather(Weather.Hail, 5, null);
            
            var action = new SetSideConditionAction(_userSlot, _targetSide, SideCondition.AuroraVeil, 5, conditionData);
            action.ExecuteLogic(_field);

            Assert.That(_targetSide.HasSideCondition(SideCondition.AuroraVeil), Is.True);
        }

        [Test]
        public void ExecuteLogic_AuroraVeil_NoHail_Fails()
        {
            var conditionData = SideConditionCatalog.GetByType(SideCondition.AuroraVeil);
            
            // No Hail weather set

            var action = new SetSideConditionAction(_userSlot, _targetSide, SideCondition.AuroraVeil, 5, conditionData);
            action.ExecuteLogic(_field);

            // Aurora Veil should not be set without Hail
            Assert.That(_targetSide.HasSideCondition(SideCondition.AuroraVeil), Is.False);
        }
    }
}

