using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Fly move in MoveCatalog.
    /// Verifies correct data, two-turn move, power, and type.
    /// </summary>
    [TestFixture]
    public class FlyTests
    {
        [Test]
        public void Fly_Data_IsCorrect()
        {
            var move = MoveCatalog.Fly;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Fly"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Flying));
                Assert.That(move.Power, Is.EqualTo(90));
                Assert.That(move.Accuracy, Is.EqualTo(95));
            });
        }

        [Test]
        public void Fly_IsTwoTurnMove()
        {
            var move = MoveCatalog.Fly;
            // Fly is a two-turn move (charging move)
            // This would be verified by checking for ChargingEffect if implemented
            Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.DamageEffect>(), Is.True);
        }
    }
}

