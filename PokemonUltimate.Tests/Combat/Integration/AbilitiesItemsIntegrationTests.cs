using System.Linq;
using System.Threading.Tasks;
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

namespace PokemonUltimate.Tests.Combat.Integration
{
    /// <summary>
    /// Integration tests for Abilities & Items system - verifies triggers work in full battle flow.
    /// </summary>
    [TestFixture]
    public class AbilitiesItemsIntegrationTests
    {
        private CombatEngine _engine;
        private BattleRules _rules;
        private NullBattleView _view;

        [SetUp]
        public void SetUp()
        {
            _engine = new CombatEngine();
            _rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            _view = new NullBattleView();
        }

        #region OnSwitchIn Integration

        [Test]
        public async Task CombatEngine_SwitchAction_WithIntimidate_TriggersAbility()
        {
            // Arrange
            var intimidateAbility = Ability.Define("Intimidate")
                .Description("Lowers opposing Pokémon's Attack stat.")
                .Gen(3)
                .OnTrigger(AbilityTrigger.OnSwitchIn)
                .LowersOpponentStat(Stat.Attack, -1)
                .Build();

            var playerParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };
            playerParty[1].SetAbility(intimidateAbility);

            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var enemySlot = _engine.Field?.EnemySide?.Slots?[0];

            _engine.Initialize(_rules, playerParty, enemyParty,
                new TestActionProvider(new MessageAction("Pass")),
                new TestActionProvider(new MessageAction("Pass")),
                _view);

            var playerSlot = _engine.Field.PlayerSide.Slots[0];
            var newPokemon = playerParty[1];
            var switchAction = new SwitchAction(playerSlot, newPokemon);
            playerSlot.ActionProvider = new TestActionProvider(switchAction);

            int initialEnemyAttackStage = _engine.Field.EnemySide.Slots[0].GetStatStage(Stat.Attack);

            // Act - Process switch
            await _engine.RunTurn();

            // Assert - Intimidate should have lowered enemy Attack
            int finalEnemyAttackStage = _engine.Field.EnemySide.Slots[0].GetStatStage(Stat.Attack);
            Assert.That(finalEnemyAttackStage, Is.LessThan(initialEnemyAttackStage), "Intimidate should lower enemy Attack");
            Assert.That(finalEnemyAttackStage, Is.EqualTo(-1), "Attack should be lowered by 1 stage");
        }

        #endregion

        #region OnTurnEnd Integration

        [Test]
        public async Task CombatEngine_EndOfTurn_WithLeftovers_HealsPokemon()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            playerParty[0].HeldItem = ItemCatalog.Leftovers;
            int maxHP = playerParty[0].MaxHP;
            playerParty[0].CurrentHP = maxHP - 20; // Damage Pokemon

            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };

            _engine.Initialize(_rules, playerParty, enemyParty,
                new TestActionProvider(new MessageAction("Pass")),
                new TestActionProvider(new MessageAction("Pass")),
                _view);

            int initialHP = _engine.Field.PlayerSide.Slots[0].Pokemon.CurrentHP;

            // Act - Process turn (should trigger end-of-turn effects)
            await _engine.RunTurn();

            // Assert - Leftovers should have healed Pokemon
            int finalHP = _engine.Field.PlayerSide.Slots[0].Pokemon.CurrentHP;
            Assert.That(finalHP, Is.GreaterThan(initialHP), "Leftovers should heal Pokemon");
            int expectedHeal = maxHP / 16; // 1/16 of max HP
            Assert.That(finalHP, Is.EqualTo(initialHP + expectedHeal), $"Should heal {expectedHeal} HP");
        }

        [Test]
        public async Task CombatEngine_EndOfTurn_StatusDamageAndLeftovers_BothProcess()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            playerParty[0].HeldItem = ItemCatalog.Leftovers;
            playerParty[0].Status = PersistentStatus.Burn; // Burn deals damage
            int maxHP = playerParty[0].MaxHP;
            playerParty[0].CurrentHP = maxHP - 10; // Damage Pokemon

            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };

            _engine.Initialize(_rules, playerParty, enemyParty,
                new TestActionProvider(new MessageAction("Pass")),
                new TestActionProvider(new MessageAction("Pass")),
                _view);

            int initialHP = _engine.Field.PlayerSide.Slots[0].Pokemon.CurrentHP;

            // Act - Process turn
            await _engine.RunTurn();

            // Assert - Both status damage and Leftovers should process
            int finalHP = _engine.Field.PlayerSide.Slots[0].Pokemon.CurrentHP;
            int burnDamage = maxHP / 16; // 1/16 damage
            int leftoversHeal = maxHP / 16; // 1/16 heal
            int netChange = leftoversHeal - burnDamage; // Should be 0 or close to it
            
            // HP should change by net effect (heal - damage)
            Assert.That(finalHP, Is.EqualTo(initialHP + netChange), 
                $"Net change should be {netChange} (heal {leftoversHeal} - damage {burnDamage})");
        }

        #endregion

        #region Multiple Triggers Integration

        [Test]
        public async Task CombatEngine_SwitchWithAbility_ThenEndOfTurnWithItem_BothWork()
        {
            // Arrange
            var intimidateAbility = Ability.Define("Intimidate")
                .Description("Lowers opposing Pokémon's Attack stat.")
                .Gen(3)
                .OnTrigger(AbilityTrigger.OnSwitchIn)
                .LowersOpponentStat(Stat.Attack, -1)
                .Build();

            var playerParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };
            playerParty[1].SetAbility(intimidateAbility);
            playerParty[1].HeldItem = ItemCatalog.Leftovers;
            int maxHP = playerParty[1].MaxHP;
            playerParty[1].CurrentHP = maxHP - 20;

            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };

            _engine.Initialize(_rules, playerParty, enemyParty,
                new TestActionProvider(new MessageAction("Pass")),
                new TestActionProvider(new MessageAction("Pass")),
                _view);

            var playerSlot = _engine.Field.PlayerSide.Slots[0];
            var newPokemon = playerParty[1];
            var switchAction = new SwitchAction(playerSlot, newPokemon);
            playerSlot.ActionProvider = new TestActionProvider(switchAction);

            int initialEnemyAttackStage = _engine.Field.EnemySide.Slots[0].GetStatStage(Stat.Attack);
            int initialHP = newPokemon.CurrentHP;

            // Act - Process switch (triggers OnSwitchIn)
            await _engine.RunTurn();

            // Assert - Intimidate should have triggered
            int finalEnemyAttackStage = _engine.Field.EnemySide.Slots[0].GetStatStage(Stat.Attack);
            Assert.That(finalEnemyAttackStage, Is.EqualTo(-1), "Intimidate should lower Attack");

            // Assert - Leftovers should have healed (end-of-turn)
            int finalHP = _engine.Field.PlayerSide.Slots[0].Pokemon.CurrentHP;
            Assert.That(finalHP, Is.GreaterThan(initialHP), "Leftovers should heal after switch");
        }

        #endregion
    }
}

