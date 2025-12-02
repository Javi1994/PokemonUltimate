using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Sludge Bomb move in MoveCatalog.
    /// Verifies correct data, special category, power, and poison effect.
    /// </summary>
    [TestFixture]
    public class SludgeBombTests
    {
        [Test]
        public void SludgeBomb_Data_IsCorrect()
        {
            var move = MoveCatalog.SludgeBomb;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Sludge Bomb"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Poison));
                Assert.That(move.Power, Is.EqualTo(90));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Special));
            });
        }

        [Test]
        public void SludgeBomb_MayPoison()
        {
            var move = MoveCatalog.SludgeBomb;
            var status = move.GetEffect<PokemonUltimate.Core.Effects.StatusEffect>();

            Assert.Multiple(() =>
            {
                Assert.That(status, Is.Not.Null);
                Assert.That(status.Status, Is.EqualTo(PersistentStatus.Poison));
            });
        }
    }
}

