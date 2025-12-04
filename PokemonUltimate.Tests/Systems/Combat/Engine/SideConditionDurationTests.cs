using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Field;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Engine
{
    /// <summary>
    /// Tests for side condition duration management in CombatEngine.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.16: Field Conditions
    /// **Documentation**: See `docs/features/2-combat-system/2.16-field-conditions/README.md`
    /// </remarks>
    [TestFixture]
    public class SideConditionDurationTests
    {
        private BattleField _field;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            var playerParty = new List<PokemonInstance> { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new List<PokemonInstance> { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) };
            _field.Initialize(rules, playerParty, enemyParty);
        }

        [Test]
        public void DecrementAllSideConditionDurations_DecrementsCorrectly()
        {
            var reflectData = SideConditionCatalog.GetByType(SideCondition.Reflect);
            _field.PlayerSide.AddSideCondition(reflectData, 5);

            _field.PlayerSide.DecrementAllSideConditionDurations();

            Assert.That(_field.PlayerSide.GetSideConditionDuration(SideCondition.Reflect), Is.EqualTo(4));
        }

        [Test]
        public void DecrementAllSideConditionDurations_ReachesZero_RemovesCondition()
        {
            var reflectData = SideConditionCatalog.GetByType(SideCondition.Reflect);
            _field.PlayerSide.AddSideCondition(reflectData, 1);

            _field.PlayerSide.DecrementAllSideConditionDurations();

            Assert.That(_field.PlayerSide.HasSideCondition(SideCondition.Reflect), Is.False);
        }

        [Test]
        public void DecrementAllSideConditionDurations_MultipleConditions_DecrementsAll()
        {
            var reflectData = SideConditionCatalog.GetByType(SideCondition.Reflect);
            var tailwindData = SideConditionCatalog.GetByType(SideCondition.Tailwind);
            var safeguardData = SideConditionCatalog.GetByType(SideCondition.Safeguard);

            _field.PlayerSide.AddSideCondition(reflectData, 5);
            _field.PlayerSide.AddSideCondition(tailwindData, 4);
            _field.PlayerSide.AddSideCondition(safeguardData, 3);

            _field.PlayerSide.DecrementAllSideConditionDurations();

            Assert.That(_field.PlayerSide.GetSideConditionDuration(SideCondition.Reflect), Is.EqualTo(4));
            Assert.That(_field.PlayerSide.GetSideConditionDuration(SideCondition.Tailwind), Is.EqualTo(3));
            Assert.That(_field.PlayerSide.GetSideConditionDuration(SideCondition.Safeguard), Is.EqualTo(2));
        }

        [Test]
        public void DecrementAllSideConditionDurations_SingleTurnConditions_RemovesAfterOneTurn()
        {
            var wideGuardData = SideConditionCatalog.GetByType(SideCondition.WideGuard);
            _field.PlayerSide.AddSideCondition(wideGuardData, 1);

            _field.PlayerSide.DecrementAllSideConditionDurations();

            // Single-turn conditions should be removed after decrement
            Assert.That(_field.PlayerSide.HasSideCondition(SideCondition.WideGuard), Is.False);
        }
    }
}

