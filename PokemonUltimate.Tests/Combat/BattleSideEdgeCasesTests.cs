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
    /// Edge case tests for BattleSide - large parties, all fainted, edge scenarios.
    /// </summary>
    [TestFixture]
    public class BattleSideEdgeCasesTests
    {
        #region Large Party Tests

        [Test]
        public void SetParty_SixPokemon_Allowed()
        {
            var side = new BattleSide(1, true);
            var party = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 25),
                PokemonFactory.Create(PokemonCatalog.Charmander, 20),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 15),
                PokemonFactory.Create(PokemonCatalog.Squirtle, 18),
                PokemonFactory.Create(PokemonCatalog.Eevee, 22),
                PokemonFactory.Create(PokemonCatalog.Snorlax, 30)
            };

            side.SetParty(party);

            Assert.That(side.Party.Count, Is.EqualTo(6));
        }

        [Test]
        public void SetParty_SinglePokemon_Allowed()
        {
            var side = new BattleSide(1, true);
            var party = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 25)
            };

            side.SetParty(party);

            Assert.That(side.Party.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetAvailableSwitches_LargeParty_ReturnsAllExceptActive()
        {
            var side = new BattleSide(1, true);
            var party = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 25),
                PokemonFactory.Create(PokemonCatalog.Charmander, 20),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 15),
                PokemonFactory.Create(PokemonCatalog.Squirtle, 18),
                PokemonFactory.Create(PokemonCatalog.Eevee, 22),
                PokemonFactory.Create(PokemonCatalog.Snorlax, 30)
            };
            side.SetParty(party);
            side.Slots[0].SetPokemon(party[0]);

            var available = side.GetAvailableSwitches().ToList();

            Assert.That(available.Count, Is.EqualTo(5));
        }

        #endregion

        #region All Fainted Tests

        [Test]
        public void IsDefeated_AllSixFainted_ReturnsTrue()
        {
            var side = new BattleSide(1, true);
            var party = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 25),
                PokemonFactory.Create(PokemonCatalog.Charmander, 20),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 15)
            };
            foreach (var p in party)
            {
                p.TakeDamage(p.MaxHP);
            }
            side.SetParty(party);

            Assert.That(side.IsDefeated(), Is.True);
        }

        [Test]
        public void IsDefeated_LastOneAlive_ReturnsFalse()
        {
            var side = new BattleSide(1, true);
            var party = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 25),
                PokemonFactory.Create(PokemonCatalog.Charmander, 20),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 15)
            };
            party[0].TakeDamage(party[0].MaxHP);
            party[1].TakeDamage(party[1].MaxHP);
            // party[2] is still alive
            side.SetParty(party);

            Assert.That(side.IsDefeated(), Is.False);
        }

        [Test]
        public void GetAvailableSwitches_AllFainted_ReturnsEmpty()
        {
            var side = new BattleSide(1, true);
            var party = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 25),
                PokemonFactory.Create(PokemonCatalog.Charmander, 20)
            };
            foreach (var p in party)
            {
                p.TakeDamage(p.MaxHP);
            }
            side.SetParty(party);

            var available = side.GetAvailableSwitches().ToList();

            Assert.That(available, Is.Empty);
        }

        #endregion

        #region Multiple Slots Tests

        [Test]
        public void GetActiveSlots_TripleBattle_AllFilled()
        {
            var side = new BattleSide(3, true);
            var party = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 25),
                PokemonFactory.Create(PokemonCatalog.Charmander, 20),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 15)
            };
            side.SetParty(party);
            side.Slots[0].SetPokemon(party[0]);
            side.Slots[1].SetPokemon(party[1]);
            side.Slots[2].SetPokemon(party[2]);

            var active = side.GetActiveSlots().ToList();

            Assert.That(active.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetActiveSlots_TripleBattle_MiddleFainted()
        {
            var side = new BattleSide(3, true);
            var party = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 25),
                PokemonFactory.Create(PokemonCatalog.Charmander, 20),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 15)
            };
            party[1].TakeDamage(party[1].MaxHP); // Middle fainted
            side.SetParty(party);
            side.Slots[0].SetPokemon(party[0]);
            side.Slots[1].SetPokemon(party[1]);
            side.Slots[2].SetPokemon(party[2]);

            var active = side.GetActiveSlots().ToList();

            Assert.That(active.Count, Is.EqualTo(2));
            Assert.That(active.Any(s => s.SlotIndex == 1), Is.False);
        }

        [Test]
        public void GetActiveSlots_TripleBattle_OnlyOneRemaining()
        {
            var side = new BattleSide(3, true);
            var party = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 25),
                PokemonFactory.Create(PokemonCatalog.Charmander, 20),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 15)
            };
            party[0].TakeDamage(party[0].MaxHP);
            party[1].TakeDamage(party[1].MaxHP);
            side.SetParty(party);
            side.Slots[0].SetPokemon(party[0]);
            side.Slots[1].SetPokemon(party[1]);
            side.Slots[2].SetPokemon(party[2]);

            var active = side.GetActiveSlots().ToList();

            Assert.That(active.Count, Is.EqualTo(1));
            Assert.That(active[0].SlotIndex, Is.EqualTo(2));
        }

        #endregion

        #region Doubles Battle Specific Tests

        [Test]
        public void GetAvailableSwitches_Doubles_ExcludesBothActive()
        {
            var side = new BattleSide(2, true);
            var party = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 25),
                PokemonFactory.Create(PokemonCatalog.Charmander, 20),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 15),
                PokemonFactory.Create(PokemonCatalog.Squirtle, 18)
            };
            side.SetParty(party);
            side.Slots[0].SetPokemon(party[0]);
            side.Slots[1].SetPokemon(party[1]);

            var available = side.GetAvailableSwitches().ToList();

            Assert.That(available.Count, Is.EqualTo(2));
            Assert.That(available, Does.Not.Contain(party[0]));
            Assert.That(available, Does.Not.Contain(party[1]));
        }

        [Test]
        public void HasActivePokemon_Doubles_OneFaintedOneAlive_ReturnsTrue()
        {
            var side = new BattleSide(2, true);
            var party = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 25),
                PokemonFactory.Create(PokemonCatalog.Charmander, 20)
            };
            party[0].TakeDamage(party[0].MaxHP);
            side.SetParty(party);
            side.Slots[0].SetPokemon(party[0]);
            side.Slots[1].SetPokemon(party[1]);

            Assert.That(side.HasActivePokemon(), Is.True);
        }

        #endregion

        #region Party Not Set Tests

        [Test]
        public void GetAvailableSwitches_PartyNotSet_ReturnsEmpty()
        {
            var side = new BattleSide(1, true);
            side.Slots[0].SetPokemon(PokemonFactory.Create(PokemonCatalog.Pikachu, 25));

            var available = side.GetAvailableSwitches().ToList();

            Assert.That(available, Is.Empty);
        }

        [Test]
        public void IsDefeated_PartyNotSet_ReturnsFalse()
        {
            var side = new BattleSide(1, true);

            Assert.That(side.IsDefeated(), Is.False);
        }

        #endregion

        #region Maximum Slots Tests

        [Test]
        public void Constructor_MaxSlots6_Allowed()
        {
            var side = new BattleSide(6, true);

            Assert.That(side.Slots.Count, Is.EqualTo(6));
        }

        [Test]
        public void Slots_IndicesAreSequential()
        {
            var side = new BattleSide(5, true);

            for (int i = 0; i < 5; i++)
            {
                Assert.That(side.Slots[i].SlotIndex, Is.EqualTo(i));
            }
        }

        #endregion
    }
}

