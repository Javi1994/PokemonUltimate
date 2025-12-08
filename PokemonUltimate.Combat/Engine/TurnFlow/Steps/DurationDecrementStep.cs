using System.Threading.Tasks;
using PokemonUltimate.Combat.Engine.TurnFlow.Definition;

namespace PokemonUltimate.Combat.Engine.TurnFlow.Steps
{
    /// <summary>
    /// Step que decrementa duraciones (clima, terreno, condiciones de lado).
    /// </summary>
    public class DurationDecrementStep : ITurnStep
    {
        public string StepName => "Duration Decrement";
        public bool ExecuteEvenIfFainted => false;

        public Task<bool> ExecuteAsync(TurnContext context)
        {
            // Decrement weather duration
            context.Field.DecrementWeatherDuration();

            // Decrement terrain duration
            context.Field.DecrementTerrainDuration();

            // Decrement side condition durations
            context.Field.PlayerSide.DecrementAllSideConditionDurations();
            context.Field.EnemySide.DecrementAllSideConditionDurations();

            return Task.FromResult(true);
        }
    }
}
