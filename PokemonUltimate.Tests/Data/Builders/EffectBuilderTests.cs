using NUnit.Framework;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;
using System.Linq;
using PokemonUltimate.Content.Builders;

namespace PokemonUltimate.Tests.Data.Builders
{
    [TestFixture]
    public class EffectBuilderTests
    {
        #region Damage Effects

        [Test]
        public void Damage_Should_Add_DamageEffect()
        {
            var effects = new EffectBuilder()
                .Damage()
                .Build();

            Assert.That(effects, Has.Count.EqualTo(1));
            Assert.That(effects[0], Is.TypeOf<DamageEffect>());
        }

        [Test]
        public void DamageHighCrit_Should_Add_DamageEffect_With_CritStages()
        {
            var effects = new EffectBuilder()
                .DamageHighCrit(2)
                .Build();

            var damage = effects[0] as DamageEffect;
            Assert.That(damage.CritStages, Is.EqualTo(2));
        }

        [Test]
        public void FixedDamage_Should_Add_FixedDamageEffect()
        {
            var effects = new EffectBuilder()
                .FixedDamage(40)
                .Build();

            var damage = effects[0] as FixedDamageEffect;
            Assert.Multiple(() =>
            {
                Assert.That(damage.Amount, Is.EqualTo(40));
                Assert.That(damage.UseLevelAsDamage, Is.False);
            });
        }

        [Test]
        public void LevelDamage_Should_Use_Level_As_Damage()
        {
            var effects = new EffectBuilder()
                .LevelDamage()
                .Build();

            var damage = effects[0] as FixedDamageEffect;
            Assert.That(damage.UseLevelAsDamage, Is.True);
        }

        #endregion

        #region Status Effects

        [Test]
        public void MayBurn_Should_Add_Burn_Status()
        {
            var effects = new EffectBuilder()
                .MayBurn(30)
                .Build();

            var status = effects[0] as StatusEffect;
            Assert.Multiple(() =>
            {
                Assert.That(status.Status, Is.EqualTo(PersistentStatus.Burn));
                Assert.That(status.ChancePercent, Is.EqualTo(30));
            });
        }

        [Test]
        public void MayParalyze_Should_Add_Paralysis_Status()
        {
            var effects = new EffectBuilder()
                .MayParalyze(100)
                .Build();

            var status = effects[0] as StatusEffect;
            Assert.That(status.Status, Is.EqualTo(PersistentStatus.Paralysis));
        }

        [Test]
        public void MayPoison_Should_Add_Poison_Status()
        {
            var effects = new EffectBuilder()
                .MayPoison(30)
                .Build();

            var status = effects[0] as StatusEffect;
            Assert.That(status.Status, Is.EqualTo(PersistentStatus.Poison));
        }

        [Test]
        public void MayBadlyPoison_Should_Add_BadlyPoisoned_Status()
        {
            var effects = new EffectBuilder()
                .MayBadlyPoison()
                .Build();

            var status = effects[0] as StatusEffect;
            Assert.That(status.Status, Is.EqualTo(PersistentStatus.BadlyPoisoned));
        }

        [Test]
        public void MaySleep_Should_Add_Sleep_Status()
        {
            var effects = new EffectBuilder()
                .MaySleep()
                .Build();

            var status = effects[0] as StatusEffect;
            Assert.That(status.Status, Is.EqualTo(PersistentStatus.Sleep));
        }

        [Test]
        public void MayFreeze_Should_Add_Freeze_Status()
        {
            var effects = new EffectBuilder()
                .MayFreeze(10)
                .Build();

            var status = effects[0] as StatusEffect;
            Assert.Multiple(() =>
            {
                Assert.That(status.Status, Is.EqualTo(PersistentStatus.Freeze));
                Assert.That(status.ChancePercent, Is.EqualTo(10));
            });
        }

