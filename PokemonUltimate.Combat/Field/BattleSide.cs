using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Combat
{
    /// <summary>
    /// Represents one side of the battlefield (player or enemy).
    /// Contains slots for active Pokemon and a reference to the full party.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    public class BattleSide
    {
        private readonly List<BattleSlot> _slots;
        private IReadOnlyList<PokemonInstance> _party;
        private readonly Dictionary<HazardType, int> _hazards;

        /// <summary>
        /// The slots on this side of the field.
        /// </summary>
        public IReadOnlyList<BattleSlot> Slots => _slots;

        /// <summary>
        /// The full party of Pokemon for this side.
        /// </summary>
        public IReadOnlyList<PokemonInstance> Party => _party;

        /// <summary>
        /// True if this is the player's side.
        /// </summary>
        public bool IsPlayer { get; }

        /// <summary>
        /// Creates a new battle side.
        /// </summary>
        /// <param name="slotCount">Number of active slots (1 for singles, 2 for doubles, etc.).</param>
        /// <param name="isPlayer">True if this is the player's side.</param>
        /// <exception cref="ArgumentException">If slotCount is less than 1.</exception>
        public BattleSide(int slotCount, bool isPlayer)
        {
            if (slotCount < 1)
                throw new ArgumentException(ErrorMessages.SlotCountMustBePositive, nameof(slotCount));

            IsPlayer = isPlayer;
            _slots = new List<BattleSlot>(slotCount);
            _hazards = new Dictionary<HazardType, int>();

            for (int i = 0; i < slotCount; i++)
            {
                _slots.Add(new BattleSlot(i, this));
            }
        }

        /// <summary>
        /// Sets the party for this side.
        /// </summary>
        /// <param name="party">The Pokemon party. Cannot be null or empty.</param>
        /// <exception cref="ArgumentNullException">If party is null.</exception>
        /// <exception cref="ArgumentException">If party is empty.</exception>
        public void SetParty(IReadOnlyList<PokemonInstance> party)
        {
            if (party == null)
                throw new ArgumentNullException(nameof(party), ErrorMessages.PartyCannotBeNull);
            if (party.Count == 0)
                throw new ArgumentException(ErrorMessages.PartyCannotBeEmpty, nameof(party));

            _party = party;
        }

        /// <summary>
        /// Gets a specific slot by index.
        /// </summary>
        /// <param name="index">The slot index.</param>
        /// <returns>The slot at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If index is out of range.</exception>
        public BattleSlot GetSlot(int index)
        {
            if (index < 0 || index >= _slots.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return _slots[index];
        }

        /// <summary>
        /// Gets all slots that have a non-fainted Pokemon.
        /// </summary>
        /// <returns>Active (non-empty, non-fainted) slots.</returns>
        public IEnumerable<BattleSlot> GetActiveSlots()
        {
            return _slots.Where(s => !s.IsEmpty && !s.HasFainted);
        }

        /// <summary>
        /// Gets Pokemon from the party that can be switched in.
        /// Excludes currently active Pokemon and fainted Pokemon.
        /// </summary>
        /// <returns>Pokemon available for switching.</returns>
        public IEnumerable<PokemonInstance> GetAvailableSwitches()
        {
            if (_party == null)
                return Enumerable.Empty<PokemonInstance>();

            var activePokemon = _slots
                .Where(s => !s.IsEmpty)
                .Select(s => s.Pokemon)
                .ToHashSet();

            return _party.Where(p => !p.IsFainted && !activePokemon.Contains(p));
        }

        /// <summary>
        /// Checks if this side has at least one active (non-fainted) Pokemon on the field.
        /// </summary>
        /// <returns>True if there's at least one active Pokemon.</returns>
        public bool HasActivePokemon()
        {
            return GetActiveSlots().Any();
        }

        /// <summary>
        /// Checks if this side is completely defeated (all party Pokemon fainted).
        /// </summary>
        /// <returns>True if all Pokemon in the party have fainted.</returns>
        public bool IsDefeated()
        {
            if (_party == null || _party.Count == 0)
                return false;

            return _party.All(p => p.IsFainted);
        }

        #region Entry Hazards

        /// <summary>
        /// Adds an entry hazard to this side.
        /// For hazards with layers (Spikes, Toxic Spikes), increments the layer count.
        /// For single-layer hazards (Stealth Rock, Sticky Web), sets to 1 if not present.
        /// </summary>
        /// <param name="hazardData">The hazard data to add. Cannot be null.</param>
        /// <remarks>
        /// **Feature**: 2: Combat System
        /// **Sub-Feature**: 2.14: Hazards System
        /// **Documentation**: See `docs/features/2-combat-system/2.14-hazards-system/README.md`
        /// </remarks>
        public void AddHazard(HazardData hazardData)
        {
            if (hazardData == null)
                throw new ArgumentNullException(nameof(hazardData));

            var hazardType = hazardData.Type;

            if (_hazards.ContainsKey(hazardType))
            {
                // Increment layers if hazard supports multiple layers
                if (hazardData.HasLayers)
                {
                    int currentLayers = _hazards[hazardType];
                    int newLayers = Math.Min(currentLayers + 1, hazardData.MaxLayers);
                    _hazards[hazardType] = newLayers;
                }
                // Single-layer hazards stay at 1
            }
            else
            {
                // Add new hazard with 1 layer
                _hazards[hazardType] = 1;
            }
        }

        /// <summary>
        /// Removes a specific entry hazard from this side.
        /// </summary>
        /// <param name="hazardType">The type of hazard to remove.</param>
        /// <remarks>
        /// **Feature**: 2: Combat System
        /// **Sub-Feature**: 2.14: Hazards System
        /// **Documentation**: See `docs/features/2-combat-system/2.14-hazards-system/README.md`
        /// </remarks>
        public void RemoveHazard(HazardType hazardType)
        {
            _hazards.Remove(hazardType);
        }

        /// <summary>
        /// Removes all entry hazards from this side.
        /// </summary>
        /// <remarks>
        /// **Feature**: 2: Combat System
        /// **Sub-Feature**: 2.14: Hazards System
        /// **Documentation**: See `docs/features/2-combat-system/2.14-hazards-system/README.md`
        /// </remarks>
        public void RemoveAllHazards()
        {
            _hazards.Clear();
        }

        /// <summary>
        /// Checks if this side has a specific entry hazard.
        /// </summary>
        /// <param name="hazardType">The type of hazard to check.</param>
        /// <returns>True if the hazard is present.</returns>
        /// <remarks>
        /// **Feature**: 2: Combat System
        /// **Sub-Feature**: 2.14: Hazards System
        /// **Documentation**: See `docs/features/2-combat-system/2.14-hazards-system/README.md`
        /// </remarks>
        public bool HasHazard(HazardType hazardType)
        {
            return _hazards.ContainsKey(hazardType) && _hazards[hazardType] > 0;
        }

        /// <summary>
        /// Gets the number of layers for a specific entry hazard.
        /// </summary>
        /// <param name="hazardType">The type of hazard to check.</param>
        /// <returns>The number of layers (0 if not present).</returns>
        /// <remarks>
        /// **Feature**: 2: Combat System
        /// **Sub-Feature**: 2.14: Hazards System
        /// **Documentation**: See `docs/features/2-combat-system/2.14-hazards-system/README.md`
        /// </remarks>
        public int GetHazardLayers(HazardType hazardType)
        {
            return _hazards.TryGetValue(hazardType, out int layers) ? layers : 0;
        }


        #endregion
    }
}

