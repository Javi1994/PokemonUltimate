using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Combat
{
    /// <summary>
    /// Represents the complete battlefield with both player and enemy sides.
    /// </summary>
    public class BattleField
    {
        private BattleSide _playerSide;
        private BattleSide _enemySide;
        private BattleRules _rules;

        /// <summary>
        /// The player's side of the field.
        /// </summary>
        public BattleSide PlayerSide => _playerSide;

        /// <summary>
        /// The enemy's side of the field.
        /// </summary>
        public BattleSide EnemySide => _enemySide;

        /// <summary>
        /// The rules governing this battle.
        /// </summary>
        public BattleRules Rules => _rules;

        /// <summary>
        /// Initializes the battlefield with the given rules and parties.
        /// </summary>
        /// <param name="rules">Battle configuration. Cannot be null.</param>
        /// <param name="playerParty">Player's Pokemon party. Cannot be null.</param>
        /// <param name="enemyParty">Enemy's Pokemon party. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If any parameter is null.</exception>
        public void Initialize(
            BattleRules rules,
            IReadOnlyList<PokemonInstance> playerParty,
            IReadOnlyList<PokemonInstance> enemyParty)
        {
            if (rules == null)
                throw new ArgumentNullException(nameof(rules));
            if (playerParty == null)
                throw new ArgumentNullException(nameof(playerParty), ErrorMessages.PartyCannotBeNull);
            if (enemyParty == null)
                throw new ArgumentNullException(nameof(enemyParty), ErrorMessages.PartyCannotBeNull);

            _rules = rules;

            // Create sides
            _playerSide = new BattleSide(rules.PlayerSlots, isPlayer: true);
            _enemySide = new BattleSide(rules.EnemySlots, isPlayer: false);

            // Set parties
            _playerSide.SetParty(playerParty);
            _enemySide.SetParty(enemyParty);

            // Place initial Pokemon in slots
            PlaceInitialPokemon(_playerSide, playerParty);
            PlaceInitialPokemon(_enemySide, enemyParty);
        }

        /// <summary>
        /// Gets a specific slot from either side.
        /// </summary>
        /// <param name="isPlayer">True for player side, false for enemy.</param>
        /// <param name="index">The slot index.</param>
        /// <returns>The requested slot.</returns>
        public BattleSlot GetSlot(bool isPlayer, int index)
        {
            var side = isPlayer ? _playerSide : _enemySide;
            return side.GetSlot(index);
        }

        /// <summary>
        /// Gets all active (non-empty, non-fainted) slots from both sides.
        /// </summary>
        /// <returns>All active slots.</returns>
        public IEnumerable<BattleSlot> GetAllActiveSlots()
        {
            return _playerSide.GetActiveSlots().Concat(_enemySide.GetActiveSlots());
        }

        /// <summary>
        /// Gets the opposite side from the given side.
        /// </summary>
        /// <param name="side">The side to get the opposite of.</param>
        /// <returns>The opposite side.</returns>
        /// <exception cref="ArgumentException">If the side is not part of this field.</exception>
        public BattleSide GetOppositeSide(BattleSide side)
        {
            if (side == _playerSide)
                return _enemySide;
            if (side == _enemySide)
                return _playerSide;

            throw new ArgumentException("Side is not part of this battlefield", nameof(side));
        }

        private void PlaceInitialPokemon(BattleSide side, IReadOnlyList<PokemonInstance> party)
        {
            var healthyPokemon = party.Where(p => !p.IsFainted).ToList();

            for (int i = 0; i < side.Slots.Count && i < healthyPokemon.Count; i++)
            {
                side.Slots[i].SetPokemon(healthyPokemon[i]);
            }
        }
    }
}

