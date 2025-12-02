using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Confusion move in MoveCatalog.
    /// Verifies correct data, special category, power, and type.
    /// </summary>
    [TestFixture]
    public class ConfusionTests
    {
        [Test]
        public void Confusion_Data_IsCorrect()
        {
            var move = MoveCatalog.Confusion;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Confusion"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Psychic));
                Assert.That(move.Power, Is.EqualTo(50));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Special));
            });
        }

        [Test]
        public void Confusion_HasDamageEffect()
        {
            var move = MoveCatalog.Confusion;
            Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.DamageEffect>(), Is.True);
        }
    }
}

