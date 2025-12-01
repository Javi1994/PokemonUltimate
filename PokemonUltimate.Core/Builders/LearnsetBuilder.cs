using System.Collections.Generic;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Builders
{
    /// <summary>
    /// Fluent builder for creating a Pokemon's learnset.
    /// </summary>
    public class LearnsetBuilder
    {
        private readonly List<LearnableMove> _moves = new List<LearnableMove>();

        /// <summary>
        /// Moves the Pokemon starts with at level 1.
        /// </summary>
        public LearnsetBuilder StartsWith(params MoveData[] moves)
        {
            foreach (var move in moves)
            {
                _moves.Add(new LearnableMove(move, LearnMethod.Start, 0));
            }
            return this;
        }

        /// <summary>
        /// Move(s) learned at a specific level.
        /// </summary>
        public LearnsetBuilder AtLevel(int level, params MoveData[] moves)
        {
            foreach (var move in moves)
            {
                _moves.Add(new LearnableMove(move, LearnMethod.LevelUp, level));
            }
            return this;
        }

        /// <summary>
        /// Move(s) learned via TM/HM.
        /// </summary>
        public LearnsetBuilder ByTM(params MoveData[] moves)
        {
            foreach (var move in moves)
            {
                _moves.Add(new LearnableMove(move, LearnMethod.TM, 0));
            }
            return this;
        }

        /// <summary>
        /// Move(s) learned from breeding (egg moves).
        /// </summary>
        public LearnsetBuilder ByEgg(params MoveData[] moves)
        {
            foreach (var move in moves)
            {
                _moves.Add(new LearnableMove(move, LearnMethod.Egg, 0));
            }
            return this;
        }

        /// <summary>
        /// Move(s) learned upon evolution.
        /// </summary>
        public LearnsetBuilder OnEvolution(params MoveData[] moves)
        {
            foreach (var move in moves)
            {
                _moves.Add(new LearnableMove(move, LearnMethod.Evolution, 0));
            }
            return this;
        }

        /// <summary>
        /// Move(s) learned from a tutor.
        /// </summary>
        public LearnsetBuilder ByTutor(params MoveData[] moves)
        {
            foreach (var move in moves)
            {
                _moves.Add(new LearnableMove(move, LearnMethod.Tutor, 0));
            }
            return this;
        }

        /// <summary>
        /// Build the learnset.
        /// </summary>
        public List<LearnableMove> Build()
        {
            return _moves;
        }
    }
}

