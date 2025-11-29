using PokemonUltimate.Core.Catalogs;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Catalogs.Moves
{
    /// <summary>
    /// Tests for Electric-type moves in MoveCatalog.
    /// </summary>
    public class MoveCatalogElectricTests
    {
        [Test]
        public void Test_Thunderbolt_Data()
        {
            var move = MoveCatalog.Thunderbolt;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Thunderbolt"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Electric));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Special));
                Assert.That(move.Power, Is.EqualTo(90));
                Assert.That(move.Accuracy, Is.EqualTo(100));
            });
        }

        [Test]
        public void Test_Thunderbolt_Can_Paralyze()
        {
            var move = MoveCatalog.Thunderbolt;
            var status = move.GetEffect<StatusEffect>();

            Assert.Multiple(() =>
            {
                Assert.That(status.Status, Is.EqualTo(PersistentStatus.Paralysis));
                Assert.That(status.ChancePercent, Is.EqualTo(10));
            });
        }

        [Test]
        public void Test_Thunder_Has_Higher_Paralysis_Chance()
        {
            var thunder = MoveCatalog.Thunder;
            var thunderbolt = MoveCatalog.Thunderbolt;

            var thunderStatus = thunder.GetEffect<StatusEffect>();
            var thunderboltStatus = thunderbolt.GetEffect<StatusEffect>();

            Assert.That(thunderStatus.ChancePercent, Is.GreaterThan(thunderboltStatus.ChancePercent),
                "Thunder should have higher paralysis chance than Thunderbolt");
        }

        [Test]
        public void Test_ThunderWave_Is_Guaranteed_Paralysis()
        {
            var move = MoveCatalog.ThunderWave;

            Assert.Multiple(() =>
            {
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Status));
                Assert.That(move.Power, Is.EqualTo(0));
                Assert.That(move.HasEffect<DamageEffect>(), Is.False);
                
                var status = move.GetEffect<StatusEffect>();
                Assert.That(status.Status, Is.EqualTo(PersistentStatus.Paralysis));
                Assert.That(status.ChancePercent, Is.EqualTo(100));
            });
        }

        [Test]
        public void Test_Thunder_Power_And_Accuracy()
        {
            var move = MoveCatalog.Thunder;

            Assert.Multiple(() =>
            {
                Assert.That(move.Power, Is.EqualTo(110));
                Assert.That(move.Accuracy, Is.EqualTo(70)); // Lower accuracy for higher power
            });
        }
    }
}

