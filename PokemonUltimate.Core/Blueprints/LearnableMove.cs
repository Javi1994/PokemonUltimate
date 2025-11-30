using System;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Blueprints
{
    /// <summary>
    /// Represents a move that a Pokemon species can learn.
    /// Part of the species' Learnset.
    /// </summary>
    public class LearnableMove
    {
        /// <summary>
        /// Reference to the move data.
        /// </summary>
        public MoveData Move { get; set; }

        /// <summary>
        /// How this move is learned.
        /// </summary>
        public LearnMethod Method { get; set; }

        /// <summary>
        /// Level at which the move is learned (only relevant for LevelUp method).
        /// </summary>
        public int Level { get; set; }

        public LearnableMove() { }

        public LearnableMove(MoveData move, LearnMethod method, int level = 0)
        {
            Move = move ?? throw new ArgumentNullException(nameof(move), ErrorMessages.MoveCannotBeNull);
            Method = method;
            Level = level;
        }
    }
}

