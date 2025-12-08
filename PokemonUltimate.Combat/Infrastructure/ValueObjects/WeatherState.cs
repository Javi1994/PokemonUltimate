using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Infrastructure.ValueObjects
{
    /// <summary>
    /// Value Object representing the current weather state on the battlefield.
    /// Encapsulates weather type, duration, and associated data.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.12: Weather System
    /// **Documentation**: See `docs/features/2-combat-system/2.12-weather-system/README.md`
    /// </remarks>
    public class WeatherState
    {
        /// <summary>
        /// Creates a new WeatherState instance with no weather active.
        /// </summary>
        public WeatherState()
        {
            Weather = Weather.None;
            Duration = 0;
            WeatherData = null;
        }

        /// <summary>
        /// Creates a new WeatherState instance with the specified values.
        /// </summary>
        /// <param name="weather">The weather type.</param>
        /// <param name="duration">The remaining duration in turns. 0 means infinite duration.</param>
        /// <param name="weatherData">The weather data. Can be null.</param>
        private WeatherState(Weather weather, int duration, WeatherData weatherData)
        {
            Weather = weather;
            Duration = duration;
            WeatherData = weatherData;
        }

        /// <summary>
        /// The current weather condition on the battlefield.
        /// </summary>
        public Weather Weather { get; }

        /// <summary>
        /// The remaining duration of the current weather in turns.
        /// 0 means infinite duration (primal weather or indefinite weather).
        /// </summary>
        public int Duration { get; }

        /// <summary>
        /// The weather data for the current weather condition.
        /// Null if no weather is active.
        /// </summary>
        public WeatherData WeatherData { get; }

        /// <summary>
        /// True if weather is currently active (not None).
        /// </summary>
        public bool IsActive => Weather != Weather.None;

        /// <summary>
        /// True if the weather has infinite duration (Duration == 0 and weather is active).
        /// </summary>
        public bool IsInfinite => IsActive && Duration == 0;

        /// <summary>
        /// Creates a new WeatherState instance with the weather set.
        /// </summary>
        /// <param name="weather">The weather to set. Use Weather.None to clear.</param>
        /// <param name="duration">The duration in turns. 0 means infinite.</param>
        /// <param name="weatherData">The weather data. Can be null.</param>
        /// <returns>A new WeatherState instance with the weather set.</returns>
        public WeatherState SetWeather(Weather weather, int duration, WeatherData weatherData = null)
        {
            if (weather == Weather.None)
                return new WeatherState();

            return new WeatherState(weather, duration, weatherData);
        }

        /// <summary>
        /// Creates a new WeatherState instance with the weather cleared.
        /// </summary>
        /// <returns>A new WeatherState instance with no weather active.</returns>
        public WeatherState Clear()
        {
            return new WeatherState();
        }

        /// <summary>
        /// Creates a new WeatherState instance with the duration decremented by one turn.
        /// If duration reaches 0 and weather is not infinite, clears the weather.
        /// </summary>
        /// <returns>A new WeatherState instance with decremented duration, or cleared if duration expired.</returns>
        public WeatherState DecrementDuration()
        {
            if (!IsActive)
                return this;

            if (Duration == 0)
                return this; // Infinite duration

            var newDuration = Duration - 1;
            if (newDuration <= 0)
                return new WeatherState(); // Duration expired, clear weather

            return new WeatherState(Weather, newDuration, WeatherData);
        }
    }
}
