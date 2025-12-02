using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Catalogs.Moves
{
    /// <summary>
    /// Tests for Water, Grass, Ground, Psychic, Ghost, Rock, Flying, Poison, Dragon moves in MoveCatalog.
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

        #region Ghost Moves

        [Test]
        public void Test_Lick_May_Paralyze()
        {
            var move = MoveCatalog.Lick;

            Assert.Multiple(() =>
            {
                Assert.That(move.Type, Is.EqualTo(PokemonType.Ghost));
                Assert.That(move.Power, Is.EqualTo(30));
                Assert.That(move.HasEffect<StatusEffect>(), Is.True);
                
                var effect = move.GetEffect<StatusEffect>();
                Assert.That(effect.Status, Is.EqualTo(PersistentStatus.Paralysis));
                Assert.That(effect.ChancePercent, Is.EqualTo(30));
            });
        }

        [Test]
        public void Test_ShadowBall_May_Lower_SpDef()
        {
            var move = MoveCatalog.ShadowBall;

            Assert.Multiple(() =>
            {
                Assert.That(move.Type, Is.EqualTo(PokemonType.Ghost));
                Assert.That(move.Power, Is.EqualTo(80));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Special));
                Assert.That(move.HasEffect<StatChangeEffect>(), Is.True);
                
                var effect = move.GetEffect<StatChangeEffect>();
                Assert.That(effect.TargetStat, Is.EqualTo(Stat.SpDefense));
                Assert.That(effect.Stages, Is.EqualTo(-1));
                Assert.That(effect.ChancePercent, Is.EqualTo(20));
            });
        }

        #endregion

        #region Rock Moves

        [Test]
        public void Test_RockThrow_Is_Physical()
        {
            var move = MoveCatalog.RockThrow;

            Assert.Multiple(() =>
            {
                Assert.That(move.Type, Is.EqualTo(PokemonType.Rock));
                Assert.That(move.Power, Is.EqualTo(50));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Physical));
                Assert.That(move.Accuracy, Is.EqualTo(90));
            });
        }

        [Test]
        public void Test_RockSlide_May_Flinch()
        {
            var move = MoveCatalog.RockSlide;

            Assert.Multiple(() =>
            {
                Assert.That(move.Type, Is.EqualTo(PokemonType.Rock));
                Assert.That(move.Power, Is.EqualTo(75));
                Assert.That(move.TargetScope, Is.EqualTo(TargetScope.AllEnemies));
                Assert.That(move.HasEffect<FlinchEffect>(), Is.True);
                
                var effect = move.GetEffect<FlinchEffect>();
                Assert.That(effect.ChancePercent, Is.EqualTo(30));
            });
        }

        #endregion

        #region Flying Moves

        [Test]
        public void Test_WingAttack_Is_Physical()
        {
            var move = MoveCatalog.WingAttack;

            Assert.Multiple(() =>
            {
                Assert.That(move.Type, Is.EqualTo(PokemonType.Flying));
                Assert.That(move.Power, Is.EqualTo(60));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Physical));
            });
        }

        [Test]
        public void Test_Fly_Is_Powerful()
        {
            var move = MoveCatalog.Fly;

            Assert.Multiple(() =>
            {
                Assert.That(move.Type, Is.EqualTo(PokemonType.Flying));
                Assert.That(move.Power, Is.EqualTo(90));
                Assert.That(move.Accuracy, Is.EqualTo(95));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Physical));
            });
        }

        #endregion

        #region Poison Moves

        [Test]
        public void Test_PoisonSting_May_Poison()
        {
            var move = MoveCatalog.PoisonSting;

            Assert.Multiple(() =>
            {
                Assert.That(move.Type, Is.EqualTo(PokemonType.Poison));
                Assert.That(move.Power, Is.EqualTo(15));
                Assert.That(move.HasEffect<StatusEffect>(), Is.True);
                
                var effect = move.GetEffect<StatusEffect>();
                Assert.That(effect.Status, Is.EqualTo(PersistentStatus.Poison));
                Assert.That(effect.ChancePercent, Is.EqualTo(30));
            });
        }

        [Test]
        public void Test_SludgeBomb_Is_Powerful()
        {
            var move = MoveCatalog.SludgeBomb;

            Assert.Multiple(() =>
            {
                Assert.That(move.Type, Is.EqualTo(PokemonType.Poison));
                Assert.That(move.Power, Is.EqualTo(90));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Special));
                Assert.That(move.HasEffect<StatusEffect>(), Is.True);
                
                var effect = move.GetEffect<StatusEffect>();
                Assert.That(effect.Status, Is.EqualTo(PersistentStatus.Poison));
                Assert.That(effect.ChancePercent, Is.EqualTo(30));
            });
        }

        #endregion

        #region Dragon Moves

        [Test]
        public void Test_DragonRage_Is_Fixed_Damage()
        {
            var move = MoveCatalog.DragonRage;

            Assert.Multiple(() =>
            {
                Assert.That(move.Type, Is.EqualTo(PokemonType.Dragon));
                Assert.That(move.Power, Is.EqualTo(0)); // Fixed damage moves have 0 power
                Assert.That(move.HasEffect<FixedDamageEffect>(), Is.True);
                
                var effect = move.GetEffect<FixedDamageEffect>();
                Assert.That(effect.Amount, Is.EqualTo(40));
            });
        }

        #endregion

        #region Additional Water Moves

        [Test]
        public void Test_Waterfall_May_Flinch()
        {
            var move = MoveCatalog.Waterfall;

            Assert.Multiple(() =>
            {
                Assert.That(move.Type, Is.EqualTo(PokemonType.Water));
                Assert.That(move.Power, Is.EqualTo(80));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Physical));
                Assert.That(move.HasEffect<FlinchEffect>(), Is.True);
                
                var effect = move.GetEffect<FlinchEffect>();
                Assert.That(effect.ChancePercent, Is.EqualTo(20));
            });
        }

        #endregion

        #region Additional Psychic Moves

        [Test]
        public void Test_Teleport_Has_No_Effects()
        {
            var move = MoveCatalog.Teleport;

            Assert.Multiple(() =>
            {
                Assert.That(move.Type, Is.EqualTo(PokemonType.Psychic));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Status));
                Assert.That(move.Effects.Count, Is.EqualTo(0)); // Switching move, no effects
            });
        }

        [Test]
        public void Test_Confusion_Is_Damaging()
        {
            var move = MoveCatalog.Confusion;

            Assert.Multiple(() =>
            {
                Assert.That(move.Type, Is.EqualTo(PokemonType.Psychic));
                Assert.That(move.Power, Is.EqualTo(50));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Special));
                Assert.That(move.HasEffect<DamageEffect>(), Is.True);
            });
        }

        [Test]
        public void Test_Psybeam_Is_Damaging()
        {
            var move = MoveCatalog.Psybeam;

            Assert.Multiple(() =>
            {
                Assert.That(move.Type, Is.EqualTo(PokemonType.Psychic));
                Assert.That(move.Power, Is.EqualTo(65));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Special));
                Assert.That(move.HasEffect<DamageEffect>(), Is.True);
            });
        }

        [Test]
        public void Test_Hypnosis_May_Sleep()
        {
            var move = MoveCatalog.Hypnosis;

            Assert.Multiple(() =>
            {
                Assert.That(move.Type, Is.EqualTo(PokemonType.Psychic));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Status));
                Assert.That(move.Accuracy, Is.EqualTo(60)); // Lower accuracy
                Assert.That(move.HasEffect<StatusEffect>(), Is.True);
                
                var effect = move.GetEffect<StatusEffect>();
                Assert.That(effect.Status, Is.EqualTo(PersistentStatus.Sleep));
                Assert.That(effect.ChancePercent, Is.EqualTo(100));
            });
        }

        #endregion

        #region Additional Normal Moves

        [Test]
        public void Test_DefenseCurl_Raises_Defense()
        {
            var move = MoveCatalog.DefenseCurl;

            Assert.Multiple(() =>
            {
                Assert.That(move.Type, Is.EqualTo(PokemonType.Normal));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Status));
                Assert.That(move.HasEffect<StatChangeEffect>(), Is.True);
                
                var effect = move.GetEffect<StatChangeEffect>();
                Assert.That(effect.TargetStat, Is.EqualTo(Stat.Defense));
                Assert.That(effect.Stages, Is.EqualTo(1));
                Assert.That(effect.Stages, Is.GreaterThan(0)); // Positive stages = raises stat
            });
        }

        [Test]
        public void Test_Splash_Has_No_Effects()
        {
            var move = MoveCatalog.Splash;

            Assert.Multiple(() =>
            {
                Assert.That(move.Type, Is.EqualTo(PokemonType.Normal));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Status));
                Assert.That(move.Effects.Count, Is.EqualTo(0)); // Does nothing
            });
        }

        #endregion
    }
}

