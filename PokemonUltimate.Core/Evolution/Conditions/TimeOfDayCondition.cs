using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Core.Evolution.Conditions
{
    /// <summary>
    /// Evolution condition: Must be a specific time of day.
    /// This condition returns false by default - time context must be provided.
    /// </summary>
    public class TimeOfDayCondition : IEvolutionCondition
    {
        public EvolutionConditionType ConditionType => EvolutionConditionType.TimeOfDay;
        public string Description => $"During {RequiredTime}";

        /// <summary>
        /// The required time of day.
        /// </summary>
        public TimeOfDay RequiredTime { get; set; }

        public TimeOfDayCondition() { }

        public TimeOfDayCondition(TimeOfDay requiredTime)
        {
            RequiredTime = requiredTime;
        }

        /// <summary>
        /// Time conditions require context - use IsMet(pokemon, currentTime) overload.
        /// </summary>
        public bool IsMet(PokemonInstance pokemon)
        {
            // Time conditions require context
            return false;
        }

        /// <summary>
        /// Checks if the condition is met at the given time.
        /// </summary>
        public bool IsMet(PokemonInstance pokemon, TimeOfDay currentTime)
        {
            return pokemon != null && currentTime == RequiredTime;
        }
    }
}
