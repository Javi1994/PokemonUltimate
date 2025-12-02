using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Rock Throw move in MoveCatalog.
    /// Verifies correct data, physical category, power, and accuracy.
    /// </summary>
    [TestFixture]
    public class RockThrowTests
    {
        [Test]
        public void RockThrow_IsPhysical()
        {
            var move = MoveCatalog.RockThrow;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Rock Throw"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Rock));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Physical));
                Assert.That(move.Power, Is.EqualTo(50));
                Assert.That(move.Accuracy, Is.EqualTo(90));
            });
        }

        [Test]
        public void RockThrow_HasDamageEffect()
        {
            var move = MoveCatalog.RockThrow;
            Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.DamageEffect>(), Is.True);
        }
    }
}

