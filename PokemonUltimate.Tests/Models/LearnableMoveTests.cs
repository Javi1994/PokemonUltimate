using NUnit.Framework;
using PokemonUltimate.Core.Catalogs;
using PokemonUltimate.Core.Models;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Models
{
    [TestFixture]
    public class LearnableMoveTests
    {
        [Test]
        public void Constructor_Should_Set_All_Properties()
        {
            var move = MoveCatalog.Tackle;
            
            var learnable = new LearnableMove(move, LearnMethod.LevelUp, 15);

            Assert.Multiple(() =>
            {
                Assert.That(learnable.Move, Is.EqualTo(move));
                Assert.That(learnable.Method, Is.EqualTo(LearnMethod.LevelUp));
                Assert.That(learnable.Level, Is.EqualTo(15));
            });
        }

        [Test]
        public void DefaultConstructor_Should_Initialize()
        {
            var learnable = new LearnableMove();

            Assert.That(learnable.Move, Is.Null);
        }

        [Test]
        public void StartMethod_Should_Have_Level_Zero()
        {
            var learnable = new LearnableMove(MoveCatalog.Growl, LearnMethod.Start, 0);

            Assert.Multiple(() =>
            {
                Assert.That(learnable.Method, Is.EqualTo(LearnMethod.Start));
                Assert.That(learnable.Level, Is.EqualTo(0));
            });
        }

        [Test]
        public void TM_Method_Should_Not_Need_Level()
        {
            var learnable = new LearnableMove(MoveCatalog.Earthquake, LearnMethod.TM);

            Assert.Multiple(() =>
            {
                Assert.That(learnable.Method, Is.EqualTo(LearnMethod.TM));
                Assert.That(learnable.Level, Is.EqualTo(0));
            });
        }
    }
}

