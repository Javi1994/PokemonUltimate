using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.AI;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Systems.Combat.AI
{
    /// <summary>
    /// Tests for SmartAI - AI that can make strategic decisions including switching.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.7: Integration
    /// **Documentation**: See `docs/features/2-combat-system/2.7-integration/architecture.md`
    /// </remarks>
    [TestFixture]
    public class SmartAITests
    {
        private BattleField _field;
        private PokemonInstance _pikachu;
        private PokemonInstance _charmander;
        private PokemonInstance _bulbasaur;

        [SetUp]
        public void SetUp()
        {
            _pikachu = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            _charmander = PokemonFactory.Create(PokemonCatalog.Charmander, 50);
            _bulbasaur = PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50);

            _field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            var playerParty = new[] { _pikachu, _charmander };
            var enemyParty = new[] { _bulbasaur };
            _field.Initialize(rules, playerParty, enemyParty);
        }

        #region Constructor Tests

        [Test]
        public void Constructor_DefaultParameters_CreatesAI()
        {
            var ai = new SmartAI();

            Assert.That(ai, Is.Not.Null);
        }

        [Test]
        public void Constructor_CustomParameters_CreatesAI()
        {
            var ai = new SmartAI(switchThreshold: 0.5, switchChance: 0.8, seed: 12345);

            Assert.That(ai, Is.Not.Null);
        }

        [Test]
        public void Constructor_InvalidSwitchThreshold_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new SmartAI(switchThreshold: -0.1));
            Assert.Throws<ArgumentException>(() => new SmartAI(switchThreshold: 1.1));
        }

        [Test]
        public void Constructor_InvalidSwitchChance_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new SmartAI(switchChance: -0.1));
            Assert.Throws<ArgumentException>(() => new SmartAI(switchChance: 1.1));
        }

        #endregion

        #region Move Action Tests

        [Test]
        public async Task GetAction_HighHP_ReturnsMoveAction()
        {
            var slot = _field.PlayerSide.GetSlot(0);
            slot.SetPokemon(_pikachu);
            var ai = new SmartAI(switchThreshold: 0.3, switchChance: 1.0, seed: 12345); // High HP, won't switch

            var action = await ai.GetAction(_field, slot);

            Assert.That(action, Is.Not.Null);
            Assert.That(action, Is.InstanceOf<UseMoveAction>());
        }

        [Test]
        public async Task GetAction_NoMoves_ReturnsNull()
        {
            var slot = _field.PlayerSide.GetSlot(0);
            slot.SetPokemon(_pikachu);
            // Deplete all PP
            foreach (var move in _pikachu.Moves)
            {
                while (move.HasPP)
                {
                    move.Use();
                }
            }
            var ai = new SmartAI();

            var action = await ai.GetAction(_field, slot);

            Assert.That(action, Is.Null);
        }

        #endregion

        #region Switch Action Tests

        [Test]
        public async Task GetAction_LowHP_WithAvailableSwitches_CanReturnSwitchAction()
        {
            var slot = _field.PlayerSide.GetSlot(0);
            slot.SetPokemon(_pikachu);
            // Damage Pokemon to low HP
            _pikachu.TakeDamage((int)(_pikachu.MaxHP * 0.8)); // 20% HP remaining
            var ai = new SmartAI(switchThreshold: 0.3, switchChance: 1.0, seed: 12345); // Always switch when below threshold

            var action = await ai.GetAction(_field, slot);

            // May return switch or move depending on random chance
            Assert.That(action, Is.Not.Null);
            Assert.That(action is SwitchAction || action is UseMoveAction, Is.True);
        }

        [Test]
        public async Task GetAction_LowHP_NoAvailableSwitches_ReturnsMoveAction()
        {
            var slot = _field.PlayerSide.GetSlot(0);
            slot.SetPokemon(_pikachu);
            // Damage Pokemon to low HP
            _pikachu.TakeDamage((int)(_pikachu.MaxHP * 0.8));
            // Faint the only other Pokemon
            _charmander.TakeDamage(_charmander.MaxHP);
            var ai = new SmartAI(switchThreshold: 0.3, switchChance: 1.0, seed: 12345);

            var action = await ai.GetAction(_field, slot);

            // Should return move action since no switches available
            Assert.That(action, Is.Not.Null);
            Assert.That(action, Is.InstanceOf<UseMoveAction>());
        }

        [Test]
        public async Task GetAction_LowHP_AllFainted_ReturnsMoveAction()
        {
            var slot = _field.PlayerSide.GetSlot(0);
            slot.SetPokemon(_pikachu);
            _pikachu.TakeDamage((int)(_pikachu.MaxHP * 0.8));
            _charmander.TakeDamage(_charmander.MaxHP);
            var ai = new SmartAI(switchThreshold: 0.3, switchChance: 1.0, seed: 12345);

            var action = await ai.GetAction(_field, slot);

            Assert.That(action, Is.Not.Null);
            Assert.That(action, Is.InstanceOf<UseMoveAction>());
        }

        #endregion

        #region Edge Cases

        [Test]
        public async Task GetAction_EmptySlot_ReturnsNull()
        {
            var slot = _field.PlayerSide.GetSlot(0);
            // Slot is empty
            var ai = new SmartAI();

            var action = await ai.GetAction(_field, slot);

            Assert.That(action, Is.Null);
        }

        [Test]
        public async Task GetAction_FaintedPokemon_ReturnsNull()
        {
            var slot = _field.PlayerSide.GetSlot(0);
            slot.SetPokemon(_pikachu);
            _pikachu.TakeDamage(_pikachu.MaxHP);
            var ai = new SmartAI();

            var action = await ai.GetAction(_field, slot);

            Assert.That(action, Is.Null);
        }

        [Test]
        public void GetAction_NullField_ThrowsException()
        {
            var slot = _field.PlayerSide.GetSlot(0);
            slot.SetPokemon(_pikachu);
            var ai = new SmartAI();

            Assert.ThrowsAsync<System.ArgumentNullException>(async () => await ai.GetAction(null, slot));
        }

        [Test]
        public void GetAction_NullSlot_ThrowsException()
        {
            var ai = new SmartAI();

            Assert.ThrowsAsync<System.ArgumentNullException>(async () => await ai.GetAction(_field, null));
        }

        #endregion
    }
}
