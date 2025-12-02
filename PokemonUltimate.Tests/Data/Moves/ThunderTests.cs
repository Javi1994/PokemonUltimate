using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Thunder move in MoveCatalog.
    /// Verifies correct data, high power, low accuracy, and paralysis effect.
    /// </summary>
    [TestFixture]
    public class ThunderTests
    {
        [Test]
        public void Thunder_PowerAndAccuracy_IsCorrect()
        {
            var move = MoveCatalog.Thunder;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Thunder"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Electric));
                Assert.That(move.Power, Is.EqualTo(110));
                Assert.That(move.Accuracy, Is.EqualTo(70)); // Lower accuracy for higher power
            });
        }

        [Test]
        public void Thunder_HasHigherParalysisChanceThanThunderbolt()
        {
            var thunder = MoveCatalog.Thunder;
            var thunderbolt = MoveCatalog.Thunderbolt;

            var thunderStatus = thunder.GetEffect<PokemonUltimate.Core.Effects.StatusEffect>();
            var thunderboltStatus = thunderbolt.GetEffect<PokemonUltimate.Core.Effects.StatusEffect>();

            Assert.That(thunderStatus.ChancePercent, Is.GreaterThan(thunderboltStatus.ChancePercent),
                "Thunder should have higher paralysis chance than Thunderbolt");
        }
    }
}

