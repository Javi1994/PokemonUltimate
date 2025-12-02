using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Field
{
    /// <summary>
    /// Edge case tests for BattleField - asymmetric battles, edge scenarios.
    /// </summary>
    [TestFixture]
    public class BattleFieldEdgeCasesTests
    {
        private List<PokemonInstance> _largeParty;
        private List<PokemonInstance> _smallParty;

        [SetUp]
        public void SetUp()
        {
            _largeParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 25),
                PokemonFactory.Create(PokemonCatalog.Charmander, 20),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 15),
                PokemonFactory.Create(PokemonCatalog.Squirtle, 18),
                PokemonFactory.Create(PokemonCatalog.Eevee, 22),
                PokemonFactory.Create(PokemonCatalog.Snorlax, 30)
            };
            _smallParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Mewtwo, 50)
            };
        }

        #region Asymmetric Battle Tests

        [Test]
        public void Initialize_1v3Horde_CreatesAsymmetricSlots()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 3 };

            field.Initialize(rules, _largeParty, _largeParty);

            Assert.That(field.PlayerSide.Slots.Count, Is.EqualTo(1));
            Assert.That(field.EnemySide.Slots.Count, Is.EqualTo(3));
        }

        [Test]
        public void Initialize_3v1Boss_CreatesAsymmetricSlots()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 3, EnemySlots = 1 };

            field.Initialize(rules, _largeParty, _smallParty);

            Assert.That(field.PlayerSide.Slots.Count, Is.EqualTo(3));
            Assert.That(field.EnemySide.Slots.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetAllActiveSlots_1v3_ReturnsFourSlots()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 3 };
            field.Initialize(rules, _largeParty, _largeParty);

            var active = field.GetAllActiveSlots().ToList();

            Assert.That(active.Count, Is.EqualTo(4)); // 1 player + 3 enemy
        }

        #endregion

        #region Triple Battle Tests

        [Test]
        public void Initialize_TripleBattle_PlacesThreeEach()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 3, EnemySlots = 3 };

            field.Initialize(rules, _largeParty, _largeParty);

            Assert.That(field.PlayerSide.Slots[0].Pokemon, Is.Not.Null);
            Assert.That(field.PlayerSide.Slots[1].Pokemon, Is.Not.Null);
            Assert.That(field.PlayerSide.Slots[2].Pokemon, Is.Not.Null);
            Assert.That(field.EnemySide.Slots[0].Pokemon, Is.Not.Null);
            Assert.That(field.EnemySide.Slots[1].Pokemon, Is.Not.Null);
            Assert.That(field.EnemySide.Slots[2].Pokemon, Is.Not.Null);
        }

        [Test]
        public void GetAllActiveSlots_TripleBattle_ReturnsSix()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 3, EnemySlots = 3 };
            field.Initialize(rules, _largeParty, _largeParty);

            var active = field.GetAllActiveSlots().ToList();

            Assert.That(active.Count, Is.EqualTo(6));
        }

        #endregion

        #region Small Party Tests

        [Test]
        public void Initialize_PlayerHasOnePokemon_DoublesBattle()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 2, EnemySlots = 2 };

            field.Initialize(rules, _smallParty, _largeParty);

            Assert.That(field.PlayerSide.Slots[0].Pokemon, Is.Not.Null);
            Assert.That(field.PlayerSide.Slots[1].IsEmpty, Is.True);
            Assert.That(field.EnemySide.Slots[0].Pokemon, Is.Not.Null);
            Assert.That(field.EnemySide.Slots[1].Pokemon, Is.Not.Null);
        }

        [Test]
        public void Initialize_BothHaveOnePokemon_DoublesBattle()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 2, EnemySlots = 2 };

            field.Initialize(rules, _smallParty, _smallParty);

            Assert.That(field.PlayerSide.Slots[0].Pokemon, Is.Not.Null);
            Assert.That(field.PlayerSide.Slots[1].IsEmpty, Is.True);
            Assert.That(field.EnemySide.Slots[0].Pokemon, Is.Not.Null);
            Assert.That(field.EnemySide.Slots[1].IsEmpty, Is.True);
        }

        #endregion

        #region All Fainted Before Battle Tests

        [Test]
        public void Initialize_AllPlayersFainted_SlotsEmpty()
        {
            var faintedParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 25),
                PokemonFactory.Create(PokemonCatalog.Charmander, 20)
            };
            foreach (var p in faintedParty)
            {
                p.TakeDamage(p.MaxHP);
            }
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

            field.Initialize(rules, faintedParty, _largeParty);

            Assert.That(field.PlayerSide.Slots[0].IsEmpty, Is.True);
            Assert.That(field.EnemySide.Slots[0].Pokemon, Is.Not.Null);
        }

        [Test]
        public void Initialize_SomeFainted_SkipsFainted()
        {
            var mixedParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 25),
                PokemonFactory.Create(PokemonCatalog.Charmander, 20),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 15)
            };
            mixedParty[0].TakeDamage(mixedParty[0].MaxHP); // First fainted
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

            field.Initialize(rules, mixedParty, _largeParty);

            // Should place second Pokemon (first healthy one)
            Assert.That(field.PlayerSide.Slots[0].Pokemon, Is.EqualTo(mixedParty[1]));
        }

        #endregion

        #region GetSlot Tests

        [Test]
        public void GetSlot_PlayerSlot0_ReturnsCorrect()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 2, EnemySlots = 2 };
            field.Initialize(rules, _largeParty, _largeParty);

            var slot = field.GetSlot(isPlayer: true, index: 0);

            Assert.That(slot.Pokemon, Is.EqualTo(_largeParty[0]));
        }

        [Test]
        public void GetSlot_EnemySlot1_ReturnsCorrect()
        {
            var field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 2, EnemySlots = 2 };
            field.Initialize(rules, _largeParty, _largeParty);

            var slot = field.GetSlot(isPlayer: false, index: 1);

            Assert.That(slot.Pokemon, Is.EqualTo(_largeParty[1]));
        }

        #endregion

        #region BattleRules Static Helpers Tests

        [Test]
        public void BattleRules_Singles_Returns1v1()
        {
            var rules = BattleRules.Singles;

            Assert.That(rules.PlayerSlots, Is.EqualTo(1));
            Assert.That(rules.EnemySlots, Is.EqualTo(1));
        }

        [Test]
        public void BattleRules_Doubles_Returns2v2()
        {
            var rules = BattleRules.Doubles;

            Assert.That(rules.PlayerSlots, Is.EqualTo(2));
            Assert.That(rules.EnemySlots, Is.EqualTo(2));
        }

        [Test]
        public void BattleRules_Horde_Returns1v3()
        {
            var rules = BattleRules.Horde;

            Assert.That(rules.PlayerSlots, Is.EqualTo(1));
            Assert.That(rules.EnemySlots, Is.EqualTo(3));
        }

        [Test]
        public void BattleRules_DefaultValues()
        {
            var rules = new BattleRules();

            Assert.That(rules.PlayerSlots, Is.EqualTo(1));
            Assert.That(rules.EnemySlots, Is.EqualTo(1));
            Assert.That(rules.MaxTurns, Is.EqualTo(0));
            Assert.That(rules.AllowItems, Is.True);
            Assert.That(rules.AllowSwitching, Is.True);
        }

        #endregion

        #region GetOppositeSide Edge Cases

        [Test]
        public void GetOppositeSide_CalledMultipleTimes_ReturnsConsistently()
        {
            var field = new BattleField();
            var rules = BattleRules.Singles;
            field.Initialize(rules, _largeParty, _largeParty);

            var opposite1 = field.GetOppositeSide(field.PlayerSide);
            var opposite2 = field.GetOppositeSide(field.PlayerSide);

            Assert.That(opposite1, Is.EqualTo(opposite2));
            Assert.That(opposite1, Is.EqualTo(field.EnemySide));
        }

        [Test]
        public void GetOppositeSide_RoundTrip_ReturnsOriginal()
        {
            var field = new BattleField();
            var rules = BattleRules.Singles;
            field.Initialize(rules, _largeParty, _largeParty);

            var opposite = field.GetOppositeSide(field.PlayerSide);
            var original = field.GetOppositeSide(opposite);

            Assert.That(original, Is.EqualTo(field.PlayerSide));
        }

        #endregion
    }
}

