using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Scratch move in MoveCatalog.
    /// Verifies correct data, physical category, power, and accuracy.
    /// </summary>
    [TestFixture]
    public class ScratchTests
    {
        [Test]
        public void Scratch_Data_IsCorrect()
        {
            var move = MoveCatalog.Scratch;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Scratch"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Normal));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Physical));
                Assert.That(move.Power, Is.EqualTo(40));
                Assert.That(move.Accuracy, Is.EqualTo(100));
            });
        }

        [Test]
        public void Scratch_HasDamageEffect()
        {
            var move = MoveCatalog.Scratch;
            Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.DamageEffect>(), Is.True);
        }
    }
}

