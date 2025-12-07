using System;
using PokemonUltimate.Core.Data.Constants;

namespace PokemonUltimate.Combat.Systems.Damage.Steps
{
    /// <summary>
    /// Applies weather-based damage multipliers to moves.
    /// Weather can boost, weaken, or nullify certain move types.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.12: Weather System
    /// **Documentation**: See `docs/features/2-combat-system/2.12-weather-system/README.md`
    /// </remarks>
    public class WeatherStep : IDamageStep
    {
        /// <summary>
        /// Processes the damage context and applies weather-based damage multipliers.
        /// </summary>
        /// <param name="context">The damage context to modify.</param>
        public void Process(DamageContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context), ErrorMessages.ContextCannotBeNull);

            var field = context.Field;
            var moveType = context.Move.Type;

            // Check if there's active weather
            if (field.WeatherData == null)
                return;

            // Get weather multiplier for this move type
            float weatherMultiplier = field.WeatherData.GetTypePowerMultiplier(moveType);

            // Apply multiplier (1.0 means no change, 0.0 means nullified)
            context.Multiplier *= weatherMultiplier;
        }
    }
}

