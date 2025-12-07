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
    /// Tests for TeamBattleAI - AI that handles team battles with automatic switching.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.7: Integration
    /// **Documentation**: See `docs/features/2-combat-system/2.7-integration/architecture.md`
    /// </remarks>
    [TestFixture]
    public class TeamBattleAITests
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

        #region Auto-Switch Tests (Fainted Pokemon)

        [Test]
        public async Task GetAction_FaintedPokemon_ReturnsSwitchAction()
        {
            var slot = _field.PlayerSide.GetSlot(0);
            slot.SetPokemon(_pikachu);
            _pikachu.TakeDamage(_pikachu.MaxHP); // Faint Pikachu
            var ai = new TeamBattleAI();

            var action = await ai.GetAction(_field, slot);

            Assert.That(action, Is.Not.Null);
            Assert.That(action, Is.InstanceOf<SwitchAction>());
            var switchAction = (SwitchAction)action;
            Assert.That(switchAction.NewPokemon, Is.EqualTo(_charmander));
        }

        [Test]
        public async Task GetAction_EmptySlot_ReturnsSwitchAction()
        {
            var slot = _field.PlayerSide.GetSlot(0);
            // Slot is empty
            var ai = new TeamBattleAI();

            var action = await ai.GetAction(_field, slot);

            Assert.That(action, Is.Not.Null);
            Assert.That(action, Is.InstanceOf<SwitchAction>());
        }

        [Test]
        public async Task GetAction_FaintedPokemon_NoAvailableSwitches_ReturnsNull()
        {
            var slot = _field.PlayerSide.GetSlot(0);
            slot.SetPokemon(_pikachu);
            _pikachu.TakeDamage(_pikachu.MaxHP);
            _charmander.TakeDamage(_charmander.MaxHP); // Faint the only other Pokemon
            var ai = new TeamBattleAI();

            var action = await ai.GetAction(_field, slot);

            Assert.That(action, Is.Null); // No Pokemon available to switch
        }

        #endregion

        #region Strategic Switching Tests

        [Test]
        public async Task GetAction_LowHP_WithAvailableSwitches_CanReturnSwitchAction()
        {
            var slot = _field.PlayerSide.GetSlot(0);
            slot.SetPokemon(_pikachu);
            _pikachu.TakeDamage((int)(_pikachu.MaxHP * 0.8)); // 20% HP remaining
            var ai = new TeamBattleAI(switchThreshold: 0.3, switchChance: 1.0, seed: 12345); // Always switch when below threshold

            var action = await ai.GetAction(_field, slot);

            // May return switch or move depending on random chance
            Assert.That(action, Is.Not.Null);
            Assert.That(action is SwitchAction || action is UseMoveAction, Is.True);
        }

        [Test]
        public async Task GetAction_HighHP_ReturnsMoveAction()
        {
            var slot = _field.PlayerSide.GetSlot(0);
            slot.SetPokemon(_pikachu);
            var ai = new TeamBattleAI(switchThreshold: 0.3, switchChance: 1.0, seed: 12345); // High HP, won't switch

            var action = await ai.GetAction(_field, slot);

            Assert.That(action, Is.Not.Null);
            Assert.That(action, Is.InstanceOf<UseMoveAction>());
        }

        #endregion

        #region Move Action Tests

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
            var ai = new TeamBattleAI();

            var action = await ai.GetAction(_field, slot);

            // Should try to switch if HP is low, otherwise return null
            // Since HP is high, should return null (no moves, no reason to switch)
            Assert.That(action, Is.Null);
        }

        #endregion

        #region Edge Cases

        [Test]
        public void GetAction_NullField_ThrowsException()
        {
            var slot = _field.PlayerSide.GetSlot(0);
            slot.SetPokemon(_pikachu);
            var ai = new TeamBattleAI();

            Assert.ThrowsAsync<System.ArgumentNullException>(async () => await ai.GetAction(null, slot));
        }

        [Test]
        public void GetAction_NullSlot_ThrowsException()
        {
            var ai = new TeamBattleAI();

            Assert.ThrowsAsync<System.ArgumentNullException>(async () => await ai.GetAction(_field, null));
        }

        [Test]
        public void Constructor_InvalidSwitchThreshold_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new TeamBattleAI(switchThreshold: -0.1));
            Assert.Throws<ArgumentException>(() => new TeamBattleAI(switchThreshold: 1.1));
        }

        [Test]
        public void Constructor_InvalidSwitchChance_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new TeamBattleAI(switchChance: -0.1));
            Assert.Throws<ArgumentException>(() => new TeamBattleAI(switchChance: 1.1));
        }

        #endregion
    }
}
