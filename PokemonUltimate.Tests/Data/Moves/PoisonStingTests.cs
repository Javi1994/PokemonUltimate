using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Poison Sting move in MoveCatalog.
    /// Verifies correct data, power, accuracy, and poison effect.
    /// </summary>
    [TestFixture]
    public class PoisonStingTests
    {
        [Test]
        public void PoisonSting_Data_IsCorrect()
        {
            var move = MoveCatalog.PoisonSting;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Poison Sting"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Poison));
                Assert.That(move.Power, Is.EqualTo(15));
                Assert.That(move.Accuracy, Is.EqualTo(100));
            });
        }

        [Test]
        public void PoisonSting_MayPoison()
        {
            var move = MoveCatalog.PoisonSting;
            var status = move.GetEffect<PokemonUltimate.Core.Effects.StatusEffect>();

            Assert.Multiple(() =>
            {
                Assert.That(status, Is.Not.Null);
                Assert.That(status.Status, Is.EqualTo(PersistentStatus.Poison));
            });
        }
    }
}

