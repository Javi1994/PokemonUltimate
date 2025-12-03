using System;
using System.Collections.Generic;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Builders
{
    /// <summary>
    /// Fluent builder for creating MoveData instances.
    /// Usage: Move.Define("Flamethrower").Type(Fire).Special(90, 100, 15).Build()
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.11: Builders
    /// **Documentation**: See `docs/features/1-game-data/1.11-builders/architecture.md`
    /// </remarks>
    public class MoveBuilder
    {
        private readonly MoveData _move;
        private List<IMoveEffect> _effects = new List<IMoveEffect>();

        private MoveBuilder(string name)
        {
            _move = new MoveData
            {
                Name = name,
                TargetScope = TargetScope.SingleEnemy // Default
            };
        }

        /// <summary>
        /// Start defining a new move.
        /// </summary>
        public static MoveBuilder Define(string name)
        {
            return new MoveBuilder(name);
        }

        /// <summary>
        /// Set the move's elemental type.
        /// </summary>
        public MoveBuilder Type(PokemonType type)
        {
            _move.Type = type;
            return this;
        }

        /// <summary>
        /// Set the move's description.
        /// </summary>
        public MoveBuilder Description(string description)
        {
            _move.Description = description;
            return this;
        }

        /// <summary>
        /// Configure as a Physical move (uses Attack/Defense).
        /// </summary>
        public MoveBuilder Physical(int power, int accuracy, int pp)
        {
            _move.Category = MoveCategory.Physical;
            _move.Power = power;
            _move.Accuracy = accuracy;
            _move.MaxPP = pp;
            return this;
        }

        /// <summary>
        /// Configure as a Special move (uses SpAttack/SpDefense).
        /// </summary>
        public MoveBuilder Special(int power, int accuracy, int pp)
        {
            _move.Category = MoveCategory.Special;
            _move.Power = power;
            _move.Accuracy = accuracy;
            _move.MaxPP = pp;
            return this;
        }

        /// <summary>
        /// Configure as a Status move (no damage).
        /// </summary>
        public MoveBuilder Status(int accuracy, int pp)
        {
            _move.Category = MoveCategory.Status;
            _move.Power = 0;
            _move.Accuracy = accuracy;
            _move.MaxPP = pp;
            return this;
        }

        /// <summary>
        /// Set move priority (-7 to +5, default 0).
        /// </summary>
        public MoveBuilder Priority(int priority)
        {
            _move.Priority = priority;
            return this;
        }

        /// <summary>
        /// Set the move's target scope.
        /// </summary>
        public MoveBuilder Target(TargetScope scope)
        {
            _move.TargetScope = scope;
            return this;
        }

        /// <summary>
        /// Target self (for buffs, heals).
        /// </summary>
        public MoveBuilder TargetSelf()
        {
            _move.TargetScope = TargetScope.Self;
            return this;
        }

        /// <summary>
        /// Target all enemies (for spread moves).
        /// </summary>
        public MoveBuilder TargetAllEnemies()
        {
            _move.TargetScope = TargetScope.AllEnemies;
            return this;
        }

        /// <summary>
        /// Target all adjacent Pokemon including allies (Earthquake in doubles).
        /// </summary>
        public MoveBuilder TargetAllAdjacent()
        {
            _move.TargetScope = TargetScope.AllAdjacent;
            return this;
        }

        /// <summary>
        /// Target all adjacent enemies only (Heat Wave, Razor Leaf).
        /// </summary>
        public MoveBuilder TargetAllAdjacentEnemies()
        {
            _move.TargetScope = TargetScope.AllAdjacentEnemies;
            return this;
        }

        /// <summary>
        /// Define move effects using the effect builder.
        /// </summary>
        public MoveBuilder WithEffects(Action<EffectBuilder> configure)
        {
            var builder = new EffectBuilder();
            configure(builder);
            _effects = builder.Build();
            return this;
        }

        /// <summary>
        /// Build the final MoveData instance.
        /// </summary>
        public MoveData Build()
        {
            _move.Effects = _effects;
            return _move;
        }
    }

    /// <summary>
    /// Static entry point for the Move builder.
    /// </summary>
    public static class Move
    {
        /// <summary>
        /// Start defining a new move.
        /// </summary>
        public static MoveBuilder Define(string name)
        {
            return MoveBuilder.Define(name);
        }
    }
}

