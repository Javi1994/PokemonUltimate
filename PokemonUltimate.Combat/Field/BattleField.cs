using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat.ValueObjects;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Domain.Instances;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

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
        private WeatherState _weatherState;
        private TerrainState _terrainState;

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
        public Weather Weather => _weatherState.Weather;

        /// <summary>
        /// The remaining duration of the current weather in turns.
        /// 0 means infinite duration (primal weather or indefinite weather).
        /// </summary>
        public int WeatherDuration => _weatherState.Duration;

        /// <summary>
        /// The weather data for the current weather condition.
        /// Null if no weather is active.
        /// </summary>
        public WeatherData WeatherData => _weatherState.WeatherData;

        /// <summary>
        /// The current terrain condition on the battlefield.
        /// </summary>
        /// <remarks>
        /// **Feature**: 2: Combat System
        /// **Sub-Feature**: 2.13: Terrain System
        /// **Documentation**: See `docs/features/2-combat-system/2.13-terrain-system/README.md`
        /// </remarks>
        public Terrain Terrain => _terrainState.Terrain;

        /// <summary>
        /// The remaining duration of the current terrain in turns.
        /// 0 means infinite duration.
        /// </summary>
        /// <remarks>
        /// **Feature**: 2: Combat System
        /// **Sub-Feature**: 2.13: Terrain System
        /// **Documentation**: See `docs/features/2-combat-system/2.13-terrain-system/README.md`
        /// </remarks>
        public int TerrainDuration => _terrainState.Duration;

        /// <summary>
        /// The terrain data for the current terrain condition.
        /// Null if no terrain is active.
        /// </summary>
        /// <remarks>
        /// **Feature**: 2: Combat System
        /// **Sub-Feature**: 2.13: Terrain System
        /// **Documentation**: See `docs/features/2-combat-system/2.13-terrain-system/README.md`
        /// </remarks>
        public TerrainData TerrainData => _terrainState.TerrainData;

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

            // Initialize weather and terrain to None
            _weatherState = new WeatherState();
            _terrainState = new TerrainState();

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
                var pokemon = healthyPokemon[i];

                // Apply Boss multipliers if this is a Boss battle and it's an enemy Pokemon
                if (_rules.IsBossBattle && !side.IsPlayer)
                {
                    pokemon.ApplyBossMultipliers(_rules.BossMultiplier, _rules.BossStatMultiplier);
                }

                side.Slots[i].SetPokemon(pokemon);
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
            _weatherState = _weatherState.SetWeather(weather, duration, weatherData);
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
            _weatherState = _weatherState.Clear();
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
            _weatherState = _weatherState.DecrementDuration();
        }

        #endregion

        #region Terrain Management

        /// <summary>
        /// Sets the terrain condition on the battlefield.
        /// </summary>
        /// <param name="terrain">The terrain to set. Use Terrain.None to clear.</param>
        /// <param name="duration">Duration in turns. 0 means infinite duration.</param>
        /// <param name="terrainData">The terrain data for this terrain condition. Can be null if not available.</param>
        /// <remarks>
        /// **Feature**: 2: Combat System
        /// **Sub-Feature**: 2.13: Terrain System
        /// **Documentation**: See `docs/features/2-combat-system/2.13-terrain-system/README.md`
        /// </remarks>
        public void SetTerrain(Terrain terrain, int duration, TerrainData terrainData = null)
        {
            _terrainState = _terrainState.SetTerrain(terrain, duration, terrainData);
        }

        /// <summary>
        /// Clears the terrain condition from the battlefield.
        /// </summary>
        /// <remarks>
        /// **Feature**: 2: Combat System
        /// **Sub-Feature**: 2.13: Terrain System
        /// **Documentation**: See `docs/features/2-combat-system/2.13-terrain-system/README.md`
        /// </remarks>
        public void ClearTerrain()
        {
            _terrainState = _terrainState.Clear();
        }

        /// <summary>
        /// Decrements the terrain duration by one turn.
        /// If duration reaches 0 and terrain is not infinite, clears the terrain.
        /// </summary>
        /// <remarks>
        /// **Feature**: 2: Combat System
        /// **Sub-Feature**: 2.13: Terrain System
        /// **Documentation**: See `docs/features/2-combat-system/2.13-terrain-system/README.md`
        /// </remarks>
        public void DecrementTerrainDuration()
        {
            _terrainState = _terrainState.DecrementDuration();
        }

        #endregion
    }
}

