using PokemonUltimate.Combat.Field;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Actions.Checkers
{
    /// <summary>
    /// Verificador de aplicación de condiciones de campo (Weather, Terrain, Side Conditions).
    /// Valida si las condiciones pueden aplicarse según las reglas del juego.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/MEJORAS_PROPUESTAS.md`
    /// </remarks>
    public class FieldConditionApplicationChecker
    {
        /// <summary>
        /// Verifica si un clima puede ser establecido considerando protección de clima primal.
        /// </summary>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <param name="newWeather">El clima que se intenta establecer.</param>
        /// <param name="newWeatherData">Los datos del nuevo clima. Puede ser null.</param>
        /// <returns>True si el clima puede ser establecido, false en caso contrario.</returns>
        public bool CanSetWeather(BattleField field, Weather newWeather, WeatherData newWeatherData)
        {
            if (field == null)
                return false;

            // Clear weather if None is specified
            if (newWeather == Weather.None)
                return true;

            // Check if current weather is primal and cannot be overwritten
            if (field.WeatherData != null && !field.WeatherData.CanBeOverwritten)
            {
                // Current weather is primal, check if new weather can overwrite it
                if (newWeatherData != null && newWeatherData.CanBeOverwritten)
                {
                    // Trying to overwrite primal with normal weather - not allowed
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Verifica si una condición de lado puede ser establecida según las condiciones requeridas.
        /// </summary>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <param name="conditionData">Los datos de la condición. Puede ser null.</param>
        /// <returns>True si la condición puede ser establecida, false en caso contrario.</returns>
        public bool CanSetSideCondition(BattleField field, SideConditionData conditionData)
        {
            if (field == null || conditionData == null)
                return true; // Allow if no validation needed

            // Validate condition can be set (e.g., Aurora Veil requires Hail/Snow)
            if (conditionData.RequiredWeather.HasValue)
            {
                var currentWeather = field.Weather;
                if (!conditionData.CanBeSetInWeather(currentWeather))
                {
                    // Condition cannot be set in current weather
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Verifica si un terreno puede ser establecido.
        /// Los terrenos siempre pueden sobrescribirse (a diferencia del clima primal).
        /// </summary>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <param name="newTerrain">El terreno que se intenta establecer.</param>
        /// <returns>True si el terreno puede ser establecido, false en caso contrario.</returns>
        public bool CanSetTerrain(BattleField field, Terrain newTerrain)
        {
            if (field == null)
                return false;

            // Terrains can always be overwritten, unlike primal weather
            return true;
        }
    }
}
