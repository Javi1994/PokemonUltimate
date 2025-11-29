using NUnit.Framework;
using PokemonUltimate.Core.Builders;
using PokemonUltimate.Core.Catalogs;
using PokemonUltimate.Core.Models;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Evolution.Conditions;

namespace PokemonUltimate.Tests.Builders
{
    [TestFixture]
    public class EvolutionBuilderTests
    {
        private PokemonSpeciesData _target;

        [SetUp]
        public void Setup()
        {
            _target = new PokemonSpeciesData { Name = "Charmeleon", PokedexNumber = 5 };
        }

        [Test]
        public void AtLevel_Should_Add_LevelCondition()
        {
            var builder = new EvolutionBuilder(_target);

            builder.AtLevel(16);
            var evolution = builder.Build();

            Assert.Multiple(() =>
            {
                Assert.That(evolution.Target, Is.EqualTo(_target));
                Assert.That(evolution.HasCondition<LevelCondition>(), Is.True);
                Assert.That(evolution.GetCondition<LevelCondition>().MinLevel, Is.EqualTo(16));
            });
        }

        [Test]
        public void WithItem_Should_Add_ItemCondition()
        {
            var builder = new EvolutionBuilder(_target);

            builder.WithItem("Fire Stone");
            var evolution = builder.Build();

            Assert.Multiple(() =>
            {
                Assert.That(evolution.HasCondition<ItemCondition>(), Is.True);
                Assert.That(evolution.GetCondition<ItemCondition>().ItemName, Is.EqualTo("Fire Stone"));
            });
        }

        [Test]
        public void WithFriendship_Should_Add_FriendshipCondition()
        {
            var builder = new EvolutionBuilder(_target);

            builder.WithFriendship();
            var evolution = builder.Build();

            Assert.Multiple(() =>
            {
                Assert.That(evolution.HasCondition<FriendshipCondition>(), Is.True);
                Assert.That(evolution.GetCondition<FriendshipCondition>().MinFriendship, Is.EqualTo(220));
            });
        }

        [Test]
        public void WithFriendship_Should_Accept_Custom_Value()
        {
            var builder = new EvolutionBuilder(_target);

            builder.WithFriendship(160);
            var evolution = builder.Build();

            Assert.That(evolution.GetCondition<FriendshipCondition>().MinFriendship, Is.EqualTo(160));
        }

        [Test]
        public void During_Should_Add_TimeOfDayCondition()
        {
            var builder = new EvolutionBuilder(_target);

            builder.During(TimeOfDay.Night);
            var evolution = builder.Build();

            Assert.Multiple(() =>
            {
                Assert.That(evolution.HasCondition<TimeOfDayCondition>(), Is.True);
                Assert.That(evolution.GetCondition<TimeOfDayCondition>().RequiredTime, Is.EqualTo(TimeOfDay.Night));
            });
        }

        [Test]
        public void ByTrade_Should_Add_TradeCondition()
        {
            var builder = new EvolutionBuilder(_target);

            builder.ByTrade();
            var evolution = builder.Build();

            Assert.That(evolution.HasCondition<TradeCondition>(), Is.True);
        }

        [Test]
        public void KnowsMove_Should_Add_KnowsMoveCondition()
        {
            var builder = new EvolutionBuilder(_target);

            builder.KnowsMove(MoveCatalog.Psychic);
            var evolution = builder.Build();

            Assert.Multiple(() =>
            {
                Assert.That(evolution.HasCondition<KnowsMoveCondition>(), Is.True);
                Assert.That(evolution.GetCondition<KnowsMoveCondition>().RequiredMove, Is.EqualTo(MoveCatalog.Psychic));
            });
        }

        [Test]
        public void Builder_Should_Support_Multiple_Conditions()
        {
            var builder = new EvolutionBuilder(_target);

            builder
                .WithFriendship()
                .During(TimeOfDay.Day);
            var evolution = builder.Build();

            Assert.Multiple(() =>
            {
                Assert.That(evolution.Conditions, Has.Count.EqualTo(2));
                Assert.That(evolution.HasCondition<FriendshipCondition>(), Is.True);
                Assert.That(evolution.HasCondition<TimeOfDayCondition>(), Is.True);
            });
        }
    }
}

