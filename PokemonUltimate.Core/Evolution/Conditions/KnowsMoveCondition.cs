using PokemonUltimate.Core.Models;

namespace PokemonUltimate.Core.Evolution.Conditions
{
    /// <summary>
    /// Evolution condition: Pokemon must know a specific move.
    /// </summary>
    public class KnowsMoveCondition : IEvolutionCondition
    {
        public EvolutionConditionType ConditionType => EvolutionConditionType.KnowsMove;
        public string Description => $"Knows {RequiredMove?.Name ?? "unknown move"}";

        /// <summary>
        /// The move that must be known.
        /// </summary>
        public MoveData RequiredMove { get; set; }

        public KnowsMoveCondition() { }

        public KnowsMoveCondition(MoveData requiredMove)
        {
            RequiredMove = requiredMove;
        }
    }
}

