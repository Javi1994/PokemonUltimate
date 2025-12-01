using System;
using System.Collections.Generic;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Builders
{
    /// <summary>
    /// Fluent builder for creating SideConditionData.
    /// </summary>
    public class SideConditionBuilder
    {
        private readonly string _id;
        private readonly string _name;
        private string _description = string.Empty;
        private SideCondition _type = SideCondition.None;
        private int _defaultDuration = 5;
        private int _extendedDuration = 8;
        private string _extendingItem = string.Empty;
        private MoveCategory? _reducesDamageFrom;
        private float _damageMultiplierSingles = 1.0f;
        private float _damageMultiplierDoubles = 1.0f;
        private float _speedMultiplier = 1.0f;
        private bool _preventsStatus = false;
        private bool _preventsStatReduction = false;
        private bool _preventsCriticalHits = false;
        private bool _blocksSpreadMoves = false;
        private bool _blocksPriorityMoves = false;
        private bool _blocksDamagingMoves = false;
        private Weather? _requiredWeather;
        private Weather? _alternateWeather;
        private bool _firstTurnOnly = false;
        private List<string> _removedByMoves = new List<string>();

        private SideConditionBuilder(string name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _id = name.ToLowerInvariant().Replace(" ", "-");
        }

        /// <summary>
        /// Start defining a new side condition.
        /// </summary>
        public static SideConditionBuilder Define(string name) => new SideConditionBuilder(name);

        #region Identity

        public SideConditionBuilder Description(string description)
        {
            _description = description;
            return this;
        }

        public SideConditionBuilder Type(SideCondition type)
        {
            _type = type;
            return this;
        }

        #endregion

        #region Duration

        public SideConditionBuilder Duration(int turns)
        {
            _defaultDuration = turns;
            return this;
        }

        public SideConditionBuilder ExtendedBy(string itemName, int extendedTurns = 8)
        {
            _extendingItem = itemName;
            _extendedDuration = extendedTurns;
            return this;
        }

        public SideConditionBuilder SingleTurn()
        {
            _defaultDuration = 1;
            return this;
        }

        #endregion

        #region Damage Reduction

        public SideConditionBuilder ReducesPhysicalDamage(float singlesMult = 0.5f, float doublesMult = 0.66f)
        {
            _reducesDamageFrom = MoveCategory.Physical;
            _damageMultiplierSingles = singlesMult;
            _damageMultiplierDoubles = doublesMult;
            return this;
        }

        public SideConditionBuilder ReducesSpecialDamage(float singlesMult = 0.5f, float doublesMult = 0.66f)
        {
            _reducesDamageFrom = MoveCategory.Special;
            _damageMultiplierSingles = singlesMult;
            _damageMultiplierDoubles = doublesMult;
            return this;
        }

        public SideConditionBuilder ReducesAllDamage(float singlesMult = 0.5f, float doublesMult = 0.66f)
        {
            _reducesDamageFrom = null;
            _damageMultiplierSingles = singlesMult;
            _damageMultiplierDoubles = doublesMult;
            return this;
        }

        #endregion

        #region Speed

        public SideConditionBuilder DoublesSpeed()
        {
            _speedMultiplier = 2.0f;
            return this;
        }

        public SideConditionBuilder SpeedMultiplier(float multiplier)
        {
            _speedMultiplier = multiplier;
            return this;
        }

        #endregion

        #region Prevention

        public SideConditionBuilder PreventsStatus()
        {
            _preventsStatus = true;
            return this;
        }

        public SideConditionBuilder PreventsStatReduction()
        {
            _preventsStatReduction = true;
            return this;
        }

        public SideConditionBuilder PreventsCriticalHits()
        {
            _preventsCriticalHits = true;
            return this;
        }

        #endregion

        #region Move Blocking

        public SideConditionBuilder BlocksSpreadMoves()
        {
            _blocksSpreadMoves = true;
            return this;
        }

        public SideConditionBuilder BlocksPriorityMoves()
        {
            _blocksPriorityMoves = true;
            return this;
        }

        public SideConditionBuilder BlocksDamagingMoves()
        {
            _blocksDamagingMoves = true;
            return this;
        }

        #endregion

        #region Requirements

        public SideConditionBuilder RequiresWeather(Weather weather, Weather? alternate = null)
        {
            _requiredWeather = weather;
            _alternateWeather = alternate;
            return this;
        }

        public SideConditionBuilder FirstTurnOnly()
        {
            _firstTurnOnly = true;
            return this;
        }

        #endregion

        #region Removal

        public SideConditionBuilder RemovedBy(params string[] moves)
        {
            _removedByMoves = new List<string>(moves);
            return this;
        }

        #endregion

        #region Build

        public SideConditionData Build()
        {
            return new SideConditionData(
                _id,
                _name,
                _description,
                _type,
                _defaultDuration,
                _extendedDuration,
                _extendingItem,
                _reducesDamageFrom,
                _damageMultiplierSingles,
                _damageMultiplierDoubles,
                _speedMultiplier,
                _preventsStatus,
                _preventsStatReduction,
                _preventsCriticalHits,
                _blocksSpreadMoves,
                _blocksPriorityMoves,
                _blocksDamagingMoves,
                _requiredWeather,
                _alternateWeather,
                _firstTurnOnly,
                _removedByMoves.ToArray());
        }

        #endregion
    }

    /// <summary>
    /// Alias for SideConditionBuilder for cleaner syntax.
    /// </summary>
    public static class Screen
    {
        public static SideConditionBuilder Define(string name) => SideConditionBuilder.Define(name);
    }
}

