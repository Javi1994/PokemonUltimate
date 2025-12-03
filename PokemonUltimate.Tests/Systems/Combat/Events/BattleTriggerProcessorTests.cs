using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Events;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Builders;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Items;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Events
{
    /// <summary>
    /// Functional tests for BattleTriggerProcessor - processes battle triggers.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.9: Abilities & Items
    /// **Documentation**: See `docs/features/2-combat-system/2.9-abilities-items/architecture.md`
    /// </remarks>
    [TestFixture]
    public class BattleTriggerProcessorTests
    {
        private BattleField _field;
        private BattleSlot _playerSlot;
        private BattleSlot _enemySlot;
        private PokemonInstance _playerPokemon;
        private PokemonInstance _enemyPokemon;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _playerSlot = _field.PlayerSide.Slots[0];
            _enemySlot = _field.EnemySide.Slots[0];
            _playerPokemon = _playerSlot.Pokemon;
            _enemyPokemon = _enemySlot.Pokemon;
        }

        #region OnTurnEnd Tests

        [Test]
        public void ProcessTrigger_OnTurnEnd_WithLeftovers_GeneratesHealAction()
        {
            // Arrange
            _playerPokemon.HeldItem = ItemCatalog.Leftovers;
            // Ensure Pokemon is not at full HP
            int maxHP = _playerPokemon.MaxHP;
            _playerPokemon.CurrentHP = maxHP - 10; // Damage Pokemon

            // Act
            var actions = BattleTriggerProcessor.ProcessTrigger(BattleTrigger.OnTurnEnd, _field);

            // Assert
            Assert.That(actions.Count, Is.GreaterThan(0), "Should generate actions for Leftovers");
            Assert.That(actions.Any(a => a is MessageAction), Is.True, "Should generate message action");
            Assert.That(actions.Any(a => a is HealAction), Is.True, "Should generate heal action");
        }

        [Test]
        public void ProcessTrigger_OnTurnEnd_NoItems_ReturnsEmpty()
        {
            // Arrange - no items

            // Act
            var actions = BattleTriggerProcessor.ProcessTrigger(BattleTrigger.OnTurnEnd, _field);

            // Assert
            Assert.That(actions, Is.Empty);
        }

        [Test]
        public void ProcessTrigger_OnTurnEnd_FullHP_NoHealing()
        {
            // Arrange
            _playerPokemon.HeldItem = ItemCatalog.Leftovers;
            int maxHP = _playerPokemon.MaxHP;
            _playerPokemon.CurrentHP = maxHP; // Full HP

            // Act
            var actions = BattleTriggerProcessor.ProcessTrigger(BattleTrigger.OnTurnEnd, _field);

            // Assert
            Assert.That(actions, Is.Empty);
        }

        #endregion

        #region OnSwitchIn Tests

        [Test]
        public void ProcessTrigger_OnSwitchIn_WithIntimidate_GeneratesStatChangeAction()
        {
            // Arrange
            var intimidateAbility = Ability.Define("Intimidate")
                .Description("Lowers opposing Pokémon's Attack stat.")
                .Gen(3)
                .OnTrigger(AbilityTrigger.OnSwitchIn)
                .LowersOpponentStat(Stat.Attack, -1)
                .Build();

            _playerPokemon.SetAbility(intimidateAbility);

            // Act
            var actions = BattleTriggerProcessor.ProcessTrigger(BattleTrigger.OnSwitchIn, _field);

            // Assert
            Assert.That(actions.Count, Is.GreaterThan(0));
            Assert.That(actions.Any(a => a is MessageAction), Is.True);
            Assert.That(actions.Any(a => a is StatChangeAction), Is.True);
        }

        [Test]
        public void ProcessTrigger_OnSwitchIn_NoAbilities_ReturnsEmpty()
        {
            // Arrange - no abilities

            // Act
            var actions = BattleTriggerProcessor.ProcessTrigger(BattleTrigger.OnSwitchIn, _field);

            // Assert
            Assert.That(actions, Is.Empty);
        }

        #endregion

        #region Edge Cases

        [Test]
        public void ProcessTrigger_NullField_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<System.ArgumentNullException>(() =>
                BattleTriggerProcessor.ProcessTrigger(BattleTrigger.OnTurnEnd, null));
        }

        [Test]
        public void ProcessTrigger_EmptySlot_SkipsProcessing()
        {
            // Arrange
            _playerSlot.ClearSlot();

            // Act
            var actions = BattleTriggerProcessor.ProcessTrigger(BattleTrigger.OnTurnEnd, _field);

            // Assert - should not throw, just skip empty slots
            Assert.That(actions, Is.Not.Null);
        }

        [Test]
        public void ProcessTrigger_FaintedPokemon_SkipsProcessing()
        {
            // Arrange
            _playerPokemon.CurrentHP = 0; // Fainted
            _playerPokemon.HeldItem = ItemCatalog.Leftovers;

            // Act
            var actions = BattleTriggerProcessor.ProcessTrigger(BattleTrigger.OnTurnEnd, _field);

            // Assert - fainted Pokemon shouldn't trigger items
            Assert.That(actions, Is.Empty);
        }

        #endregion
    }
}

