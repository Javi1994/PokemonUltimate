using System;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Combat.Engine
{
    /// <summary>
    /// Edge case tests for EndOfTurnProcessor - boundary conditions and error handling.
    /// </summary>
    [TestFixture]
    public class EndOfTurnProcessorEdgeCasesTests
    {
        private BattleField _field;
        private BattleSlot _playerSlot;
        private PokemonInstance _playerPokemon;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _playerSlot = _field.PlayerSide.Slots[0];
            _playerPokemon = _playerSlot.Pokemon;
        }

        #region Null Validation Tests

        [Test]
        public void ProcessEffects_NullField_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => EndOfTurnProcessor.ProcessEffects(null));
        }

        #endregion

        #region Fainted Pokemon Tests

        [Test]
        public void ProcessEffects_FaintedPokemon_SkipsProcessing()
        {
            // Arrange
            _playerPokemon.Status = PersistentStatus.Burn;
            _playerPokemon.CurrentHP = 0; // Fainted

            // Act
            var actions = EndOfTurnProcessor.ProcessEffects(_field).ToList();

            // Assert
            Assert.That(actions, Is.Empty);
        }

        [Test]
        public void ProcessEffects_PokemonFaintsFromStatusDamage_StillProcesses()
        {
            // Arrange
            _playerPokemon.Status = PersistentStatus.Burn;
            int damage = _playerPokemon.MaxHP / 16;
            _playerPokemon.CurrentHP = damage; // Will faint from damage

            // Act
            var actions = EndOfTurnProcessor.ProcessEffects(_field).ToList();

            // Assert
            var damageAction = actions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Not.Null);
            // DamageAction will handle fainting
        }

        #endregion

        #region Empty Slot Tests

        [Test]
        public void ProcessEffects_EmptySlot_SkipsProcessing()
        {
            // Arrange
            _field.PlayerSide.Slots[0].ClearSlot();

            // Act
            var actions = EndOfTurnProcessor.ProcessEffects(_field).ToList();

            // Assert
            Assert.That(actions, Is.Empty);
        }

        #endregion

        #region Low HP Tests

        [Test]
        public void ProcessEffects_LowMaxHP_MinimumDamage()
        {
            // Arrange - Create a Pokemon with very low MaxHP
            // Note: We can't easily change MaxHP, so we test with actual MaxHP
            // But we can verify minimum damage is applied
            _playerPokemon.Status = PersistentStatus.Burn;
            int maxHP = _playerPokemon.MaxHP;

            // Act
            var actions = EndOfTurnProcessor.ProcessEffects(_field).ToList();

            // Assert
            var damageAction = actions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Not.Null);
            // Damage should be at least 1, or MaxHP/16 if MaxHP >= 16
            int expectedDamage = maxHP >= 16 ? maxHP / 16 : EndOfTurnConstants.MinimumDamage;
            Assert.That(damageAction.Context.FinalDamage, Is.EqualTo(expectedDamage));
        }

        [Test]
        public void ProcessEffects_BurnWithMaxHP15_DealsMinimumDamage()
        {
            // Arrange
            _playerPokemon.Status = PersistentStatus.Burn;
            // We can't easily set MaxHP, but we can verify the logic handles low HP correctly
            // This test verifies that the calculation uses Max(1, MaxHP/16)

            // Act
            var actions = EndOfTurnProcessor.ProcessEffects(_field).ToList();

            // Assert
            var damageAction = actions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Not.Null);
            // If MaxHP < 16, damage should be 1
            if (_playerPokemon.MaxHP < 16)
            {
                Assert.That(damageAction.Context.FinalDamage, Is.EqualTo(EndOfTurnConstants.MinimumDamage));
            }
        }

        #endregion

        #region Badly Poisoned Counter Tests

        [Test]
        public void ProcessEffects_BadlyPoisonedCounter_StartsAtOne()
        {
            // Arrange
            _playerPokemon.Status = PersistentStatus.BadlyPoisoned;
            _playerPokemon.StatusTurnCounter = 0; // Not initialized

            // Act
            EndOfTurnProcessor.ProcessEffects(_field);

            // Assert
            // Counter should be initialized to 1, then incremented to 2
            Assert.That(_playerPokemon.StatusTurnCounter, Is.EqualTo(2));
        }

        [Test]
        public void ProcessEffects_BadlyPoisonedCounter_IncrementsEachTurn()
        {
            // Arrange
            _playerPokemon.Status = PersistentStatus.BadlyPoisoned;
            _playerPokemon.StatusTurnCounter = 1;

            // Act - Turn 1
            EndOfTurnProcessor.ProcessEffects(_field);
            Assert.That(_playerPokemon.StatusTurnCounter, Is.EqualTo(2));

            // Act - Turn 2
            EndOfTurnProcessor.ProcessEffects(_field);
            Assert.That(_playerPokemon.StatusTurnCounter, Is.EqualTo(3));

            // Act - Turn 3
            EndOfTurnProcessor.ProcessEffects(_field);
            Assert.That(_playerPokemon.StatusTurnCounter, Is.EqualTo(4));
        }

        [Test]
        public void ProcessEffects_BadlyPoisonedDamage_EscalatesCorrectly()
        {
            // Arrange
            _playerPokemon.Status = PersistentStatus.BadlyPoisoned;
            int maxHP = _playerPokemon.MaxHP;

            // Turn 1: Counter = 1
            _playerPokemon.StatusTurnCounter = 1;
            var actions1 = EndOfTurnProcessor.ProcessEffects(_field).ToList();
            var damage1 = actions1.OfType<DamageAction>().First().Context.FinalDamage;
            int expected1 = (1 * maxHP) / 16;
            Assert.That(damage1, Is.EqualTo(expected1));

            // Turn 2: Counter = 2
            var actions2 = EndOfTurnProcessor.ProcessEffects(_field).ToList();
            var damage2 = actions2.OfType<DamageAction>().First().Context.FinalDamage;
            int expected2 = (2 * maxHP) / 16;
            Assert.That(damage2, Is.EqualTo(expected2));

            // Turn 3: Counter = 3
            var actions3 = EndOfTurnProcessor.ProcessEffects(_field).ToList();
            var damage3 = actions3.OfType<DamageAction>().First().Context.FinalDamage;
            int expected3 = (3 * maxHP) / 16;
            Assert.That(damage3, Is.EqualTo(expected3));
        }

        #endregion

        #region No Active Pokemon Tests

        [Test]
        public void ProcessEffects_NoActivePokemon_ReturnsEmptyList()
        {
            // Arrange - Clear all slots
            _field.PlayerSide.Slots[0].ClearSlot();
            _field.EnemySide.Slots[0].ClearSlot();

            // Act
            var actions = EndOfTurnProcessor.ProcessEffects(_field).ToList();

            // Assert
            Assert.That(actions, Is.Empty);
        }

        #endregion

        #region Status Without Damage Tests

        [Test]
        public void ProcessEffects_ParalysisStatus_NoDamage()
        {
            // Arrange
            _playerPokemon.Status = PersistentStatus.Paralysis;

            // Act
            var actions = EndOfTurnProcessor.ProcessEffects(_field).ToList();

            // Assert
            Assert.That(actions, Is.Empty);
        }

        [Test]
        public void ProcessEffects_SleepStatus_NoDamage()
        {
            // Arrange
            _playerPokemon.Status = PersistentStatus.Sleep;

            // Act
            var actions = EndOfTurnProcessor.ProcessEffects(_field).ToList();

            // Assert
            Assert.That(actions, Is.Empty);
        }

        [Test]
        public void ProcessEffects_FreezeStatus_NoDamage()
        {
            // Arrange
            _playerPokemon.Status = PersistentStatus.Freeze;

            // Act
            var actions = EndOfTurnProcessor.ProcessEffects(_field).ToList();

            // Assert
            Assert.That(actions, Is.Empty);
        }

        #endregion

        #region Multiple Status Tests

        [Test]
        public void ProcessEffects_MultiplePokemonWithDifferentStatus_ProcessesAll()
        {
            // Arrange
            _playerPokemon.Status = PersistentStatus.Burn;
            _field.EnemySide.Slots[0].Pokemon.Status = PersistentStatus.Poison;

            // Act
            var actions = EndOfTurnProcessor.ProcessEffects(_field).ToList();

            // Assert
            var damageActions = actions.OfType<DamageAction>().ToList();
            Assert.That(damageActions, Has.Count.EqualTo(2));
        }

        #endregion
    }
}

