using System;
using System.Linq;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Domain.Evolution.Definition;
using PokemonInstance = PokemonUltimate.Core.Domain.Instances.Pokemon.PokemonInstance;

namespace PokemonUltimate.Core.Domain.Evolution.Conditions
{
    /// <summary>
    /// Evolution condition: Pokemon must know a specific move.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.11: Evolution System
    /// **Documentation**: See `docs/features/1-game-data/1.11-evolution-system/README.md`
    /// </remarks>
    public class KnowsMoveCondition : IEvolutionCondition
    {
        public EvolutionConditionType ConditionType => EvolutionConditionType.KnowsMove;
        public string Description => $"Knows {RequiredMove.Name}";

        /// <summary>
        /// The move that must be known.
        /// </summary>
        public MoveData RequiredMove { get; set; }

        public KnowsMoveCondition() { }

        public KnowsMoveCondition(MoveData requiredMove)
        {
            RequiredMove = requiredMove ?? throw new ArgumentNullException(nameof(requiredMove), ErrorMessages.MoveCannotBeNull);
        }

        public bool IsMet(PokemonInstance pokemon)
        {
            if (pokemon == null)
                return false;

            return pokemon.Moves.Any(m => m.Move == RequiredMove);
        }
    }
}
