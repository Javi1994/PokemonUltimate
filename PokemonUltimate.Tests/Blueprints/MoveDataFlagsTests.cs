using NUnit.Framework;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Blueprints
{
    /// <summary>
    /// Tests for MoveData flags and computed properties.
    /// </summary>
    [TestFixture]
    public class MoveDataFlagsTests
    {
        #region Default Values

        [Test]
        public void AllFlags_Default_To_False()
        {
            var move = new MoveData { Name = "Test" };

            Assert.That(move.MakesContact, Is.False);
            Assert.That(move.IsSoundBased, Is.False);
            Assert.That(move.NeverMisses, Is.False);
            Assert.That(move.HighCritRatio, Is.False);
            Assert.That(move.AlwaysCrits, Is.False);
            Assert.That(move.RequiresRecharge, Is.False);
            Assert.That(move.IsTwoTurn, Is.False);
            Assert.That(move.IsPunch, Is.False);
            Assert.That(move.IsBite, Is.False);
            Assert.That(move.IsPulse, Is.False);
            Assert.That(move.IsBullet, Is.False);
            Assert.That(move.IsReflectable, Is.False);
            Assert.That(move.IsSnatched, Is.False);
            Assert.That(move.IgnoresTargetStatChanges, Is.False);
            Assert.That(move.IgnoresUserStatChanges, Is.False);
            Assert.That(move.BypassesProtect, Is.False);
        }

        #endregion

        #region Contact Move Tests

        [Test]
        public void MakesContact_Can_Be_Set()
        {
            var tackle = new MoveData 
            { 
                Name = "Tackle", 
                Category = MoveCategory.Physical,
                MakesContact = true 
            };

            Assert.That(tackle.MakesContact, Is.True);
        }

        [Test]
        public void Earthquake_Physical_But_No_Contact()
        {
            var earthquake = new MoveData 
            { 
                Name = "Earthquake", 
                Category = MoveCategory.Physical,
                MakesContact = false  // Earthquake doesn't make contact
            };

            Assert.That(earthquake.IsPhysical, Is.True);
            Assert.That(earthquake.MakesContact, Is.False);
        }

        #endregion

        #region Sound Based Tests

        [Test]
        public void SoundBased_Move()
        {
            var hyperVoice = new MoveData 
            { 
                Name = "Hyper Voice", 
                IsSoundBased = true 
            };

            Assert.That(hyperVoice.IsSoundBased, Is.True);
        }

        #endregion

        #region Accuracy Tests

        [Test]
        public void NeverMisses_Move()
        {
            var swift = new MoveData 
            { 
                Name = "Swift",
                NeverMisses = true,
                Accuracy = 0  // Accuracy is irrelevant
            };

            Assert.That(swift.NeverMisses, Is.True);
        }

        [Test]
        public void AerialAce_NeverMisses()
        {
            var aerialAce = new MoveData 
            { 
                Name = "Aerial Ace",
                Type = PokemonType.Flying,
                Category = MoveCategory.Physical,
                Power = 60,
                NeverMisses = true
            };

            Assert.That(aerialAce.NeverMisses, Is.True);
        }

        #endregion

        #region Critical Hit Tests

        [Test]
        public void HighCritRatio_Move()
        {
            var slash = new MoveData 
            { 
                Name = "Slash",
                HighCritRatio = true 
            };

            Assert.That(slash.HighCritRatio, Is.True);
            Assert.That(slash.AlwaysCrits, Is.False);
        }

        [Test]
        public void AlwaysCrits_Move()
        {
            var frostBreath = new MoveData 
            { 
                Name = "Frost Breath",
                AlwaysCrits = true 
            };

            Assert.That(frostBreath.AlwaysCrits, Is.True);
        }

        [Test]
        public void StoneEdge_HighCrit()
        {
            var stoneEdge = new MoveData 
            { 
                Name = "Stone Edge",
                Type = PokemonType.Rock,
                Category = MoveCategory.Physical,
                Power = 100,
                Accuracy = 80,
                HighCritRatio = true
            };

            Assert.That(stoneEdge.HighCritRatio, Is.True);
        }

        #endregion

        #region Recharge/Two-Turn Tests

        [Test]
        public void RequiresRecharge_Move()
        {
            var hyperBeam = new MoveData 
            { 
                Name = "Hyper Beam",
                Power = 150,
                RequiresRecharge = true 
            };

            Assert.That(hyperBeam.RequiresRecharge, Is.True);
        }

        [Test]
        public void TwoTurn_Move()
        {
            var fly = new MoveData 
            { 
                Name = "Fly",
                IsTwoTurn = true 
            };

            Assert.That(fly.IsTwoTurn, Is.True);
        }

        [Test]
        public void Dig_IsTwoTurn()
        {
            var dig = new MoveData 
            { 
                Name = "Dig",
                Type = PokemonType.Ground,
                Category = MoveCategory.Physical,
                Power = 80,
                IsTwoTurn = true
            };

            Assert.That(dig.IsTwoTurn, Is.True);
        }

        #endregion

        #region Ability-Boosted Move Tests

        [Test]
        public void IsPunch_Move()
        {
            var thunderPunch = new MoveData 
            { 
                Name = "Thunder Punch",
                IsPunch = true 
            };

            Assert.That(thunderPunch.IsPunch, Is.True);
        }

        [Test]
        public void IsBite_Move()
        {
            var crunch = new MoveData 
            { 
                Name = "Crunch",
                IsBite = true 
            };

            Assert.That(crunch.IsBite, Is.True);
        }

        [Test]
        public void IsPulse_Move()
        {
            var auraSphere = new MoveData 
            { 
                Name = "Aura Sphere",
                IsPulse = true,
                NeverMisses = true  // Aura Sphere never misses
            };

            Assert.That(auraSphere.IsPulse, Is.True);
        }

        [Test]
        public void IsBullet_Move()
        {
            var shadowBall = new MoveData 
            { 
                Name = "Shadow Ball",
                IsBullet = true 
            };

            Assert.That(shadowBall.IsBullet, Is.True);
        }

        #endregion

        #region Protect/Reflect Tests

        [Test]
        public void BypassesProtect_Move()
        {
            var feint = new MoveData 
            { 
                Name = "Feint",
                BypassesProtect = true 
            };

            Assert.That(feint.BypassesProtect, Is.True);
        }

        [Test]
        public void IsReflectable_Move()
        {
            var thunderWave = new MoveData 
            { 
                Name = "Thunder Wave",
                Category = MoveCategory.Status,
                IsReflectable = true 
            };

            Assert.That(thunderWave.IsReflectable, Is.True);
        }

        [Test]
        public void IsSnatched_Move()
        {
            var swordsDance = new MoveData 
            { 
                Name = "Swords Dance",
                Category = MoveCategory.Status,
                IsSnatched = true 
            };

            Assert.That(swordsDance.IsSnatched, Is.True);
        }

        #endregion

        #region Stat Ignore Tests

        [Test]
        public void IgnoresTargetStatChanges_Move()
        {
            var chipAway = new MoveData 
            { 
                Name = "Chip Away",
                IgnoresTargetStatChanges = true 
            };

            Assert.That(chipAway.IgnoresTargetStatChanges, Is.True);
        }

        [Test]
        public void IgnoresUserStatChanges_Move()
        {
            // Some moves ignore user's negative stat changes
            var move = new MoveData 
            { 
                Name = "Test Move",
                IgnoresUserStatChanges = true 
            };

            Assert.That(move.IgnoresUserStatChanges, Is.True);
        }

        #endregion

        #region Computed Properties Tests

        [Test]
        public void IsDamaging_True_For_Physical()
        {
            var tackle = new MoveData 
            { 
                Name = "Tackle", 
                Category = MoveCategory.Physical 
            };

            Assert.That(tackle.IsDamaging, Is.True);
            Assert.That(tackle.IsPhysical, Is.True);
            Assert.That(tackle.IsSpecial, Is.False);
            Assert.That(tackle.IsStatus, Is.False);
        }

        [Test]
        public void IsDamaging_True_For_Special()
        {
            var flamethrower = new MoveData 
            { 
                Name = "Flamethrower", 
                Category = MoveCategory.Special 
            };

            Assert.That(flamethrower.IsDamaging, Is.True);
            Assert.That(flamethrower.IsSpecial, Is.True);
            Assert.That(flamethrower.IsPhysical, Is.False);
            Assert.That(flamethrower.IsStatus, Is.False);
        }

        [Test]
        public void IsDamaging_False_For_Status()
        {
            var thunderWave = new MoveData 
            { 
                Name = "Thunder Wave", 
                Category = MoveCategory.Status 
            };

            Assert.That(thunderWave.IsDamaging, Is.False);
            Assert.That(thunderWave.IsStatus, Is.True);
            Assert.That(thunderWave.IsPhysical, Is.False);
            Assert.That(thunderWave.IsSpecial, Is.False);
        }

        #endregion

        #region Realistic Move Examples

        [Test]
        public void ThunderPunch_Complete_Definition()
        {
            var thunderPunch = new MoveData
            {
                Name = "Thunder Punch",
                Type = PokemonType.Electric,
                Category = MoveCategory.Physical,
                Power = 75,
                Accuracy = 100,
                MaxPP = 15,
                MakesContact = true,
                IsPunch = true
            };

            Assert.That(thunderPunch.MakesContact, Is.True);
            Assert.That(thunderPunch.IsPunch, Is.True);
            Assert.That(thunderPunch.IsPhysical, Is.True);
            Assert.That(thunderPunch.IsDamaging, Is.True);
        }

        [Test]
        public void HyperBeam_Complete_Definition()
        {
            var hyperBeam = new MoveData
            {
                Name = "Hyper Beam",
                Type = PokemonType.Normal,
                Category = MoveCategory.Special,
                Power = 150,
                Accuracy = 90,
                MaxPP = 5,
                RequiresRecharge = true
            };

            Assert.That(hyperBeam.RequiresRecharge, Is.True);
            Assert.That(hyperBeam.IsSpecial, Is.True);
            Assert.That(hyperBeam.MakesContact, Is.False);
        }

        [Test]
        public void ShadowForce_Complete_Definition()
        {
            var shadowForce = new MoveData
            {
                Name = "Shadow Force",
                Type = PokemonType.Ghost,
                Category = MoveCategory.Physical,
                Power = 120,
                Accuracy = 100,
                MaxPP = 5,
                MakesContact = true,
                IsTwoTurn = true,
                BypassesProtect = true
            };

            Assert.That(shadowForce.IsTwoTurn, Is.True);
            Assert.That(shadowForce.BypassesProtect, Is.True);
            Assert.That(shadowForce.MakesContact, Is.True);
        }

        [Test]
        public void Boomburst_Complete_Definition()
        {
            var boomburst = new MoveData
            {
                Name = "Boomburst",
                Type = PokemonType.Normal,
                Category = MoveCategory.Special,
                Power = 140,
                Accuracy = 100,
                MaxPP = 10,
                IsSoundBased = true,
                TargetScope = TargetScope.AllOthers
            };

            Assert.That(boomburst.IsSoundBased, Is.True);
            Assert.That(boomburst.TargetScope, Is.EqualTo(TargetScope.AllOthers));
        }

        [Test]
        public void SacredSword_Complete_Definition()
        {
            var sacredSword = new MoveData
            {
                Name = "Sacred Sword",
                Type = PokemonType.Fighting,
                Category = MoveCategory.Physical,
                Power = 90,
                Accuracy = 100,
                MaxPP = 15,
                MakesContact = true,
                IgnoresTargetStatChanges = true
            };

            Assert.That(sacredSword.IgnoresTargetStatChanges, Is.True);
            Assert.That(sacredSword.MakesContact, Is.True);
        }

        #endregion

        #region Multiple Flags Combination

        [Test]
        public void Move_Can_Have_Multiple_Flags()
        {
            var move = new MoveData
            {
                Name = "Multi-Flag Move",
                MakesContact = true,
                IsPunch = true,
                HighCritRatio = true
            };

            Assert.That(move.MakesContact, Is.True);
            Assert.That(move.IsPunch, Is.True);
            Assert.That(move.HighCritRatio, Is.True);
        }

        [Test]
        public void FocusPunch_Multiple_Flags()
        {
            var focusPunch = new MoveData
            {
                Name = "Focus Punch",
                Type = PokemonType.Fighting,
                Category = MoveCategory.Physical,
                Power = 150,
                Accuracy = 100,
                MaxPP = 20,
                Priority = -3,
                MakesContact = true,
                IsPunch = true
            };

            Assert.That(focusPunch.MakesContact, Is.True);
            Assert.That(focusPunch.IsPunch, Is.True);
            Assert.That(focusPunch.Priority, Is.EqualTo(-3));
        }

        #endregion
    }
}

