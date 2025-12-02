using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Surf move in MoveCatalog.
    /// Verifies correct data, AoE target scope, power, and type.
    /// </summary>
    [TestFixture]
    public class SurfTests
    {
        [Test]
        public void Surf_IsAoE()
        {
            var move = MoveCatalog.Surf;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Surf"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Water));
                Assert.That(move.TargetScope, Is.EqualTo(TargetScope.AllEnemies));
                Assert.That(move.Power, Is.EqualTo(90));
            });
        }

        [Test]
        public void Surf_HasDamageEffect()
        {
            var move = MoveCatalog.Surf;
            Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.DamageEffect>(), Is.True);
        }
    }
}

