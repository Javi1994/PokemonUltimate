using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Data.Constants;
using PokemonInstance = PokemonUltimate.Core.Domain.Instances.Pokemon.PokemonInstance;

namespace PokemonUltimate.Core.Domain.Instances.Party
{
    /// <summary>
    /// Represents a Pokemon party (team) with validation and management methods.
    /// Supports up to 6 Pokemon and integrates seamlessly with the battle system.
    /// </summary>
    /// <remarks>
    /// **Feature**: 5: Game Features
    /// **Sub-Feature**: 5.2: Pokemon Management
    /// **Documentation**: See `docs/features/5-game-features/5.2-pokemon-management/architecture.md`
    /// </remarks>
    public class PokemonParty : IReadOnlyList<PokemonInstance>
    {
        private readonly List<PokemonInstance> _pokemon;

        /// <summary>
        /// Maximum party size (standard Pokemon limit).
        /// </summary>
        public const int MaxPartySize = 6;

        /// <summary>
        /// Minimum party size required for battles.
        /// </summary>
        public const int MinBattlePartySize = 1;

        /// <summary>
        /// Current number of Pokemon in party.
        /// </summary>
        public int Count => _pokemon.Count;

        /// <summary>
        /// True if party is full (at max capacity).
        /// </summary>
        public bool IsFull => Count >= MaxPartySize;

        /// <summary>
        /// True if party is empty.
        /// </summary>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// True if party has at least one non-fainted Pokemon (valid for battle).
        /// </summary>
        public bool HasActivePokemon => _pokemon.Any(p => !p.IsFainted);

        /// <summary>
        /// Gets Pokemon at specified index.
        /// </summary>
        /// <param name="index">Index of Pokemon.</param>
        /// <returns>Pokemon at index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If index is out of range.</exception>
        public PokemonInstance this[int index] => _pokemon[index];

        /// <summary>
        /// Creates an empty party.
        /// </summary>
        public PokemonParty()
        {
            _pokemon = new List<PokemonInstance>();
        }

        /// <summary>
        /// Creates a party with initial Pokemon.
        /// </summary>
        /// <param name="pokemon">Initial Pokemon. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If pokemon is null.</exception>
        /// <exception cref="ArgumentException">If party exceeds MaxPartySize.</exception>
        public PokemonParty(IEnumerable<PokemonInstance> pokemon)
        {
            if (pokemon == null)
                throw new ArgumentNullException(nameof(pokemon), ErrorMessages.PokemonCannotBeNull);

            var pokemonList = pokemon.ToList();
            if (pokemonList.Count > MaxPartySize)
                throw new ArgumentException(ErrorMessages.Format(ErrorMessages.PartyIsFull, MaxPartySize), nameof(pokemon));

            _pokemon = new List<PokemonInstance>(pokemonList);
        }

        /// <summary>
        /// Adds a Pokemon to the party.
        /// </summary>
        /// <param name="pokemon">Pokemon to add. Cannot be null.</param>
        /// <returns>True if added successfully, false if party is full.</returns>
        /// <exception cref="ArgumentNullException">If pokemon is null.</exception>
        public bool TryAdd(PokemonInstance pokemon)
        {
            if (pokemon == null)
                throw new ArgumentNullException(nameof(pokemon), ErrorMessages.PokemonCannotBeNull);

            if (IsFull)
                return false;

            _pokemon.Add(pokemon);
            return true;
        }

        /// <summary>
        /// Adds a Pokemon to the party.
        /// </summary>
        /// <param name="pokemon">Pokemon to add. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If pokemon is null.</exception>
        /// <exception cref="InvalidOperationException">If party is full.</exception>
        public void Add(PokemonInstance pokemon)
        {
            if (pokemon == null)
                throw new ArgumentNullException(nameof(pokemon), ErrorMessages.PokemonCannotBeNull);

            if (IsFull)
                throw new InvalidOperationException(ErrorMessages.Format(ErrorMessages.PartyIsFull, MaxPartySize));

            _pokemon.Add(pokemon);
        }

        /// <summary>
        /// Removes Pokemon at specified index.
        /// </summary>
        /// <param name="index">Index of Pokemon to remove.</param>
        /// <returns>True if removed successfully, false if index invalid.</returns>
        public bool TryRemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                return false;

            _pokemon.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Removes Pokemon at specified index.
        /// </summary>
        /// <param name="index">Index of Pokemon to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">If index is invalid.</exception>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), ErrorMessages.Format(ErrorMessages.InvalidPartyIndex, index, Count));

            _pokemon.RemoveAt(index);
        }

        /// <summary>
        /// Removes specified Pokemon from party.
        /// </summary>
        /// <param name="pokemon">Pokemon to remove.</param>
        /// <returns>True if removed successfully, false if Pokemon not found.</returns>
        public bool Remove(PokemonInstance pokemon)
        {
            if (pokemon == null)
                return false;

            return _pokemon.Remove(pokemon);
        }

