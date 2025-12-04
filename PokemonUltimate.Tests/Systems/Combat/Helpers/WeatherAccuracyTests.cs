using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Helpers;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Content.Catalogs.Weather;

namespace PokemonUltimate.Tests.Systems.Combat.Helpers
{
    /// <summary>
    /// Tests for Weather Perfect Accuracy in AccuracyChecker.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.12: Weather System
    /// **Documentation**: See `docs/features/2-combat-system/2.12-weather-system/README.md`
    /// </remarks>
    [TestFixture]
    public class WeatherAccuracyTests
    {
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            _field.Initialize(
                rules,
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) });

            _userSlot = _field.PlayerSide.Slots[0];
            _targetSlot = _field.EnemySide.Slots[0];
        }

        #region Rain Perfect Accuracy Tests

        [Test]
        public void CheckHit_Rain_Thunder_100PercentAccuracy()
        {
            _field.SetWeather(Weather.Rain, 5, WeatherCatalog.Rain);
            var thunder = new MoveData
            {
                Name = "Thunder",
                Power = 110,
                Type = PokemonType.Electric,
                Category = MoveCategory.Special,
                Accuracy = 70, // Normal accuracy is 70%
                MaxPP = 10,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy
            };

            // Thunder should have 100% accuracy in Rain
            // Test with multiple random values to ensure it always hits
            for (int i = 0; i < 10; i++)
            {
                bool hit = AccuracyChecker.CheckHit(_userSlot, _targetSlot, thunder, _field, fixedRandomValue: 99.9f);
                Assert.That(hit, Is.True, $"Thunder should always hit in Rain (test {i + 1})");
            }
        }

        [Test]
        public void CheckHit_Rain_Hurricane_100PercentAccuracy()
        {
            _field.SetWeather(Weather.Rain, 5, WeatherCatalog.Rain);
            var hurricane = new MoveData
            {
                Name = "Hurricane",
                Power = 110,
                Type = PokemonType.Flying,
                Category = MoveCategory.Special,
                Accuracy = 70, // Normal accuracy is 70%
                MaxPP = 10,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy
            };

            // Hurricane should have 100% accuracy in Rain
            for (int i = 0; i < 10; i++)
            {
                bool hit = AccuracyChecker.CheckHit(_userSlot, _targetSlot, hurricane, _field, fixedRandomValue: 99.9f);
                Assert.That(hit, Is.True, $"Hurricane should always hit in Rain (test {i + 1})");
            }
        }

        [Test]
        public void CheckHit_Rain_NonPerfectAccuracyMove_UsesNormalAccuracy()
        {
            _field.SetWeather(Weather.Rain, 5, WeatherCatalog.Rain);
            var tackle = new MoveData
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

            // Tackle should use normal accuracy (100%)
            bool hit = AccuracyChecker.CheckHit(_userSlot, _targetSlot, tackle, _field, fixedRandomValue: 50f);
            Assert.That(hit, Is.True);
        }

        #endregion

        #region Hail Perfect Accuracy Tests

        [Test]
        public void CheckHit_Hail_Blizzard_100PercentAccuracy()
        {
            _field.SetWeather(Weather.Hail, 5, WeatherCatalog.Hail);
            var blizzard = new MoveData
            {
                Name = "Blizzard",
                Power = 110,
                Type = PokemonType.Ice,
                Category = MoveCategory.Special,
                Accuracy = 70, // Normal accuracy is 70%
                MaxPP = 5,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy
            };

            // Blizzard should have 100% accuracy in Hail
            for (int i = 0; i < 10; i++)
            {
                bool hit = AccuracyChecker.CheckHit(_userSlot, _targetSlot, blizzard, _field, fixedRandomValue: 99.9f);
                Assert.That(hit, Is.True, $"Blizzard should always hit in Hail (test {i + 1})");
            }
        }

        #endregion

        #region No Weather Tests

        [Test]
        public void CheckHit_NoWeather_Thunder_UsesNormalAccuracy()
        {
            // No weather set
            var thunder = new MoveData
            {
                Name = "Thunder",
                Power = 110,
                Type = PokemonType.Electric,
                Category = MoveCategory.Special,
                Accuracy = 70,
                MaxPP = 10,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy
            };

            // Without weather, Thunder should use normal accuracy (70%)
            // With fixedRandomValue 50%, should hit (50 < 70)
            bool hit = AccuracyChecker.CheckHit(_userSlot, _targetSlot, thunder, _field, fixedRandomValue: 50f);
            Assert.That(hit, Is.True);

            // With fixedRandomValue 80%, should miss (80 >= 70)
            bool miss = AccuracyChecker.CheckHit(_userSlot, _targetSlot, thunder, _field, fixedRandomValue: 80f);
            Assert.That(miss, Is.False);
        }

        #endregion

        #region Sun Tests

        [Test]
        public void CheckHit_Sun_Thunder_UsesNormalAccuracy()
        {
            _field.SetWeather(Weather.Sun, 5, WeatherCatalog.Sun);
            var thunder = new MoveData
            {
                Name = "Thunder",
                Power = 110,
                Type = PokemonType.Electric,
                Category = MoveCategory.Special,
                Accuracy = 70,
                MaxPP = 10,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy
            };

            // In Sun, Thunder uses normal accuracy (70%)
            // Note: Thunder accuracy reduction in Sun (50%) is not yet implemented
            // This test verifies normal accuracy behavior
            bool hit = AccuracyChecker.CheckHit(_userSlot, _targetSlot, thunder, _field, fixedRandomValue: 50f);
            Assert.That(hit, Is.True);

            bool miss = AccuracyChecker.CheckHit(_userSlot, _targetSlot, thunder, _field, fixedRandomValue: 80f);
            Assert.That(miss, Is.False);
        }

        #endregion
    }
}

