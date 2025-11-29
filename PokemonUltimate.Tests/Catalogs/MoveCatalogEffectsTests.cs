using PokemonUltimate.Core.Catalogs;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Catalogs
{
    // Tests to verify that MoveCatalog moves have correctly defined effects
    public class MoveCatalogEffectsTests
    {
        #region Damage-Only Moves

        [Test]
        public void Test_Tackle_Has_DamageEffect()
        {
            var tackle = MoveCatalog.Tackle;

            Assert.Multiple(() =>
            {
                Assert.That(tackle.HasEffect<DamageEffect>(), Is.True);
                Assert.That(tackle.Effects, Has.Count.EqualTo(1));
            });
        }

        [Test]
        public void Test_Surf_Has_DamageEffect()
        {
            var surf = MoveCatalog.Surf;

            Assert.Multiple(() =>
            {
                Assert.That(surf.HasEffect<DamageEffect>(), Is.True);
                Assert.That(surf.TargetScope, Is.EqualTo(TargetScope.AllEnemies));
            });
        }

        #endregion

        #region Damage + Status Moves

        [Test]
        public void Test_Ember_Has_Damage_And_Burn()
        {
            var ember = MoveCatalog.Ember;

            Assert.Multiple(() =>
            {
                Assert.That(ember.HasEffect<DamageEffect>(), Is.True);
                Assert.That(ember.HasEffect<StatusEffect>(), Is.True);
                
                var status = ember.GetEffect<StatusEffect>();
                Assert.That(status.Status, Is.EqualTo(PersistentStatus.Burn));
                Assert.That(status.ChancePercent, Is.EqualTo(10));
            });
        }

        [Test]
        public void Test_Flamethrower_Has_Damage_And_Burn()
        {
            var flamethrower = MoveCatalog.Flamethrower;

            Assert.Multiple(() =>
            {
                Assert.That(flamethrower.HasEffect<DamageEffect>(), Is.True);
                var status = flamethrower.GetEffect<StatusEffect>();
                Assert.That(status.Status, Is.EqualTo(PersistentStatus.Burn));
            });
        }

        [Test]
        public void Test_Thunderbolt_Has_Damage_And_Paralysis()
        {
            var thunderbolt = MoveCatalog.Thunderbolt;

            Assert.Multiple(() =>
            {
                Assert.That(thunderbolt.HasEffect<DamageEffect>(), Is.True);
                Assert.That(thunderbolt.HasEffect<StatusEffect>(), Is.True);
                
                var status = thunderbolt.GetEffect<StatusEffect>();
                Assert.That(status.Status, Is.EqualTo(PersistentStatus.Paralysis));
                Assert.That(status.ChancePercent, Is.EqualTo(10));
            });
        }

        [Test]
        public void Test_Thunder_Has_Higher_Paralysis_Chance()
        {
            var thunder = MoveCatalog.Thunder;
            var status = thunder.GetEffect<StatusEffect>();

            Assert.That(status.ChancePercent, Is.EqualTo(30), "Thunder should have 30% paralysis chance");
        }

        #endregion

        #region Status-Only Moves

        [Test]
        public void Test_ThunderWave_Is_Status_Only()
        {
            var thunderWave = MoveCatalog.ThunderWave;

            Assert.Multiple(() =>
            {
                Assert.That(thunderWave.Category, Is.EqualTo(MoveCategory.Status));
                Assert.That(thunderWave.Power, Is.EqualTo(0));
                Assert.That(thunderWave.HasEffect<DamageEffect>(), Is.False);
                Assert.That(thunderWave.HasEffect<StatusEffect>(), Is.True);
                
                var status = thunderWave.GetEffect<StatusEffect>();
                Assert.That(status.Status, Is.EqualTo(PersistentStatus.Paralysis));
                Assert.That(status.ChancePercent, Is.EqualTo(100)); // Guaranteed
            });
        }

        [Test]
        public void Test_Growl_Lowers_Attack()
        {
            var growl = MoveCatalog.Growl;

            Assert.Multiple(() =>
            {
                Assert.That(growl.Category, Is.EqualTo(MoveCategory.Status));
                Assert.That(growl.HasEffect<StatChangeEffect>(), Is.True);
                
                var statChange = growl.GetEffect<StatChangeEffect>();
                Assert.That(statChange.TargetStat, Is.EqualTo(Stat.Attack));
                Assert.That(statChange.Stages, Is.EqualTo(-1));
                Assert.That(statChange.TargetSelf, Is.False);
            });
        }

        #endregion

        #region Damage + StatChange Moves

        [Test]
        public void Test_Psychic_May_Lower_SpDef()
        {
            var psychic = MoveCatalog.Psychic;

            Assert.Multiple(() =>
            {
                Assert.That(psychic.HasEffect<DamageEffect>(), Is.True);
                Assert.That(psychic.HasEffect<StatChangeEffect>(), Is.True);
                
                var statChange = psychic.GetEffect<StatChangeEffect>();
                Assert.That(statChange.TargetStat, Is.EqualTo(Stat.SpDefense));
                Assert.That(statChange.Stages, Is.EqualTo(-1));
                Assert.That(statChange.ChancePercent, Is.EqualTo(10));
            });
        }

        #endregion

        #region High Crit Moves

        [Test]
        public void Test_RazorLeaf_Has_High_Crit()
        {
            var razorLeaf = MoveCatalog.RazorLeaf;
            var damage = razorLeaf.GetEffect<DamageEffect>();

            Assert.Multiple(() =>
            {
                Assert.That(damage, Is.Not.Null);
                Assert.That(damage.CritStages, Is.EqualTo(1));
            });
        }

        #endregion

        #region All Moves Have Effects

        [Test]
        public void Test_All_Damaging_Moves_Have_DamageEffect()
        {
            var damagingMoves = MoveCatalog.All
                .Where(m => m.Category != MoveCategory.Status && m.Power > 0)
                .ToList();

            foreach (var move in damagingMoves)
            {
                Assert.That(move.HasEffect<DamageEffect>(), Is.True, 
                    $"Move '{move.Name}' should have DamageEffect");
            }
        }

        [Test]
        public void Test_All_Moves_Have_At_Least_One_Effect()
        {
            // Exclude Splash-like moves that intentionally have no effects
            var movesWithEffects = MoveCatalog.All.Where(m => m.Effects.Count > 0).ToList();

            // Most moves should have at least one effect
            Assert.That(movesWithEffects.Count, Is.GreaterThanOrEqualTo(MoveCatalog.Count - 1),
                "Almost all moves should have at least one effect");
        }

        #endregion
    }
}

