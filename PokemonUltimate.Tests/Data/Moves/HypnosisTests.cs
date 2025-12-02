using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Hypnosis move in MoveCatalog.
    /// Verifies correct data, status category, and sleep effect.
    /// </summary>
    [TestFixture]
    public class HypnosisTests
    {
        [Test]
        public void Hypnosis_Data_IsCorrect()
        {
            var move = MoveCatalog.Hypnosis;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Hypnosis"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Psychic));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Status));
                Assert.That(move.Power, Is.EqualTo(0));
            });
        }

        [Test]
        public void Hypnosis_CausesSleep()
        {
            var move = MoveCatalog.Hypnosis;
            var status = move.GetEffect<PokemonUltimate.Core.Effects.StatusEffect>();

            Assert.Multiple(() =>
            {
                Assert.That(status, Is.Not.Null);
                Assert.That(status.Status, Is.EqualTo(PersistentStatus.Sleep));
            });
        }
    }
}

