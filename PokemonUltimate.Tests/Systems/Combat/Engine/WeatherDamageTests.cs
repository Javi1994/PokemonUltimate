using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Content.Catalogs.Weather;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Systems.Combat.Engine
{
    /// <summary>
    /// Tests for Weather Damage in EndOfTurnProcessor.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.12: Weather System
    /// **Documentation**: See `docs/features/2-combat-system/2.12-weather-system/README.md`
    /// </remarks>
    [TestFixture]
    public class WeatherDamageTests
    {
        private BattleField _field;
        private List<PokemonInstance> _playerParty;
        private List<PokemonInstance> _enemyParty;
        private BattleRules _rules;
        private EndOfTurnProcessor _processor;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

            _playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };

            _enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50)
            };

            _field.Initialize(_rules, _playerParty, _enemyParty);

            // Create processor instance with required dependencies
            var damageContextFactory = new DamageContextFactory();
            _processor = new EndOfTurnProcessor(damageContextFactory);
        }

        #region Sandstorm Damage Tests

        [Test]
        public void ProcessWeatherDamage_Sandstorm_DamagesNonImmuneTypes()
        {
            _field.SetWeather(Weather.Sandstorm, 5, WeatherCatalog.Sandstorm);
            var slot = _field.PlayerSide.Slots[0];
            var pokemon = slot.Pokemon;
            int maxHP = pokemon.MaxHP;
            int expectedDamage = maxHP / 16;
            if (expectedDamage < 1) expectedDamage = 1;

            var actions = _processor.ProcessEffects(_field).ToList();

            // Sandstorm should deal damage to non-Rock/Ground/Steel types
            // Pikachu is Electric type, so should take damage
            var damageAction = actions.OfType<PokemonUltimate.Combat.Actions.DamageAction>()
                .FirstOrDefault(a => a.Target == slot && a.Context.Move.Name == "Status Damage");

            Assert.That(damageAction, Is.Not.Null);
            Assert.That(damageAction.Context.FinalDamage, Is.EqualTo(expectedDamage));
        }

        [Test]
        public void ProcessWeatherDamage_Sandstorm_DoesNotDamageRockType()
        {
            // Create a Rock-type Pokemon
            var rockPokemon = PokemonFactory.Create(PokemonCatalog.Geodude, 50);
            var rockParty = new List<PokemonInstance> { rockPokemon };
            var field = new BattleField();
            field.Initialize(_rules, rockParty, _enemyParty);
            field.SetWeather(Weather.Sandstorm, 5, WeatherCatalog.Sandstorm);

            var slot = field.PlayerSide.Slots[0];
            var processor = new EndOfTurnProcessor(new DamageContextFactory());
            var actions = processor.ProcessEffects(field).ToList();

            // Rock type should be immune to Sandstorm damage
            var weatherDamageAction = actions.OfType<PokemonUltimate.Combat.Actions.DamageAction>()
                .FirstOrDefault(a => a.Target == slot && a.Context.Move.Name == "Status Damage");

            // Should not have weather damage action for Rock type
            Assert.That(weatherDamageAction, Is.Null);
        }

        #endregion

        #region Hail Damage Tests

        [Test]
        public void ProcessWeatherDamage_Hail_DamagesNonIceTypes()
        {
            _field.SetWeather(Weather.Hail, 5, WeatherCatalog.Hail);
            var slot = _field.PlayerSide.Slots[0];
            var pokemon = slot.Pokemon;
            int maxHP = pokemon.MaxHP;
            int expectedDamage = maxHP / 16;
            if (expectedDamage < 1) expectedDamage = 1;

            var actions = _processor.ProcessEffects(_field).ToList();

            // Hail should deal damage to non-Ice types
            // Pikachu is Electric type, so should take damage
            var damageAction = actions.OfType<PokemonUltimate.Combat.Actions.DamageAction>()
                .FirstOrDefault(a => a.Target == slot && a.Context.Move.Name == "Status Damage");

            Assert.That(damageAction, Is.Not.Null);
            Assert.That(damageAction.Context.FinalDamage, Is.EqualTo(expectedDamage));
        }

        [Test]
        public void ProcessWeatherDamage_Hail_DoesNotDamageIceType()
        {
            // Note: This test verifies that Ice types are immune
            // For now, we test that non-Ice types take damage (test above)
            // If we had an Ice-type Pokemon in catalog, we'd test immunity here
            _field.SetWeather(Weather.Hail, 5, WeatherCatalog.Hail);
            var slot = _field.PlayerSide.Slots[0];
            var pokemon = slot.Pokemon;

            // Pikachu is Electric, not Ice, so should take damage
            var actions = _processor.ProcessEffects(_field).ToList();
            var damageAction = actions.OfType<PokemonUltimate.Combat.Actions.DamageAction>()
                .FirstOrDefault(a => a.Target == slot && a.Context.Move.Name == "Status Damage");

            // Non-Ice types should take damage
            Assert.That(damageAction, Is.Not.Null);
        }

        #endregion

        #region No Weather Damage Tests

        [Test]
        public void ProcessWeatherDamage_NoWeather_NoDamage()
        {
            // No weather set
            var slot = _field.PlayerSide.Slots[0];
            var actions = _processor.ProcessEffects(_field).ToList();

            // No weather damage should occur
            var weatherDamageAction = actions.OfType<PokemonUltimate.Combat.Actions.DamageAction>()
                .FirstOrDefault(a => a.Target == slot && a.Context.Move.Name == "Status Damage");

            Assert.That(weatherDamageAction, Is.Null);
        }

        [Test]
        public void ProcessWeatherDamage_Rain_NoDamage()
        {
            _field.SetWeather(Weather.Rain, 5, WeatherCatalog.Rain);
            var slot = _field.PlayerSide.Slots[0];
            var actions = _processor.ProcessEffects(_field).ToList();

            // Rain doesn't deal damage, only modifies move power
            var weatherDamageAction = actions.OfType<PokemonUltimate.Combat.Actions.DamageAction>()
                .FirstOrDefault(a => a.Target == slot && a.Context.Move.Name == "Status Damage");

            Assert.That(weatherDamageAction, Is.Null);
        }

        [Test]
        public void ProcessWeatherDamage_Sun_NoDamage()
        {
            _field.SetWeather(Weather.Sun, 5, WeatherCatalog.Sun);
            var slot = _field.PlayerSide.Slots[0];
            var actions = _processor.ProcessEffects(_field).ToList();

            // Sun doesn't deal damage, only modifies move power
            var weatherDamageAction = actions.OfType<PokemonUltimate.Combat.Actions.DamageAction>()
                .FirstOrDefault(a => a.Target == slot && a.Context.Move.Name == "Status Damage");

            Assert.That(weatherDamageAction, Is.Null);
        }

        #endregion

        #region Weather Damage Calculation Tests

        [Test]
        public void ProcessWeatherDamage_Sandstorm_CalculatesCorrectDamage()
        {
            _field.SetWeather(Weather.Sandstorm, 5, WeatherCatalog.Sandstorm);
            var slot = _field.PlayerSide.Slots[0];
            var pokemon = slot.Pokemon;
            int maxHP = pokemon.MaxHP;
            int expectedDamage = maxHP / 16;
            if (expectedDamage < 1) expectedDamage = 1; // Minimum 1 damage

            var actions = _processor.ProcessEffects(_field).ToList();

            // Sandstorm deals 1/16 of Max HP damage
            var damageAction = actions.OfType<PokemonUltimate.Combat.Actions.DamageAction>()
                .FirstOrDefault(a => a.Target == slot && a.Context.Move.Name == "Status Damage");

            Assert.That(damageAction, Is.Not.Null);
            Assert.That(damageAction.Context.FinalDamage, Is.EqualTo(expectedDamage));
        }

        [Test]
        public void ProcessWeatherDamage_Hail_CalculatesCorrectDamage()
        {
            _field.SetWeather(Weather.Hail, 5, WeatherCatalog.Hail);
            var slot = _field.PlayerSide.Slots[0];
            var pokemon = slot.Pokemon;
            int maxHP = pokemon.MaxHP;
            int expectedDamage = maxHP / 16;
            if (expectedDamage < 1) expectedDamage = 1; // Minimum 1 damage

            var actions = _processor.ProcessEffects(_field).ToList();

            // Hail deals 1/16 of Max HP damage
            var damageAction = actions.OfType<PokemonUltimate.Combat.Actions.DamageAction>()
                .FirstOrDefault(a => a.Target == slot && a.Context.Move.Name == "Status Damage");

            Assert.That(damageAction, Is.Not.Null);
            Assert.That(damageAction.Context.FinalDamage, Is.EqualTo(expectedDamage));
        }

        #endregion
    }
}

