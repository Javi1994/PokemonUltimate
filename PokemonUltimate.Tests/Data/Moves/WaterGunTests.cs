using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Water Gun move in MoveCatalog.
    /// Verifies correct data, special category, power, and type.
    /// </summary>
    [TestFixture]
    public class WaterGunTests
    {
        [Test]
        public void WaterGun_Data_IsCorrect()
        {
            var move = MoveCatalog.WaterGun;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Water Gun"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Water));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Special));
                Assert.That(move.Power, Is.EqualTo(40));
                Assert.That(move.Accuracy, Is.EqualTo(100));
            });
        }

        [Test]
        public void WaterGun_HasDamageEffect()
        {
            var move = MoveCatalog.WaterGun;
            Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.DamageEffect>(), Is.True);
        }
    }
}

