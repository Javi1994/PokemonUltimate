using NUnit.Framework;
using PokemonUltimate.Content.Catalogs.Status;
using PokemonUltimate.Content.Builders;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Blueprints
{
    /// <summary>
    /// Edge case tests for StatusEffectData.
    /// </summary>
    [TestFixture]
    public class StatusEffectDataEdgeCasesTests
    {
        #region Duration Edge Cases

        [Test]
        public void GetRandomDuration_MinEqualsMax_ReturnsExactValue()
        {
            var status = Status.Define("Test").LastsTurns(5, 5).Build();

            // Even though min==max, HasRandomDuration should be false
            Assert.That(status.HasRandomDuration, Is.False);
            Assert.That(status.GetRandomDuration(), Is.EqualTo(5));
        }

        [Test]
        public void GetRandomDuration_ZeroMax_IsIndefinite()
        {
            var status = Status.Define("Test").LastsTurns(0, 0).Build();

            Assert.That(status.IsIndefinite, Is.True);
        }

        [Test]
        public void GetRandomDuration_NullRandom_UsesNewInstance()
        {
            var status = Status.Define("Test").LastsTurns(1, 100).Build();

            // Should not throw
            int duration = status.GetRandomDuration(null);
            Assert.That(duration, Is.InRange(1, 100));
        }

        #endregion

        #region Damage Edge Cases

        [Test]
        public void GetEscalatingDamage_TurnZero_ReturnsBaseDamageTimesStart()
        {
            // Turn 0 shouldn't happen in practice, but test boundary
            var status = Status.Define("Toxic")
                .DealsEscalatingDamage(0.0625f, 1)
                .Build();

            // Turn 0 with start=1: (1 + 0 - 1) * base = 0 * base = 0
            // Actually the formula is: base * (start + turn - 1)
            // So turn 0: 0.0625 * (1 + 0 - 1) = 0
            float damage = status.GetEscalatingDamage(0);
            Assert.That(damage, Is.EqualTo(0f).Within(0.001f));
        }

        [Test]
        public void GetEscalatingDamage_HighTurnCount_CalculatesCorrectly()
        {
            var status = Status.Define("Toxic")
                .DealsEscalatingDamage(0.0625f, 1)
                .Build();

            // Turn 15: 0.0625 * 15 = 0.9375 (almost full HP)
            float damage = status.GetEscalatingDamage(15);
            Assert.That(damage, Is.EqualTo(0.9375f).Within(0.001f));
        }

        [Test]
        public void HealsPerTurn_NegativeDamage_RepresentsHealing()
        {
            var status = Status.Define("Healing")
                .HealsPerTurn(0.0625f)
                .Build();

            Assert.That(status.EndOfTurnDamage, Is.LessThan(0));
            Assert.That(status.EndOfTurnDamage, Is.EqualTo(-0.0625f).Within(0.001f));
        }

        #endregion

        #region Type Immunity Edge Cases

        [Test]
        public void IsTypeImmune_EmptyImmuneTypes_AlwaysReturnsFalse()
        {
            var status = Status.Define("Test").Build();

            Assert.That(status.IsTypeImmune(PokemonType.Normal), Is.False);
            Assert.That(status.IsTypeImmune(PokemonType.Fire), Is.False);
        }

        [Test]
        public void IsTypeImmune_MultipleTypes_ChecksAll()
        {
            var status = Status.Define("Test")
                .ImmuneTypes(PokemonType.Fire, PokemonType.Ice, PokemonType.Electric)
                .Build();

            Assert.That(status.ImmuneTypes.Length, Is.EqualTo(3));
            Assert.That(status.IsTypeImmune(PokemonType.Fire), Is.True);
            Assert.That(status.IsTypeImmune(PokemonType.Ice), Is.True);
            Assert.That(status.IsTypeImmune(PokemonType.Electric), Is.True);
            Assert.That(status.IsTypeImmune(PokemonType.Water), Is.False);
        }

        [Test]
        public void CanBeCuredByMoveType_EmptyCureTypes_AlwaysReturnsFalse()
        {
            var status = Status.Define("Test").Build();

            Assert.That(status.CanBeCuredByMoveType(PokemonType.Fire), Is.False);
        }

        #endregion

        #region Stat Modifier Edge Cases

        [Test]
        public void SpeedMultiplier_Default_IsOne()
        {
            var status = Status.Define("Test").Build();

            Assert.That(status.SpeedMultiplier, Is.EqualTo(1.0f));
        }

        [Test]
        public void AttackMultiplier_Default_IsOne()
        {
            var status = Status.Define("Test").Build();

            Assert.That(status.AttackMultiplier, Is.EqualTo(1.0f));
        }

        [Test]
        public void AttackModifier_NotPhysicalOnly_AppliesToAll()
        {
            var status = Status.Define("Test")
                .AttackModifier(0.75f, false)
                .Build();

            Assert.That(status.AttackMultiplier, Is.EqualTo(0.75f));
            Assert.That(status.AttackModifierIsPhysicalOnly, Is.False);
        }

        #endregion

        #region Self Hit Edge Cases

        [Test]
        public void SelfHitChance_Zero_NoSelfDamage()
        {
            var status = Status.Define("Test").Build();

            Assert.That(status.SelfHitChance, Is.EqualTo(0f));
            Assert.That(status.SelfHitPower, Is.EqualTo(0));
        }

        [Test]
        public void SelfHitChance_HundredPercent_AlwaysHitsSelf()
        {
            var status = Status.Define("Extreme Confusion")
                .SelfHitChance(1.0f, 100)
                .Build();

            Assert.That(status.SelfHitChance, Is.EqualTo(1.0f));
            Assert.That(status.SelfHitPower, Is.EqualTo(100));
        }

        #endregion

        #region Persistent vs Volatile Edge Cases

        [Test]
        public void Persistent_ThenVolatile_BecomesVolatileOnly()
        {
            var status = Status.Define("Test")
                .Persistent(PersistentStatus.Burn)
                .Volatile(VolatileStatus.Confusion)
                .Build();

            // Last one wins
            Assert.That(status.IsVolatile, Is.True);
            Assert.That(status.IsPersistent, Is.False);
        }

        [Test]
        public void Volatile_ThenPersistent_BecomesPersistentOnly()
        {
            var status = Status.Define("Test")
                .Volatile(VolatileStatus.Confusion)
                .Persistent(PersistentStatus.Burn)
                .Build();

            Assert.That(status.IsPersistent, Is.True);
            Assert.That(status.IsVolatile, Is.False);
        }

        [Test]
        public void NoStatusSet_BothAreFalse()
        {
            var status = Status.Define("Test").Build();

            Assert.That(status.IsPersistent, Is.False);
            Assert.That(status.IsVolatile, Is.False);
        }

        #endregion

        #region Recovery Edge Cases

        [Test]
        public void RecoveryChance_Zero_NeverRecovers()
        {
            var status = Status.Define("Test")
                .PreventsAction()
                .Build();

            Assert.That(status.RecoveryChancePerTurn, Is.EqualTo(0f));
        }

        [Test]
        public void RecoveryChance_OneHundredPercent_AlwaysRecovers()
        {
            var status = Status.Define("Test")
                .RecoveryChance(1.0f)
                .Build();

            Assert.That(status.RecoveryChancePerTurn, Is.EqualTo(1.0f));
        }

        #endregion

        #region Catalog Lookup Edge Cases

        [Test]
        public void GetByStatus_CombinedVolatileFlags_ReturnsNull()
        {
            // Combining flags shouldn't match
            var combined = VolatileStatus.Confusion | VolatileStatus.Attract;
            
            // Our lookup doesn't handle combined flags
            Assert.That(StatusCatalog.GetByStatus(combined), Is.Null);
        }

        [Test]
        public void GetByName_NonExistent_ReturnsNull()
        {
            Assert.That(StatusCatalog.GetByName("NonExistentStatus"), Is.Null);
        }

        [Test]
        public void GetByName_CorrectName_ReturnsStatus()
        {
            Assert.That(StatusCatalog.GetByName("Burn"), Is.EqualTo(StatusCatalog.Burn));
        }

        #endregion

        #region Special Flags Edge Cases

        [Test]
        public void ForcesMove_WithEncoreLikeBehavior()
        {
            var status = Status.Define("Test Encore")
                .ForcesMove()
                .LastsTurns(3)
                .Build();

            Assert.That(status.ForcesMove, Is.True);
        }

        [Test]
        public void PreventsSwitching_ForTrappingMoves()
        {
            var status = Status.Define("Trapped")
                .PreventsSwitching()
                .Build();

            Assert.That(status.PreventsSwitching, Is.True);
        }

        [Test]
        public void DrainsToOpponent_CorrectlySet()
        {
            var status = Status.Define("Drain")
                .DrainsToOpponent(0.125f)
                .Build();

            Assert.That(status.DrainsToOpponent, Is.True);
            Assert.That(status.EndOfTurnDamage, Is.EqualTo(0.125f).Within(0.001f));
        }

        #endregion
    }
}