        [Test]
        public void SelfStatus_Should_Target_Self()
        {
            var effects = new EffectBuilder()
                .SelfStatus(PersistentStatus.Sleep)
                .Build();

            var status = effects[0] as StatusEffect;
            Assert.Multiple(() =>
            {
                Assert.That(status.Status, Is.EqualTo(PersistentStatus.Sleep));
                Assert.That(status.TargetSelf, Is.True);
            });
        }

        #endregion

        #region Stat Change Effects - Raise

        [Test]
        public void RaiseAttack_Should_Add_Buff()
        {
            var effects = new EffectBuilder()
                .RaiseAttack(2)
                .Build();

            var stat = effects[0] as StatChangeEffect;
            Assert.Multiple(() =>
            {
                Assert.That(stat.TargetStat, Is.EqualTo(Stat.Attack));
                Assert.That(stat.Stages, Is.EqualTo(2));
                Assert.That(stat.TargetSelf, Is.True);
            });
        }

        [Test]
        public void RaiseDefense_Should_Add_Buff()
        {
            var effects = new EffectBuilder()
                .RaiseDefense(1)
                .Build();

            var stat = effects[0] as StatChangeEffect;
            Assert.That(stat.TargetStat, Is.EqualTo(Stat.Defense));
        }

        [Test]
        public void RaiseSpAttack_Should_Add_Buff()
        {
            var effects = new EffectBuilder()
                .RaiseSpAttack()
                .Build();

            var stat = effects[0] as StatChangeEffect;
            Assert.That(stat.TargetStat, Is.EqualTo(Stat.SpAttack));
        }

        [Test]
        public void RaiseSpeed_Should_Add_Buff()
        {
            var effects = new EffectBuilder()
                .RaiseSpeed(2)
                .Build();

            var stat = effects[0] as StatChangeEffect;
            Assert.Multiple(() =>
            {
                Assert.That(stat.TargetStat, Is.EqualTo(Stat.Speed));
                Assert.That(stat.Stages, Is.EqualTo(2));
            });
        }

        [Test]
        public void RaiseEvasion_Should_Add_Buff()
        {
            var effects = new EffectBuilder()
                .RaiseEvasion()
                .Build();

            var stat = effects[0] as StatChangeEffect;
            Assert.That(stat.TargetStat, Is.EqualTo(Stat.Evasion));
        }

        [Test]
        public void RaiseSpDefense_Should_Add_Buff()
        {
            var effects = new EffectBuilder()
                .RaiseSpDefense(2)
                .Build();

            var stat = effects[0] as StatChangeEffect;
            Assert.Multiple(() =>
            {
                Assert.That(stat.TargetStat, Is.EqualTo(Stat.SpDefense));
                Assert.That(stat.Stages, Is.EqualTo(2));
                Assert.That(stat.TargetSelf, Is.True);
            });
        }

        #endregion

        #region Stat Change Effects - Lower

        [Test]
        public void LowerAttack_Should_Add_Debuff()
        {
            var effects = new EffectBuilder()
                .LowerAttack(1)
                .Build();

            var stat = effects[0] as StatChangeEffect;
            Assert.Multiple(() =>
            {
                Assert.That(stat.TargetStat, Is.EqualTo(Stat.Attack));
                Assert.That(stat.Stages, Is.EqualTo(-1));
                Assert.That(stat.TargetSelf, Is.False);
            });
        }

        [Test]
        public void LowerDefense_Should_Add_Debuff()
        {
            var effects = new EffectBuilder()
                .LowerDefense(2)
                .Build();

            var stat = effects[0] as StatChangeEffect;
            Assert.That(stat.Stages, Is.EqualTo(-2));
        }

        [Test]
        public void LowerSpeed_Should_Add_Debuff()
        {
            var effects = new EffectBuilder()
                .LowerSpeed()
                .Build();

            var stat = effects[0] as StatChangeEffect;
            Assert.Multiple(() =>
            {
                Assert.That(stat.TargetStat, Is.EqualTo(Stat.Speed));
                Assert.That(stat.Stages, Is.EqualTo(-1));
            });
        }

