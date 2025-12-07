using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Combat.Integration.View;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Actions
{
    /// <summary>
    /// Sets or changes the weather condition on the battlefield.
    /// Handles primal weather overwrite protection.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.12: Weather System
    /// **Documentation**: See `docs/features/2-combat-system/2.12-weather-system/README.md`
    /// </remarks>
    public class SetWeatherAction : BattleAction
    {
        /// <summary>
        /// The weather condition to set.
        /// </summary>
        public Weather Weather { get; }

        /// <summary>
        /// The duration of the weather in turns (0 = infinite).
        /// </summary>
        public int Duration { get; }

        /// <summary>
        /// The weather data for this weather condition.
        /// </summary>
        public WeatherData WeatherData { get; }

        /// <summary>
        /// Creates a new set weather action.
        /// </summary>
        /// <param name="user">The slot that initiated this weather change. Can be null for system actions.</param>
        /// <param name="weather">The weather to set. Use Weather.None to clear.</param>
        /// <param name="duration">Duration in turns. 0 means infinite duration.</param>
        /// <param name="weatherData">The weather data for this weather condition. Can be null if not available.</param>
        public SetWeatherAction(BattleSlot user, Weather weather, int duration, WeatherData weatherData = null) : base(user)
        {
            Weather = weather;
            Duration = duration;
            WeatherData = weatherData;
        }

        /// <summary>
        /// Sets the weather on the battlefield.
        /// Handles primal weather overwrite protection.
        /// </summary>
        public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            // Clear weather if None is specified
            if (Weather == Weather.None)
            {
                field.ClearWeather();
                return Enumerable.Empty<BattleAction>();
            }

            // Check if current weather is primal and cannot be overwritten
            if (field.WeatherData != null && !field.WeatherData.CanBeOverwritten)
            {
                // Current weather is primal, check if new weather can overwrite it
                if (WeatherData != null && WeatherData.CanBeOverwritten)
                {
                    // Trying to overwrite primal with normal weather - ignore
                    return Enumerable.Empty<BattleAction>();
                }
            }

            // Set the weather
            field.SetWeather(Weather, Duration, WeatherData);

            // Note: Weather-change effects are processed by ActionProcessorObserver
            // This keeps actions simple and decoupled from processors

            return Enumerable.Empty<BattleAction>();
        }

        /// <summary>
        /// Plays the weather change animation.
        /// </summary>
        public override Task ExecuteVisual(IBattleView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            // Weather animation not yet implemented in IBattleView
            // For now, just return completed task
            return Task.CompletedTask;
        }
    }
}

