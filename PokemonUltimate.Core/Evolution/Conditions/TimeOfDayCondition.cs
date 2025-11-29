using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Evolution.Conditions
{
    /// <summary>
    /// Evolution condition: Must be a specific time of day.
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
    }
}

