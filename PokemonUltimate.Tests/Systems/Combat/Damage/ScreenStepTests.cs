using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Damage.Steps;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Field;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Damage
{
    /// <summary>
    /// Tests for ScreenStep - Screen damage reduction in DamagePipeline.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.16: Field Conditions
    /// **Documentation**: See `docs/features/2-combat-system/2.16-field-conditions/README.md`
    /// </remarks>
    [TestFixture]
    public class ScreenStepTests
    {
        private DamagePipeline _pipeline;
        private BattleField _field;
        private BattleSlot _attackerSlot;
        private BattleSlot _defenderSlot;
        private MoveData _physicalMove;
        private MoveData _specialMove;

        [SetUp]
        public void SetUp()
        {
            _pipeline = new DamagePipeline(new List<IDamageStep>
            {
                new BaseDamageStep(),
                new ScreenStep(), // Screen step for testing
                new TypeEffectivenessStep()
            });

            _field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            var playerParty = new List<PokemonInstance> { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new List<PokemonInstance> { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) };
            _field.Initialize(rules, playerParty, enemyParty);

            _attackerSlot = _field.PlayerSide.Slots[0];
            _defenderSlot = _field.EnemySide.Slots[0];

            _physicalMove = new MoveData
            {
                Name = "Tackle",
                Power = 40,
                Accuracy = 100,
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                MaxPP = 35,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy
            };

            _specialMove = new MoveData
            {
                Name = "Water Gun",
                Power = 40,
                Accuracy = 100,
                Type = PokemonType.Water,
                Category = MoveCategory.Special,
                MaxPP = 25,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy
            };
        }

        #region Reflect Tests

        [Test]
        public void Process_Reflect_PhysicalMove_ReducesDamage()
        {
            var reflectData = SideConditionCatalog.GetByType(SideCondition.Reflect);
            _defenderSlot.Side.AddSideCondition(reflectData, 5);

            var contextWithReflect = _pipeline.Calculate(_attackerSlot, _defenderSlot, _physicalMove, _field, fixedRandomValue: 1.0f);
            _defenderSlot.Side.RemoveSideCondition(SideCondition.Reflect);
            var contextWithoutReflect = _pipeline.Calculate(_attackerSlot, _defenderSlot, _physicalMove, _field, fixedRandomValue: 1.0f);

            // Reflect reduces physical damage by 50% in singles
            Assert.That(contextWithReflect.FinalDamage, Is.LessThan(contextWithoutReflect.FinalDamage));
            Assert.That(contextWithReflect.Multiplier, Is.LessThanOrEqualTo(contextWithoutReflect.Multiplier * 0.51f)); // Allow small rounding
        }

        [Test]
        public void Process_Reflect_SpecialMove_NoEffect()
        {
            var reflectData = SideConditionCatalog.GetByType(SideCondition.Reflect);
            _defenderSlot.Side.AddSideCondition(reflectData, 5);

            var contextWithReflect = _pipeline.Calculate(_attackerSlot, _defenderSlot, _specialMove, _field, fixedRandomValue: 1.0f);
            _defenderSlot.Side.RemoveSideCondition(SideCondition.Reflect);
            var contextWithoutReflect = _pipeline.Calculate(_attackerSlot, _defenderSlot, _specialMove, _field, fixedRandomValue: 1.0f);

            // Reflect does not affect special moves
            Assert.That(contextWithReflect.FinalDamage, Is.EqualTo(contextWithoutReflect.FinalDamage));
        }

        #endregion

        #region Light Screen Tests

        [Test]
        public void Process_LightScreen_SpecialMove_ReducesDamage()
        {
            var lightScreenData = SideConditionCatalog.GetByType(SideCondition.LightScreen);
            _defenderSlot.Side.AddSideCondition(lightScreenData, 5);

            var contextWithScreen = _pipeline.Calculate(_attackerSlot, _defenderSlot, _specialMove, _field, fixedRandomValue: 1.0f);
            _defenderSlot.Side.RemoveSideCondition(SideCondition.LightScreen);
            var contextWithoutScreen = _pipeline.Calculate(_attackerSlot, _defenderSlot, _specialMove, _field, fixedRandomValue: 1.0f);

            // Light Screen reduces special damage by 50% in singles
            Assert.That(contextWithScreen.FinalDamage, Is.LessThan(contextWithoutScreen.FinalDamage));
            Assert.That(contextWithScreen.Multiplier, Is.LessThanOrEqualTo(contextWithoutScreen.Multiplier * 0.51f));
        }

        [Test]
        public void Process_LightScreen_PhysicalMove_NoEffect()
        {
            var lightScreenData = SideConditionCatalog.GetByType(SideCondition.LightScreen);
            _defenderSlot.Side.AddSideCondition(lightScreenData, 5);

            var contextWithScreen = _pipeline.Calculate(_attackerSlot, _defenderSlot, _physicalMove, _field, fixedRandomValue: 1.0f);
            _defenderSlot.Side.RemoveSideCondition(SideCondition.LightScreen);
            var contextWithoutScreen = _pipeline.Calculate(_attackerSlot, _defenderSlot, _physicalMove, _field, fixedRandomValue: 1.0f);

            // Light Screen does not affect physical moves
            Assert.That(contextWithScreen.FinalDamage, Is.EqualTo(contextWithoutScreen.FinalDamage));
        }

        #endregion

        #region Aurora Veil Tests

        [Test]
        public void Process_AuroraVeil_PhysicalMove_ReducesDamage()
        {
            var auroraVeilData = SideConditionCatalog.GetByType(SideCondition.AuroraVeil);
            _defenderSlot.Side.AddSideCondition(auroraVeilData, 5);

            var contextWithVeil = _pipeline.Calculate(_attackerSlot, _defenderSlot, _physicalMove, _field, fixedRandomValue: 1.0f);
            _defenderSlot.Side.RemoveSideCondition(SideCondition.AuroraVeil);
            var contextWithoutVeil = _pipeline.Calculate(_attackerSlot, _defenderSlot, _physicalMove, _field, fixedRandomValue: 1.0f);

            // Aurora Veil reduces all damage by 50% in singles
            Assert.That(contextWithVeil.FinalDamage, Is.LessThan(contextWithoutVeil.FinalDamage));
            Assert.That(contextWithVeil.Multiplier, Is.LessThanOrEqualTo(contextWithoutVeil.Multiplier * 0.51f));
        }

        [Test]
        public void Process_AuroraVeil_SpecialMove_ReducesDamage()
        {
            var auroraVeilData = SideConditionCatalog.GetByType(SideCondition.AuroraVeil);
            _defenderSlot.Side.AddSideCondition(auroraVeilData, 5);

            var contextWithVeil = _pipeline.Calculate(_attackerSlot, _defenderSlot, _specialMove, _field, fixedRandomValue: 1.0f);
            _defenderSlot.Side.RemoveSideCondition(SideCondition.AuroraVeil);
            var contextWithoutVeil = _pipeline.Calculate(_attackerSlot, _defenderSlot, _specialMove, _field, fixedRandomValue: 1.0f);

            // Aurora Veil reduces all damage by 50% in singles
            Assert.That(contextWithVeil.FinalDamage, Is.LessThan(contextWithoutVeil.FinalDamage));
            Assert.That(contextWithVeil.Multiplier, Is.LessThanOrEqualTo(contextWithoutVeil.Multiplier * 0.51f));
        }

        #endregion

        #region No Screen Tests

        [Test]
        public void Process_NoScreen_NoEffect()
        {
            // No screens added

            var context = _pipeline.Calculate(_attackerSlot, _defenderSlot, _physicalMove, _field, fixedRandomValue: 1.0f);

            // Should calculate normally without screen reduction
            Assert.That(context.FinalDamage, Is.GreaterThan(0));
        }

        #endregion
    }
}

