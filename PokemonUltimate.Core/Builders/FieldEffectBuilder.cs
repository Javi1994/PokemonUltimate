using System;
using System.Collections.Generic;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Builders
{
    /// <summary>
    /// Fluent builder for creating FieldEffectData.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.9: Builders
    /// **Documentation**: See `docs/features/3-content-expansion/3.9-builders/README.md`
    /// </remarks>
    public class FieldEffectBuilder
    {
        private readonly string _id;
        private readonly string _name;
        private string _description = string.Empty;
        private FieldEffect _type = FieldEffect.None;
        private int _defaultDuration = 5;
        private bool _isToggle = false;
        private bool _reversesSpeedOrder = false;
        private bool _disablesItems = false;
        private bool _swapsDefenses = false;
        private bool _groundsAllPokemon = false;
        private List<string> _disabledMoves = new List<string>();
        private PokemonType? _changesMovesToType;
        private PokemonType? _changesMovesFromType;
        private bool _preventsSwitching = false;
        private PokemonType? _reducesPowerOfType;
        private float _powerMultiplier = 1.0f;

        private FieldEffectBuilder(string name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _id = name.ToLowerInvariant().Replace(" ", "-");
        }

        /// <summary>
        /// Start defining a new field effect.
        /// </summary>
        public static FieldEffectBuilder Define(string name) => new FieldEffectBuilder(name);

        #region Identity

        public FieldEffectBuilder Description(string description)
        {
            _description = description;
            return this;
        }

        public FieldEffectBuilder Type(FieldEffect type)
        {
            _type = type;
            return this;
        }

        #endregion

        #region Duration

        public FieldEffectBuilder Duration(int turns)
        {
            _defaultDuration = turns;
            return this;
        }

        public FieldEffectBuilder Toggle()
        {
            _isToggle = true;
            return this;
        }

        #endregion

        #region Speed

        public FieldEffectBuilder ReversesSpeedOrder()
        {
            _reversesSpeedOrder = true;
            return this;
        }

        #endregion

        #region Items

        public FieldEffectBuilder DisablesItems()
        {
            _disablesItems = true;
            return this;
        }

        #endregion

        #region Stats

        public FieldEffectBuilder SwapsDefenses()
        {
            _swapsDefenses = true;
            return this;
        }

        #endregion

        #region Grounding

        public FieldEffectBuilder GroundsAllPokemon()
        {
            _groundsAllPokemon = true;
            return this;
        }

        #endregion

        #region Moves

        public FieldEffectBuilder DisablesMoves(params string[] moves)
        {
            _disabledMoves.AddRange(moves);
            return this;
        }

        public FieldEffectBuilder ChangesMoveType(PokemonType from, PokemonType to)
        {
            _changesMovesFromType = from;
            _changesMovesToType = to;
            return this;
        }

        #endregion

        #region Switching

        public FieldEffectBuilder PreventsSwitching()
        {
            _preventsSwitching = true;
            return this;
        }

        #endregion

        #region Power

        public FieldEffectBuilder ReducesTypePower(PokemonType type, float multiplier = 0.33f)
        {
            _reducesPowerOfType = type;
            _powerMultiplier = multiplier;
            return this;
        }

        #endregion

        #region Build

        public FieldEffectData Build()
        {
            return new FieldEffectData(
                _id,
                _name,
                _description,
                _type,
                _defaultDuration,
                _isToggle,
                _reversesSpeedOrder,
                _disablesItems,
                _swapsDefenses,
                _groundsAllPokemon,
                _disabledMoves.ToArray(),
                _changesMovesToType,
                _changesMovesFromType,
                _preventsSwitching,
                _reducesPowerOfType,
                _powerMultiplier);
        }

        #endregion
    }

    /// <summary>
    /// Alias for FieldEffectBuilder for cleaner syntax.
    /// </summary>
    public static class Room
    {
        public static FieldEffectBuilder Define(string name) => FieldEffectBuilder.Define(name);
    }
}

