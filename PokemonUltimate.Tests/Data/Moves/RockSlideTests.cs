using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Rock Slide move in MoveCatalog.
    /// Verifies correct data, AoE target scope, power, and flinch effect.
    /// </summary>
    [TestFixture]
    public class RockSlideTests
    {
        [Test]
        public void RockSlide_IsAoE()
        {
            var move = MoveCatalog.RockSlide;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Rock Slide"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Rock));
                Assert.That(move.TargetScope, Is.EqualTo(TargetScope.AllEnemies));
                Assert.That(move.Power, Is.EqualTo(75));
            });
        }

        [Test]
        public void RockSlide_MayFlinch()
        {
            var move = MoveCatalog.RockSlide;
            var flinch = move.GetEffect<PokemonUltimate.Core.Effects.FlinchEffect>();

            Assert.Multiple(() =>
            {
                Assert.That(flinch, Is.Not.Null);
                Assert.That(flinch.ChancePercent, Is.EqualTo(30));
            });
        }
    }
}

