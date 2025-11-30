using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Effects
{
    /// <summary>
    /// Edge case tests for IMoveEffect implementations.
    /// Tests boundary values, effect combinations, and extreme scenarios.
    /// </summary>
    [TestFixture]
    public class EffectsEdgeCasesTests
    {
        #region DamageEffect Edge Cases

        [Test]
        public void DamageEffect_DefaultValues()
        {
            var effect = new DamageEffect();
            Assert.That(effect.DamageMultiplier, Is.EqualTo(1.0f));
            Assert.That(effect.CanCrit, Is.True);
            Assert.That(effect.CritStages, Is.EqualTo(0));
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.Damage));
        }

        [Test]
        public void DamageEffect_ZeroMultiplier()
        {
            var effect = new DamageEffect { DamageMultiplier = 0f };
            Assert.That(effect.DamageMultiplier, Is.EqualTo(0f));
        }

        [Test]
        public void DamageEffect_NegativeMultiplier_Allowed()
        {
            // Edge case: negative multiplier (would heal instead of damage)
            var effect = new DamageEffect { DamageMultiplier = -0.5f };
            Assert.That(effect.DamageMultiplier, Is.EqualTo(-0.5f));
        }

        [Test]
        public void DamageEffect_VeryHighMultiplier()
        {
            var effect = new DamageEffect { DamageMultiplier = 10.0f };
            Assert.That(effect.DamageMultiplier, Is.EqualTo(10.0f));
        }

        [Test]
        public void DamageEffect_HighCritStages()
        {
            var effect = new DamageEffect { CritStages = 3 };
            Assert.That(effect.CritStages, Is.EqualTo(3));
        }

        [Test]
        public void DamageEffect_NoCrit()
        {
            var effect = new DamageEffect { CanCrit = false };
            Assert.That(effect.CanCrit, Is.False);
        }

        [Test]
        public void DamageEffect_Description_IsNotEmpty()
        {
            var effect = new DamageEffect();
            Assert.That(effect.Description, Is.Not.Null.And.Not.Empty);
        }

        #endregion

        #region StatChangeEffect Edge Cases

        [Test]
        public void StatChangeEffect_MaxPositiveStages()
        {
            var effect = new StatChangeEffect(Stat.Attack, 6, true);
            Assert.That(effect.Stages, Is.EqualTo(6));
        }

        [Test]
        public void StatChangeEffect_MaxNegativeStages()
        {
            var effect = new StatChangeEffect(Stat.Defense, -6, false);
            Assert.That(effect.Stages, Is.EqualTo(-6));
        }

        [Test]
        public void StatChangeEffect_BeyondMaxStages_Allowed()
        {
            // The effect stores the value; clamping happens during battle
            var effect = new StatChangeEffect(Stat.Speed, 12, true);
            Assert.That(effect.Stages, Is.EqualTo(12));
        }

        [Test]
        public void StatChangeEffect_ZeroStages()
        {
            var effect = new StatChangeEffect(Stat.Attack, 0, true);
            Assert.That(effect.Stages, Is.EqualTo(0));
        }

        [Test]
        public void StatChangeEffect_ZeroChance()
        {
            var effect = new StatChangeEffect(Stat.Attack, 1, true, 0);
            Assert.That(effect.ChancePercent, Is.EqualTo(0));
        }

        [Test]
        public void StatChangeEffect_Over100Chance()
        {
            var effect = new StatChangeEffect(Stat.Attack, 1, true, 200);
            Assert.That(effect.ChancePercent, Is.EqualTo(200));
        }

        [Test]
        public void StatChangeEffect_AllStats()
        {
            var stats = new[] { Stat.Attack, Stat.Defense, Stat.SpAttack, Stat.SpDefense, Stat.Speed, Stat.Accuracy, Stat.Evasion };
            foreach (var stat in stats)
            {
                var effect = new StatChangeEffect(stat, 1, true);
                Assert.That(effect.TargetStat, Is.EqualTo(stat));
            }
        }

        [Test]
        public void StatChangeEffect_Description_ShowsPositiveStages()
        {
            var effect = new StatChangeEffect(Stat.Attack, 2, true);
            Assert.That(effect.Description, Does.Contain("+2"));
        }

        [Test]
        public void StatChangeEffect_Description_ShowsNegativeStages()
        {
            var effect = new StatChangeEffect(Stat.Defense, -1, false);
            Assert.That(effect.Description, Does.Contain("-1"));
        }

        #endregion

        #region StatusEffect Edge Cases

        [Test]
        public void StatusEffect_AllPersistentStatuses()
        {
            var statuses = new[] 
            { 
                PersistentStatus.Burn, 
                PersistentStatus.Paralysis, 
                PersistentStatus.Poison, 
                PersistentStatus.BadlyPoisoned,
                PersistentStatus.Sleep,
                PersistentStatus.Freeze
            };

            foreach (var status in statuses)
            {
                var effect = new StatusEffect { Status = status };
                Assert.That(effect.Status, Is.EqualTo(status));
                Assert.That(effect.EffectType, Is.EqualTo(EffectType.Status));
            }
        }

        [Test]
        public void StatusEffect_ZeroChance()
        {
            var effect = new StatusEffect { Status = PersistentStatus.Burn, ChancePercent = 0 };
            Assert.That(effect.ChancePercent, Is.EqualTo(0));
        }

        [Test]
        public void StatusEffect_GuaranteedChance()
        {
            var effect = new StatusEffect { Status = PersistentStatus.Paralysis, ChancePercent = 100 };
            Assert.That(effect.ChancePercent, Is.EqualTo(100));
        }

        [Test]
        public void StatusEffect_LowChance()
        {
            // 10% burn chance like Fire Punch
            var effect = new StatusEffect { Status = PersistentStatus.Burn, ChancePercent = 10 };
            Assert.That(effect.ChancePercent, Is.EqualTo(10));
        }

        [Test]
        public void StatusEffect_30PercentChance()
        {
            // 30% chance like Flamethrower burn
            var effect = new StatusEffect { Status = PersistentStatus.Burn, ChancePercent = 30 };
            Assert.That(effect.ChancePercent, Is.EqualTo(30));
        }

        #endregion

        #region RecoilEffect Edge Cases

        [Test]
        public void RecoilEffect_StandardRecoil()
        {
            // 1/4 recoil like Take Down
            var effect = new RecoilEffect { Percent = 25 };
            Assert.That(effect.Percent, Is.EqualTo(25));
        }

        [Test]
        public void RecoilEffect_HighRecoil()
        {
            // 1/3 recoil like Double-Edge
            var effect = new RecoilEffect { Percent = 33 };
            Assert.That(effect.Percent, Is.EqualTo(33));
        }

        [Test]
        public void RecoilEffect_HalfRecoil()
        {
            // 50% recoil like Head Smash
            var effect = new RecoilEffect { Percent = 50 };
            Assert.That(effect.Percent, Is.EqualTo(50));
        }

        [Test]
        public void RecoilEffect_ZeroRecoil()
        {
            var effect = new RecoilEffect { Percent = 0 };
            Assert.That(effect.Percent, Is.EqualTo(0));
        }

        [Test]
        public void RecoilEffect_Over100Percent()
        {
            // Edge case: more than 100% recoil
            var effect = new RecoilEffect { Percent = 150 };
            Assert.That(effect.Percent, Is.EqualTo(150));
        }

        #endregion

        #region DrainEffect Edge Cases

        [Test]
        public void DrainEffect_StandardDrain()
        {
            // 50% drain like Giga Drain
            var effect = new DrainEffect { Percent = 50 };
            Assert.That(effect.Percent, Is.EqualTo(50));
        }

        [Test]
        public void DrainEffect_HighDrain()
        {
            // 75% drain like Drain Punch with Big Root
            var effect = new DrainEffect { Percent = 75 };
            Assert.That(effect.Percent, Is.EqualTo(75));
        }

        [Test]
        public void DrainEffect_FullDrain()
        {
            // 100% drain (all damage healed)
            var effect = new DrainEffect { Percent = 100 };
            Assert.That(effect.Percent, Is.EqualTo(100));
        }

        [Test]
        public void DrainEffect_ZeroDrain()
        {
            var effect = new DrainEffect { Percent = 0 };
            Assert.That(effect.Percent, Is.EqualTo(0));
        }

        #endregion

        #region HealEffect Edge Cases

        [Test]
        public void HealEffect_StandardHeal()
        {
            // 50% heal like Recover
            var effect = new HealEffect { Percent = 50 };
            Assert.That(effect.Percent, Is.EqualTo(50));
        }

        [Test]
        public void HealEffect_FullHeal()
        {
            // 100% heal
            var effect = new HealEffect { Percent = 100 };
            Assert.That(effect.Percent, Is.EqualTo(100));
        }

        [Test]
        public void HealEffect_SmallHeal()
        {
            // 25% heal like Roost in some cases
            var effect = new HealEffect { Percent = 25 };
            Assert.That(effect.Percent, Is.EqualTo(25));
        }

        [Test]
        public void HealEffect_ZeroHeal()
        {
            var effect = new HealEffect { Percent = 0 };
            Assert.That(effect.Percent, Is.EqualTo(0));
        }

        [Test]
        public void HealEffect_OverHeal()
        {
            // Over 100% (edge case)
            var effect = new HealEffect { Percent = 200 };
            Assert.That(effect.Percent, Is.EqualTo(200));
        }

        #endregion

        #region FlinchEffect Edge Cases

        [Test]
        public void FlinchEffect_StandardChance()
        {
            // 30% flinch like Bite
            var effect = new FlinchEffect { ChancePercent = 30 };
            Assert.That(effect.ChancePercent, Is.EqualTo(30));
        }

        [Test]
        public void FlinchEffect_GuaranteedFlinch()
        {
            // 100% flinch like Fake Out
            var effect = new FlinchEffect { ChancePercent = 100 };
            Assert.That(effect.ChancePercent, Is.EqualTo(100));
        }

        [Test]
        public void FlinchEffect_LowChance()
        {
            // 10% flinch
            var effect = new FlinchEffect { ChancePercent = 10 };
            Assert.That(effect.ChancePercent, Is.EqualTo(10));
        }

        [Test]
        public void FlinchEffect_ZeroChance()
        {
            var effect = new FlinchEffect { ChancePercent = 0 };
            Assert.That(effect.ChancePercent, Is.EqualTo(0));
        }

        #endregion

        #region MultiHitEffect Edge Cases

        [Test]
        public void MultiHitEffect_Standard2to5()
        {
            var effect = new MultiHitEffect { MinHits = 2, MaxHits = 5 };
            Assert.That(effect.MinHits, Is.EqualTo(2));
            Assert.That(effect.MaxHits, Is.EqualTo(5));
        }

        [Test]
        public void MultiHitEffect_Fixed2Hits()
        {
            // Like Double Kick
            var effect = new MultiHitEffect { MinHits = 2, MaxHits = 2 };
            Assert.That(effect.MinHits, Is.EqualTo(2));
            Assert.That(effect.MaxHits, Is.EqualTo(2));
        }

        [Test]
        public void MultiHitEffect_Fixed3Hits()
        {
            // Like Triple Kick
            var effect = new MultiHitEffect { MinHits = 3, MaxHits = 3 };
            Assert.That(effect.MinHits, Is.EqualTo(3));
            Assert.That(effect.MaxHits, Is.EqualTo(3));
        }

        [Test]
        public void MultiHitEffect_Fixed5Hits()
        {
            // Like Population Bomb with Loaded Dice
            var effect = new MultiHitEffect { MinHits = 5, MaxHits = 5 };
            Assert.That(effect.MinHits, Is.EqualTo(5));
            Assert.That(effect.MaxHits, Is.EqualTo(5));
        }

        [Test]
        public void MultiHitEffect_HighHitCount()
        {
            // Extreme edge case
            var effect = new MultiHitEffect { MinHits = 10, MaxHits = 10 };
            Assert.That(effect.MinHits, Is.EqualTo(10));
        }

        [Test]
        public void MultiHitEffect_SingleHit()
        {
            var effect = new MultiHitEffect { MinHits = 1, MaxHits = 1 };
            Assert.That(effect.MinHits, Is.EqualTo(1));
        }

        #endregion

        #region FixedDamageEffect Edge Cases

        [Test]
        public void FixedDamageEffect_Standard20()
        {
            // Like Sonic Boom
            var effect = new FixedDamageEffect { Damage = 20 };
            Assert.That(effect.Damage, Is.EqualTo(20));
        }

        [Test]
        public void FixedDamageEffect_Standard40()
        {
            // Like Dragon Rage
            var effect = new FixedDamageEffect { Damage = 40 };
            Assert.That(effect.Damage, Is.EqualTo(40));
        }

        [Test]
        public void FixedDamageEffect_ZeroDamage()
        {
            var effect = new FixedDamageEffect { Damage = 0 };
            Assert.That(effect.Damage, Is.EqualTo(0));
        }

        [Test]
        public void FixedDamageEffect_HighDamage()
        {
            var effect = new FixedDamageEffect { Damage = 999 };
            Assert.That(effect.Damage, Is.EqualTo(999));
        }

        [Test]
        public void FixedDamageEffect_LevelBased()
        {
            // Level-based damage (like Seismic Toss)
            var effect = new FixedDamageEffect { UsesLevel = true };
            Assert.That(effect.UsesLevel, Is.True);
        }

        #endregion

        #region Effect Composition Edge Cases

        [Test]
        public void MoveWithMultipleEffects_AllDifferentTypes()
        {
            var effects = new List<IMoveEffect>
            {
                new DamageEffect(),
                new StatusEffect { Status = PersistentStatus.Burn, ChancePercent = 30 },
                new StatChangeEffect(Stat.SpAttack, 1, true, 100)
            };

            Assert.That(effects.Count, Is.EqualTo(3));
            Assert.That(effects.OfType<DamageEffect>().Count(), Is.EqualTo(1));
            Assert.That(effects.OfType<StatusEffect>().Count(), Is.EqualTo(1));
            Assert.That(effects.OfType<StatChangeEffect>().Count(), Is.EqualTo(1));
        }

        [Test]
        public void MoveWithMultipleStatChanges()
        {
            // Like Ancient Power (all stats +1 at 10%)
            var effects = new List<IMoveEffect>
            {
                new DamageEffect(),
                new StatChangeEffect(Stat.Attack, 1, true, 10),
                new StatChangeEffect(Stat.Defense, 1, true, 10),
                new StatChangeEffect(Stat.SpAttack, 1, true, 10),
                new StatChangeEffect(Stat.SpDefense, 1, true, 10),
                new StatChangeEffect(Stat.Speed, 1, true, 10)
            };

            Assert.That(effects.Count, Is.EqualTo(6));
            Assert.That(effects.OfType<StatChangeEffect>().Count(), Is.EqualTo(5));
        }

        [Test]
        public void MoveWithDrainAndRecoil_ConflictingEffects()
        {
            // Weird edge case: both drain and recoil
            var effects = new List<IMoveEffect>
            {
                new DamageEffect(),
                new DrainEffect { Percent = 50 },
                new RecoilEffect { Percent = 25 }
            };

            Assert.That(effects.Count, Is.EqualTo(3));
            Assert.That(effects.OfType<DrainEffect>().First().Percent, Is.EqualTo(50));
            Assert.That(effects.OfType<RecoilEffect>().First().Percent, Is.EqualTo(25));
        }

        [Test]
        public void MoveWithNoEffects()
        {
            var effects = new List<IMoveEffect>();
            Assert.That(effects, Is.Empty);
        }

        [Test]
        public void MoveWithOnlyDamage()
        {
            var effects = new List<IMoveEffect> { new DamageEffect() };
            Assert.That(effects.Count, Is.EqualTo(1));
            Assert.That(effects.All(e => e.EffectType == EffectType.Damage), Is.True);
        }

        [Test]
        public void MoveWithOnlyStatus()
        {
            var effects = new List<IMoveEffect>
            {
                new StatusEffect { Status = PersistentStatus.Sleep, ChancePercent = 100 }
            };

            Assert.That(effects.Count, Is.EqualTo(1));
            Assert.That(effects.All(e => e.EffectType == EffectType.Status), Is.True);
        }

        #endregion

        #region Effect Type Verification

        [Test]
        public void AllEffects_HaveCorrectEffectType()
        {
            Assert.That(new DamageEffect().EffectType, Is.EqualTo(EffectType.Damage));
            Assert.That(new StatusEffect().EffectType, Is.EqualTo(EffectType.Status));
            Assert.That(new StatChangeEffect().EffectType, Is.EqualTo(EffectType.StatChange));
            Assert.That(new RecoilEffect().EffectType, Is.EqualTo(EffectType.Recoil));
            Assert.That(new DrainEffect().EffectType, Is.EqualTo(EffectType.Drain));
            Assert.That(new HealEffect().EffectType, Is.EqualTo(EffectType.Heal));
            Assert.That(new FlinchEffect().EffectType, Is.EqualTo(EffectType.Flinch));
            Assert.That(new MultiHitEffect().EffectType, Is.EqualTo(EffectType.MultiHit));
            Assert.That(new FixedDamageEffect().EffectType, Is.EqualTo(EffectType.FixedDamage));
        }

        [Test]
        public void AllEffects_HaveDescription()
        {
            var effects = new IMoveEffect[]
            {
                new DamageEffect(),
                new StatusEffect { Status = PersistentStatus.Burn },
                new StatChangeEffect(Stat.Attack, 1, true),
                new RecoilEffect { Percent = 25 },
                new DrainEffect { Percent = 50 },
                new HealEffect { Percent = 50 },
                new FlinchEffect { ChancePercent = 30 },
                new MultiHitEffect { MinHits = 2, MaxHits = 5 },
                new FixedDamageEffect { Damage = 40 }
            };

            foreach (var effect in effects)
            {
                Assert.That(effect.Description, Is.Not.Null, $"{effect.GetType().Name} has null description");
                Assert.That(effect.Description, Is.Not.Empty, $"{effect.GetType().Name} has empty description");
            }
        }

        #endregion
    }
}

