using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Constants;
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
    }
}

