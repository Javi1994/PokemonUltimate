using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Quick Attack move in MoveCatalog.
    /// Verifies correct data, power, accuracy, priority, and category.
    /// </summary>
    [TestFixture]
    public class QuickAttackTests
    {
        [Test]
        public void QuickAttack_Data_IsCorrect()
        {
            var move = MoveCatalog.QuickAttack;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Quick Attack"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Normal));
                Assert.That(move.Power, Is.EqualTo(40));
                Assert.That(move.Accuracy, Is.EqualTo(100));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Physical));
            });
        }

        [Test]
        public void QuickAttack_HasPriority()
        {
            var move = MoveCatalog.QuickAttack;
            Assert.That(move.Priority, Is.EqualTo(1), "Quick Attack should have +1 priority");
        }

        [Test]
        public void QuickAttack_HasDamageEffect()
        {
            var move = MoveCatalog.QuickAttack;
            Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.DamageEffect>(), Is.True);
        }
    }
}

