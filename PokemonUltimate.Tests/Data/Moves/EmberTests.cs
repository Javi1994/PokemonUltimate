using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Ember move in MoveCatalog.
    /// Verifies correct data, power, accuracy, and burn effect.
    /// </summary>
    [TestFixture]
    public class EmberTests
    {
        [Test]
        public void Ember_Data_IsCorrect()
        {
            var move = MoveCatalog.Ember;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Ember"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Fire));
                Assert.That(move.Power, Is.EqualTo(40));
                Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.DamageEffect>(), Is.True);
                Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.StatusEffect>(), Is.True);
            });
        }

        [Test]
        public void Ember_CanBurn()
        {
            var move = MoveCatalog.Ember;
            var status = move.GetEffect<PokemonUltimate.Core.Effects.StatusEffect>();

            Assert.Multiple(() =>
            {
                Assert.That(status.Status, Is.EqualTo(PersistentStatus.Burn));
                Assert.That(status.ChancePercent, Is.EqualTo(10));
            });
        }
    }
}

