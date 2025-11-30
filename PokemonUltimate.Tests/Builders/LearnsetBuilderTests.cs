using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using System.Linq;
using PokemonUltimate.Content.Builders;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Builders
{
    [TestFixture]
    public class LearnsetBuilderTests
    {
        [Test]
        public void StartsWith_Should_Add_Moves_With_Start_Method()
        {
            var builder = new LearnsetBuilder();

            builder.StartsWith(MoveCatalog.Tackle, MoveCatalog.Growl);
            var learnset = builder.Build();

            Assert.Multiple(() =>
            {
                Assert.That(learnset, Has.Count.EqualTo(2));
                Assert.That(learnset.All(m => m.Method == LearnMethod.Start), Is.True);
                Assert.That(learnset.All(m => m.Level == 0), Is.True);
            });
        }

        [Test]
        public void AtLevel_Should_Add_Moves_With_LevelUp_Method()
        {
            var builder = new LearnsetBuilder();

            builder.AtLevel(15, MoveCatalog.Ember);
            var learnset = builder.Build();

            Assert.Multiple(() =>
            {
                Assert.That(learnset, Has.Count.EqualTo(1));
                Assert.That(learnset[0].Method, Is.EqualTo(LearnMethod.LevelUp));
                Assert.That(learnset[0].Level, Is.EqualTo(15));
                Assert.That(learnset[0].Move, Is.EqualTo(MoveCatalog.Ember));
            });
        }

        [Test]
        public void AtLevel_Should_Support_Multiple_Moves()
        {
            var builder = new LearnsetBuilder();

            builder.AtLevel(25, MoveCatalog.Flamethrower, MoveCatalog.FireBlast);
            var learnset = builder.Build();

            Assert.Multiple(() =>
            {
                Assert.That(learnset, Has.Count.EqualTo(2));
                Assert.That(learnset.All(m => m.Level == 25), Is.True);
            });
        }

        [Test]
        public void ByTM_Should_Add_Moves_With_TM_Method()
        {
            var builder = new LearnsetBuilder();

            builder.ByTM(MoveCatalog.Earthquake);
            var learnset = builder.Build();

            Assert.Multiple(() =>
            {
                Assert.That(learnset, Has.Count.EqualTo(1));
                Assert.That(learnset[0].Method, Is.EqualTo(LearnMethod.TM));
            });
        }

        [Test]
        public void ByEgg_Should_Add_Moves_With_Egg_Method()
        {
            var builder = new LearnsetBuilder();

            builder.ByEgg(MoveCatalog.Thunder);
            var learnset = builder.Build();

            Assert.That(learnset[0].Method, Is.EqualTo(LearnMethod.Egg));
        }

        [Test]
        public void OnEvolution_Should_Add_Moves_With_Evolution_Method()
        {
            var builder = new LearnsetBuilder();

            builder.OnEvolution(MoveCatalog.HyperBeam);
            var learnset = builder.Build();

            Assert.That(learnset[0].Method, Is.EqualTo(LearnMethod.Evolution));
        }

        [Test]
        public void ByTutor_Should_Add_Moves_With_Tutor_Method()
        {
            var builder = new LearnsetBuilder();

            builder.ByTutor(MoveCatalog.Psychic);
            var learnset = builder.Build();

            Assert.That(learnset[0].Method, Is.EqualTo(LearnMethod.Tutor));
        }

        [Test]
        public void Builder_Should_Support_Method_Chaining()
        {
            var builder = new LearnsetBuilder();

            var learnset = builder
                .StartsWith(MoveCatalog.Tackle)
                .AtLevel(10, MoveCatalog.Ember)
                .AtLevel(20, MoveCatalog.Flamethrower)
                .ByTM(MoveCatalog.FireBlast)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(learnset, Has.Count.EqualTo(4));
                Assert.That(learnset.Count(m => m.Method == LearnMethod.Start), Is.EqualTo(1));
                Assert.That(learnset.Count(m => m.Method == LearnMethod.LevelUp), Is.EqualTo(2));
                Assert.That(learnset.Count(m => m.Method == LearnMethod.TM), Is.EqualTo(1));
            });
        }
    }
}

