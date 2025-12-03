using System;
using System.Collections.Generic;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Builders
{
    /// <summary>
    /// Fluent builder for creating TerrainData.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.11: Builders
    /// **Documentation**: See `docs/features/1-game-data/1.11-builders/architecture.md`
    /// </remarks>
    public class TerrainBuilder
    {
        private readonly string _id;
        private readonly string _name;
        private string _description = string.Empty;
        private Terrain _terrain = Terrain.None;
        private int _defaultDuration = 5;
        private PokemonType? _boostedType;
        private float _boostMultiplier = 1.3f; // Gen 7+ standard
        private PokemonType? _reducedDamageType;
        private float _damageReductionMultiplier = 0.5f;
        private float _endOfTurnHealing;
        private List<PersistentStatus> _preventedStatuses = new List<PersistentStatus>();
        private bool _blocksPriorityMoves;
        private List<string> _halvedDamageMoves = new List<string>();
        private string _naturePowerMove = string.Empty;
        private PokemonType? _camouflageType;
        private string _secretPowerEffect = string.Empty;
        private List<string> _setterAbilities = new List<string>();
        private List<string> _benefitingAbilities = new List<string>();

        private TerrainBuilder(string name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _id = name.ToLowerInvariant().Replace(" ", "-");
        }

        /// <summary>
        /// Start defining a new terrain condition.
        /// </summary>
        public static TerrainBuilder Define(string name) => new TerrainBuilder(name);

        #region Identity

        public TerrainBuilder Description(string description)
        {
            _description = description;
            return this;
        }

        /// <summary>
        /// Set the terrain enum value.
        /// </summary>
        public TerrainBuilder Type(Terrain terrain)
        {
            _terrain = terrain;
            return this;
        }

        #endregion

        #region Duration

        /// <summary>
        /// Set default duration (5 turns is standard).
        /// </summary>
        public TerrainBuilder Duration(int turns)
        {
            _defaultDuration = turns;
            return this;
        }

        #endregion

        #region Type Power Modifiers

        /// <summary>
        /// Boost a type's moves (default 1.3x for Gen 7+).
        /// </summary>
        public TerrainBuilder Boosts(PokemonType type, float multiplier = 1.3f)
        {
            _boostedType = type;
            _boostMultiplier = multiplier;
            return this;
        }

        /// <summary>
        /// Reduce damage from a type (Misty reduces Dragon).
        /// </summary>
        public TerrainBuilder ReducesDamageFrom(PokemonType type, float multiplier = 0.5f)
        {
            _reducedDamageType = type;
            _damageReductionMultiplier = multiplier;
            return this;
        }

        #endregion

        #region End of Turn Effects

        /// <summary>
        /// Heals grounded Pokemon each turn (Grassy = 1/16).
        /// </summary>
        public TerrainBuilder HealsEachTurn(float fraction)
        {
            _endOfTurnHealing = fraction;
            return this;
        }

        #endregion

        #region Status Prevention

        /// <summary>
        /// Prevents a status condition for grounded Pokemon.
        /// </summary>
        public TerrainBuilder Prevents(params PersistentStatus[] statuses)
        {
            _preventedStatuses.AddRange(statuses);
            return this;
        }

        /// <summary>
        /// Prevents Sleep for grounded Pokemon (Electric Terrain).
        /// </summary>
        public TerrainBuilder PreventsSleep()
        {
            _preventedStatuses.Add(PersistentStatus.Sleep);
            return this;
        }

        /// <summary>
        /// Prevents all major status conditions (Misty Terrain).
        /// </summary>
        public TerrainBuilder PreventsAllStatuses()
        {
            _preventedStatuses.Add(PersistentStatus.Burn);
            _preventedStatuses.Add(PersistentStatus.Paralysis);
            _preventedStatuses.Add(PersistentStatus.Sleep);
            _preventedStatuses.Add(PersistentStatus.Poison);
            _preventedStatuses.Add(PersistentStatus.BadlyPoisoned);
            _preventedStatuses.Add(PersistentStatus.Freeze);
            return this;
        }

        #endregion

        #region Move Modifications

        /// <summary>
        /// Blocks priority moves against grounded targets (Psychic Terrain).
        /// </summary>
        public TerrainBuilder BlocksPriorityMoves()
        {
            _blocksPriorityMoves = true;
            return this;
        }

        /// <summary>
        /// Halves damage from specific moves (Earthquake, Bulldoze, Magnitude).
        /// </summary>
        public TerrainBuilder HalvesDamageFrom(params string[] moveNames)
        {
            _halvedDamageMoves.AddRange(moveNames);
            return this;
        }

        /// <summary>
        /// Sets the move Nature Power becomes in this terrain.
        /// </summary>
        public TerrainBuilder NaturePowerBecomes(string moveName)
        {
            _naturePowerMove = moveName;
            return this;
        }

        /// <summary>
        /// Sets the type Camouflage changes to in this terrain.
        /// </summary>
        public TerrainBuilder CamouflageChangesTo(PokemonType type)
        {
            _camouflageType = type;
            return this;
        }

        /// <summary>
        /// Sets the effect Secret Power has in this terrain.
        /// </summary>
        public TerrainBuilder SecretPowerCauses(string effect)
        {
            _secretPowerEffect = effect;
            return this;
        }

        #endregion

        #region Ability Interactions

        /// <summary>
        /// Abilities that set this terrain on switch-in.
        /// </summary>
        public TerrainBuilder SetByAbilities(params string[] abilityNames)
        {
            _setterAbilities.AddRange(abilityNames);
            return this;
        }

        /// <summary>
        /// Abilities that benefit from this terrain.
        /// </summary>
        public TerrainBuilder BenefitsAbilities(params string[] abilityNames)
        {
            _benefitingAbilities.AddRange(abilityNames);
            return this;
        }

        #endregion

        #region Build

        public TerrainData Build()
        {
            return new TerrainData(
                _id,
                _name,
                _description,
                _terrain,
                _defaultDuration,
                _boostedType,
                _boostMultiplier,
                _reducedDamageType,
                _damageReductionMultiplier,
                _endOfTurnHealing,
                _preventedStatuses.ToArray(),
                _blocksPriorityMoves,
                _halvedDamageMoves.ToArray(),
                _naturePowerMove,
                _camouflageType,
                _secretPowerEffect,
                _setterAbilities.ToArray(),
                _benefitingAbilities.ToArray());
        }

        #endregion
    }

    /// <summary>
    /// Alias for TerrainBuilder for cleaner syntax.
    /// </summary>
    public static class TerrainEffect
    {
        public static TerrainBuilder Define(string name) => TerrainBuilder.Define(name);
    }
}

