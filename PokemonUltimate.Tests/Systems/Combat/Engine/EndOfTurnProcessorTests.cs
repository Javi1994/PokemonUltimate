using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Systems.Combat.Engine
{
    /// <summary>
    /// Functional tests for EndOfTurnProcessor - processes end-of-turn effects.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.8: End-of-Turn Effects
    /// **Documentation**: See `docs/features/2-combat-system/2.8-end-of-turn-effects/architecture.md`
    /// </remarks>
    [TestFixture]
    public class EndOfTurnProcessorTests
    {
        private BattleField _field;
        private BattleSlot _playerSlot;
        private BattleSlot _enemySlot;
        private PokemonInstance _playerPokemon;
        private PokemonInstance _enemyPokemon;
        private EndOfTurnProcessor _processor;

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

            // Create processor instance with required dependencies
            var damageContextFactory = new DamageContextFactory();
            _processor = new EndOfTurnProcessor(damageContextFactory);
        }

        #region ProcessEffects Tests

        [Test]
        public void ProcessEffects_BurnStatus_DealsDamage()
        {
            // Arrange
            _playerPokemon.Status = PersistentStatus.Burn;
            int initialHP = _playerPokemon.CurrentHP;
            int maxHP = _playerPokemon.MaxHP;
            int expectedDamage = maxHP / 16; // 1/16 of Max HP

            // Act
            var actions = _processor.ProcessEffects(_field).ToList();

            // Assert
            Assert.That(actions, Has.Count.GreaterThanOrEqualTo(1));
            var damageAction = actions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Not.Null);
            Assert.That(damageAction.Target, Is.EqualTo(_playerSlot));
            Assert.That(damageAction.Context.FinalDamage, Is.EqualTo(expectedDamage));
        }

        [Test]
        public void ProcessEffects_PoisonStatus_DealsDamage()
        {
            // Arrange
            _playerPokemon.Status = PersistentStatus.Poison;
            int maxHP = _playerPokemon.MaxHP;
            int expectedDamage = maxHP / 8; // 1/8 of Max HP

            // Act
            var actions = _processor.ProcessEffects(_field).ToList();

            // Assert
            var damageAction = actions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Not.Null);
            Assert.That(damageAction.Context.FinalDamage, Is.EqualTo(expectedDamage));
        }

        [Test]
        public void ProcessEffects_BadlyPoisonedStatus_DealsEscalatingDamage()
        {
            // Arrange
            _playerPokemon.Status = PersistentStatus.BadlyPoisoned;
            _playerPokemon.StatusTurnCounter = 1; // First turn
            int maxHP = _playerPokemon.MaxHP;
            int expectedDamageTurn1 = (1 * maxHP) / 16; // Counter 1

            // Act - Turn 1
            var actions1 = _processor.ProcessEffects(_field).ToList();
            var damageAction1 = actions1.OfType<DamageAction>().FirstOrDefault();

            // Assert - Turn 1
            Assert.That(damageAction1, Is.Not.Null);
            Assert.That(damageAction1.Context.FinalDamage, Is.EqualTo(expectedDamageTurn1));
            Assert.That(_playerPokemon.StatusTurnCounter, Is.EqualTo(2)); // Counter incremented

            // Act - Turn 2
            int expectedDamageTurn2 = (2 * maxHP) / 16; // Counter 2
            var actions2 = _processor.ProcessEffects(_field).ToList();
            var damageAction2 = actions2.OfType<DamageAction>().FirstOrDefault();

            // Assert - Turn 2
            Assert.That(damageAction2, Is.Not.Null);
            Assert.That(damageAction2.Context.FinalDamage, Is.EqualTo(expectedDamageTurn2));
            Assert.That(_playerPokemon.StatusTurnCounter, Is.EqualTo(3)); // Counter incremented
        }

        [Test]
        public void ProcessEffects_MultiplePokemon_ProcessesAll()
        {
            // Arrange
            _playerPokemon.Status = PersistentStatus.Burn;
            _enemyPokemon.Status = PersistentStatus.Poison;

            // Act
            var actions = _processor.ProcessEffects(_field).ToList();

            // Assert
            var damageActions = actions.OfType<DamageAction>().ToList();
            Assert.That(damageActions, Has.Count.EqualTo(2));
            Assert.That(damageActions.Any(a => a.Target == _playerSlot), Is.True);
            Assert.That(damageActions.Any(a => a.Target == _enemySlot), Is.True);
        }

        [Test]
        public void ProcessEffects_NoStatus_ReturnsEmptyList()
        {
            // Arrange - No status conditions
            _playerPokemon.Status = PersistentStatus.None;
            _enemyPokemon.Status = PersistentStatus.None;

            // Act
            var actions = _processor.ProcessEffects(_field).ToList();

            // Assert
            Assert.That(actions, Is.Empty);
        }

        [Test]
        public void ProcessEffects_BurnDamage_CalculatesCorrectly()
        {
            // Arrange
            _playerPokemon.Status = PersistentStatus.Burn;
            int maxHP = 100;
            _playerPokemon.CurrentHP = maxHP; // Set to max for calculation
            // Note: MaxHP is read-only, so we test with actual MaxHP
            int expectedDamage = _playerPokemon.MaxHP / 16;

            // Act
            var actions = _processor.ProcessEffects(_field).ToList();

            // Assert
            var damageAction = actions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Not.Null);
            Assert.That(damageAction.Context.FinalDamage, Is.EqualTo(expectedDamage));
        }

        [Test]
        public void ProcessEffects_PoisonDamage_CalculatesCorrectly()
        {
            // Arrange
            _playerPokemon.Status = PersistentStatus.Poison;
            int expectedDamage = _playerPokemon.MaxHP / 8;

            // Act
            var actions = _processor.ProcessEffects(_field).ToList();

            // Assert
            var damageAction = actions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Not.Null);
            Assert.That(damageAction.Context.FinalDamage, Is.EqualTo(expectedDamage));
        }

        [Test]
        public void ProcessEffects_BadlyPoisoned_IncrementsCounter()
        {
            // Arrange
            _playerPokemon.Status = PersistentStatus.BadlyPoisoned;
            _playerPokemon.StatusTurnCounter = 1;

            // Act
            _processor.ProcessEffects(_field);

            // Assert
            Assert.That(_playerPokemon.StatusTurnCounter, Is.EqualTo(2));
        }

        [Test]
        public void ProcessEffects_BurnStatus_GeneratesMessage()
        {
            // Arrange
            _playerPokemon.Status = PersistentStatus.Burn;

            // Act
            var actions = _processor.ProcessEffects(_field).ToList();

            // Assert
            var messageAction = actions.OfType<MessageAction>().FirstOrDefault();
            Assert.That(messageAction, Is.Not.Null);
            Assert.That(messageAction.Message, Does.Contain("hurt by its burn"));
        }

        [Test]
        public void ProcessEffects_PoisonStatus_GeneratesMessage()
        {
            // Arrange
            _playerPokemon.Status = PersistentStatus.Poison;

            // Act
            var actions = _processor.ProcessEffects(_field).ToList();

            // Assert
            var messageAction = actions.OfType<MessageAction>().FirstOrDefault();
            Assert.That(messageAction, Is.Not.Null);
            Assert.That(messageAction.Message, Does.Contain("hurt by poison"));
        }

        #endregion
    }
}

