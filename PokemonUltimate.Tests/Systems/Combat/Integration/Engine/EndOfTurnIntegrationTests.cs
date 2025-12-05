using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Tests.Systems.Combat.Engine;

namespace PokemonUltimate.Tests.Systems.Combat.Integration.Engine
{
    /// <summary>
    /// Integration tests for End-of-Turn Effects with other combat systems.
    /// </summary>
    [TestFixture]
    public class EndOfTurnIntegrationTests
    {
        private CombatEngine _engine;
        private BattleRules _rules;
        private NullBattleView _view;

        [SetUp]
        public void SetUp()
        {
            _engine = CombatEngineTestHelper.CreateCombatEngine();
            _rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            _view = new NullBattleView();
        }


        #region CombatEngine Integration

        [Test]
        public async Task RunTurn_WithBurnStatus_ProcessesEndOfTurnDamage()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerProvider = new TestActionProvider(new MessageAction("Pass"));
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Apply Burn status
            var playerSlot = _engine.Field.PlayerSide.Slots[0];
            playerSlot.Pokemon.Status = PersistentStatus.Burn;
            int initialHP = playerSlot.Pokemon.CurrentHP;

            // Act
            await _engine.RunTurn();

            // Assert - Burn damage should have been applied
            Assert.That(playerSlot.Pokemon.CurrentHP, Is.LessThan(initialHP));
        }

        [Test]
        public async Task RunTurn_StatusDamageCausesFaint_GeneratesFaintAction()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerProvider = new TestActionProvider(new MessageAction("Pass"));
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Apply Poison and set HP low enough to faint
            var playerSlot = _engine.Field.PlayerSide.Slots[0];
            playerSlot.Pokemon.Status = PersistentStatus.Poison;
            int poisonDamage = playerSlot.Pokemon.MaxHP / 8;
            playerSlot.Pokemon.CurrentHP = poisonDamage; // Will faint from poison

            // Act
            await _engine.RunTurn();

            // Assert - Pokemon should be fainted
            Assert.That(playerSlot.Pokemon.IsFainted, Is.True);
        }

        [Test]
        public async Task RunTurn_MultiplePokemonWithStatus_ProcessesAll()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerProvider = new TestActionProvider(new MessageAction("Pass"));
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Apply different statuses
            _engine.Field.PlayerSide.Slots[0].Pokemon.Status = PersistentStatus.Burn;
            _engine.Field.EnemySide.Slots[0].Pokemon.Status = PersistentStatus.Poison;

            int playerInitialHP = _engine.Field.PlayerSide.Slots[0].Pokemon.CurrentHP;
            int enemyInitialHP = _engine.Field.EnemySide.Slots[0].Pokemon.CurrentHP;

            // Act
            await _engine.RunTurn();

            // Assert - Both should have taken damage
            Assert.That(_engine.Field.PlayerSide.Slots[0].Pokemon.CurrentHP, Is.LessThan(playerInitialHP));
            Assert.That(_engine.Field.EnemySide.Slots[0].Pokemon.CurrentHP, Is.LessThan(enemyInitialHP));
        }

        #endregion

        #region Badly Poisoned Counter Integration

        [Test]
        public async Task RunTurn_BadlyPoisoned_IncrementsCounterEachTurn()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerProvider = new TestActionProvider(new MessageAction("Pass"));
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            var playerSlot = _engine.Field.PlayerSide.Slots[0];
            playerSlot.Pokemon.Status = PersistentStatus.BadlyPoisoned;
            playerSlot.Pokemon.StatusTurnCounter = 1;

            // Act - Turn 1
            await _engine.RunTurn();
            Assert.That(playerSlot.Pokemon.StatusTurnCounter, Is.EqualTo(2));

            // Act - Turn 2
            await _engine.RunTurn();
            Assert.That(playerSlot.Pokemon.StatusTurnCounter, Is.EqualTo(3));

            // Act - Turn 3
            await _engine.RunTurn();
            Assert.That(playerSlot.Pokemon.StatusTurnCounter, Is.EqualTo(4));
        }

        #endregion

        #region BattleQueue Integration

        [Test]
        public void ProcessEffects_GeneratesActions_CanBeEnqueued()
        {
            // Arrange
            var field = new BattleField();
            field.Initialize(_rules,
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            field.PlayerSide.Slots[0].Pokemon.Status = PersistentStatus.Burn;

            // Act
            var processor = new EndOfTurnProcessor(new PokemonUltimate.Combat.Factories.DamageContextFactory());
            var actions = processor.ProcessEffects(field);

            // Assert
            Assert.That(actions, Is.Not.Empty);
            Assert.That(actions.All(a => a != null), Is.True);
            Assert.That(actions.OfType<DamageAction>().Any(), Is.True);
            Assert.That(actions.OfType<MessageAction>().Any(), Is.True);
        }

        #endregion

        #region DamageAction Integration

        [Test]
        public void ProcessEffects_BurnDamage_UsesDamageAction()
        {
            // Arrange
            var field = new BattleField();
            field.Initialize(_rules,
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            var slot = field.PlayerSide.Slots[0];
            slot.Pokemon.Status = PersistentStatus.Burn;
            int initialHP = slot.Pokemon.CurrentHP;

            // Act
            var processor = new EndOfTurnProcessor(new PokemonUltimate.Combat.Factories.DamageContextFactory());
            var actions = processor.ProcessEffects(field);
            var damageAction = actions.OfType<DamageAction>().FirstOrDefault();

            // Assert
            Assert.That(damageAction, Is.Not.Null);
            Assert.That(damageAction.Target, Is.EqualTo(slot));
            Assert.That(damageAction.Context.FinalDamage, Is.GreaterThan(0));

            // Execute the action
            damageAction.ExecuteLogic(field);
            Assert.That(slot.Pokemon.CurrentHP, Is.LessThan(initialHP));
        }

        #endregion
    }
}

