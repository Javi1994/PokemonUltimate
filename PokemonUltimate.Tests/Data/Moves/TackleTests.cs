using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Tackle move in MoveCatalog.
    /// Verifies correct data, power, accuracy, and category.
    /// </summary>
    [TestFixture]
    public class TackleTests
    {
        [Test]
        public void Tackle_Data_IsCorrect()
        {
            var move = MoveCatalog.Tackle;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Tackle"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Normal));
                Assert.That(move.Power, Is.EqualTo(40));
                Assert.That(move.Accuracy, Is.EqualTo(100));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Physical));
            });
        }

        [Test]
        public void Tackle_HasDamageEffect()
        {
            var move = MoveCatalog.Tackle;
            Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.DamageEffect>(), Is.True);
        }
    }
}

