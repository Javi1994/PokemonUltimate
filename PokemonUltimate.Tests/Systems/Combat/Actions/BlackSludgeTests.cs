using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Events;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Items;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Actions
{
    /// <summary>
    /// Functional tests for Black Sludge item - heals Poison types, damages others.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.18: Advanced Items
    /// **Documentation**: See `docs/features/2-combat-system/2.18-advanced-items/README.md`
    /// </remarks>
    [TestFixture]
    public class BlackSludgeTests
    {
        private BattleField _field;
        private BattleSlot _playerSlot;
        private BattleSlot _enemySlot;
        private PokemonInstance _playerPokemon;
        private PokemonInstance _enemyPokemon;
        private BattleTriggerProcessor _processor;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _playerSlot = _field.PlayerSide.Slots[0];
            _enemySlot = _field.EnemySide.Slots[0];
            _playerPokemon = _playerSlot.Pokemon;
            _enemyPokemon = _enemySlot.Pokemon;
            
            _processor = new BattleTriggerProcessor();
        }

        #region OnTurnEnd Tests

        [Test]
        public void ProcessTrigger_OnTurnEnd_WithBlackSludge_OnPoisonType_Heals()
        {
            // Arrange - Bulbasaur is Grass/Poison type
            _playerPokemon.HeldItem = ItemCatalog.BlackSludge;
            int maxHP = _playerPokemon.MaxHP;
            int initialHP = maxHP - 20; // Damage Pokemon
            _playerPokemon.CurrentHP = initialHP;

            // Act - Process trigger and execute actions
            var actions = _processor.ProcessTrigger(BattleTrigger.OnTurnEnd, _field).ToList();
            foreach (var action in actions)
            {
                action.ExecuteLogic(_field).ToList();
            }

            // Assert - Black Sludge should heal Poison types
            int expectedHeal = maxHP / 16; // 1/16 of max HP
            Assert.That(_playerPokemon.CurrentHP, Is.EqualTo(initialHP + expectedHeal), 
                $"Black Sludge should heal Poison types by {expectedHeal} HP");
        }

        [Test]
        public void ProcessTrigger_OnTurnEnd_WithBlackSludge_OnNonPoisonType_Damages()
        {
            // Arrange - Charmander is Fire type (not Poison)
            _enemyPokemon.HeldItem = ItemCatalog.BlackSludge;
            int initialHP = _enemyPokemon.CurrentHP;

            // Act - Process trigger and execute actions
            var actions = _processor.ProcessTrigger(BattleTrigger.OnTurnEnd, _field).ToList();
            foreach (var action in actions)
            {
                action.ExecuteLogic(_field).ToList();
            }

            // Assert - Black Sludge should damage non-Poison types
            // Black Sludge uses LeftoversHealDivisor (16) for damage calculation
            int expectedDamage = _enemyPokemon.MaxHP / 16; // 1/16 of max HP (same divisor as Leftovers heal)
            Assert.That(_enemyPokemon.CurrentHP, Is.EqualTo(initialHP - expectedDamage), 
                $"Black Sludge should damage non-Poison types by {expectedDamage} HP");
        }

        [Test]
        public void ProcessTrigger_OnTurnEnd_WithBlackSludge_OnPoisonType_AtFullHP_NoHealing()
        {
            // Arrange - Bulbasaur at full HP
            _playerPokemon.HeldItem = ItemCatalog.BlackSludge;
            int maxHP = _playerPokemon.MaxHP;
            _playerPokemon.CurrentHP = maxHP; // Full HP

            // Act
            var actions = _processor.ProcessTrigger(BattleTrigger.OnTurnEnd, _field).ToList();

            // Assert - Should not heal if at full HP
            Assert.That(_playerPokemon.CurrentHP, Is.EqualTo(maxHP), "Should not heal if at full HP");
        }

        [Test]
        public void ProcessTrigger_OnTurnEnd_WithoutBlackSludge_NoEffect()
        {
            // Arrange - No Black Sludge
            int initialHP = _playerPokemon.CurrentHP;

            // Act
            var actions = _processor.ProcessTrigger(BattleTrigger.OnTurnEnd, _field).ToList();

            // Assert - No effect without Black Sludge
            Assert.That(_playerPokemon.CurrentHP, Is.EqualTo(initialHP), "No effect without Black Sludge");
        }

        #endregion
    }
}

