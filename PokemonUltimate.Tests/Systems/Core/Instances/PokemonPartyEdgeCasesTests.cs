using System;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Systems.Core.Instances
{
    /// <summary>
    /// Edge case tests for PokemonParty - boundary conditions and special scenarios.
    /// </summary>
    /// <remarks>
    /// **Feature**: 5: Game Features
    /// **Sub-Feature**: 5.2: Pokemon Management
    /// **Documentation**: See `docs/features/5-game-features/5.2-pokemon-management/architecture.md`
    /// </remarks>
    [TestFixture]
    public class PokemonPartyEdgeCasesTests
    {
        private PokemonInstance _pikachu;
        private PokemonInstance _charmander;
        private PokemonInstance _bulbasaur;

        [SetUp]
        public void SetUp()
        {
            _pikachu = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
            _charmander = PokemonFactory.Create(PokemonCatalog.Charmander, 20);
            _bulbasaur = PokemonFactory.Create(PokemonCatalog.Bulbasaur, 15);
        }

        #region Maximum Size Edge Cases

        [Test]
        public void Add_AtMaxSize_ThrowsException()
        {
            var party = new PokemonParty();
            for (int i = 0; i < PokemonParty.MaxPartySize; i++)
            {
                party.Add(PokemonFactory.Create(PokemonCatalog.Pikachu, 25));
            }

            Assert.Throws<InvalidOperationException>(() => party.Add(_charmander));
            Assert.That(party.Count, Is.EqualTo(PokemonParty.MaxPartySize));
        }

        [Test]
        public void TryAdd_AtMaxSize_ReturnsFalse()
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
        public void Constructor_AtMaxSize_CreatesParty()
        {
            var pokemon = Enumerable.Range(0, PokemonParty.MaxPartySize)
                .Select(_ => PokemonFactory.Create(PokemonCatalog.Pikachu, 25))
                .ToArray();

            var party = new PokemonParty(pokemon);

            Assert.That(party.Count, Is.EqualTo(PokemonParty.MaxPartySize));
            Assert.That(party.IsFull, Is.True);
        }

        #endregion

        #region Empty Party Edge Cases

        [Test]
        public void RemoveAt_EmptyParty_ThrowsException()
        {
            var party = new PokemonParty();

            Assert.Throws<ArgumentOutOfRangeException>(() => party.RemoveAt(0));
        }

        [Test]
        public void TryRemoveAt_EmptyParty_ReturnsFalse()
        {
            var party = new PokemonParty();

            var result = party.TryRemoveAt(0);

            Assert.That(result, Is.False);
        }

        [Test]
        public void Swap_EmptyParty_ThrowsException()
        {
            var party = new PokemonParty();

            Assert.Throws<ArgumentOutOfRangeException>(() => party.Swap(0, 1));
        }

        [Test]
        public void Move_EmptyParty_ThrowsException()
        {
            var party = new PokemonParty();

            Assert.Throws<ArgumentOutOfRangeException>(() => party.Move(0, 1));
        }

        [Test]
        public void GetFirstActivePokemon_EmptyParty_ReturnsNull()
        {
            var party = new PokemonParty();

            var result = party.GetFirstActivePokemon();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetActivePokemon_EmptyParty_ReturnsEmpty()
        {
            var party = new PokemonParty();

            var active = party.GetActivePokemon();

            Assert.That(active.Count, Is.EqualTo(0));
        }

        [Test]
        public void Indexer_EmptyParty_ThrowsException()
        {
            var party = new PokemonParty();

            Assert.Throws<ArgumentOutOfRangeException>(() => _ = party[0]);
        }

        #endregion

        #region Single Pokemon Edge Cases

        [Test]
        public void Swap_SinglePokemon_SameIndex_DoesNothing()
        {
            var party = new PokemonParty { _pikachu };

            party.Swap(0, 0);

            Assert.That(party.Count, Is.EqualTo(1));
            Assert.That(party[0], Is.EqualTo(_pikachu));
        }

        [Test]
        public void Move_SinglePokemon_SameIndex_DoesNothing()
        {
            var party = new PokemonParty { _pikachu };

            party.Move(0, 0);

            Assert.That(party.Count, Is.EqualTo(1));
            Assert.That(party[0], Is.EqualTo(_pikachu));
        }

        [Test]
        public void RemoveAt_SinglePokemon_LeavesEmpty()
        {
            var party = new PokemonParty { _pikachu };

            party.RemoveAt(0);

            Assert.That(party.Count, Is.EqualTo(0));
            Assert.That(party.IsEmpty, Is.True);
        }

        #endregion

        #region Fainted Pokemon Edge Cases

        [Test]
        public void GetFirstActivePokemon_FirstFainted_ReturnsSecond()
        {
            var fainted = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
            fainted.TakeDamage(fainted.MaxHP);
            var party = new PokemonParty { fainted, _charmander, _bulbasaur };

            var result = party.GetFirstActivePokemon();

            Assert.That(result, Is.EqualTo(_charmander));
        }

        [Test]
        public void GetFirstActivePokemon_MiddleFainted_ReturnsFirst()
        {
            var fainted = PokemonFactory.Create(PokemonCatalog.Charmander, 20);
            fainted.TakeDamage(fainted.MaxHP);
            var party = new PokemonParty { _pikachu, fainted, _bulbasaur };

            var result = party.GetFirstActivePokemon();

            Assert.That(result, Is.EqualTo(_pikachu));
        }

        [Test]
        public void GetActivePokemon_SomeFainted_ReturnsOnlyActive()
        {
            var fainted1 = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
            var fainted2 = PokemonFactory.Create(PokemonCatalog.Charmander, 20);
            fainted1.TakeDamage(fainted1.MaxHP);
            fainted2.TakeDamage(fainted2.MaxHP);
            var party = new PokemonParty { fainted1, _bulbasaur, fainted2 };

            var active = party.GetActivePokemon();

            Assert.That(active.Count, Is.EqualTo(1));
            Assert.That(active[0], Is.EqualTo(_bulbasaur));
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

        #region Negative Index Edge Cases

        [Test]
        public void RemoveAt_NegativeIndex_ThrowsException()
        {
            var party = new PokemonParty { _pikachu };

            Assert.Throws<ArgumentOutOfRangeException>(() => party.RemoveAt(-1));
        }

        [Test]
        public void TryRemoveAt_NegativeIndex_ReturnsFalse()
        {
            var party = new PokemonParty { _pikachu };

            var result = party.TryRemoveAt(-1);

            Assert.That(result, Is.False);
        }

        [Test]
        public void Swap_NegativeIndex_ThrowsException()
        {
            var party = new PokemonParty { _pikachu, _charmander };

            Assert.Throws<ArgumentOutOfRangeException>(() => party.Swap(-1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => party.Swap(0, -1));
        }

        [Test]
        public void Move_NegativeIndex_ThrowsException()
        {
            var party = new PokemonParty { _pikachu, _charmander };

            Assert.Throws<ArgumentOutOfRangeException>(() => party.Move(-1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => party.Move(0, -1));
        }

        [Test]
        public void Indexer_NegativeIndex_ThrowsException()
        {
            var party = new PokemonParty { _pikachu };

            Assert.Throws<ArgumentOutOfRangeException>(() => _ = party[-1]);
        }

        [Test]
        public void TryGetAt_NegativeIndex_ReturnsFalse()
        {
            var party = new PokemonParty { _pikachu };

            var result = party.TryGetAt(-1, out var pokemon);

            Assert.That(result, Is.False);
            Assert.That(pokemon, Is.Null);
        }

        #endregion

        #region Out of Range Index Edge Cases

        [Test]
        public void RemoveAt_IndexEqualsCount_ThrowsException()
        {
            var party = new PokemonParty { _pikachu };

            Assert.Throws<ArgumentOutOfRangeException>(() => party.RemoveAt(1));
        }

        [Test]
        public void Swap_IndexEqualsCount_ThrowsException()
        {
            var party = new PokemonParty { _pikachu, _charmander };

            Assert.Throws<ArgumentOutOfRangeException>(() => party.Swap(0, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => party.Swap(2, 0));
        }

        [Test]
        public void Move_IndexEqualsCount_ThrowsException()
        {
            var party = new PokemonParty { _pikachu, _charmander };

            Assert.Throws<ArgumentOutOfRangeException>(() => party.Move(0, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => party.Move(2, 0));
        }

        [Test]
        public void Indexer_IndexEqualsCount_ThrowsException()
        {
            var party = new PokemonParty { _pikachu };

            Assert.Throws<ArgumentOutOfRangeException>(() => _ = party[1]);
        }

        #endregion

        #region Duplicate Pokemon Edge Cases

        [Test]
        public void Add_DuplicateInstance_AddsBoth()
        {
            var party = new PokemonParty { _pikachu };

            party.Add(_pikachu); // Same instance

            Assert.That(party.Count, Is.EqualTo(2));
            Assert.That(party[0], Is.EqualTo(_pikachu));
            Assert.That(party[1], Is.EqualTo(_pikachu));
        }

        [Test]
        public void Contains_DuplicateInstance_ReturnsTrueForBoth()
        {
            var party = new PokemonParty { _pikachu };
            party.Add(_pikachu);

            Assert.That(party.Contains(_pikachu), Is.True);
        }

        [Test]
        public void IndexOf_DuplicateInstance_ReturnsFirstIndex()
        {
            var party = new PokemonParty { _pikachu };
            party.Add(_pikachu);

            Assert.That(party.IndexOf(_pikachu), Is.EqualTo(0));
        }

        [Test]
        public void Remove_DuplicateInstance_RemovesFirst()
        {
            var party = new PokemonParty { _pikachu };
            party.Add(_pikachu);

            var result = party.Remove(_pikachu);

            Assert.That(result, Is.True);
            Assert.That(party.Count, Is.EqualTo(1));
            Assert.That(party.Contains(_pikachu), Is.True); // Still contains the second one
        }

        #endregion

        #region Validation Edge Cases

        [Test]
        public void IsValidForBattle_EmptyParty_ReturnsFalse()
        {
            var party = new PokemonParty();

            Assert.That(party.IsValidForBattle(), Is.False);
        }

        [Test]
        public void IsValidForBattle_OneActivePokemon_ReturnsTrue()
        {
            var party = new PokemonParty { _pikachu };

            Assert.That(party.IsValidForBattle(), Is.True);
        }

        [Test]
        public void IsValidForBattle_OneFaintedPokemon_ReturnsFalse()
        {
            var fainted = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
            fainted.TakeDamage(fainted.MaxHP);
            var party = new PokemonParty { fainted };

            Assert.That(party.IsValidForBattle(), Is.False);
        }

        [Test]
        public void IsValidForBattle_WithErrorMessage_EmptyParty_ReturnsFalse()
        {
            var party = new PokemonParty();

            var result = party.IsValidForBattle(out var errorMessage);

            Assert.That(result, Is.False);
            Assert.That(errorMessage, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void IsValidForBattle_WithErrorMessage_AllFainted_ReturnsFalse()
        {
            var fainted1 = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
            var fainted2 = PokemonFactory.Create(PokemonCatalog.Charmander, 20);
            fainted1.TakeDamage(fainted1.MaxHP);
            fainted2.TakeDamage(fainted2.MaxHP);
            var party = new PokemonParty { fainted1, fainted2 };

            var result = party.IsValidForBattle(out var errorMessage);

            Assert.That(result, Is.False);
            Assert.That(errorMessage, Is.Not.Null.And.Not.Empty);
        }

        #endregion
    }
}
