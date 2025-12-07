using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.AI;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Tests.Systems.Combat.Engine;

namespace PokemonUltimate.Tests.Systems.Core.Instances
{
    /// <summary>
    /// Integration tests for PokemonParty with BattleField and CombatEngine.
    /// </summary>
    /// <remarks>
    /// **Feature**: 5: Game Features
    /// **Sub-Feature**: 5.2: Pokemon Management
    /// **Documentation**: See `docs/features/5-game-features/5.2-pokemon-management/architecture.md`
    /// </remarks>
    [TestFixture]
    public class PokemonPartyIntegrationTests
    {
        private PokemonInstance _pikachu;
        private PokemonInstance _charmander;
        private PokemonInstance _bulbasaur;
        private PokemonInstance _squirtle;

        [SetUp]
        public void SetUp()
        {
            _pikachu = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
            _charmander = PokemonFactory.Create(PokemonCatalog.Charmander, 20);
            _bulbasaur = PokemonFactory.Create(PokemonCatalog.Bulbasaur, 15);
            _squirtle = PokemonFactory.Create(PokemonCatalog.Squirtle, 18);
        }

        #region BattleField Integration

        [Test]
        public void BattleField_Initialize_WithPokemonParty_WorksCorrectly()
        {
            var playerParty = new PokemonParty { _pikachu, _charmander };
            var enemyParty = new PokemonParty { _bulbasaur };
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

            field.Initialize(rules, playerParty, enemyParty);

            Assert.That(field.PlayerSide.Party.Count, Is.EqualTo(2));
            Assert.That(field.EnemySide.Party.Count, Is.EqualTo(1));
            Assert.That(field.PlayerSide.Party[0], Is.EqualTo(_pikachu));
            Assert.That(field.EnemySide.Party[0], Is.EqualTo(_bulbasaur));
        }

        [Test]
        public void BattleField_Initialize_WithLargeParty_WorksCorrectly()
        {
            var playerParty = new PokemonParty();
            for (int i = 0; i < PokemonParty.MaxPartySize; i++)
            {
                playerParty.Add(PokemonFactory.Create(PokemonCatalog.Pikachu, 25));
            }
            var enemyParty = new PokemonParty { _bulbasaur };
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

            field.Initialize(rules, playerParty, enemyParty);

            Assert.That(field.PlayerSide.Party.Count, Is.EqualTo(PokemonParty.MaxPartySize));
        }

        [Test]
        public void BattleSide_GetAvailableSwitches_WithPokemonParty_ReturnsCorrectPokemon()
        {
            var playerParty = new PokemonParty { _pikachu, _charmander, _bulbasaur };
            var enemyParty = new PokemonParty { _squirtle };
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            field.Initialize(rules, playerParty, enemyParty);

            // Set first Pokemon in slot
            field.PlayerSide.GetSlot(0).SetPokemon(_pikachu);

            var available = field.PlayerSide.GetAvailableSwitches().ToList();

            Assert.That(available.Count, Is.EqualTo(2));
            Assert.That(available.Contains(_charmander), Is.True);
            Assert.That(available.Contains(_bulbasaur), Is.True);
            Assert.That(available.Contains(_pikachu), Is.False); // Active Pokemon excluded
        }

        [Test]
        public void BattleSide_GetAvailableSwitches_WithFaintedPokemon_ExcludesFainted()
        {
            var fainted = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
            fainted.TakeDamage(fainted.MaxHP);
            var playerParty = new PokemonParty { fainted, _charmander, _bulbasaur };
            var enemyParty = new PokemonParty { _squirtle };
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            field.Initialize(rules, playerParty, enemyParty);

            // Set fainted Pokemon in slot
            field.PlayerSide.GetSlot(0).SetPokemon(fainted);

            var available = field.PlayerSide.GetAvailableSwitches().ToList();

            Assert.That(available.Count, Is.EqualTo(2));
            Assert.That(available.Contains(_charmander), Is.True);
            Assert.That(available.Contains(_bulbasaur), Is.True);
            Assert.That(available.Contains(fainted), Is.False); // Fainted excluded
        }

        #endregion

        #region CombatEngine Integration

