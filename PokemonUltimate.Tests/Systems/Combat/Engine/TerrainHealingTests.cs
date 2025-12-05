using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Content.Catalogs.Terrain;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Systems.Combat.Engine
{
    /// <summary>
    /// Tests for Terrain Healing in EndOfTurnProcessor.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.13: Terrain System
    /// **Documentation**: See `docs/features/2-combat-system/2.13-terrain-system/README.md`
    /// </remarks>
    [TestFixture]
    public class TerrainHealingTests
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
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50) // Grounded Pokemon
            };

            _enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50) // Grounded Pokemon
            };

            _field.Initialize(_rules, _playerParty, _enemyParty);

            // Create processor instance with required dependencies
            var damageContextFactory = new DamageContextFactory();
            _processor = new EndOfTurnProcessor(damageContextFactory);
        }

        #region Grassy Terrain Healing Tests

        [Test]
        public void ProcessTerrainHealing_GrassyTerrain_HealsGroundedPokemon()
        {
            _field.SetTerrain(Terrain.Grassy, 5, TerrainCatalog.GetByTerrain(Terrain.Grassy));
            var slot = _field.PlayerSide.Slots[0];
            var pokemon = slot.Pokemon;
            int maxHP = pokemon.MaxHP;
            int currentHP = pokemon.CurrentHP;
            int expectedHealing = maxHP / 16;

            // Damage the Pokemon first
            pokemon.TakeDamage(maxHP / 2);

            var actions = _processor.ProcessEffects(_field).ToList();

            // Grassy Terrain should heal grounded Pokemon by 1/16 max HP
            var healAction = actions.OfType<HealAction>()
                .FirstOrDefault(a => a.Target == slot);

            Assert.That(healAction, Is.Not.Null);
            Assert.That(healAction.Amount, Is.EqualTo(expectedHealing));
        }

        [Test]
        public void ProcessTerrainHealing_GrassyTerrain_FlyingTypeNotHealed()
        {
            // Create a Flying-type Pokemon (not grounded)
            var flyingPokemon = PokemonFactory.Create(PokemonCatalog.Charizard, 50);
            var flyingParty = new List<PokemonInstance> { flyingPokemon };
            var field = new BattleField();
            field.Initialize(_rules, flyingParty, _enemyParty);
            field.SetTerrain(Terrain.Grassy, 5, TerrainCatalog.GetByTerrain(Terrain.Grassy));

            var slot = field.PlayerSide.Slots[0];
            var pokemon = slot.Pokemon;
            int maxHP = pokemon.MaxHP;

            // Damage the Pokemon first
            pokemon.TakeDamage(maxHP / 2);

            var processor = new EndOfTurnProcessor(new DamageContextFactory());
            var actions = processor.ProcessEffects(field).ToList();

            // Flying types should not be healed by Grassy Terrain (not grounded)
            var healAction = actions.OfType<HealAction>()
                .FirstOrDefault(a => a.Target == slot);

            Assert.That(healAction, Is.Null);
        }

        [Test]
        public void ProcessTerrainHealing_GrassyTerrain_HealsAllGroundedPokemon()
        {
            _field.SetTerrain(Terrain.Grassy, 5, TerrainCatalog.GetByTerrain(Terrain.Grassy));

            var playerSlot = _field.PlayerSide.Slots[0];
            var enemySlot = _field.EnemySide.Slots[0];

            // Damage both Pokemon
            playerSlot.Pokemon.TakeDamage(playerSlot.Pokemon.MaxHP / 2);
            enemySlot.Pokemon.TakeDamage(enemySlot.Pokemon.MaxHP / 2);

            var actions = _processor.ProcessEffects(_field).ToList();

            // Both grounded Pokemon should be healed
            var playerHealAction = actions.OfType<HealAction>()
                .FirstOrDefault(a => a.Target == playerSlot);
            var enemyHealAction = actions.OfType<HealAction>()
                .FirstOrDefault(a => a.Target == enemySlot);

            Assert.That(playerHealAction, Is.Not.Null);
            Assert.That(enemyHealAction, Is.Not.Null);
        }

        [Test]
        public void ProcessTerrainHealing_GrassyTerrain_DoesNotHealFaintedPokemon()
        {
            _field.SetTerrain(Terrain.Grassy, 5, TerrainCatalog.GetByTerrain(Terrain.Grassy));
            var slot = _field.PlayerSide.Slots[0];
            var pokemon = slot.Pokemon;

            // Faint the Pokemon
            pokemon.TakeDamage(pokemon.MaxHP);

            var actions = _processor.ProcessEffects(_field).ToList();

            // Fainted Pokemon should not be healed
            var healAction = actions.OfType<HealAction>()
                .FirstOrDefault(a => a.Target == slot);

            Assert.That(healAction, Is.Null);
        }

        [Test]
        public void ProcessTerrainHealing_GrassyTerrain_DoesNotHealAtFullHP()
        {
            _field.SetTerrain(Terrain.Grassy, 5, TerrainCatalog.GetByTerrain(Terrain.Grassy));
            var slot = _field.PlayerSide.Slots[0];
            var pokemon = slot.Pokemon;

            // Ensure Pokemon is at full HP
            pokemon.Heal(pokemon.MaxHP);

            var actions = _processor.ProcessEffects(_field).ToList();

            // Pokemon at full HP should not be healed (or healing should be 0)
            var healAction = actions.OfType<HealAction>()
                .FirstOrDefault(a => a.Target == slot);

            // Healing can still be generated, but amount should be 0 or action should not exist
            if (healAction != null)
            {
                Assert.That(healAction.Amount, Is.EqualTo(0));
            }
        }

        #endregion

        #region Other Terrain Tests

        [Test]
        public void ProcessTerrainHealing_ElectricTerrain_NoHealing()
        {
            _field.SetTerrain(Terrain.Electric, 5, TerrainCatalog.GetByTerrain(Terrain.Electric));
            var slot = _field.PlayerSide.Slots[0];
            var pokemon = slot.Pokemon;

            // Damage the Pokemon
            pokemon.TakeDamage(pokemon.MaxHP / 2);

            var actions = _processor.ProcessEffects(_field).ToList();

            // Electric Terrain does not heal
            var healAction = actions.OfType<HealAction>()
                .FirstOrDefault(a => a.Target == slot);

            Assert.That(healAction, Is.Null);
        }

        [Test]
        public void ProcessTerrainHealing_NoTerrain_NoHealing()
        {
            // No terrain set
            var slot = _field.PlayerSide.Slots[0];
            var pokemon = slot.Pokemon;

            // Damage the Pokemon
            pokemon.TakeDamage(pokemon.MaxHP / 2);

            var actions = _processor.ProcessEffects(_field).ToList();

            // No terrain should mean no healing
            var healAction = actions.OfType<HealAction>()
                .FirstOrDefault(a => a.Target == slot);

            Assert.That(healAction, Is.Null);
        }

        #endregion
    }
}

