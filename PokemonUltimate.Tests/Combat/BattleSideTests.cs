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
    /// Tests for BattleSide - represents one side of the battlefield (player or enemy).
    /// </summary>
    [TestFixture]
    public class BattleSideTests
    {
        private List<PokemonInstance> _party;
        private PokemonInstance _pikachu;
        private PokemonInstance _charmander;
        private PokemonInstance _bulbasaur;

        [SetUp]
        public void SetUp()
        {
            _pikachu = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
            _charmander = PokemonFactory.Create(PokemonCatalog.Charmander, 20);
            _bulbasaur = PokemonFactory.Create(PokemonCatalog.Bulbasaur, 15);
            _party = new List<PokemonInstance> { _pikachu, _charmander, _bulbasaur };
        }

        #region Constructor Tests

        [Test]
        public void Constructor_SingleSlot_CreatesOneSlot()
        {
            var side = new BattleSide(slotCount: 1, isPlayer: true);

            Assert.That(side.Slots.Count, Is.EqualTo(1));
            Assert.That(side.IsPlayer, Is.True);
        }

        [Test]
        public void Constructor_DoubleSlots_CreatesTwoSlots()
        {
            var side = new BattleSide(slotCount: 2, isPlayer: false);

            Assert.That(side.Slots.Count, Is.EqualTo(2));
            Assert.That(side.IsPlayer, Is.False);
        }

        [Test]
        public void Constructor_TripleSlots_CreatesThreeSlots()
        {
            var side = new BattleSide(slotCount: 3, isPlayer: true);

            Assert.That(side.Slots.Count, Is.EqualTo(3));
        }

        [Test]
        public void Constructor_SlotsHaveReferenceToSide()
        {
            var side = new BattleSide(slotCount: 2, isPlayer: true);

            Assert.That(side.Slots[0].Side, Is.SameAs(side));
            Assert.That(side.Slots[1].Side, Is.SameAs(side));
        }

        [Test]
        public void Constructor_ZeroSlots_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new BattleSide(slotCount: 0, isPlayer: true));
        }

        [Test]
        public void Constructor_NegativeSlots_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new BattleSide(slotCount: -1, isPlayer: true));
        }

        [Test]
        public void Constructor_SlotsHaveCorrectIndices()
        {
            var side = new BattleSide(slotCount: 3, isPlayer: true);

            Assert.That(side.Slots[0].SlotIndex, Is.EqualTo(0));
            Assert.That(side.Slots[1].SlotIndex, Is.EqualTo(1));
            Assert.That(side.Slots[2].SlotIndex, Is.EqualTo(2));
        }

        #endregion

        #region Party Management Tests

        [Test]
        public void SetParty_StoresPartyReference()
        {
            var side = new BattleSide(slotCount: 1, isPlayer: true);

            side.SetParty(_party);

            Assert.That(side.Party, Is.EqualTo(_party));
        }

        [Test]
        public void SetParty_Null_ThrowsArgumentNullException()
        {
            var side = new BattleSide(slotCount: 1, isPlayer: true);

            Assert.Throws<ArgumentNullException>(() => side.SetParty(null));
        }

        [Test]
        public void SetParty_Empty_ThrowsArgumentException()
        {
            var side = new BattleSide(slotCount: 1, isPlayer: true);

            Assert.Throws<ArgumentException>(() => side.SetParty(new List<PokemonInstance>()));
        }

        #endregion

        #region Active Slots Tests

        [Test]
        public void GetActiveSlots_NoSlotsOccupied_ReturnsEmpty()
        {
            var side = new BattleSide(slotCount: 2, isPlayer: true);

            var active = side.GetActiveSlots().ToList();

            Assert.That(active, Is.Empty);
        }

        [Test]
        public void GetActiveSlots_OneSlotOccupied_ReturnsOne()
        {
            var side = new BattleSide(slotCount: 2, isPlayer: true);
            side.Slots[0].SetPokemon(_pikachu);

            var active = side.GetActiveSlots().ToList();

            Assert.That(active.Count, Is.EqualTo(1));
            Assert.That(active[0].Pokemon, Is.EqualTo(_pikachu));
        }

        [Test]
        public void GetActiveSlots_AllSlotsOccupied_ReturnsAll()
        {
            var side = new BattleSide(slotCount: 2, isPlayer: true);
            side.Slots[0].SetPokemon(_pikachu);
            side.Slots[1].SetPokemon(_charmander);

            var active = side.GetActiveSlots().ToList();

            Assert.That(active.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetActiveSlots_ExcludesFainted()
        {
            var side = new BattleSide(slotCount: 2, isPlayer: true);
            _pikachu.TakeDamage(_pikachu.MaxHP); // Faint
            side.Slots[0].SetPokemon(_pikachu);
            side.Slots[1].SetPokemon(_charmander);

            var active = side.GetActiveSlots().ToList();

            Assert.That(active.Count, Is.EqualTo(1));
            Assert.That(active[0].Pokemon, Is.EqualTo(_charmander));
        }

        #endregion

        #region Available Switches Tests

        [Test]
        public void GetAvailableSwitches_AllInParty_ReturnsExcludingActive()
        {
            var side = new BattleSide(slotCount: 1, isPlayer: true);
            side.SetParty(_party);
            side.Slots[0].SetPokemon(_pikachu);

            var available = side.GetAvailableSwitches().ToList();

            Assert.That(available.Count, Is.EqualTo(2));
            Assert.That(available, Contains.Item(_charmander));
            Assert.That(available, Contains.Item(_bulbasaur));
            Assert.That(available, Does.Not.Contain(_pikachu));
        }

        [Test]
        public void GetAvailableSwitches_ExcludesFainted()
        {
            var side = new BattleSide(slotCount: 1, isPlayer: true);
            _charmander.TakeDamage(_charmander.MaxHP); // Faint
            side.SetParty(_party);
            side.Slots[0].SetPokemon(_pikachu);

            var available = side.GetAvailableSwitches().ToList();

            Assert.That(available.Count, Is.EqualTo(1));
            Assert.That(available, Contains.Item(_bulbasaur));
            Assert.That(available, Does.Not.Contain(_charmander));
        }

        [Test]
        public void GetAvailableSwitches_AllActiveOrFainted_ReturnsEmpty()
        {
            var party = new List<PokemonInstance> { _pikachu };
            var side = new BattleSide(slotCount: 1, isPlayer: true);
            side.SetParty(party);
            side.Slots[0].SetPokemon(_pikachu);

            var available = side.GetAvailableSwitches().ToList();

            Assert.That(available, Is.Empty);
        }

        [Test]
        public void GetAvailableSwitches_DoublesBattle_ExcludesBothActive()
        {
            var side = new BattleSide(slotCount: 2, isPlayer: true);
            side.SetParty(_party);
            side.Slots[0].SetPokemon(_pikachu);
            side.Slots[1].SetPokemon(_charmander);

            var available = side.GetAvailableSwitches().ToList();

            Assert.That(available.Count, Is.EqualTo(1));
            Assert.That(available, Contains.Item(_bulbasaur));
        }

        #endregion

        #region Battle State Tests

        [Test]
        public void HasActivePokemon_NoSlotsFilled_ReturnsFalse()
        {
            var side = new BattleSide(slotCount: 1, isPlayer: true);

            Assert.That(side.HasActivePokemon(), Is.False);
        }

        [Test]
        public void HasActivePokemon_OneAlive_ReturnsTrue()
        {
            var side = new BattleSide(slotCount: 1, isPlayer: true);
            side.Slots[0].SetPokemon(_pikachu);

            Assert.That(side.HasActivePokemon(), Is.True);
        }

        [Test]
        public void HasActivePokemon_AllFainted_ReturnsFalse()
        {
            var side = new BattleSide(slotCount: 1, isPlayer: true);
            _pikachu.TakeDamage(_pikachu.MaxHP);
            side.Slots[0].SetPokemon(_pikachu);

            Assert.That(side.HasActivePokemon(), Is.False);
        }

        [Test]
        public void IsDefeated_NoParty_ReturnsFalse()
        {
            var side = new BattleSide(slotCount: 1, isPlayer: true);

            // No party set, so nothing to be defeated
            Assert.That(side.IsDefeated(), Is.False);
        }

        [Test]
        public void IsDefeated_AllPartyFainted_ReturnsTrue()
        {
            var side = new BattleSide(slotCount: 1, isPlayer: true);
            _pikachu.TakeDamage(_pikachu.MaxHP);
            _charmander.TakeDamage(_charmander.MaxHP);
            _bulbasaur.TakeDamage(_bulbasaur.MaxHP);
            side.SetParty(_party);

            Assert.That(side.IsDefeated(), Is.True);
        }

        [Test]
        public void IsDefeated_OneAliveInParty_ReturnsFalse()
        {
            var side = new BattleSide(slotCount: 1, isPlayer: true);
            _pikachu.TakeDamage(_pikachu.MaxHP);
            _charmander.TakeDamage(_charmander.MaxHP);
            // _bulbasaur is still alive
            side.SetParty(_party);

            Assert.That(side.IsDefeated(), Is.False);
        }

        #endregion

        #region Slot Access Tests

        [Test]
        public void GetSlot_ValidIndex_ReturnsSlot()
        {
            var side = new BattleSide(slotCount: 2, isPlayer: true);

            var slot = side.GetSlot(1);

            Assert.That(slot, Is.Not.Null);
            Assert.That(slot.SlotIndex, Is.EqualTo(1));
        }

        [Test]
        public void GetSlot_InvalidIndex_ThrowsArgumentOutOfRange()
        {
            var side = new BattleSide(slotCount: 1, isPlayer: true);

            Assert.Throws<ArgumentOutOfRangeException>(() => side.GetSlot(1));
        }

        [Test]
        public void GetSlot_NegativeIndex_ThrowsArgumentOutOfRange()
        {
            var side = new BattleSide(slotCount: 1, isPlayer: true);

            Assert.Throws<ArgumentOutOfRangeException>(() => side.GetSlot(-1));
        }

        #endregion
    }
}

