using System;
using PokemonUltimate.Core.Data.Enums;


namespace PokemonUltimate.Core.Data.Blueprints
{
    /// <summary>
    /// Immutable data defining a field effect's behavior.
    /// Field effects affect the entire battlefield (both sides).
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Field Effect Data
    /// **Documentation**: See `docs/features/1-game-data/1.10-field-effect-data/README.md`
    /// </remarks>
    public sealed class FieldEffectData
    {
        #region Identity

        /// <summary>
        /// Internal identifier for the effect.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Display name of the effect.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Description of what the effect does.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The effect type this data represents.
        /// </summary>
        public FieldEffect Type { get; }

        #endregion

        #region Duration

        /// <summary>
        /// Default duration in turns (0 = indefinite).
        /// </summary>
        public int DefaultDuration { get; }

        /// <summary>
        /// Whether this effect ends on the same turn it's used again (toggle).
        /// </summary>
        public bool IsToggle { get; }

        #endregion

        #region Speed Modification

        /// <summary>
        /// Whether this effect reverses speed order (Trick Room).
        /// </summary>
        public bool ReversesSpeedOrder { get; }

        #endregion

        #region Item Effects

        /// <summary>
        /// Whether held items have no effect (Magic Room).
        /// </summary>
        public bool DisablesItems { get; }

        #endregion

        #region Stat Swapping

        /// <summary>
        /// Whether Defense and Special Defense are swapped (Wonder Room).
        /// </summary>
        public bool SwapsDefenses { get; }

        #endregion

        #region Grounding

        /// <summary>
        /// Whether all Pokemon are grounded (Gravity).
        /// </summary>
        public bool GroundsAllPokemon { get; }

        /// <summary>
        /// Whether Ground moves can hit Flying types (Gravity).
        /// </summary>
        public bool GroundHitsFlying => GroundsAllPokemon;

        #endregion

        #region Move Restrictions

        /// <summary>
        /// Moves disabled by this effect.
        /// </summary>
        public string[] DisabledMoves { get; }

        /// <summary>
        /// Whether this effect disables certain moves.
        /// </summary>
        public bool DisablesMoves => DisabledMoves.Length > 0;

        #endregion

        #region Type Changes

        /// <summary>
        /// Type that moves are changed to (Electric for Ion Deluge).
        /// </summary>
        public PokemonType? ChangesMovesToType { get; }

        /// <summary>
        /// Original type that gets changed (Normal for Ion Deluge).
        /// </summary>
        public PokemonType? ChangesMovesFromType { get; }

        /// <summary>
        /// Whether this effect changes move types.
        /// </summary>
        public bool ChangesMoveTypes => ChangesMovesToType.HasValue;

        #endregion

        #region Movement Restriction

        /// <summary>
        /// Whether switching/fleeing is blocked (Fairy Lock).
        /// </summary>
        public bool PreventsSwitching { get; }

        #endregion

        #region Power Modification

        /// <summary>
        /// Type whose power is reduced.
        /// </summary>
        public PokemonType? ReducesPowerOfType { get; }

        /// <summary>
        /// Power multiplier for the affected type (0.33 for Mud/Water Sport).
        /// </summary>
        public float PowerMultiplier { get; }

        /// <summary>
        /// Whether this effect reduces move power.
        /// </summary>
        public bool ReducesMovePower => ReducesPowerOfType.HasValue;

        #endregion

        #region Constructor

        internal FieldEffectData(
            string id,
            string name,
            string description,
            FieldEffect type,
            int defaultDuration,
            bool isToggle,
            bool reversesSpeedOrder,
            bool disablesItems,
            bool swapsDefenses,
            bool groundsAllPokemon,
            string[] disabledMoves,
            PokemonType? changesMovesToType,
            PokemonType? changesMovesFromType,
            bool preventsSwitching,
            PokemonType? reducesPowerOfType,
            float powerMultiplier)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? string.Empty;
            Type = type;
            DefaultDuration = defaultDuration;
            IsToggle = isToggle;
            ReversesSpeedOrder = reversesSpeedOrder;
            DisablesItems = disablesItems;
            SwapsDefenses = swapsDefenses;
            GroundsAllPokemon = groundsAllPokemon;
            DisabledMoves = disabledMoves ?? Array.Empty<string>();
            ChangesMovesToType = changesMovesToType;
            ChangesMovesFromType = changesMovesFromType;
            PreventsSwitching = preventsSwitching;
            ReducesPowerOfType = reducesPowerOfType;
            PowerMultiplier = powerMultiplier;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Checks if a move is disabled by this effect.
        /// </summary>
        public bool IsMoveDisabled(string moveName)
        {
            foreach (var disabled in DisabledMoves)
            {
                if (disabled.Equals(moveName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the power multiplier for a move type.
        /// </summary>
        public float GetPowerMultiplier(PokemonType moveType)
        {
            if (ReducesPowerOfType.HasValue && ReducesPowerOfType.Value == moveType)
                return PowerMultiplier;
            return 1.0f;
        }

        /// <summary>
        /// Gets the effective move type after transformation.
        /// </summary>
        public PokemonType GetEffectiveMoveType(PokemonType originalType)
        {
            if (ChangesMovesFromType.HasValue &&
                ChangesMovesToType.HasValue &&
                originalType == ChangesMovesFromType.Value)
            {
                return ChangesMovesToType.Value;
            }
            return originalType;
        }

        public override string ToString() => Name;

        #endregion
    }
}

