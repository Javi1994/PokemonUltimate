using System;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Systems.Core.Instances
{
    /// <summary>
    /// Functional tests for PokemonParty - Pokemon team management system.
    /// </summary>
    /// <remarks>
    /// **Feature**: 5: Game Features
    /// **Sub-Feature**: 5.2: Pokemon Management
    /// **Documentation**: See `docs/features/5-game-features/5.2-pokemon-management/architecture.md`
    /// </remarks>
    [TestFixture]
    public class PokemonPartyTests
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

        #region Constructor Tests

        [Test]
        public void Constructor_Empty_CreatesEmptyParty()
        {
            var party = new PokemonParty();

            Assert.That(party.Count, Is.EqualTo(0));
            Assert.That(party.IsEmpty, Is.True);
            Assert.That(party.IsFull, Is.False);
        }

        [Test]
        public void Constructor_WithInitialPokemon_CreatesPartyWithPokemon()
        {
            var party = new PokemonParty(new[] { _pikachu, _charmander });

            Assert.That(party.Count, Is.EqualTo(2));
            Assert.That(party[0], Is.EqualTo(_pikachu));
            Assert.That(party[1], Is.EqualTo(_charmander));
        }

        [Test]
        public void Constructor_WithInitialPokemon_ExceedsMaxSize_ThrowsArgumentException()
        {
            var tooManyPokemon = Enumerable.Range(0, PokemonParty.MaxPartySize + 1)
                .Select(_ => PokemonFactory.Create(PokemonCatalog.Pikachu, 25))
                .ToArray();

            Assert.Throws<ArgumentException>(() => new PokemonParty(tooManyPokemon));
        }

        [Test]
        public void Constructor_WithNullPokemon_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new PokemonParty(null));
        }

        #endregion

        #region Add Tests

        [Test]
        public void Add_EmptyParty_AddsPokemon()
        {
            var party = new PokemonParty();

            party.Add(_pikachu);

            Assert.That(party.Count, Is.EqualTo(1));
            Assert.That(party[0], Is.EqualTo(_pikachu));
        }

        [Test]
        public void Add_MultiplePokemon_AddsInOrder()
        {
            var party = new PokemonParty();

            party.Add(_pikachu);
            party.Add(_charmander);
            party.Add(_bulbasaur);

            Assert.That(party.Count, Is.EqualTo(3));
            Assert.That(party[0], Is.EqualTo(_pikachu));
            Assert.That(party[1], Is.EqualTo(_charmander));
            Assert.That(party[2], Is.EqualTo(_bulbasaur));
        }

        [Test]
        public void Add_PartyFull_ThrowsInvalidOperationException()
        {
            var party = new PokemonParty();
            for (int i = 0; i < PokemonParty.MaxPartySize; i++)
            {
                party.Add(PokemonFactory.Create(PokemonCatalog.Pikachu, 25));
            }

            Assert.Throws<InvalidOperationException>(() => party.Add(_charmander));
        }

        [Test]
        public void Add_NullPokemon_ThrowsArgumentNullException()
        {
            var party = new PokemonParty();

            Assert.Throws<ArgumentNullException>(() => party.Add(null));
        }

        [Test]
        public void TryAdd_EmptyParty_ReturnsTrue()
        {
            var party = new PokemonParty();

            var result = party.TryAdd(_pikachu);

            Assert.That(result, Is.True);
            Assert.That(party.Count, Is.EqualTo(1));
        }

        [Test]
        public void TryAdd_PartyFull_ReturnsFalse()
        {
            var party = new PokemonParty();
            for (int i = 0; i < PokemonParty.MaxPartySize; i++)
            {
                party.Add(PokemonFactory.Create(PokemonCatalog.Pikachu, 25));
            }

            var result = party.TryAdd(_charmander);

            Assert.That(result, Is.False);
            Assert.That(party.Count, Is.EqualTo(PokemonParty.MaxPartySize));
        }

        [Test]
        public void TryAdd_NullPokemon_ThrowsArgumentNullException()
        {
            var party = new PokemonParty();

            Assert.Throws<ArgumentNullException>(() => party.TryAdd(null));
        }

        #endregion

        #region Remove Tests

        [Test]
        public void RemoveAt_ValidIndex_RemovesPokemon()
        {
            var party = new PokemonParty { _pikachu, _charmander, _bulbasaur };

            party.RemoveAt(1);

            Assert.That(party.Count, Is.EqualTo(2));
            Assert.That(party[0], Is.EqualTo(_pikachu));
            Assert.That(party[1], Is.EqualTo(_bulbasaur));
        }

        [Test]
        public void RemoveAt_InvalidIndex_ThrowsArgumentOutOfRangeException()
        {
            var party = new PokemonParty { _pikachu };

            Assert.Throws<ArgumentOutOfRangeException>(() => party.RemoveAt(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => party.RemoveAt(-1));
        }

        [Test]
        public void TryRemoveAt_ValidIndex_ReturnsTrue()
        {
            var party = new PokemonParty { _pikachu, _charmander };

            var result = party.TryRemoveAt(0);

            Assert.That(result, Is.True);
            Assert.That(party.Count, Is.EqualTo(1));
        }

        [Test]
        public void TryRemoveAt_InvalidIndex_ReturnsFalse()
        {
            var party = new PokemonParty { _pikachu };

            var result = party.TryRemoveAt(1);

            Assert.That(result, Is.False);
            Assert.That(party.Count, Is.EqualTo(1));
        }

        [Test]
        public void Remove_ExistingPokemon_ReturnsTrue()
        {
            var party = new PokemonParty { _pikachu, _charmander };

            var result = party.Remove(_pikachu);

            Assert.That(result, Is.True);
            Assert.That(party.Count, Is.EqualTo(1));
            Assert.That(party.Contains(_pikachu), Is.False);
        }

        [Test]
        public void Remove_NonExistentPokemon_ReturnsFalse()
        {
            var party = new PokemonParty { _pikachu };

            var result = party.Remove(_charmander);

            Assert.That(result, Is.False);
            Assert.That(party.Count, Is.EqualTo(1));
        }

        [Test]
        public void Remove_NullPokemon_ReturnsFalse()
        {
            var party = new PokemonParty { _pikachu };

            var result = party.Remove(null);

            Assert.That(result, Is.False);
            Assert.That(party.Count, Is.EqualTo(1));
        }

        #endregion

        #region Reordering Tests

        [Test]
        public void Swap_ValidIndices_SwapsPokemon()
        {
            var party = new PokemonParty { _pikachu, _charmander, _bulbasaur };

            party.Swap(0, 2);

            Assert.That(party[0], Is.EqualTo(_bulbasaur));
            Assert.That(party[1], Is.EqualTo(_charmander));
            Assert.That(party[2], Is.EqualTo(_pikachu));
        }

        [Test]
        public void Swap_SameIndex_DoesNothing()
        {
            var party = new PokemonParty { _pikachu, _charmander };

            party.Swap(0, 0);

            Assert.That(party[0], Is.EqualTo(_pikachu));
            Assert.That(party[1], Is.EqualTo(_charmander));
        }

        [Test]
        public void Swap_InvalidIndex_ThrowsArgumentOutOfRangeException()
        {
            var party = new PokemonParty { _pikachu };

            Assert.Throws<ArgumentOutOfRangeException>(() => party.Swap(0, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => party.Swap(-1, 0));
        }

        [Test]
        public void Move_ValidIndices_MovesPokemon()
        {
            var party = new PokemonParty { _pikachu, _charmander, _bulbasaur };

            party.Move(0, 2);

            Assert.That(party[0], Is.EqualTo(_charmander));
            Assert.That(party[1], Is.EqualTo(_bulbasaur));
            Assert.That(party[2], Is.EqualTo(_pikachu));
        }

        [Test]
        public void Move_SameIndex_DoesNothing()
        {
            var party = new PokemonParty { _pikachu, _charmander };

            party.Move(0, 0);

            Assert.That(party[0], Is.EqualTo(_pikachu));
            Assert.That(party[1], Is.EqualTo(_charmander));
        }

        [Test]
        public void Move_InvalidIndex_ThrowsArgumentOutOfRangeException()
        {
            var party = new PokemonParty { _pikachu };

            Assert.Throws<ArgumentOutOfRangeException>(() => party.Move(0, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => party.Move(-1, 0));
        }

        #endregion

        #region Query Tests

        [Test]
        public void GetFirstActivePokemon_HasActivePokemon_ReturnsFirstActive()
        {
            var fainted = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
            fainted.TakeDamage(fainted.MaxHP); // Faint it
            var party = new PokemonParty { fainted, _charmander, _bulbasaur };

            var result = party.GetFirstActivePokemon();

            Assert.That(result, Is.EqualTo(_charmander));
        }

        [Test]
        public void GetFirstActivePokemon_AllFainted_ReturnsNull()
        {
            var fainted1 = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
            var fainted2 = PokemonFactory.Create(PokemonCatalog.Charmander, 20);
            fainted1.TakeDamage(fainted1.MaxHP);
            fainted2.TakeDamage(fainted2.MaxHP);
            var party = new PokemonParty { fainted1, fainted2 };

            var result = party.GetFirstActivePokemon();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetActivePokemon_ReturnsOnlyActivePokemon()
        {
            var fainted = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
            fainted.TakeDamage(fainted.MaxHP);
            var party = new PokemonParty { fainted, _charmander, _bulbasaur };

            var active = party.GetActivePokemon();

            Assert.That(active.Count, Is.EqualTo(2));
            Assert.That(active.Contains(_charmander), Is.True);
            Assert.That(active.Contains(_bulbasaur), Is.True);
        }

        [Test]
        public void Contains_ExistingPokemon_ReturnsTrue()
        {
            var party = new PokemonParty { _pikachu, _charmander };

            Assert.That(party.Contains(_pikachu), Is.True);
            Assert.That(party.Contains(_charmander), Is.True);
        }

        [Test]
        public void Contains_NonExistentPokemon_ReturnsFalse()
        {
            var party = new PokemonParty { _pikachu };

            Assert.That(party.Contains(_charmander), Is.False);
            Assert.That(party.Contains(null), Is.False);
        }

        [Test]
        public void IndexOf_ExistingPokemon_ReturnsIndex()
        {
            var party = new PokemonParty { _pikachu, _charmander, _bulbasaur };

            Assert.That(party.IndexOf(_pikachu), Is.EqualTo(0));
            Assert.That(party.IndexOf(_charmander), Is.EqualTo(1));
            Assert.That(party.IndexOf(_bulbasaur), Is.EqualTo(2));
        }

        [Test]
        public void IndexOf_NonExistentPokemon_ReturnsMinusOne()
        {
            var party = new PokemonParty { _pikachu };

            Assert.That(party.IndexOf(_charmander), Is.EqualTo(-1));
            Assert.That(party.IndexOf(null), Is.EqualTo(-1));
        }

        [Test]
        public void TryGetAt_ValidIndex_ReturnsTrue()
        {
            var party = new PokemonParty { _pikachu, _charmander };

            var result = party.TryGetAt(0, out var pokemon);

            Assert.That(result, Is.True);
            Assert.That(pokemon, Is.EqualTo(_pikachu));
        }

        [Test]
        public void TryGetAt_InvalidIndex_ReturnsFalse()
        {
            var party = new PokemonParty { _pikachu };

            var result = party.TryGetAt(1, out var pokemon);

            Assert.That(result, Is.False);
            Assert.That(pokemon, Is.Null);
        }

        #endregion

        #region Validation Tests

        [Test]
        public void IsValidForBattle_HasActivePokemon_ReturnsTrue()
        {
            var party = new PokemonParty { _pikachu, _charmander };

            Assert.That(party.IsValidForBattle(), Is.True);
        }

        [Test]
        public void IsValidForBattle_AllFainted_ReturnsFalse()
        {
            var fainted1 = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
            var fainted2 = PokemonFactory.Create(PokemonCatalog.Charmander, 20);
            fainted1.TakeDamage(fainted1.MaxHP);
            fainted2.TakeDamage(fainted2.MaxHP);
            var party = new PokemonParty { fainted1, fainted2 };

            Assert.That(party.IsValidForBattle(), Is.False);
        }

        [Test]
        public void IsValidForBattle_EmptyParty_ReturnsFalse()
        {
            var party = new PokemonParty();

            Assert.That(party.IsValidForBattle(), Is.False);
        }

        [Test]
        public void IsValidForBattle_WithErrorMessage_ValidParty_ReturnsTrue()
        {
            var party = new PokemonParty { _pikachu };

            var result = party.IsValidForBattle(out var errorMessage);

            Assert.That(result, Is.True);
            Assert.That(errorMessage, Is.Null);
        }

        [Test]
        public void IsValidForBattle_WithErrorMessage_InvalidParty_ReturnsFalse()
        {
            var party = new PokemonParty();

            var result = party.IsValidForBattle(out var errorMessage);

            Assert.That(result, Is.False);
            Assert.That(errorMessage, Is.Not.Null.And.Not.Empty);
        }

        #endregion

        #region IReadOnlyList Tests

        [Test]
        public void GetEnumerator_ReturnsAllPokemon()
        {
            var party = new PokemonParty { _pikachu, _charmander, _bulbasaur };

            var count = 0;
            foreach (var pokemon in party)
            {
                count++;
            }

            Assert.That(count, Is.EqualTo(3));
        }

        [Test]
        public void Indexer_ValidIndex_ReturnsPokemon()
        {
            var party = new PokemonParty { _pikachu, _charmander };

            Assert.That(party[0], Is.EqualTo(_pikachu));
            Assert.That(party[1], Is.EqualTo(_charmander));
        }

        [Test]
        public void Indexer_InvalidIndex_ThrowsArgumentOutOfRangeException()
        {
            var party = new PokemonParty { _pikachu };

            Assert.Throws<ArgumentOutOfRangeException>(() => _ = party[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = party[-1]);
        }

        #endregion

        #region Property Tests

        [Test]
        public void IsFull_MaxSize_ReturnsTrue()
        {
            var party = new PokemonParty();
            for (int i = 0; i < PokemonParty.MaxPartySize; i++)
            {
                party.Add(PokemonFactory.Create(PokemonCatalog.Pikachu, 25));
            }

            Assert.That(party.IsFull, Is.True);
        }

        [Test]
        public void IsFull_BelowMaxSize_ReturnsFalse()
        {
            var party = new PokemonParty { _pikachu };

            Assert.That(party.IsFull, Is.False);
        }

        [Test]
        public void IsEmpty_EmptyParty_ReturnsTrue()
        {
            var party = new PokemonParty();

            Assert.That(party.IsEmpty, Is.True);
        }

        [Test]
        public void IsEmpty_NonEmptyParty_ReturnsFalse()
        {
            var party = new PokemonParty { _pikachu };

            Assert.That(party.IsEmpty, Is.False);
        }

        [Test]
        public void HasActivePokemon_WithActivePokemon_ReturnsTrue()
        {
            var party = new PokemonParty { _pikachu, _charmander };

            Assert.That(party.HasActivePokemon, Is.True);
        }

        [Test]
        public void HasActivePokemon_AllFainted_ReturnsFalse()
        {
            var fainted1 = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
            var fainted2 = PokemonFactory.Create(PokemonCatalog.Charmander, 20);
            fainted1.TakeDamage(fainted1.MaxHP);
            fainted2.TakeDamage(fainted2.MaxHP);
            var party = new PokemonParty { fainted1, fainted2 };

            Assert.That(party.HasActivePokemon, Is.False);
        }

        #endregion
    }
}
