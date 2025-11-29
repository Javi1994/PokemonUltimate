using NUnit.Framework;
using PokemonUltimate.Core.Builders;
using PokemonUltimate.Core.Catalogs;
using PokemonUltimate.Core.Models;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Evolution.Conditions;
using System.Linq;

namespace PokemonUltimate.Tests.Builders
{
    [TestFixture]
    public class PokemonBuilderTests
    {
        [Test]
        public void Define_Should_Set_Name_And_PokedexNumber()
        {
            var pokemon = Pokemon.Define("Pikachu", 25).Build();

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.Name, Is.EqualTo("Pikachu"));
                Assert.That(pokemon.PokedexNumber, Is.EqualTo(25));
            });
        }

        [Test]
        public void Type_Should_Set_Single_Type()
        {
            var pokemon = Pokemon.Define("Pikachu", 25)
                .Type(PokemonType.Electric)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Electric));
                Assert.That(pokemon.SecondaryType, Is.Null);
                Assert.That(pokemon.IsDualType, Is.False);
            });
        }

        [Test]
        public void Types_Should_Set_Dual_Types()
        {
            var pokemon = Pokemon.Define("Charizard", 6)
                .Types(PokemonType.Fire, PokemonType.Flying)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Fire));
                Assert.That(pokemon.SecondaryType, Is.EqualTo(PokemonType.Flying));
                Assert.That(pokemon.IsDualType, Is.True);
            });
        }

        [Test]
        public void Stats_Should_Set_BaseStats()
        {
            var pokemon = Pokemon.Define("Pikachu", 25)
                .Stats(35, 55, 40, 50, 50, 90)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.BaseStats.HP, Is.EqualTo(35));
                Assert.That(pokemon.BaseStats.Attack, Is.EqualTo(55));
                Assert.That(pokemon.BaseStats.Defense, Is.EqualTo(40));
                Assert.That(pokemon.BaseStats.SpAttack, Is.EqualTo(50));
                Assert.That(pokemon.BaseStats.SpDefense, Is.EqualTo(50));
                Assert.That(pokemon.BaseStats.Speed, Is.EqualTo(90));
            });
        }

        [Test]
        public void Moves_Should_Configure_Learnset()
        {
            var pokemon = Pokemon.Define("Pikachu", 25)
                .Moves(m => m
                    .StartsWith(MoveCatalog.ThunderShock, MoveCatalog.Growl)
                    .AtLevel(10, MoveCatalog.QuickAttack)
                    .ByTM(MoveCatalog.Thunderbolt))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.Learnset, Has.Count.EqualTo(4));
                Assert.That(pokemon.GetStartingMoves().Count(), Is.EqualTo(2));
                Assert.That(pokemon.CanLearn(MoveCatalog.ThunderShock), Is.True);
                Assert.That(pokemon.CanLearn(MoveCatalog.Thunderbolt), Is.True);
            });
        }

        [Test]
        public void EvolvesTo_Should_Add_Evolution()
        {
            var raichu = Pokemon.Define("Raichu", 26)
                .Type(PokemonType.Electric)
                .Build();

            var pikachu = Pokemon.Define("Pikachu", 25)
                .Type(PokemonType.Electric)
                .EvolvesTo(raichu, e => e.WithItem("Thunder Stone"))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(pikachu.CanEvolve, Is.True);
                Assert.That(pikachu.Evolutions, Has.Count.EqualTo(1));
                Assert.That(pikachu.Evolutions[0].Target, Is.EqualTo(raichu));
                Assert.That(pikachu.Evolutions[0].HasCondition<ItemCondition>(), Is.True);
            });
        }

        [Test]
        public void EvolvesTo_Should_Support_Multiple_Evolutions()
        {
            // Simulate Eevee with multiple evolution paths
            var vaporeon = new PokemonSpeciesData { Name = "Vaporeon", PokedexNumber = 134 };
            var jolteon = new PokemonSpeciesData { Name = "Jolteon", PokedexNumber = 135 };
            var flareon = new PokemonSpeciesData { Name = "Flareon", PokedexNumber = 136 };

            var eevee = Pokemon.Define("Eevee", 133)
                .Type(PokemonType.Normal)
                .EvolvesTo(vaporeon, e => e.WithItem("Water Stone"))
                .EvolvesTo(jolteon, e => e.WithItem("Thunder Stone"))
                .EvolvesTo(flareon, e => e.WithItem("Fire Stone"))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(eevee.Evolutions, Has.Count.EqualTo(3));
                Assert.That(eevee.Evolutions[0].Target.Name, Is.EqualTo("Vaporeon"));
                Assert.That(eevee.Evolutions[1].Target.Name, Is.EqualTo("Jolteon"));
                Assert.That(eevee.Evolutions[2].Target.Name, Is.EqualTo("Flareon"));
            });
        }

        [Test]
        public void EvolvesTo_Should_Support_Complex_Conditions()
        {
            var espeon = new PokemonSpeciesData { Name = "Espeon", PokedexNumber = 196 };

            var eevee = Pokemon.Define("Eevee", 133)
                .Type(PokemonType.Normal)
                .EvolvesTo(espeon, e => e
                    .WithFriendship()
                    .During(TimeOfDay.Day))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(eevee.Evolutions[0].Conditions, Has.Count.EqualTo(2));
                Assert.That(eevee.Evolutions[0].HasCondition<FriendshipCondition>(), Is.True);
                Assert.That(eevee.Evolutions[0].HasCondition<TimeOfDayCondition>(), Is.True);
            });
        }

        [Test]
        public void Full_Builder_Chain_Should_Work()
        {
            var charizard = Pokemon.Define("Charizard", 6)
                .Types(PokemonType.Fire, PokemonType.Flying)
                .Stats(78, 84, 78, 109, 85, 100)
                .Moves(m => m
                    .StartsWith(MoveCatalog.Scratch, MoveCatalog.Ember)
                    .AtLevel(36, MoveCatalog.Flamethrower)
                    .ByTM(MoveCatalog.Earthquake))
                .Build();

            var charmeleon = Pokemon.Define("Charmeleon", 5)
                .Type(PokemonType.Fire)
                .Stats(58, 64, 58, 80, 65, 80)
                .Moves(m => m
                    .StartsWith(MoveCatalog.Scratch, MoveCatalog.Growl)
                    .AtLevel(17, MoveCatalog.Ember))
                .EvolvesTo(charizard, e => e.AtLevel(36))
                .Build();

            Assert.Multiple(() =>
            {
                // Charizard checks
                Assert.That(charizard.Name, Is.EqualTo("Charizard"));
                Assert.That(charizard.IsDualType, Is.True);
                Assert.That(charizard.BaseStats.Total, Is.EqualTo(534));
                Assert.That(charizard.CanEvolve, Is.False);

                // Charmeleon checks
                Assert.That(charmeleon.Name, Is.EqualTo("Charmeleon"));
                Assert.That(charmeleon.IsDualType, Is.False);
                Assert.That(charmeleon.CanEvolve, Is.True);
                Assert.That(charmeleon.Evolutions[0].Target, Is.EqualTo(charizard));
                Assert.That(charmeleon.Evolutions[0].GetCondition<LevelCondition>().MinLevel, Is.EqualTo(36));
            });
        }
    }
}

