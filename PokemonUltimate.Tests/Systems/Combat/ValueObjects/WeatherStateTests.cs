using NUnit.Framework;
using PokemonUltimate.Combat.ValueObjects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Systems.Combat.ValueObjects
{
    /// <summary>
    /// Tests for WeatherState - Value Object representing weather state on the battlefield.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.12: Weather System
    /// **Documentation**: See `docs/features/2-combat-system/2.12-weather-system/README.md`
    /// </remarks>
    [TestFixture]
    public class WeatherStateTests
    {
        #region Constructor Tests

        [Test]
        public void Constructor_Default_NoWeatherActive()
        {
            // Act
            var state = new WeatherState();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(state.Weather, Is.EqualTo(Weather.None));
                Assert.That(state.Duration, Is.EqualTo(0));
                Assert.That(state.WeatherData, Is.Null);
                Assert.That(state.IsActive, Is.False);
                Assert.That(state.IsInfinite, Is.False);
            });
        }

        #endregion

        #region SetWeather Tests

        [Test]
        public void SetWeather_RainWithDuration_SetsWeather()
        {
            // Arrange
            var state = new WeatherState();
            const int duration = 5;

            // Act
            var newState = state.SetWeather(Weather.Rain, duration);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(newState.Weather, Is.EqualTo(Weather.Rain));
                Assert.That(newState.Duration, Is.EqualTo(duration));
                Assert.That(newState.IsActive, Is.True);
                Assert.That(newState.IsInfinite, Is.False);
            });
        }

        [Test]
        public void SetWeather_None_ClearsWeather()
        {
            // Arrange
            var state = new WeatherState();
            state = state.SetWeather(Weather.Rain, 5);

            // Act
            var cleared = state.SetWeather(Weather.None, 0);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(cleared.Weather, Is.EqualTo(Weather.None));
                Assert.That(cleared.Duration, Is.EqualTo(0));
                Assert.That(cleared.IsActive, Is.False);
            });
        }

        [Test]
        public void SetWeather_ZeroDuration_SetsInfinite()
        {
            // Arrange
            var state = new WeatherState();

            // Act
            var newState = state.SetWeather(Weather.Sun, 0);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(newState.Weather, Is.EqualTo(Weather.Sun));
                Assert.That(newState.Duration, Is.EqualTo(0));
                Assert.That(newState.IsActive, Is.True);
                Assert.That(newState.IsInfinite, Is.True);
            });
        }

        [Test]
        public void SetWeather_WithWeatherData_SetsWeatherData()
        {
            // Arrange
            var state = new WeatherState();
            // WeatherData can be null for testing purposes
            var weatherData = (PokemonUltimate.Core.Blueprints.WeatherData)null;

            // Act
            var newState = state.SetWeather(Weather.Rain, 5, weatherData);

            // Assert
            Assert.That(newState.WeatherData, Is.EqualTo(weatherData));
        }

        [Test]
        public void SetWeather_Immutable_OriginalUnchanged()
        {
            // Arrange
            var original = new WeatherState();

            // Act
            var modified = original.SetWeather(Weather.Rain, 5);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.Weather, Is.EqualTo(Weather.None));
                Assert.That(modified.Weather, Is.EqualTo(Weather.Rain));
            });
        }

        #endregion

        #region Clear Tests

        [Test]
        public void Clear_WithActiveWeather_ClearsWeather()
        {
            // Arrange
            var state = new WeatherState();
            state = state.SetWeather(Weather.Rain, 5);

            // Act
            var cleared = state.Clear();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(cleared.Weather, Is.EqualTo(Weather.None));
                Assert.That(cleared.Duration, Is.EqualTo(0));
                Assert.That(cleared.IsActive, Is.False);
            });
        }

        [Test]
        public void Clear_AlreadyCleared_RemainsCleared()
        {
            // Arrange
            var state = new WeatherState();

            // Act
            var cleared = state.Clear();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(cleared.Weather, Is.EqualTo(Weather.None));
                Assert.That(cleared.IsActive, Is.False);
            });
        }

        [Test]
        public void Clear_Immutable_OriginalUnchanged()
        {
            // Arrange
            var original = new WeatherState();
            original = original.SetWeather(Weather.Rain, 5);

            // Act
            var cleared = original.Clear();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.Weather, Is.EqualTo(Weather.Rain));
                Assert.That(cleared.Weather, Is.EqualTo(Weather.None));
            });
        }

        #endregion

        #region DecrementDuration Tests

        [Test]
        public void DecrementDuration_WithDuration_Decrements()
        {
            // Arrange
            var state = new WeatherState();
            state = state.SetWeather(Weather.Rain, 5);

            // Act
            var decremented = state.DecrementDuration();

            // Assert
            Assert.That(decremented.Duration, Is.EqualTo(4));
        }

        [Test]
        public void DecrementDuration_DurationReachesZero_ClearsWeather()
        {
            // Arrange
            var state = new WeatherState();
            state = state.SetWeather(Weather.Rain, 1);

            // Act
            var decremented = state.DecrementDuration();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(decremented.Weather, Is.EqualTo(Weather.None));
                Assert.That(decremented.IsActive, Is.False);
            });
        }

        [Test]
        public void DecrementDuration_InfiniteDuration_NoChange()
        {
            // Arrange
            var state = new WeatherState();
            state = state.SetWeather(Weather.Sun, 0); // Infinite duration

            // Act
            var decremented = state.DecrementDuration();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(decremented.Weather, Is.EqualTo(Weather.Sun));
                Assert.That(decremented.Duration, Is.EqualTo(0));
                Assert.That(decremented.IsInfinite, Is.True);
            });
        }

        [Test]
        public void DecrementDuration_NoWeather_NoChange()
        {
            // Arrange
            var state = new WeatherState();

            // Act
            var decremented = state.DecrementDuration();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(decremented.Weather, Is.EqualTo(Weather.None));
                Assert.That(decremented.IsActive, Is.False);
            });
        }

        [Test]
        public void DecrementDuration_Immutable_OriginalUnchanged()
        {
            // Arrange
            var original = new WeatherState();
            original = original.SetWeather(Weather.Rain, 5);

            // Act
            var decremented = original.DecrementDuration();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.Duration, Is.EqualTo(5));
                Assert.That(decremented.Duration, Is.EqualTo(4));
            });
        }

        #endregion
    }
}