        /// <summary>
        /// Swaps positions of two Pokemon in party.
        /// </summary>
        /// <param name="index1">First Pokemon index.</param>
        /// <param name="index2">Second Pokemon index.</param>
        /// <exception cref="ArgumentOutOfRangeException">If either index is invalid.</exception>
        public void Swap(int index1, int index2)
        {
            if (index1 < 0 || index1 >= Count)
                throw new ArgumentOutOfRangeException(nameof(index1), ErrorMessages.Format(ErrorMessages.InvalidPartyIndex, index1, Count));
            if (index2 < 0 || index2 >= Count)
                throw new ArgumentOutOfRangeException(nameof(index2), ErrorMessages.Format(ErrorMessages.InvalidPartyIndex, index2, Count));

            if (index1 == index2)
                return;

            var temp = _pokemon[index1];
            _pokemon[index1] = _pokemon[index2];
            _pokemon[index2] = temp;
        }

        /// <summary>
        /// Moves Pokemon from one position to another.
        /// </summary>
        /// <param name="fromIndex">Source index.</param>
        /// <param name="toIndex">Destination index.</param>
        /// <exception cref="ArgumentOutOfRangeException">If either index is invalid.</exception>
        public void Move(int fromIndex, int toIndex)
        {
            if (fromIndex < 0 || fromIndex >= Count)
                throw new ArgumentOutOfRangeException(nameof(fromIndex), ErrorMessages.Format(ErrorMessages.InvalidPartyIndex, fromIndex, Count));
            if (toIndex < 0 || toIndex >= Count)
                throw new ArgumentOutOfRangeException(nameof(toIndex), ErrorMessages.Format(ErrorMessages.InvalidPartyIndex, toIndex, Count));

            if (fromIndex == toIndex)
                return;

            var pokemon = _pokemon[fromIndex];
            _pokemon.RemoveAt(fromIndex);
            _pokemon.Insert(toIndex, pokemon);
        }

        /// <summary>
        /// Gets the first non-fainted Pokemon (for battle lead).
        /// </summary>
        /// <returns>First active Pokemon, or null if all fainted.</returns>
        public PokemonInstance GetFirstActivePokemon()
        {
            return _pokemon.FirstOrDefault(p => !p.IsFainted);
        }

        /// <summary>
        /// Gets all non-fainted Pokemon.
        /// </summary>
        /// <returns>List of active Pokemon.</returns>
        public IReadOnlyList<PokemonInstance> GetActivePokemon()
        {
            return _pokemon.Where(p => !p.IsFainted).ToList();
        }

        /// <summary>
        /// Gets Pokemon at index if valid.
        /// </summary>
        /// <param name="index">Index to check.</param>
        /// <param name="pokemon">Pokemon at index if found.</param>
        /// <returns>True if index is valid.</returns>
        public bool TryGetAt(int index, out PokemonInstance pokemon)
        {
            if (index < 0 || index >= Count)
            {
                pokemon = null;
                return false;
            }

            pokemon = _pokemon[index];
            return true;
        }

        /// <summary>
        /// Checks if party contains specified Pokemon.
        /// </summary>
        /// <param name="pokemon">Pokemon to check.</param>
        /// <returns>True if Pokemon is in party.</returns>
        public bool Contains(PokemonInstance pokemon)
        {
            return pokemon != null && _pokemon.Contains(pokemon);
        }

        /// <summary>
        /// Gets index of specified Pokemon.
        /// </summary>
        /// <param name="pokemon">Pokemon to find.</param>
        /// <returns>Index if found, -1 otherwise.</returns>
        public int IndexOf(PokemonInstance pokemon)
        {
            if (pokemon == null)
                return -1;

            return _pokemon.IndexOf(pokemon);
        }

        /// <summary>
        /// Validates party is valid for battle (at least one active Pokemon).
        /// </summary>
        /// <returns>True if party can participate in battle.</returns>
        public bool IsValidForBattle()
        {
            return Count >= MinBattlePartySize && HasActivePokemon;
        }

        /// <summary>
        /// Validates party is valid for battle.
        /// </summary>
        /// <param name="errorMessage">Error message if invalid.</param>
        /// <returns>True if valid.</returns>
        public bool IsValidForBattle(out string errorMessage)
        {
            if (Count < MinBattlePartySize)
            {
                errorMessage = ErrorMessages.Format(ErrorMessages.PartyTooSmallForBattle, MinBattlePartySize);
                return false;
            }

            if (!HasActivePokemon)
            {
                errorMessage = ErrorMessages.Format(ErrorMessages.PartyTooSmallForBattle, MinBattlePartySize);
                return false;
            }

            errorMessage = null;
            return true;
        }

        #region IReadOnlyList Implementation

        /// <summary>
        /// Returns an enumerator that iterates through the party.
        /// </summary>
        /// <returns>An enumerator for the party.</returns>
        public IEnumerator<PokemonInstance> GetEnumerator()
        {
            return _pokemon.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the party.
        /// </summary>
        /// <returns>An enumerator for the party.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
