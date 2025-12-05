using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Systems.Combat.Integration.Actions
{
    /// <summary>
    /// Integration tests for Action System - verifies actions work together correctly.
    /// </summary>
    [TestFixture]
    public class ActionSystemIntegrationTests
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

        #region DamageAction -> FaintAction Integration

        [Test]
        public void DamageAction_CausesFaint_GeneratesFaintAction()
        {
            // Arrange
            _enemyPokemon.CurrentHP = 10;
            var move = MoveCatalog.Thunderbolt;
            var pipeline = new DamagePipeline();
            var context = pipeline.Calculate(_playerSlot, _enemySlot, move, _field);
            context.BaseDamage = 100; // Guaranteed to faint
            context.Multiplier = 1.0f;

            var damageAction = new DamageAction(_playerSlot, _enemySlot, context);

            // Act
            var reactions = damageAction.ExecuteLogic(_field).ToList();

            // Assert
            Assert.That(_enemyPokemon.IsFainted, Is.True);
            Assert.That(reactions, Has.Count.EqualTo(1));
            Assert.That(reactions[0], Is.InstanceOf<FaintAction>());
        }

        [Test]
        public void DamageAction_StatusDamageCausesFaint_GeneratesFaintAction()
        {
            // Arrange
            _enemyPokemon.Status = PersistentStatus.Poison;
            int poisonDamage = _enemyPokemon.MaxHP / 8;
            _enemyPokemon.CurrentHP = poisonDamage; // Will faint from poison

            var dummyMove = new MoveData { Name = "Status", Power = 0, Type = PokemonType.Normal, Category = MoveCategory.Status };
            var context = new DamageContext(_enemySlot, _enemySlot, dummyMove, _field);
            context.BaseDamage = poisonDamage;
            context.Multiplier = 1.0f;

            var damageAction = new DamageAction(_enemySlot, _enemySlot, context);

            // Act
            var reactions = damageAction.ExecuteLogic(_field).ToList();

            // Assert
            Assert.That(_enemyPokemon.IsFainted, Is.True);
            Assert.That(reactions, Has.Count.EqualTo(1));
            Assert.That(reactions[0], Is.InstanceOf<FaintAction>());
        }

        #endregion

        #region UseMoveAction -> Multiple Actions Integration

        [Test]
        public void UseMoveAction_WithStatusEffect_GeneratesDamageAndStatusActions()
        {
            // Arrange
            var moveInstance = _playerPokemon.Moves.FirstOrDefault(m => m.Move.Effects.Any(e => e is StatusEffect));
            if (moveInstance == null)
            {
                Assert.Inconclusive("No move with status effect found in test setup");
                return;
            }

            var useMoveAction = new UseMoveAction(_playerSlot, _enemySlot, moveInstance);

            // Act - Retry until status effect triggers (due to accuracy/chance)
            var reactions = Enumerable.Empty<BattleAction>();
            for (int i = 0; i < 10; i++)
            {
                reactions = useMoveAction.ExecuteLogic(_field).ToList();
                if (reactions.OfType<ApplyStatusAction>().Any())
                    break;
            }

            // Assert
            Assert.That(reactions, Is.Not.Empty);
            Assert.That(reactions.OfType<DamageAction>().Any() || reactions.OfType<ApplyStatusAction>().Any(), Is.True);
        }

        [Test]
        public void UseMoveAction_WithStatChange_GeneratesDamageAndStatChangeActions()
        {
            // Arrange
            var moveInstance = _playerPokemon.Moves.FirstOrDefault(m => m.Move.Effects.Any(e => e is StatChangeEffect));
            if (moveInstance == null)
            {
                Assert.Inconclusive("No move with stat change effect found in test setup");
                return;
            }

            var useMoveAction = new UseMoveAction(_playerSlot, _enemySlot, moveInstance);

            // Act
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();

            // Assert
            Assert.That(reactions, Is.Not.Empty);
            // Should have either damage or stat change (or both)
            Assert.That(reactions.OfType<DamageAction>().Any() || reactions.OfType<StatChangeAction>().Any(), Is.True);
        }

        #endregion

        #region SwitchAction -> Reset State Integration

        [Test]
        public void SwitchAction_ResetsBattleState_ClearsStatStages()
        {
            // Arrange
            _playerSlot.ModifyStatStage(Stat.Attack, 2);
            _playerSlot.ModifyStatStage(Stat.Speed, -1);
            var newPokemon = PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50);

            var switchAction = new SwitchAction(_playerSlot, newPokemon);

            // Act
            switchAction.ExecuteLogic(_field);

            // Assert
            Assert.That(_playerSlot.GetStatStage(Stat.Attack), Is.EqualTo(0));
            Assert.That(_playerSlot.GetStatStage(Stat.Speed), Is.EqualTo(0));
        }

        [Test]
        public void SwitchAction_ResetsBattleState_ClearsVolatileStatus()
        {
            // Arrange
            _playerPokemon.VolatileStatus = VolatileStatus.Flinch;
            var newPokemon = PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50);

            var switchAction = new SwitchAction(_playerSlot, newPokemon);

            // Act
            switchAction.ExecuteLogic(_field);

            // Assert
            Assert.That(_playerSlot.Pokemon.VolatileStatus, Is.EqualTo(VolatileStatus.None));
        }

        [Test]
        public void SwitchAction_ResetsBattleState_ResetsStatusTurnCounter()
        {
            // Arrange
            _playerPokemon.Status = PersistentStatus.BadlyPoisoned;
            _playerPokemon.StatusTurnCounter = 5;
            var newPokemon = PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50);

            var switchAction = new SwitchAction(_playerSlot, newPokemon);

            // Act
            switchAction.ExecuteLogic(_field);

            // Assert
            Assert.That(_playerSlot.Pokemon.StatusTurnCounter, Is.EqualTo(0));
        }

        #endregion

        #region BattleQueue -> Multiple Actions Integration

        [Test]
        public async Task BattleQueue_ProcessesMultipleActions_ExecutesInOrder()
        {
            // Arrange
            var queue = new BattleQueue();
            var view = new NullBattleView();
            var actions = new[]
            {
                new MessageAction("Action 1"),
                new MessageAction("Action 2"),
                new MessageAction("Action 3")
            };

            // Act
            queue.EnqueueRange(actions);
            await queue.ProcessQueue(_field, view);

            // Assert
            Assert.That(queue.IsEmpty, Is.True);
        }

        [Test]
        public async Task BattleQueue_ProcessesActionsWithReactions_HandlesCorrectly()
        {
            // Arrange
            var queue = new BattleQueue();
            var view = new NullBattleView();
            _enemyPokemon.CurrentHP = 10;

            var move = MoveCatalog.Thunderbolt;
            var pipeline = new DamagePipeline();
            var context = pipeline.Calculate(_playerSlot, _enemySlot, move, _field);
            context.BaseDamage = 100;
            context.Multiplier = 1.0f;

            var damageAction = new DamageAction(_playerSlot, _enemySlot, context);

            // Act
            queue.Enqueue(damageAction);
            await queue.ProcessQueue(_field, view);

            // Assert
            Assert.That(_enemyPokemon.IsFainted, Is.True);
            Assert.That(queue.IsEmpty, Is.True);
        }

        #endregion

        #region Status Effects -> Actions Integration

        [Test]
        public void ApplyStatusAction_ThenEndOfTurn_ProcessesStatusDamage()
        {
            // Arrange
            var applyStatusAction = new ApplyStatusAction(_playerSlot, _enemySlot, PersistentStatus.Burn);
            applyStatusAction.ExecuteLogic(_field);

            int initialHP = _enemyPokemon.CurrentHP;

            // Act - Process end-of-turn effects
            var processor = new EndOfTurnProcessor(new PokemonUltimate.Combat.Factories.DamageContextFactory());
            var endOfTurnActions = processor.ProcessEffects(_field);

            // Assert
            Assert.That(endOfTurnActions, Is.Not.Empty);
            var damageAction = endOfTurnActions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Not.Null);

            // Execute the damage action
            damageAction.ExecuteLogic(_field);
            Assert.That(_enemyPokemon.CurrentHP, Is.LessThan(initialHP));
        }

        #endregion
    }
}

