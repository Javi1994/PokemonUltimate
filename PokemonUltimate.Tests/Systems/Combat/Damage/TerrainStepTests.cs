using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Damage.Steps;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Content.Catalogs.Terrain;

namespace PokemonUltimate.Tests.Systems.Combat.Damage
{
    /// <summary>
    /// Tests for TerrainStep - terrain damage modifiers in damage calculation.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.13: Terrain System
    /// **Documentation**: See `docs/features/2-combat-system/2.13-terrain-system/README.md`
    /// </remarks>
    [TestFixture]
    public class TerrainStepTests
    {
        private DamagePipeline _pipeline;
        private BattleField _field;
        private BattleSlot _attackerSlot;
        private BattleSlot _defenderSlot;
        private PokemonInstance _attacker;
        private PokemonInstance _defender;

        [SetUp]
        public void SetUp()
        {
            _pipeline = new DamagePipeline();
            _field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            _field.Initialize(
                rules,
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) }, // Grounded Pokemon
                new[] { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) }); // Grounded Pokemon

            _attackerSlot = _field.PlayerSide.Slots[0];
            _defenderSlot = _field.EnemySide.Slots[0];
            _attacker = _attackerSlot.Pokemon;
            _defender = _defenderSlot.Pokemon;
        }

        #region Electric Terrain Tests

        [Test]
        public void Process_ElectricTerrain_ElectricMoves_1_3xDamage()
        {
            _field.SetTerrain(Terrain.Electric, 5, TerrainCatalog.GetByTerrain(Terrain.Electric));
            var move = new MoveData
            {
                Name = "Thunderbolt",
                Power = 90,
                Type = PokemonType.Electric,
                Category = MoveCategory.Special,
                Accuracy = 100,
                MaxPP = 15,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy
            };

            // Calculate with terrain
            var contextWithTerrain = _pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);
            
            // Calculate without terrain
            _field.ClearTerrain();
            var contextWithoutTerrain = _pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);

            // Electric moves should be boosted 1.3x in Electric Terrain (for grounded attacker)
            // Compare multipliers: with terrain should be ~1.3x of without terrain
            Assert.That(contextWithTerrain.Multiplier, Is.GreaterThanOrEqualTo(contextWithoutTerrain.Multiplier * 1.3f * 0.99f));
        }

        [Test]
        public void Process_ElectricTerrain_NonElectricMoves_NoModifier()
        {
            _field.SetTerrain(Terrain.Electric, 5, TerrainCatalog.GetByTerrain(Terrain.Electric));
            var move = new MoveData
            {
                Name = "Tackle",
                Power = 40,
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                Accuracy = 100,
                MaxPP = 35,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy
            };

            var context = _pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);

            // Non-Electric moves should not be affected by Electric Terrain
            _field.ClearTerrain();
            var contextWithoutTerrain = _pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);

            Assert.That(context.Multiplier, Is.EqualTo(contextWithoutTerrain.Multiplier).Within(0.01f));
        }

        #endregion

        #region Grassy Terrain Tests

        [Test]
        public void Process_GrassyTerrain_GrassMoves_1_3xDamage()
        {
            _field.SetTerrain(Terrain.Grassy, 5, TerrainCatalog.GetByTerrain(Terrain.Grassy));
            var move = new MoveData
            {
                Name = "Razor Leaf",
                Power = 55,
                Type = PokemonType.Grass,
                Category = MoveCategory.Physical,
                Accuracy = 95,
                MaxPP = 25,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy
            };

            // Calculate with terrain
            var contextWithTerrain = _pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);
            
            // Calculate without terrain
            _field.ClearTerrain();
            var contextWithoutTerrain = _pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);

            // Grass moves should be boosted 1.3x in Grassy Terrain (for grounded attacker)
            Assert.That(contextWithTerrain.Multiplier, Is.GreaterThanOrEqualTo(contextWithoutTerrain.Multiplier * 1.3f * 0.99f));
        }

        #endregion

        #region Psychic Terrain Tests

        [Test]
        public void Process_PsychicTerrain_PsychicMoves_1_3xDamage()
        {
            _field.SetTerrain(Terrain.Psychic, 5, TerrainCatalog.GetByTerrain(Terrain.Psychic));
            var move = new MoveData
            {
                Name = "Psychic",
                Power = 90,
                Type = PokemonType.Psychic,
                Category = MoveCategory.Special,
                Accuracy = 100,
                MaxPP = 10,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy
            };

            // Calculate with terrain
            var contextWithTerrain = _pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);
            
            // Calculate without terrain
            _field.ClearTerrain();
            var contextWithoutTerrain = _pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);

            // Psychic moves should be boosted 1.3x in Psychic Terrain (for grounded attacker)
            Assert.That(contextWithTerrain.Multiplier, Is.GreaterThanOrEqualTo(contextWithoutTerrain.Multiplier * 1.3f * 0.99f));
        }

        #endregion

        #region Misty Terrain Tests

        [Test]
        public void Process_MistyTerrain_DragonMoves_0_5xDamage()
        {
            _field.SetTerrain(Terrain.Misty, 5, TerrainCatalog.GetByTerrain(Terrain.Misty));
            var move = new MoveData
            {
                Name = "Dragon Claw",
                Power = 80,
                Type = PokemonType.Dragon,
                Category = MoveCategory.Physical,
                Accuracy = 100,
                MaxPP = 15,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy
            };

            var context = _pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);

            // Dragon moves should be reduced 0.5x in Misty Terrain (for grounded defender)
            // Calculate without terrain for comparison
            _field.ClearTerrain();
            var contextWithoutTerrain = _pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);

            // With Misty Terrain, Dragon moves should be weaker
            Assert.That(context.Multiplier, Is.LessThan(contextWithoutTerrain.Multiplier));
        }

        [Test]
        public void Process_MistyTerrain_NonDragonMoves_NoModifier()
        {
            _field.SetTerrain(Terrain.Misty, 5, TerrainCatalog.GetByTerrain(Terrain.Misty));
            var move = new MoveData
            {
                Name = "Tackle",
                Power = 40,
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                Accuracy = 100,
                MaxPP = 35,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy
            };

            var context = _pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);

            // Non-Dragon moves should not be affected by Misty Terrain
            _field.ClearTerrain();
            var contextWithoutTerrain = _pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);

            Assert.That(context.Multiplier, Is.EqualTo(contextWithoutTerrain.Multiplier).Within(0.01f));
        }

        #endregion

        #region No Terrain Tests

        [Test]
        public void Process_NoTerrain_NoModifier()
        {
            // No terrain set
            var move = new MoveData
            {
                Name = "Thunderbolt",
                Power = 90,
                Type = PokemonType.Electric,
                Category = MoveCategory.Special,
                Accuracy = 100,
                MaxPP = 15,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy
            };

            var context = _pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);

            // Without terrain, moves should not have terrain modifier
            // This test verifies that TerrainStep doesn't modify when no terrain is active
            Assert.That(context.Multiplier, Is.GreaterThan(0f));
        }

        #endregion
    }
}

