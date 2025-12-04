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
    /// Represents the complete battlefield with both player and enemy sides.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    public class BattleField
    {
        private BattleSide _playerSide;
        private BattleSide _enemySide;
        private BattleRules _rules;
        private Weather _weather;
        private int _weatherDuration;
        private WeatherData _weatherData;

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
        /// The current weather condition on the battlefield.
        /// </summary>
        public Weather Weather => _weather;

        /// <summary>
        /// The remaining duration of the current weather in turns.
        /// 0 means infinite duration (primal weather or indefinite weather).
        /// </summary>
        public int WeatherDuration => _weatherDuration;

        /// <summary>
        /// The weather data for the current weather condition.
        /// Null if no weather is active.
        /// </summary>
        public WeatherData WeatherData => _weatherData;

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

            // Initialize weather to None
            _weather = Weather.None;
            _weatherDuration = 0;
            _weatherData = null;

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

        #region Weather Management

        /// <summary>
        /// Sets the weather condition on the battlefield.
        /// </summary>
        /// <param name="weather">The weather to set. Use Weather.None to clear.</param>
        /// <param name="duration">Duration in turns. 0 means infinite duration.</param>
        /// <param name="weatherData">The weather data for this weather condition. Can be null if not available.</param>
        /// <remarks>
        /// **Feature**: 2: Combat System
        /// **Sub-Feature**: 2.12: Weather System
        /// **Documentation**: See `docs/features/2-combat-system/2.12-weather-system/README.md`
        /// 
        /// Note: Primal weather overwrite protection should be handled by the caller (e.g., SetWeatherAction)
        /// by checking WeatherData.CanBeOverwritten before calling this method.
        /// </remarks>
        public void SetWeather(Weather weather, int duration, WeatherData weatherData = null)
        {
            // Clear weather if None is specified
            if (weather == Weather.None)
            {
                ClearWeather();
                return;
            }

            _weather = weather;
            _weatherDuration = duration;
            _weatherData = weatherData;
        }

        /// <summary>
        /// Clears the weather condition from the battlefield.
        /// </summary>
        /// <remarks>
        /// **Feature**: 2: Combat System
        /// **Sub-Feature**: 2.12: Weather System
        /// **Documentation**: See `docs/features/2-combat-system/2.12-weather-system/README.md`
        /// </remarks>
        public void ClearWeather()
        {
            _weather = Weather.None;
            _weatherDuration = 0;
            _weatherData = null;
        }

        /// <summary>
        /// Decrements the weather duration by one turn.
        /// If duration reaches 0 and weather is not infinite, clears the weather.
        /// </summary>
        /// <remarks>
        /// **Feature**: 2: Combat System
        /// **Sub-Feature**: 2.12: Weather System
        /// **Documentation**: See `docs/features/2-combat-system/2.12-weather-system/README.md`
        /// </remarks>
        public void DecrementWeatherDuration()
        {
            if (_weather == Weather.None)
                return;

            // Infinite duration (0) does not decrement
            if (_weatherDuration == 0)
                return;

            _weatherDuration--;

            // If duration reached 0, clear weather
            if (_weatherDuration <= 0)
            {
                ClearWeather();
            }
        }

        #endregion
    }
}

