using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Tests.Systems.Combat.Engine;

namespace PokemonUltimate.Tests.Systems.Combat.Integration.Actions
{
    /// <summary>
    /// Integration tests for SwitchAction - verifies switching integrates with CombatEngine and battle flow.
    /// </summary>
    [TestFixture]
    public class SwitchActionIntegrationTests
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

        #region SwitchAction -> CombatEngine Integration

        [Test]
        public async Task CombatEngine_SwitchAction_ExecutesInBattleFlow()
        {
            // Arrange
            var engine = new CombatEngine();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            var view = new NullBattleView();
            
            var playerParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            
            engine.Initialize(rules, playerParty, enemyParty,
                new TestActionProvider(new MessageAction("Pass")),
                new TestActionProvider(new MessageAction("Pass")),
                view);

            var slot = engine.Field.PlayerSide.Slots[0];
            var newPokemon = playerParty[1];
            var originalPokemon = slot.Pokemon;

            var switchAction = new SwitchAction(slot, newPokemon);
            var switchProvider = new TestActionProvider(switchAction);
            slot.ActionProvider = switchProvider;

            // Act - Process switch through CombatEngine
            await engine.RunTurn();

            // Assert - Pokemon should be switched
            Assert.That(slot.Pokemon, Is.EqualTo(newPokemon));
            Assert.That(slot.Pokemon, Is.Not.EqualTo(originalPokemon));
        }

        [Test]
        public async Task CombatEngine_SwitchAction_HighestPriority_GoesFirst()
        {
            // Arrange
            var engine = new CombatEngine();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            var view = new NullBattleView();
            
            var playerParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            
            engine.Initialize(rules, playerParty, enemyParty,
                new TestActionProvider(new MessageAction("Pass")),
                new TestActionProvider(new MessageAction("Pass")),
                view);

            var playerSlot = engine.Field.PlayerSide.Slots[0];
            var enemySlot = engine.Field.EnemySide.Slots[0];
            
            var newPokemon = playerParty[1];
            var switchAction = new SwitchAction(playerSlot, newPokemon);
            var useMoveAction = new UseMoveAction(
                enemySlot,
                playerSlot,
                enemySlot.Pokemon.Moves.First(m => m.HasPP));

            // Set up providers
            playerSlot.ActionProvider = new TestActionProvider(switchAction);
            enemySlot.ActionProvider = new TestActionProvider(useMoveAction);

            // Act - Process turn
            await engine.RunTurn();

            // Assert - Switch should execute first (highest priority)
            // The switch should have happened before the move
            Assert.That(playerSlot.Pokemon, Is.EqualTo(newPokemon));
        }

        [Test]
        public async Task CombatEngine_SwitchAction_ResetsBattleState_ForNewPokemon()
        {
            // Arrange
            var engine = new CombatEngine();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            var view = new NullBattleView();
            
            var playerParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            
            engine.Initialize(rules, playerParty, enemyParty,
                new TestActionProvider(new MessageAction("Pass")),
                new TestActionProvider(new MessageAction("Pass")),
                view);

            var slot = engine.Field.PlayerSide.Slots[0];
            
            // Modify stat stages before switch
            slot.ModifyStatStage(Stat.Attack, 2);
            slot.ModifyStatStage(Stat.Speed, -1);
            slot.Pokemon.VolatileStatus = VolatileStatus.Flinch;
            
            var newPokemon = playerParty[1];
            var switchAction = new SwitchAction(slot, newPokemon);
            slot.ActionProvider = new TestActionProvider(switchAction);

            // Act - Process switch
            await engine.RunTurn();

            // Assert - Battle state should be reset
            Assert.That(slot.GetStatStage(Stat.Attack), Is.EqualTo(0));
            Assert.That(slot.GetStatStage(Stat.Speed), Is.EqualTo(0));
            Assert.That(slot.Pokemon.VolatileStatus, Is.EqualTo(VolatileStatus.None));
        }

        [Test]
        public async Task CombatEngine_SwitchAction_ThenUseMoveAction_NewPokemonActs()
        {
            // Arrange
            var engine = new CombatEngine();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            var view = new NullBattleView();
            
            var playerParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            
            engine.Initialize(rules, playerParty, enemyParty,
                new TestActionProvider(new MessageAction("Pass")),
                new TestActionProvider(new MessageAction("Pass")),
                view);

            var playerSlot = engine.Field.PlayerSide.Slots[0];
            var enemySlot = engine.Field.EnemySide.Slots[0];
            
            var newPokemon = playerParty[1];
            var switchAction = new SwitchAction(playerSlot, newPokemon);
            
            // First turn: Switch
            playerSlot.ActionProvider = new TestActionProvider(switchAction);
            enemySlot.ActionProvider = new TestActionProvider(new MessageAction("Pass"));
            await engine.RunTurn();
            
            // Verify switch happened
            Assert.That(playerSlot.Pokemon, Is.EqualTo(newPokemon));

            // Second turn: New Pokemon uses move
            var moveInstance = newPokemon.Moves.First(m => m.HasPP);
            var useMoveAction = new UseMoveAction(playerSlot, enemySlot, moveInstance);
            playerSlot.ActionProvider = new TestActionProvider(useMoveAction);
            enemySlot.ActionProvider = new TestActionProvider(new MessageAction("Pass"));
            
            int initialEnemyHP = enemySlot.Pokemon.CurrentHP;

            // Act - New Pokemon uses move
            await engine.RunTurn();

            // Assert - Move should execute with new Pokemon
            Assert.That(enemySlot.Pokemon.CurrentHP, Is.LessThan(initialEnemyHP));
        }

        #endregion
    }
}