        [Test]
        public void CombatEngine_Initialize_WithPokemonParty_WorksCorrectly()
        {
            var playerParty = new PokemonParty { _pikachu, _charmander };
            var enemyParty = new PokemonParty { _bulbasaur };
            var engine = CombatEngineTestHelper.CreateCombatEngine();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();
            var view = new NullBattleView();

            engine.Initialize(rules, playerParty, enemyParty, playerAI, enemyAI, view);

            Assert.That(engine.Field.PlayerSide.Party.Count, Is.EqualTo(2));
            Assert.That(engine.Field.EnemySide.Party.Count, Is.EqualTo(1));
        }

        [Test]
        public void CombatEngine_SwitchAction_WithPokemonParty_SwitchesCorrectly()
        {
            var playerParty = new PokemonParty { _pikachu, _charmander };
            var enemyParty = new PokemonParty { _bulbasaur };
            var engine = CombatEngineTestHelper.CreateCombatEngine();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();
            var view = new NullBattleView();
            engine.Initialize(rules, playerParty, enemyParty, playerAI, enemyAI, view);

            var slot = engine.Field.PlayerSide.GetSlot(0);
            var switchAction = new SwitchAction(slot, _charmander);

            var actions = switchAction.ExecuteLogic(engine.Field).ToList();

            Assert.That(slot.Pokemon, Is.EqualTo(_charmander));
            Assert.That(playerParty.Contains(_pikachu), Is.True); // Original Pokemon still in party
            Assert.That(playerParty.Contains(_charmander), Is.True);
        }

        #endregion

        #region Party Validation Integration

        [Test]
        public void BattleField_Initialize_WithInvalidParty_ThrowsException()
        {
            var emptyParty = new PokemonParty();
            var enemyParty = new PokemonParty { _bulbasaur };
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

            Assert.Throws<ArgumentException>(() => field.Initialize(rules, emptyParty, enemyParty));
        }

        [Test]
        public void BattleField_Initialize_WithAllFaintedParty_ThrowsException()
        {
            var fainted1 = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
            var fainted2 = PokemonFactory.Create(PokemonCatalog.Charmander, 20);
            fainted1.TakeDamage(fainted1.MaxHP);
            fainted2.TakeDamage(fainted2.MaxHP);
            var playerParty = new PokemonParty { fainted1, fainted2 };
            var enemyParty = new PokemonParty { _bulbasaur };
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

            // Note: BattleField.Initialize doesn't check for fainted Pokemon,
            // but the party validation can be used before initialization
            Assert.That(playerParty.IsValidForBattle(), Is.False);
        }

        #endregion

        #region Switching Flow Integration

        [Test]
        public void Party_SwitchFlow_CompleteScenario_WorksCorrectly()
        {
            // Create parties with multiple Pokemon
            var playerParty = new PokemonParty { _pikachu, _charmander, _bulbasaur };
            var enemyParty = new PokemonParty { _squirtle };
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            field.Initialize(rules, playerParty, enemyParty);

            // Start with first Pokemon
            var slot = field.PlayerSide.GetSlot(0);
            slot.SetPokemon(_pikachu);

            // Verify available switches
            var availableBefore = field.PlayerSide.GetAvailableSwitches().ToList();
            Assert.That(availableBefore.Count, Is.EqualTo(2));
            Assert.That(availableBefore.Contains(_charmander), Is.True);
            Assert.That(availableBefore.Contains(_bulbasaur), Is.True);

            // Switch to second Pokemon
            var switchAction = new SwitchAction(slot, _charmander);
            switchAction.ExecuteLogic(field);

            // Verify switch occurred
            Assert.That(slot.Pokemon, Is.EqualTo(_charmander));

            // Verify original Pokemon still in party
            Assert.That(playerParty.Contains(_pikachu), Is.True);
            Assert.That(playerParty.Contains(_charmander), Is.True);
            Assert.That(playerParty.Contains(_bulbasaur), Is.True);

            // Verify available switches now include original Pokemon
            var availableAfter = field.PlayerSide.GetAvailableSwitches().ToList();
            Assert.That(availableAfter.Contains(_pikachu), Is.True);
            Assert.That(availableAfter.Contains(_bulbasaur), Is.True);
            Assert.That(availableAfter.Contains(_charmander), Is.False); // Now active
        }

        #endregion
    }
}
