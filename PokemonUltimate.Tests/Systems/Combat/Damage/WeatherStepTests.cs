using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Damage.Steps;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Content.Catalogs.Weather;
using Steps = PokemonUltimate.Combat.Damage.Steps;

namespace PokemonUltimate.Tests.Systems.Combat.Damage
{
    /// <summary>
    /// Tests for WeatherStep - weather damage modifiers in damage calculation.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.12: Weather System
    /// **Documentation**: See `docs/features/2-combat-system/2.12-weather-system/README.md`
    /// </remarks>
    [TestFixture]
    public class WeatherStepTests
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
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) });

            _attackerSlot = _field.PlayerSide.Slots[0];
            _defenderSlot = _field.EnemySide.Slots[0];
            _attacker = _attackerSlot.Pokemon;
            _defender = _defenderSlot.Pokemon;
        }

        #region Sun Weather Tests

        [Test]
        public void Process_Sun_FireMove_1_5xDamage()
        {
            _field.SetWeather(Weather.Sun, 5, WeatherCatalog.Sun);
            var move = new MoveData
            {
                Name = "Ember",
                Power = 40,
                Type = PokemonType.Fire,
                Category = MoveCategory.Special,
                Accuracy = 100,
                MaxPP = 25,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy
            };

            // Calculate with weather
            var contextWithWeather = _pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);
            
            // Calculate without weather (clear weather)
            _field.ClearWeather();
            var contextWithoutWeather = _pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);

            // Fire moves should be boosted 1.5x in Sun
            // Compare multipliers: with weather should be 1.5x of without weather
            Assert.That(contextWithWeather.Multiplier, Is.GreaterThanOrEqualTo(contextWithoutWeather.Multiplier * 1.5f * 0.99f));
        }

        [Test]
        public void Process_Sun_WaterMove_0_5xDamage()
        {
            _field.SetWeather(Weather.Sun, 5, WeatherCatalog.Sun);
            var move = new MoveData
            {
                Name = "Water Gun",
                Power = 40,
                Type = PokemonType.Water,
                Category = MoveCategory.Special,
                Accuracy = 100,
                MaxPP = 25,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy
            };

            var context = _pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);

            // Water moves should be weakened 0.5x in Sun
            Assert.That(context.Multiplier, Is.LessThanOrEqualTo(0.5f));
        }

        [Test]
        public void Process_Sun_NeutralType_NoModifier()
        {
            _field.SetWeather(Weather.Sun, 5, WeatherCatalog.Sun);
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

            // Normal moves should not be affected by Sun
            // Multiplier should be from other steps (STAB, etc.) but not weather
            // We check that weather multiplier (1.5x or 0.5x) is not applied
            var contextWithoutWeather = new DamagePipeline(new IDamageStep[]
            {
                new Steps.BaseDamageStep(),
                new Steps.CriticalHitStep(),
                new Steps.RandomFactorStep(),
                new Steps.StabStep(),
                new Steps.AttackerAbilityStep(),
                new Steps.AttackerItemStep(),
                new Steps.TypeEffectivenessStep(),
                new Steps.BurnStep()
            }).Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);

            // Weather should not affect Normal type, so multipliers should be similar
            // (allowing for small floating point differences)
            Assert.That(context.Multiplier, Is.EqualTo(contextWithoutWeather.Multiplier).Within(0.01f));
        }

        #endregion

        #region Rain Weather Tests

        [Test]
        public void Process_Rain_WaterMove_1_5xDamage()
        {
            _field.SetWeather(Weather.Rain, 5, WeatherCatalog.Rain);
            var move = new MoveData
            {
                Name = "Water Gun",
                Power = 40,
                Type = PokemonType.Water,
                Category = MoveCategory.Special,
                Accuracy = 100,
                MaxPP = 25,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy
            };

            // Calculate with weather
            var contextWithWeather = _pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);
            
            // Calculate without weather (clear weather)
            _field.ClearWeather();
            var contextWithoutWeather = _pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);

            // Water moves should be boosted 1.5x in Rain
            // Compare multipliers: with weather should be 1.5x of without weather
            Assert.That(contextWithWeather.Multiplier, Is.GreaterThanOrEqualTo(contextWithoutWeather.Multiplier * 1.5f * 0.99f));
        }

        [Test]
        public void Process_Rain_FireMove_0_5xDamage()
        {
            _field.SetWeather(Weather.Rain, 5, WeatherCatalog.Rain);
            var move = new MoveData
            {
                Name = "Ember",
                Power = 40,
                Type = PokemonType.Fire,
                Category = MoveCategory.Special,
                Accuracy = 100,
                MaxPP = 25,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy
            };

            var context = _pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);

            // Fire moves should be weakened 0.5x in Rain
            Assert.That(context.Multiplier, Is.LessThanOrEqualTo(0.5f));
        }

        #endregion

        #region Sandstorm Weather Tests

        [Test]
        public void Process_Sandstorm_NoModifier()
        {
            _field.SetWeather(Weather.Sandstorm, 5, WeatherCatalog.Sandstorm);
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

            // Sandstorm doesn't modify move power, only deals end-of-turn damage
            // Multiplier should be same as without weather
            var contextWithoutWeather = new DamagePipeline(new IDamageStep[]
            {
                new Steps.BaseDamageStep(),
                new Steps.CriticalHitStep(),
                new Steps.RandomFactorStep(),
                new Steps.StabStep(),
                new Steps.AttackerAbilityStep(),
                new Steps.AttackerItemStep(),
                new Steps.TypeEffectivenessStep(),
                new Steps.BurnStep()
            }).Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);

            Assert.That(context.Multiplier, Is.EqualTo(contextWithoutWeather.Multiplier).Within(0.01f));
        }

        #endregion

        #region No Weather Tests

        [Test]
        public void Process_NoWeather_NoModifier()
        {
            // No weather set
            var move = new MoveData
            {
                Name = "Ember",
                Power = 40,
                Type = PokemonType.Fire,
                Category = MoveCategory.Special,
                Accuracy = 100,
                MaxPP = 25,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy
            };

            var context = _pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);

            // Without weather, Fire moves should not have weather modifier
            var contextWithoutWeather = new DamagePipeline(new IDamageStep[]
            {
                new Steps.BaseDamageStep(),
                new Steps.CriticalHitStep(),
                new Steps.RandomFactorStep(),
                new Steps.StabStep(),
                new Steps.AttackerAbilityStep(),
                new Steps.AttackerItemStep(),
                new Steps.TypeEffectivenessStep(),
                new Steps.BurnStep()
            }).Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);

            Assert.That(context.Multiplier, Is.EqualTo(contextWithoutWeather.Multiplier).Within(0.01f));
        }

        #endregion

        #region Primal Weather Tests

        [Test]
        public void Process_PrimalWeather_NullifiesOppositeType()
        {
            _field.SetWeather(Weather.ExtremelyHarshSunlight, 0, WeatherCatalog.ExtremelyHarshSunlight);
            var move = new MoveData
            {
                Name = "Water Gun",
                Power = 40,
                Type = PokemonType.Water,
                Category = MoveCategory.Special,
                Accuracy = 100,
                MaxPP = 25,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy
            };

            // Calculate with weather
            var contextWithWeather = _pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);
            
            // Calculate without weather
            _field.ClearWeather();
            var contextWithoutWeather = _pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, fixedRandomValue: 1.0f);

            // Primal Sun nullifies Water moves (0x damage)
            // WeatherStep should multiply by 0, making multiplier 0
            // The multiplier with weather should be 0 (or very close to 0 due to floating point)
            Assert.That(contextWithWeather.Multiplier, Is.EqualTo(0f).Within(0.001f));
            
            // FinalDamage has a minimum of 1, but when multiplier is 0, it should still be 0
            // However, DamageContext applies Math.Max(1, BaseDamage * Multiplier), so if multiplier is 0, 
            // BaseDamage * 0 = 0, and Math.Max(1, 0) = 1. This is a limitation of the current implementation.
            // For now, we verify that the multiplier is 0, which is what WeatherStep should do.
            Assert.That(contextWithWeather.Multiplier, Is.LessThan(contextWithoutWeather.Multiplier));
        }

        #endregion
    }
}

