using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Catalogs.Moves
{
    /// <summary>
    /// Tests for Water, Grass, Ground, and Psychic moves in MoveCatalog.
    /// Grouped together since there are fewer moves of each type.
    /// </summary>
    public class MoveCatalogOtherTypesTests
    {
        #region Water Moves

        [Test]
        public void Test_Surf_Is_AoE()
        {
            var move = MoveCatalog.Surf;

            Assert.Multiple(() =>
            {
                Assert.That(move.Type, Is.EqualTo(PokemonType.Water));
                Assert.That(move.TargetScope, Is.EqualTo(TargetScope.AllEnemies));
                Assert.That(move.Power, Is.EqualTo(90));
            });
        }

        [Test]
        public void Test_HydroPump_High_Power_Low_Accuracy()
        {
            var move = MoveCatalog.HydroPump;

            Assert.Multiple(() =>
            {
                Assert.That(move.Power, Is.EqualTo(110));
                Assert.That(move.Accuracy, Is.EqualTo(80));
            });
        }

        #endregion

        #region Grass Moves

        [Test]
        public void Test_RazorLeaf_Has_High_Crit()
        {
            var move = MoveCatalog.RazorLeaf;
            var damage = move.GetEffect<DamageEffect>();

            Assert.Multiple(() =>
            {
                Assert.That(move.Type, Is.EqualTo(PokemonType.Grass));
                Assert.That(damage.CritStages, Is.EqualTo(1));
            });
        }

        [Test]
        public void Test_SolarBeam_Is_Powerful()
        {
            var move = MoveCatalog.SolarBeam;

            Assert.Multiple(() =>
            {
                Assert.That(move.Power, Is.EqualTo(120));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Special));
            });
        }

        #endregion

        #region Ground Moves

        [Test]
        public void Test_Earthquake_Hits_All_Others()
        {
            var move = MoveCatalog.Earthquake;

            Assert.Multiple(() =>
            {
                Assert.That(move.Type, Is.EqualTo(PokemonType.Ground));
                Assert.That(move.TargetScope, Is.EqualTo(TargetScope.AllOthers));
                Assert.That(move.Power, Is.EqualTo(100));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Physical));
            });
        }

        #endregion

        #region Psychic Moves

        [Test]
        public void Test_Psychic_May_Lower_SpDef()
        {
            var move = MoveCatalog.Psychic;

            Assert.Multiple(() =>
            {
                Assert.That(move.Type, Is.EqualTo(PokemonType.Psychic));
                Assert.That(move.Power, Is.EqualTo(90));
                Assert.That(move.HasEffect<StatChangeEffect>(), Is.True);
                
                var effect = move.GetEffect<StatChangeEffect>();
                Assert.That(effect.TargetStat, Is.EqualTo(Stat.SpDefense));
                Assert.That(effect.Stages, Is.EqualTo(-1));
                Assert.That(effect.ChancePercent, Is.EqualTo(10));
            });
        }

        #endregion
    }
}

