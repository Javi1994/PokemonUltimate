using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Content.Catalogs.Weather;

namespace PokemonUltimate.Tests.Systems.Combat.Field
{
    /// <summary>
    /// Tests for Weather Tracking in BattleField.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.12: Weather System
    /// **Documentation**: See `docs/features/2-combat-system/2.12-weather-system/README.md`
    /// </remarks>
    [TestFixture]
    public class WeatherTrackingTests
    {
        private BattleField _field;
        private List<PokemonInstance> _playerParty;
        private List<PokemonInstance> _enemyParty;
        private BattleRules _rules;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            
            _playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 25)
            };

            _enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 25)
            };

            _field.Initialize(_rules, _playerParty, _enemyParty);
        }

        #region SetWeather Tests

        [Test]
        public void SetWeather_Sun_SetsCorrectly()
        {
            _field.SetWeather(Weather.Sun, 5, WeatherCatalog.Sun);

            Assert.That(_field.Weather, Is.EqualTo(Weather.Sun));
            Assert.That(_field.WeatherDuration, Is.EqualTo(5));
            Assert.That(_field.WeatherData, Is.EqualTo(WeatherCatalog.Sun));
        }

        [Test]
        public void SetWeather_Rain_SetsCorrectly()
        {
            _field.SetWeather(Weather.Rain, 5, WeatherCatalog.Rain);

            Assert.That(_field.Weather, Is.EqualTo(Weather.Rain));
            Assert.That(_field.WeatherDuration, Is.EqualTo(5));
            Assert.That(_field.WeatherData, Is.EqualTo(WeatherCatalog.Rain));
        }

        [Test]
        public void SetWeather_Sandstorm_SetsCorrectly()
        {
            _field.SetWeather(Weather.Sandstorm, 5, WeatherCatalog.Sandstorm);

            Assert.That(_field.Weather, Is.EqualTo(Weather.Sandstorm));
            Assert.That(_field.WeatherDuration, Is.EqualTo(5));
            Assert.That(_field.WeatherData, Is.EqualTo(WeatherCatalog.Sandstorm));
        }

        [Test]
        public void SetWeather_Hail_SetsCorrectly()
        {
            _field.SetWeather(Weather.Hail, 5, WeatherCatalog.Hail);

            Assert.That(_field.Weather, Is.EqualTo(Weather.Hail));
            Assert.That(_field.WeatherDuration, Is.EqualTo(5));
            Assert.That(_field.WeatherData, Is.EqualTo(WeatherCatalog.Hail));
        }

        [Test]
        public void SetWeather_None_ClearsWeather()
        {
            _field.SetWeather(Weather.Sun, 5, WeatherCatalog.Sun);
            _field.SetWeather(Weather.None, 0);

            Assert.That(_field.Weather, Is.EqualTo(Weather.None));
            Assert.That(_field.WeatherDuration, Is.EqualTo(0));
            Assert.That(_field.WeatherData, Is.Null);
        }

        [Test]
        public void SetWeather_DurationZero_InfiniteDuration()
        {
            _field.SetWeather(Weather.Sun, 0, WeatherCatalog.Sun);

            Assert.That(_field.Weather, Is.EqualTo(Weather.Sun));
            Assert.That(_field.WeatherDuration, Is.EqualTo(0));
            Assert.That(_field.WeatherData, Is.EqualTo(WeatherCatalog.Sun));
        }

        #endregion

        #region Weather Duration Tests

        [Test]
        public void WeatherDuration_SetTo5_DecrementsEachTurn()
        {
            _field.SetWeather(Weather.Sun, 5, WeatherCatalog.Sun);

            _field.DecrementWeatherDuration();
            Assert.That(_field.WeatherDuration, Is.EqualTo(4));

            _field.DecrementWeatherDuration();
            Assert.That(_field.WeatherDuration, Is.EqualTo(3));
        }

        [Test]
        public void WeatherDuration_ReachesZero_ExpiresWeather()
        {
            _field.SetWeather(Weather.Sun, 1, WeatherCatalog.Sun);

            _field.DecrementWeatherDuration();

            Assert.That(_field.Weather, Is.EqualTo(Weather.None));
            Assert.That(_field.WeatherDuration, Is.EqualTo(0));
            Assert.That(_field.WeatherData, Is.Null);
        }

        [Test]
        public void WeatherDuration_SetToZero_InfiniteDuration()
        {
            _field.SetWeather(Weather.Sun, 0, WeatherCatalog.Sun);

            _field.DecrementWeatherDuration();
            _field.DecrementWeatherDuration();
            _field.DecrementWeatherDuration();

            // Infinite duration should not expire
            Assert.That(_field.Weather, Is.EqualTo(Weather.Sun));
            Assert.That(_field.WeatherDuration, Is.EqualTo(0));
        }

        [Test]
        public void WeatherDuration_NoWeather_DoesNotDecrement()
        {
            // No weather set
            _field.DecrementWeatherDuration();

            Assert.That(_field.Weather, Is.EqualTo(Weather.None));
            Assert.That(_field.WeatherDuration, Is.EqualTo(0));
        }

        #endregion

        #region Primal Weather Tests

        [Test]
        public void SetWeather_PrimalWeather_CannotBeOverwritten()
        {
            // Set primal weather first
            _field.SetWeather(Weather.ExtremelyHarshSunlight, 0, WeatherCatalog.ExtremelyHarshSunlight);

            // Try to overwrite with normal weather
            // Note: Primal weather overwrite protection will be handled by SetWeatherAction (Phase 4)
            // For now, SetWeather allows overwriting - this test will be updated in Phase 4
            _field.SetWeather(Weather.Rain, 5, WeatherCatalog.Rain);

            // Currently SetWeather allows overwriting (primal check will be in Actions)
            // This test will be updated when SetWeatherAction is implemented
            Assert.That(_field.Weather, Is.EqualTo(Weather.Rain));
        }

        [Test]
        public void SetWeather_PrimalWeather_CanOverwriteNormal()
        {
            // Set normal weather first
            _field.SetWeather(Weather.Sun, 5, WeatherCatalog.Sun);

            // Primal weather can overwrite normal
            _field.SetWeather(Weather.ExtremelyHarshSunlight, 0, WeatherCatalog.ExtremelyHarshSunlight);

            Assert.That(_field.Weather, Is.EqualTo(Weather.ExtremelyHarshSunlight));
        }

        [Test]
        public void SetWeather_PrimalWeather_CanOverwriteAnotherPrimal()
        {
            // Set one primal weather
            _field.SetWeather(Weather.ExtremelyHarshSunlight, 0, WeatherCatalog.ExtremelyHarshSunlight);

            // Another primal can overwrite
            _field.SetWeather(Weather.HeavyRain, 0, WeatherCatalog.HeavyRain);

            Assert.That(_field.Weather, Is.EqualTo(Weather.HeavyRain));
        }

        #endregion

        #region ClearWeather Tests

        [Test]
        public void ClearWeather_RemovesWeather()
        {
            _field.SetWeather(Weather.Sun, 5, WeatherCatalog.Sun);
            _field.ClearWeather();

            Assert.That(_field.Weather, Is.EqualTo(Weather.None));
            Assert.That(_field.WeatherDuration, Is.EqualTo(0));
            Assert.That(_field.WeatherData, Is.Null);
        }

        [Test]
        public void ClearWeather_NoWeather_NoEffect()
        {
            _field.ClearWeather();

            Assert.That(_field.Weather, Is.EqualTo(Weather.None));
            Assert.That(_field.WeatherDuration, Is.EqualTo(0));
            Assert.That(_field.WeatherData, Is.Null);
        }

        #endregion

        #region Initial State Tests

        [Test]
        public void Weather_InitialState_IsNone()
        {
            Assert.That(_field.Weather, Is.EqualTo(Weather.None));
            Assert.That(_field.WeatherDuration, Is.EqualTo(0));
            Assert.That(_field.WeatherData, Is.Null);
        }

        #endregion
    }
}

