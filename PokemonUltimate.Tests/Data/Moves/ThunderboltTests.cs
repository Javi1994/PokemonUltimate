using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Thunderbolt move in MoveCatalog.
    /// Verifies correct data, power, accuracy, category, and effects.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.2: Move Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.2-move-expansion/architecture.md`
    /// </remarks>
    [TestFixture]
    public class ThunderboltTests
    {
        [Test]
        public void Thunderbolt_Data_IsCorrect()
        {
            var move = MoveCatalog.Thunderbolt;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Thunderbolt"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Electric));
                Assert.That(move.Power, Is.EqualTo(90));
                Assert.That(move.Accuracy, Is.EqualTo(100));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Special));
            });
        }

        [Test]
        public void Thunderbolt_CanParalyze()
        {
            var move = MoveCatalog.Thunderbolt;
            var status = move.GetEffect<PokemonUltimate.Core.Effects.StatusEffect>();

            Assert.Multiple(() =>
            {
                Assert.That(status, Is.Not.Null);
                Assert.That(status.Status, Is.EqualTo(PersistentStatus.Paralysis));
                Assert.That(status.ChancePercent, Is.EqualTo(10));
            });
        }
    }
}

