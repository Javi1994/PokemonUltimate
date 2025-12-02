using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Systems.Effects.Basic
{
    // Tests for individual move effects and their properties
    public class MoveEffectTests
    {
        #region DamageEffect Tests

        [Test]
        public void Test_DamageEffect_Default_Values()
        {
            var effect = new DamageEffect();

            Assert.Multiple(() =>
            {
                Assert.That(effect.EffectType, Is.EqualTo(EffectType.Damage));
                Assert.That(effect.DamageMultiplier, Is.EqualTo(1.0f));
                Assert.That(effect.CanCrit, Is.True);
                Assert.That(effect.CritStages, Is.EqualTo(0));
            });
        }

        [Test]
        public void Test_DamageEffect_Custom_Values()
        {
            var effect = new DamageEffect
            {
                DamageMultiplier = 1.5f,
                CanCrit = false,
                CritStages = 2
            };

            Assert.Multiple(() =>
            {
                Assert.That(effect.DamageMultiplier, Is.EqualTo(1.5f));
                Assert.That(effect.CanCrit, Is.False);
                Assert.That(effect.CritStages, Is.EqualTo(2));
            });
        }

        #endregion

        #region FixedDamageEffect Tests

        [Test]
        public void Test_FixedDamageEffect_With_Amount()
        {
            var effect = new FixedDamageEffect(40);

            Assert.Multiple(() =>
            {
                Assert.That(effect.EffectType, Is.EqualTo(EffectType.FixedDamage));
                Assert.That(effect.Amount, Is.EqualTo(40));
                Assert.That(effect.UseLevelAsDamage, Is.False);
            });
        }

        [Test]
        public void Test_FixedDamageEffect_Level_Based()
        {
            var effect = new FixedDamageEffect { UseLevelAsDamage = true };

            Assert.That(effect.UseLevelAsDamage, Is.True);
        }

        #endregion

        #region StatusEffect Tests

        [Test]
        public void Test_StatusEffect_Guaranteed()
        {
            var effect = new StatusEffect(PersistentStatus.Burn, 100);

            Assert.Multiple(() =>
            {
                Assert.That(effect.EffectType, Is.EqualTo(EffectType.Status));
                Assert.That(effect.Status, Is.EqualTo(PersistentStatus.Burn));
                Assert.That(effect.ChancePercent, Is.EqualTo(100));
                Assert.That(effect.TargetSelf, Is.False);
            });
        }

        [Test]
        public void Test_StatusEffect_With_Chance()
        {
            var effect = new StatusEffect(PersistentStatus.Paralysis, 30);

            Assert.Multiple(() =>
            {
                Assert.That(effect.Status, Is.EqualTo(PersistentStatus.Paralysis));
                Assert.That(effect.ChancePercent, Is.EqualTo(30));
            });
        }

        [Test]
        public void Test_StatusEffect_Self_Target()
        {
            var effect = new StatusEffect
            {
                Status = PersistentStatus.Sleep,
                TargetSelf = true
            };

            Assert.That(effect.TargetSelf, Is.True);
        }

        [Test]
        [TestCase(PersistentStatus.Burn)]
        [TestCase(PersistentStatus.Paralysis)]
        [TestCase(PersistentStatus.Sleep)]
        [TestCase(PersistentStatus.Poison)]
        [TestCase(PersistentStatus.BadlyPoisoned)]
        [TestCase(PersistentStatus.Freeze)]
        public void Test_StatusEffect_All_Statuses(PersistentStatus status)
        {
            var effect = new StatusEffect(status);

            Assert.That(effect.Status, Is.EqualTo(status));
        }

        #endregion

        #region StatChangeEffect Tests

        [Test]
        public void Test_StatChangeEffect_Buff()
        {
            var effect = new StatChangeEffect(Stat.Attack, 2, targetSelf: true);

            Assert.Multiple(() =>
            {
                Assert.That(effect.EffectType, Is.EqualTo(EffectType.StatChange));
                Assert.That(effect.TargetStat, Is.EqualTo(Stat.Attack));
                Assert.That(effect.Stages, Is.EqualTo(2));
                Assert.That(effect.TargetSelf, Is.True);
                Assert.That(effect.ChancePercent, Is.EqualTo(100));
            });
        }

        [Test]
        public void Test_StatChangeEffect_Debuff()
        {
            var effect = new StatChangeEffect(Stat.Defense, -1, targetSelf: false);

            Assert.Multiple(() =>
            {
                Assert.That(effect.Stages, Is.EqualTo(-1));
                Assert.That(effect.TargetSelf, Is.False);
            });
        }

        [Test]
        public void Test_StatChangeEffect_With_Chance()
        {
            var effect = new StatChangeEffect(Stat.SpDefense, -1, targetSelf: false, chancePercent: 10);

            Assert.That(effect.ChancePercent, Is.EqualTo(10));
        }

        [Test]
        [TestCase(Stat.Attack)]
        [TestCase(Stat.Defense)]
        [TestCase(Stat.SpAttack)]
        [TestCase(Stat.SpDefense)]
        [TestCase(Stat.Speed)]
        [TestCase(Stat.Accuracy)]
        [TestCase(Stat.Evasion)]
        public void Test_StatChangeEffect_All_Stats(Stat stat)
        {
            var effect = new StatChangeEffect(stat, 1);

            Assert.That(effect.TargetStat, Is.EqualTo(stat));
        }

        #endregion

        #region RecoilEffect Tests

        [Test]
        public void Test_RecoilEffect()
        {
            var effect = new RecoilEffect(33);

            Assert.Multiple(() =>
            {
                Assert.That(effect.EffectType, Is.EqualTo(EffectType.Recoil));
                Assert.That(effect.RecoilPercent, Is.EqualTo(33));
            });
        }

        #endregion

        #region DrainEffect Tests

        [Test]
        public void Test_DrainEffect()
        {
            var effect = new DrainEffect(50);

            Assert.Multiple(() =>
            {
                Assert.That(effect.EffectType, Is.EqualTo(EffectType.Drain));
                Assert.That(effect.DrainPercent, Is.EqualTo(50));
            });
        }

        #endregion

        #region HealEffect Tests

        [Test]
        public void Test_HealEffect()
        {
            var effect = new HealEffect(50);

            Assert.Multiple(() =>
            {
                Assert.That(effect.EffectType, Is.EqualTo(EffectType.Heal));
                Assert.That(effect.HealPercent, Is.EqualTo(50));
            });
        }

        #endregion

        #region FlinchEffect Tests

        [Test]
        public void Test_FlinchEffect()
        {
            var effect = new FlinchEffect(30);

            Assert.Multiple(() =>
            {
                Assert.That(effect.EffectType, Is.EqualTo(EffectType.Flinch));
                Assert.That(effect.ChancePercent, Is.EqualTo(30));
            });
        }

        #endregion

        #region MultiHitEffect Tests

        [Test]
        public void Test_MultiHitEffect_Range()
        {
            var effect = new MultiHitEffect(2, 5);

            Assert.Multiple(() =>
            {
                Assert.That(effect.EffectType, Is.EqualTo(EffectType.MultiHit));
                Assert.That(effect.MinHits, Is.EqualTo(2));
                Assert.That(effect.MaxHits, Is.EqualTo(5));
            });
        }

        [Test]
        public void Test_MultiHitEffect_Fixed()
        {
            var effect = new MultiHitEffect(2);

            Assert.Multiple(() =>
            {
                Assert.That(effect.MinHits, Is.EqualTo(2));
                Assert.That(effect.MaxHits, Is.EqualTo(2));
            });
        }

        #endregion
    }
}