        [Test]
        public void LowerAccuracy_Should_Add_Debuff()
        {
            var effects = new EffectBuilder()
                .LowerAccuracy()
                .Build();

            var stat = effects[0] as StatChangeEffect;
            Assert.That(stat.TargetStat, Is.EqualTo(Stat.Accuracy));
        }

        [Test]
        public void LowerSpAttack_Should_Add_Debuff()
        {
            var effects = new EffectBuilder()
                .LowerSpAttack(2)
                .Build();

            var stat = effects[0] as StatChangeEffect;
            Assert.Multiple(() =>
            {
                Assert.That(stat.TargetStat, Is.EqualTo(Stat.SpAttack));
                Assert.That(stat.Stages, Is.EqualTo(-2));
                Assert.That(stat.TargetSelf, Is.False);
            });
        }

        [Test]
        public void Stat_Effect_With_Chance()
        {
            var effects = new EffectBuilder()
                .LowerSpDefense(1, 10)
                .Build();

            var stat = effects[0] as StatChangeEffect;
            Assert.That(stat.ChancePercent, Is.EqualTo(10));
        }

        #endregion

        #region Recoil, Drain, Heal

        [Test]
        public void Recoil_Should_Add_RecoilEffect()
        {
            var effects = new EffectBuilder()
                .Recoil(33)
                .Build();

            var recoil = effects[0] as RecoilEffect;
            Assert.That(recoil.RecoilPercent, Is.EqualTo(33));
        }

        [Test]
        public void Drain_Should_Add_DrainEffect()
        {
            var effects = new EffectBuilder()
                .Drain(50)
                .Build();

            var drain = effects[0] as DrainEffect;
            Assert.That(drain.DrainPercent, Is.EqualTo(50));
        }

        [Test]
        public void Heal_Should_Add_HealEffect()
        {
            var effects = new EffectBuilder()
                .Heal(50)
                .Build();

            var heal = effects[0] as HealEffect;
            Assert.That(heal.HealPercent, Is.EqualTo(50));
        }

        #endregion

        #region Flinch and MultiHit

        [Test]
        public void MayFlinch_Should_Add_FlinchEffect()
        {
            var effects = new EffectBuilder()
                .MayFlinch(30)
                .Build();

            var flinch = effects[0] as FlinchEffect;
            Assert.That(flinch.ChancePercent, Is.EqualTo(30));
        }

        [Test]
        public void MultiHit_Should_Add_MultiHitEffect()
        {
            var effects = new EffectBuilder()
                .MultiHit(2, 5)
                .Build();

            var multi = effects[0] as MultiHitEffect;
            Assert.Multiple(() =>
            {
                Assert.That(multi.MinHits, Is.EqualTo(2));
                Assert.That(multi.MaxHits, Is.EqualTo(5));
            });
        }

        [Test]
        public void HitsNTimes_Should_Set_Fixed_Hits()
        {
            var effects = new EffectBuilder()
                .HitsNTimes(3)
                .Build();

            var multi = effects[0] as MultiHitEffect;
            Assert.Multiple(() =>
            {
                Assert.That(multi.MinHits, Is.EqualTo(3));
                Assert.That(multi.MaxHits, Is.EqualTo(3));
            });
        }

        #endregion

        #region Chaining Multiple Effects

        [Test]
        public void Should_Chain_Multiple_Effects()
        {
            var effects = new EffectBuilder()
                .Damage()
                .MayBurn(10)
                .MayFlinch(30)
                .Build();

            Assert.That(effects, Has.Count.EqualTo(3));
        }

        [Test]
        public void Complex_Effect_Chain()
        {
            var effects = new EffectBuilder()
                .Damage()
                .Recoil(25)
                .LowerDefense(1, 10)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(effects, Has.Count.EqualTo(3));
                Assert.That(effects[0], Is.TypeOf<DamageEffect>());
                Assert.That(effects[1], Is.TypeOf<RecoilEffect>());
                Assert.That(effects[2], Is.TypeOf<StatChangeEffect>());
            });
        }

        #endregion
    }
}

