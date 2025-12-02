using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Thunder Wave move in MoveCatalog.
    /// Verifies correct data, status category, and guaranteed paralysis effect.
    /// </summary>
    [TestFixture]
    public class ThunderWaveTests
    {
        [Test]
        public void ThunderWave_IsGuaranteedParalysis()
        {
            var move = MoveCatalog.ThunderWave;

            Assert.Multiple(() =>
            {
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Status));
                Assert.That(move.Power, Is.EqualTo(0));
                Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.DamageEffect>(), Is.False);

                var status = move.GetEffect<PokemonUltimate.Core.Effects.StatusEffect>();
                Assert.That(status.Status, Is.EqualTo(PersistentStatus.Paralysis));
                Assert.That(status.ChancePercent, Is.EqualTo(100));
            });
        }
    }
}

