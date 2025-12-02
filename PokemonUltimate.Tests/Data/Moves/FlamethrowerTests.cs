using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Flamethrower move in MoveCatalog.
    /// Verifies correct data, power, accuracy, category, and effects.
    /// </summary>
    [TestFixture]
    public class FlamethrowerTests
    {
        [Test]
        public void Flamethrower_Data_IsCorrect()
        {
            var move = MoveCatalog.Flamethrower;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Flamethrower"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Fire));
                Assert.That(move.Power, Is.EqualTo(90));
                Assert.That(move.Accuracy, Is.EqualTo(100));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Special));
            });
        }

        [Test]
        public void Flamethrower_CanBurn()
        {
            var move = MoveCatalog.Flamethrower;
            var status = move.GetEffect<PokemonUltimate.Core.Effects.StatusEffect>();

            Assert.Multiple(() =>
            {
                Assert.That(status, Is.Not.Null);
                Assert.That(status.Status, Is.EqualTo(PersistentStatus.Burn));
                Assert.That(status.ChancePercent, Is.EqualTo(10));
            });
        }
    }
}

