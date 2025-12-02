using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Vine Whip move in MoveCatalog.
    /// Verifies correct data, physical category, power, and type.
    /// </summary>
    [TestFixture]
    public class VineWhipTests
    {
        [Test]
        public void VineWhip_Data_IsCorrect()
        {
            var move = MoveCatalog.VineWhip;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Vine Whip"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Grass));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Physical));
                Assert.That(move.Power, Is.EqualTo(45));
                Assert.That(move.Accuracy, Is.EqualTo(100));
            });
        }

        [Test]
        public void VineWhip_HasDamageEffect()
        {
            var move = MoveCatalog.VineWhip;
            Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.DamageEffect>(), Is.True);
        }
    }
}

