using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Wing Attack move in MoveCatalog.
    /// Verifies correct data, physical category, power, and type.
    /// </summary>
    [TestFixture]
    public class WingAttackTests
    {
        [Test]
        public void WingAttack_IsPhysical()
        {
            var move = MoveCatalog.WingAttack;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Wing Attack"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Flying));
                Assert.That(move.Power, Is.EqualTo(60));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Physical));
            });
        }

        [Test]
        public void WingAttack_HasDamageEffect()
        {
            var move = MoveCatalog.WingAttack;
            Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.DamageEffect>(), Is.True);
        }
    }
}

