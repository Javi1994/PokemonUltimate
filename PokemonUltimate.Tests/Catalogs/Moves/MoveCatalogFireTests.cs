using PokemonUltimate.Core.Catalogs;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Catalogs.Moves
{
    /// <summary>
    /// Tests for Fire-type moves in MoveCatalog.
    /// </summary>
    public class MoveCatalogFireTests
    {
        [Test]
        public void Test_Ember_Data()
        {
            var move = MoveCatalog.Ember;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Ember"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Fire));
                Assert.That(move.Power, Is.EqualTo(40));
                Assert.That(move.HasEffect<DamageEffect>(), Is.True);
                Assert.That(move.HasEffect<StatusEffect>(), Is.True);
            });
        }

        [Test]
        public void Test_Ember_Can_Burn()
        {
            var move = MoveCatalog.Ember;
            var status = move.GetEffect<StatusEffect>();

            Assert.Multiple(() =>
            {
                Assert.That(status.Status, Is.EqualTo(PersistentStatus.Burn));
                Assert.That(status.ChancePercent, Is.EqualTo(10));
            });
        }

        [Test]
        public void Test_Flamethrower_Data()
        {
            var move = MoveCatalog.Flamethrower;

            Assert.Multiple(() =>
            {
                Assert.That(move.Power, Is.EqualTo(90));
                Assert.That(move.Accuracy, Is.EqualTo(100));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Special));
            });
        }

        [Test]
        public void Test_FireBlast_High_Power_Low_Accuracy()
        {
            var move = MoveCatalog.FireBlast;

            Assert.Multiple(() =>
            {
                Assert.That(move.Power, Is.EqualTo(110));
                Assert.That(move.Accuracy, Is.EqualTo(85));
            });
        }

        [Test]
        public void Test_All_Fire_Moves_Can_Burn()
        {
            var fireMoves = new[] { MoveCatalog.Ember, MoveCatalog.Flamethrower, MoveCatalog.FireBlast };

            foreach (var move in fireMoves)
            {
                Assert.That(move.HasEffect<StatusEffect>(), Is.True,
                    $"{move.Name} should be able to burn");
                
                var effect = move.GetEffect<StatusEffect>();
                Assert.That(effect.Status, Is.EqualTo(PersistentStatus.Burn),
                    $"{move.Name} should cause Burn status");
            }
        }
    }
}

