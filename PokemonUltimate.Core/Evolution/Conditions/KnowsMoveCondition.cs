using System.Linq;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Instances;

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

        public bool IsMet(PokemonInstance pokemon)
        {
            if (pokemon == null || RequiredMove == null)
                return false;

            return pokemon.Moves.Any(m => m.Move == RequiredMove);
        }
    }
}
