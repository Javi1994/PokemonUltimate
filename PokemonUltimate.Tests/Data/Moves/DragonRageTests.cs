using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Dragon Rage move in MoveCatalog.
    /// Verifies correct data and fixed damage effect.
    /// </summary>
    [TestFixture]
    public class DragonRageTests
    {
        [Test]
        public void DragonRage_Data_IsCorrect()
        {
            var move = MoveCatalog.DragonRage;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Dragon Rage"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Dragon));
                Assert.That(move.Power, Is.EqualTo(0)); // Fixed damage moves have 0 power
                Assert.That(move.Accuracy, Is.EqualTo(100));
            });
        }

        [Test]
        public void DragonRage_HasFixedDamageEffect()
        {
            var move = MoveCatalog.DragonRage;
            var fixedDamage = move.GetEffect<PokemonUltimate.Core.Effects.FixedDamageEffect>();

            Assert.Multiple(() =>
            {
                Assert.That(fixedDamage, Is.Not.Null);
                Assert.That(fixedDamage.Amount, Is.EqualTo(40)); // Always deals 40 damage
            });
        }
    }
}

