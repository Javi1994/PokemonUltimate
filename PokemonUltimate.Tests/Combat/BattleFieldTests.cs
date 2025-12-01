using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Combat
{
    /// <summary>
    /// Tests for BattleField - the complete battlefield with both sides.
    /// </summary>
    [TestFixture]
    public class BattleFieldTests
    {
        private List<PokemonInstance> _playerParty;
        private List<PokemonInstance> _enemyParty;

        [SetUp]
        public void SetUp()
        {
            _playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 25),
                PokemonFactory.Create(PokemonCatalog.Charmander, 20),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 15)
            };

            _enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 25),
                PokemonFactory.Create(PokemonCatalog.Wartortle, 20)
            };
        }

        #region Constructor Tests

        [Test]
        public void Constructor_Default_CreatesBattleField()
        {
            var field = new BattleField();

            Assert.That(field, Is.Not.Null);
        }

        #endregion

        #region Initialize Tests

        [Test]
        public void Initialize_1v1_CreatesSingleSlots()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

            field.Initialize(rules, _playerParty, _enemyParty);

            Assert.That(field.PlayerSide.Slots.Count, Is.EqualTo(1));
            Assert.That(field.EnemySide.Slots.Count, Is.EqualTo(1));
        }

        [Test]
        public void Initialize_2v2_CreatesDoubleSlots()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 2, EnemySlots = 2 };

            field.Initialize(rules, _playerParty, _enemyParty);

            Assert.That(field.PlayerSide.Slots.Count, Is.EqualTo(2));
            Assert.That(field.EnemySide.Slots.Count, Is.EqualTo(2));
        }

        [Test]
        public void Initialize_1v3_CreatesAsymmetricSlots()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 3 };

            field.Initialize(rules, _playerParty, _enemyParty);

            Assert.That(field.PlayerSide.Slots.Count, Is.EqualTo(1));
            Assert.That(field.EnemySide.Slots.Count, Is.EqualTo(3));
        }

        [Test]
        public void Initialize_SetsParties()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

            field.Initialize(rules, _playerParty, _enemyParty);

            Assert.That(field.PlayerSide.Party, Is.EqualTo(_playerParty));
            Assert.That(field.EnemySide.Party, Is.EqualTo(_enemyParty));
        }

        [Test]
        public void Initialize_PlacesFirstPokemonInSlots()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

            field.Initialize(rules, _playerParty, _enemyParty);

            Assert.That(field.PlayerSide.Slots[0].Pokemon, Is.EqualTo(_playerParty[0]));
            Assert.That(field.EnemySide.Slots[0].Pokemon, Is.EqualTo(_enemyParty[0]));
        }

        [Test]
        public void Initialize_Doubles_PlacesFirstTwoPokemon()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 2, EnemySlots = 2 };

            field.Initialize(rules, _playerParty, _enemyParty);

            Assert.That(field.PlayerSide.Slots[0].Pokemon, Is.EqualTo(_playerParty[0]));
            Assert.That(field.PlayerSide.Slots[1].Pokemon, Is.EqualTo(_playerParty[1]));
            Assert.That(field.EnemySide.Slots[0].Pokemon, Is.EqualTo(_enemyParty[0]));
            Assert.That(field.EnemySide.Slots[1].Pokemon, Is.EqualTo(_enemyParty[1]));
        }

        [Test]
        public void Initialize_NotEnoughPokemon_LeavesExtraSlotsEmpty()
        {
            var smallParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 25)
            };
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 2, EnemySlots = 1 };

            field.Initialize(rules, smallParty, _enemyParty);

            Assert.That(field.PlayerSide.Slots[0].Pokemon, Is.Not.Null);
            Assert.That(field.PlayerSide.Slots[1].IsEmpty, Is.True);
        }

        [Test]
        public void Initialize_NullPlayerParty_ThrowsArgumentNullException()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

            Assert.Throws<ArgumentNullException>(() => field.Initialize(rules, null, _enemyParty));
        }

        [Test]
        public void Initialize_NullEnemyParty_ThrowsArgumentNullException()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

            Assert.Throws<ArgumentNullException>(() => field.Initialize(rules, _playerParty, null));
        }

        [Test]
        public void Initialize_NullRules_ThrowsArgumentNullException()
        {
            var field = new BattleField();

            Assert.Throws<ArgumentNullException>(() => field.Initialize(null, _playerParty, _enemyParty));
        }

        [Test]
        public void Initialize_StoresRules()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

            field.Initialize(rules, _playerParty, _enemyParty);

            Assert.That(field.Rules, Is.EqualTo(rules));
        }

        #endregion

        #region GetAllActiveSlots Tests

        [Test]
        public void GetAllActiveSlots_ReturnsBothSides()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            field.Initialize(rules, _playerParty, _enemyParty);

            var active = field.GetAllActiveSlots().ToList();

            Assert.That(active.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetAllActiveSlots_ExcludesFainted()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            field.Initialize(rules, _playerParty, _enemyParty);
            // Faint after initialization
            field.PlayerSide.Slots[0].Pokemon.TakeDamage(field.PlayerSide.Slots[0].Pokemon.MaxHP);

            var active = field.GetAllActiveSlots().ToList();

            Assert.That(active.Count, Is.EqualTo(1));
            Assert.That(active[0].Pokemon, Is.EqualTo(_enemyParty[0]));
        }

        [Test]
        public void GetAllActiveSlots_Doubles_ReturnsFour()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 2, EnemySlots = 2 };
            field.Initialize(rules, _playerParty, _enemyParty);

            var active = field.GetAllActiveSlots().ToList();

            Assert.That(active.Count, Is.EqualTo(4));
        }

        #endregion

        #region GetSlot Tests

        [Test]
        public void GetSlot_Player_ReturnsCorrectSlot()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 2, EnemySlots = 1 };
            field.Initialize(rules, _playerParty, _enemyParty);

            var slot = field.GetSlot(isPlayer: true, index: 1);

            Assert.That(slot.SlotIndex, Is.EqualTo(1));
            Assert.That(slot.Pokemon, Is.EqualTo(_playerParty[1]));
        }

        [Test]
        public void GetSlot_Enemy_ReturnsCorrectSlot()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            field.Initialize(rules, _playerParty, _enemyParty);

            var slot = field.GetSlot(isPlayer: false, index: 0);

            Assert.That(slot.Pokemon, Is.EqualTo(_enemyParty[0]));
        }

        #endregion

        #region GetOppositeSide Tests

        [Test]
        public void GetOppositeSide_FromPlayer_ReturnsEnemy()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            field.Initialize(rules, _playerParty, _enemyParty);

            var opposite = field.GetOppositeSide(field.PlayerSide);

            Assert.That(opposite, Is.EqualTo(field.EnemySide));
        }

        [Test]
        public void GetOppositeSide_FromEnemy_ReturnsPlayer()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            field.Initialize(rules, _playerParty, _enemyParty);

            var opposite = field.GetOppositeSide(field.EnemySide);

            Assert.That(opposite, Is.EqualTo(field.PlayerSide));
        }

        [Test]
        public void GetOppositeSide_UnknownSide_ThrowsArgumentException()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            field.Initialize(rules, _playerParty, _enemyParty);
            var otherSide = new BattleSide(1, true);

            Assert.Throws<ArgumentException>(() => field.GetOppositeSide(otherSide));
        }

        #endregion

        #region IsPlayer/IsEnemy Tests

        [Test]
        public void PlayerSide_IsPlayer_ReturnsTrue()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            field.Initialize(rules, _playerParty, _enemyParty);

            Assert.That(field.PlayerSide.IsPlayer, Is.True);
        }

        [Test]
        public void EnemySide_IsPlayer_ReturnsFalse()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            field.Initialize(rules, _playerParty, _enemyParty);

            Assert.That(field.EnemySide.IsPlayer, Is.False);
        }

        #endregion
    }
}

