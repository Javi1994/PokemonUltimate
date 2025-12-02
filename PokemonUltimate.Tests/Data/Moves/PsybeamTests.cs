using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Psybeam move in MoveCatalog.
    /// Verifies correct data, special category, power, and type.
    /// </summary>
    [TestFixture]
    public class PsybeamTests
    {
        [Test]
        public void Psybeam_Data_IsCorrect()
        {
            var move = MoveCatalog.Psybeam;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Psybeam"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Psychic));
                Assert.That(move.Power, Is.EqualTo(65));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Special));
            });
        }

        [Test]
        public void Psybeam_HasDamageEffect()
        {
            var move = MoveCatalog.Psybeam;
            Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.DamageEffect>(), Is.True);
        }
    }
}

