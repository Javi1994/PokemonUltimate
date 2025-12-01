using NUnit.Framework;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Effects
{
    [TestFixture]
    public class VolatileStatusEffectTests
    {
        [Test]
        public void Constructor_Default_SetsDefaultValues()
        {
            var effect = new VolatileStatusEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.VolatileStatus));
            Assert.That(effect.ChancePercent, Is.EqualTo(100));
            Assert.That(effect.TargetSelf, Is.False);
            Assert.That(effect.Duration, Is.EqualTo(0));
        }
        
        [Test]
        public void Constructor_WithParameters_SetsValues()
        {
            var effect = new VolatileStatusEffect(VolatileStatus.Confusion, 50);
            
            Assert.That(effect.Status, Is.EqualTo(VolatileStatus.Confusion));
            Assert.That(effect.ChancePercent, Is.EqualTo(50));
        }
        
        [Test]
        public void Description_Confusion_DescribesEffect()
        {
            var effect = new VolatileStatusEffect(VolatileStatus.Confusion, 100);
            
            Assert.That(effect.Description, Does.Contain("Confusion"));
            Assert.That(effect.Description, Does.Contain("100%"));
        }
    }
    
    [TestFixture]
    public class ProtectionEffectTests
    {
        [Test]
        public void Constructor_Default_SetsFullProtection()
        {
            var effect = new ProtectionEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.Protection));
            Assert.That(effect.Type, Is.EqualTo(ProtectionType.Full));
            Assert.That(effect.SingleTargetOnly, Is.False);
        }
        
        [Test]
        public void Constructor_WithType_SetsType()
        {
            var effect = new ProtectionEffect(ProtectionType.WideGuard);
            
            Assert.That(effect.Type, Is.EqualTo(ProtectionType.WideGuard));
        }
        
        [Test]
        public void Description_Full_DescribesProtection()
        {
            var effect = new ProtectionEffect(ProtectionType.Full);
            
            Assert.That(effect.Description, Does.Contain("Protects"));
        }
        
        [Test]
        public void Description_WideGuard_DescribesSpreadMoves()
        {
            var effect = new ProtectionEffect(ProtectionType.WideGuard);
            
            Assert.That(effect.Description, Does.Contain("spread"));
        }
        
        [Test]
        public void ContactPenalty_KingsShield_LowersAttack()
        {
            var effect = new ProtectionEffect(ProtectionType.KingsShield)
            {
                ContactEffect = ContactPenalty.StatDrop,
                ContactStatDrop = Stat.Attack,
                ContactStatStages = -2
            };
            
            Assert.That(effect.ContactEffect, Is.EqualTo(ContactPenalty.StatDrop));
            Assert.That(effect.ContactStatDrop, Is.EqualTo(Stat.Attack));
            Assert.That(effect.ContactStatStages, Is.EqualTo(-2));
        }
    }
    
    [TestFixture]
    public class ChargingEffectTests
    {
        [Test]
        public void Constructor_Default_SetsDefaultValues()
        {
            var effect = new ChargingEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.Charging));
            Assert.That(effect.SemiInvulnerable, Is.False);
            Assert.That(effect.IsRechargeMove, Is.False);
        }
        
        [Test]
        public void SemiInvulnerable_Fly_IsFlying()
        {
            var effect = new ChargingEffect
            {
                SemiInvulnerable = true,
                InvulnerableState = SemiInvulnerableState.Flying,
                ChargeMessage = "{0} flew up high!"
            };
            
            Assert.That(effect.SemiInvulnerable, Is.True);
            Assert.That(effect.InvulnerableState, Is.EqualTo(SemiInvulnerableState.Flying));
        }
        
        [Test]
        public void SkipChargeInWeather_SolarBeam_SkipsInSun()
        {
            var effect = new ChargingEffect
            {
                SkipChargeInWeather = Weather.Sun
            };
            
            Assert.That(effect.SkipChargeInWeather, Is.EqualTo(Weather.Sun));
        }
        
        [Test]
        public void Description_RechargeMove_DescribesRecharge()
        {
            var effect = new ChargingEffect { IsRechargeMove = true };
            
            Assert.That(effect.Description, Does.Contain("recharge"));
        }
    }
    
    [TestFixture]
    public class ForceSwitchEffectTests
    {
        [Test]
        public void Constructor_Default_SetsDefaultValues()
        {
            var effect = new ForceSwitchEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.ForceSwitch));
            Assert.That(effect.DealsDamage, Is.False);
            Assert.That(effect.RandomReplacement, Is.True);
        }
        
        [Test]
        public void Constructor_WithDamage_SetsDealsDamage()
        {
            var effect = new ForceSwitchEffect(dealsDamage: true);
            
            Assert.That(effect.DealsDamage, Is.True);
        }
        
        [Test]
        public void Description_WithDamage_DescribesDamageAndSwitch()
        {
            var effect = new ForceSwitchEffect(dealsDamage: true);
            
            Assert.That(effect.Description, Does.Contain("damage"));
            Assert.That(effect.Description, Does.Contain("switch"));
        }
    }
    
    [TestFixture]
    public class BindingEffectTests
    {
        [Test]
        public void Constructor_Default_SetsStandardValues()
        {
            var effect = new BindingEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.Binding));
            Assert.That(effect.MinTurns, Is.EqualTo(4));
            Assert.That(effect.MaxTurns, Is.EqualTo(5));
            Assert.That(effect.DamagePerTurn, Is.EqualTo(0.125f));
            Assert.That(effect.PreventsSwitch, Is.True);
        }
        
        [Test]
        public void Constructor_WithBindType_SetsType()
        {
            var effect = new BindingEffect("wrapped", 0.125f);
            
            Assert.That(effect.BindType, Is.EqualTo("wrapped"));
        }
        
        [Test]
        public void EnhancedDamage_BindingBand_IsHigher()
        {
            var effect = new BindingEffect();
            
            Assert.That(effect.EnhancedDamagePerTurn, Is.GreaterThan(effect.DamagePerTurn));
        }
    }
    
    [TestFixture]
    public class SwitchAfterAttackEffectTests
    {
        [Test]
        public void Constructor_Default_DealsDamage()
        {
            var effect = new SwitchAfterAttackEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.SwitchAfterAttack));
            Assert.That(effect.DealsDamage, Is.True);
            Assert.That(effect.MandatorySwitch, Is.True);
        }
        
        [Test]
        public void Constructor_WithoutDamage_OnlySwitches()
        {
            var effect = new SwitchAfterAttackEffect(dealsDamage: false);
            
            Assert.That(effect.DealsDamage, Is.False);
        }
        
        [Test]
        public void StatChanges_PartingShot_LowersStats()
        {
            var effect = new SwitchAfterAttackEffect(dealsDamage: false)
            {
                StatChanges = new[]
                {
                    new StatChangeEffect(Stat.Attack, -1),
                    new StatChangeEffect(Stat.SpAttack, -1)
                }
            };
            
            Assert.That(effect.StatChanges.Length, Is.EqualTo(2));
        }
    }
    
    [TestFixture]
    public class FieldConditionEffectTests
    {
        [Test]
        public void Constructor_Default_SetsDefaultValues()
        {
            var effect = new FieldConditionEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.FieldCondition));
            Assert.That(effect.RemovesCondition, Is.False);
        }
        
        [Test]
        public void Weather_RainDance_SetsRain()
        {
            var effect = new FieldConditionEffect
            {
                ConditionType = FieldConditionType.Weather,
                WeatherToSet = Weather.Rain
            };
            
            Assert.That(effect.WeatherToSet, Is.EqualTo(Weather.Rain));
        }
        
        [Test]
        public void Hazard_StealthRock_SetsHazard()
        {
            var effect = new FieldConditionEffect
            {
                ConditionType = FieldConditionType.Hazard,
                HazardToSet = HazardType.StealthRock,
                TargetsUserSide = false
            };
            
            Assert.That(effect.HazardToSet, Is.EqualTo(HazardType.StealthRock));
            Assert.That(effect.TargetsUserSide, Is.False);
        }
        
        [Test]
        public void Description_Weather_DescribesWeather()
        {
            var effect = new FieldConditionEffect
            {
                ConditionType = FieldConditionType.Weather,
                WeatherToSet = Weather.Sun
            };
            
            Assert.That(effect.Description, Does.Contain("Sun"));
        }
        
        [Test]
        public void RemovesCondition_Defog_RemovesHazards()
        {
            var effect = new FieldConditionEffect
            {
                RemovesCondition = true,
                ConditionsToRemove = new[] { FieldConditionType.Hazard }
            };
            
            Assert.That(effect.RemovesCondition, Is.True);
            Assert.That(effect.Description, Does.Contain("Removes"));
        }
    }
    
    [TestFixture]
    public class SelfDestructEffectTests
    {
        [Test]
        public void Constructor_Default_IsExplosion()
        {
            var effect = new SelfDestructEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.SelfDestruct));
            Assert.That(effect.Type, Is.EqualTo(SelfDestructType.Explosion));
            Assert.That(effect.DealsDamage, Is.True);
        }
        
        [Test]
        public void Constructor_Memento_DoesNotDealDamage()
        {
            var effect = new SelfDestructEffect(SelfDestructType.Memento);
            
            Assert.That(effect.DealsDamage, Is.False);
        }
        
        [Test]
        public void Constructor_HealingWish_HealsReplacement()
        {
            var effect = new SelfDestructEffect(SelfDestructType.HealingWish);
            
            Assert.That(effect.HealsReplacement, Is.True);
            Assert.That(effect.DealsDamage, Is.False);
        }
        
        [Test]
        public void Constructor_LunarDance_RestoresPP()
        {
            var effect = new SelfDestructEffect(SelfDestructType.LunarDance);
            
            Assert.That(effect.RestoresPP, Is.True);
            Assert.That(effect.HealsReplacement, Is.True);
        }
        
        [Test]
        public void Constructor_FinalGambit_DamageEqualsHP()
        {
            var effect = new SelfDestructEffect(SelfDestructType.FinalGambit);
            
            Assert.That(effect.DamageEqualsHP, Is.True);
            Assert.That(effect.DealsDamage, Is.True);
        }
    }
    
    [TestFixture]
    public class RevengeEffectTests
    {
        [Test]
        public void Constructor_Default_SetsDefaultValues()
        {
            var effect = new RevengeEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.Revenge));
            Assert.That(effect.DamageMultiplier, Is.EqualTo(2.0f));
            Assert.That(effect.RequiresHit, Is.True);
        }
        
        [Test]
        public void Constructor_Counter_CountersPhysical()
        {
            var effect = new RevengeEffect(MoveCategory.Physical, 2.0f);
            
            Assert.That(effect.CountersCategory, Is.EqualTo(MoveCategory.Physical));
        }
        
        [Test]
        public void Constructor_MirrorCoat_CountersSpecial()
        {
            var effect = new RevengeEffect(MoveCategory.Special, 2.0f);
            
            Assert.That(effect.CountersCategory, Is.EqualTo(MoveCategory.Special));
        }
        
        [Test]
        public void Constructor_MetalBurst_CountersBothWithLowerMultiplier()
        {
            var effect = new RevengeEffect(null, 1.5f);
            
            Assert.That(effect.CountersCategory, Is.Null);
            Assert.That(effect.DamageMultiplier, Is.EqualTo(1.5f));
        }
        
        [Test]
        public void Description_Physical_DescribesPhysical()
        {
            var effect = new RevengeEffect(MoveCategory.Physical, 2.0f);
            
            Assert.That(effect.Description, Does.Contain("Physical"));
        }
    }
    
    [TestFixture]
    public class MoveRestrictionEffectTests
    {
        [Test]
        public void Constructor_Default_SetsDefaultDuration()
        {
            var effect = new MoveRestrictionEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.MoveRestriction));
            Assert.That(effect.Duration, Is.EqualTo(3));
        }
        
        [Test]
        public void Constructor_Encore_ForcesRepeat()
        {
            var effect = new MoveRestrictionEffect(MoveRestrictionType.Encore, 3);
            
            Assert.That(effect.RestrictionType, Is.EqualTo(MoveRestrictionType.Encore));
        }
        
        [Test]
        public void Constructor_Taunt_BlocksStatus()
        {
            var effect = new MoveRestrictionEffect(MoveRestrictionType.Taunt, 3);
            
            Assert.That(effect.Description, Does.Contain("damaging"));
        }
        
        [Test]
        public void Constructor_Imprison_AffectsSelf()
        {
            var effect = new MoveRestrictionEffect(MoveRestrictionType.Imprison)
            {
                TargetSelf = true
            };
            
            Assert.That(effect.TargetSelf, Is.True);
        }
    }
    
    [TestFixture]
    public class DelayedDamageEffectTests
    {
        [Test]
        public void Constructor_Default_TwoTurnDelay()
        {
            var effect = new DelayedDamageEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.DelayedDamage));
            Assert.That(effect.TurnsDelay, Is.EqualTo(2));
            Assert.That(effect.IsHealing, Is.False);
        }
        
        [Test]
        public void Constructor_FutureSight_TargetsSlot()
        {
            var effect = new DelayedDamageEffect(2, false)
            {
                DamageType = PokemonType.Psychic,
                UsesCasterStats = true,
                BypassesProtect = true
            };
            
            Assert.That(effect.TargetsSlot, Is.True);
            Assert.That(effect.DamageType, Is.EqualTo(PokemonType.Psychic));
        }
        
        [Test]
        public void Constructor_Wish_Heals()
        {
            var effect = new DelayedDamageEffect(1, true)
            {
                HealFraction = 0.5f
            };
            
            Assert.That(effect.IsHealing, Is.True);
            Assert.That(effect.TurnsDelay, Is.EqualTo(1));
            Assert.That(effect.HealFraction, Is.EqualTo(0.5f));
        }
        
        [Test]
        public void Description_Damage_DescribesDamage()
        {
            var effect = new DelayedDamageEffect(2, false);
            
            Assert.That(effect.Description, Does.Contain("Damages"));
        }
        
        [Test]
        public void Description_Healing_DescribesHealing()
        {
            var effect = new DelayedDamageEffect(1, true);
            
            Assert.That(effect.Description, Does.Contain("Heals"));
        }
    }
    
    [TestFixture]
    public class PriorityModifierEffectTests
    {
        [Test]
        public void Constructor_Default_AlwaysCondition()
        {
            var effect = new PriorityModifierEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.PriorityModifier));
            Assert.That(effect.Condition, Is.EqualTo(PriorityCondition.Always));
            Assert.That(effect.PriorityChange, Is.EqualTo(1));
        }
        
        [Test]
        public void TerrainBased_GrassyGlide_RequiresTerrain()
        {
            var effect = new PriorityModifierEffect
            {
                Condition = PriorityCondition.TerrainBased,
                RequiredTerrain = Terrain.Grassy,
                PriorityChange = 1
            };
            
            Assert.That(effect.RequiredTerrain, Is.EqualTo(Terrain.Grassy));
        }
        
        [Test]
        public void FullHP_GaleWings_RequiresFullHP()
        {
            var effect = new PriorityModifierEffect
            {
                Condition = PriorityCondition.FullHP,
                HPThreshold = 1.0f,
                PriorityChange = 1
            };
            
            Assert.That(effect.HPThreshold, Is.EqualTo(1.0f));
        }
        
        [Test]
        public void Description_Always_ShowsPriority()
        {
            var effect = new PriorityModifierEffect { PriorityChange = 2 };
            
            Assert.That(effect.Description, Does.Contain("+2"));
        }
    }
}

