using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions.Validation;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Combat.View.Definition;
using PokemonUltimate.Core.Data.Blueprints;
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
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

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
        /// <param name="handlerRegistry">The handler registry. If null, creates and initializes a default one.</param>
        public SetWeatherAction(BattleSlot user, Weather weather, int duration, WeatherData weatherData = null, CombatEffectHandlerRegistry handlerRegistry = null) : base(user)
        {
            Weather = weather;
            Duration = duration;
            WeatherData = weatherData;
            _handlerRegistry = handlerRegistry ?? CombatEffectHandlerRegistry.CreateDefault();
        }

        /// <summary>
        /// Sets the weather on the battlefield.
        /// Handles primal weather overwrite protection.
        /// </summary>
        public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            ActionValidators.ValidateField(field);

            // Clear weather if None is specified
            if (Weather == Weather.None)
            {
                field.ClearWeather();
                return Enumerable.Empty<BattleAction>();
            }

            // Use Field Condition Handler to validate weather can be set (eliminates complex validation logic)
            var fieldHandler = _handlerRegistry.GetFieldConditionHandler();
            if (!fieldHandler.CanSetWeather(field, Weather, WeatherData))
            {
                // Weather cannot be set (e.g., trying to overwrite primal weather)
                return Enumerable.Empty<BattleAction>();
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
            ActionValidators.ValidateView(view);

            // Weather animation not yet implemented in IBattleView
            // For now, just return completed task
            return Task.CompletedTask;
        }
    }
}

