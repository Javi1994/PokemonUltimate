using NUnit.Framework;
using PokemonUltimate.Content.Catalogs.Status;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Content.Builders;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Blueprints
{
    /// <summary>
    /// Tests for StatusEffectData and StatusEffectBuilder.
    /// </summary>
    [TestFixture]
    public class StatusEffectDataTests
    {
        #region Builder Tests

        [Test]
        public void Define_WithName_SetsNameAndId()
        {
            var status = Status.Define("Test Status")
                .Build();

            Assert.That(status.Name, Is.EqualTo("Test Status"));
            Assert.That(status.Id, Is.EqualTo("test-status"));
        }

        [Test]
        public void Persistent_SetsPersistentStatus()
        {
            var status = Status.Define("Burn")
                .Persistent(PersistentStatus.Burn)
                .Build();

            Assert.That(status.PersistentStatus, Is.EqualTo(PersistentStatus.Burn));
            Assert.That(status.IsPersistent, Is.True);
            Assert.That(status.IsVolatile, Is.False);
        }

        [Test]
        public void Volatile_SetsVolatileStatus()
        {
            var status = Status.Define("Confusion")
                .Volatile(VolatileStatus.Confusion)
                .Build();

            Assert.That(status.VolatileStatus, Is.EqualTo(VolatileStatus.Confusion));
            Assert.That(status.IsVolatile, Is.True);
            Assert.That(status.IsPersistent, Is.False);
        }

        [Test]
        public void Indefinite_SetsZeroDuration()
        {
            var status = Status.Define("Test")
                .Indefinite()
                .Build();

            Assert.That(status.MinTurns, Is.EqualTo(0));
            Assert.That(status.MaxTurns, Is.EqualTo(0));
            Assert.That(status.IsIndefinite, Is.True);
        }

        [Test]
        public void LastsTurns_FixedDuration_SetsBothMinAndMax()
        {
            var status = Status.Define("Test")
                .LastsTurns(3)
                .Build();

            Assert.That(status.MinTurns, Is.EqualTo(3));
            Assert.That(status.MaxTurns, Is.EqualTo(3));
            Assert.That(status.HasRandomDuration, Is.False);
        }

        [Test]
        public void LastsTurns_RandomDuration_SetsMinAndMax()
        {
            var status = Status.Define("Test")
                .LastsTurns(1, 3)
                .Build();

            Assert.That(status.MinTurns, Is.EqualTo(1));
            Assert.That(status.MaxTurns, Is.EqualTo(3));
            Assert.That(status.HasRandomDuration, Is.True);
        }

        [Test]
        public void DealsDamagePerTurn_SetsDamageValue()
        {
            var status = Status.Define("Test")
                .DealsDamagePerTurn(0.0625f)
                .Build();

            Assert.That(status.EndOfTurnDamage, Is.EqualTo(0.0625f).Within(0.0001f));
            Assert.That(status.DamageEscalates, Is.False);
        }

        [Test]
        public void DealsEscalatingDamage_SetsDamageAndEscalation()
        {
            var status = Status.Define("Toxic")
                .DealsEscalatingDamage(0.0625f, 1)
                .Build();

            Assert.That(status.EndOfTurnDamage, Is.EqualTo(0.0625f).Within(0.0001f));
            Assert.That(status.DamageEscalates, Is.True);
            Assert.That(status.EscalatingDamageStart, Is.EqualTo(1));
        }

        [Test]
        public void FailsToMove_SetsMoveFailChance()
        {
            var status = Status.Define("Paralysis")
                .FailsToMove(0.25f)
                .Build();

            Assert.That(status.MoveFailChance, Is.EqualTo(0.25f).Within(0.001f));
            Assert.That(status.PreventsAction, Is.False);
        }

        [Test]
        public void PreventsAction_SetsMoveFailChanceToOne()
        {
            var status = Status.Define("Sleep")
                .PreventsAction()
                .Build();

            Assert.That(status.MoveFailChance, Is.EqualTo(1.0f));
            Assert.That(status.PreventsAction, Is.True);
        }

        [Test]
        public void HalvesSpeed_SetsSpeedMultiplier()
        {
            var status = Status.Define("Paralysis")
                .HalvesSpeed()
                .Build();

            Assert.That(status.SpeedMultiplier, Is.EqualTo(0.5f));
        }

        [Test]
        public void HalvesPhysicalAttack_SetsAttackModifier()
        {
            var status = Status.Define("Burn")
                .HalvesPhysicalAttack()
                .Build();

            Assert.That(status.AttackMultiplier, Is.EqualTo(0.5f));
            Assert.That(status.AttackModifierIsPhysicalOnly, Is.True);
        }

        [Test]
        public void SelfHitChance_SetsSelfDamageValues()
        {
            var status = Status.Define("Confusion")
                .SelfHitChance(0.33f, 40)
                .Build();

            Assert.That(status.SelfHitChance, Is.EqualTo(0.33f).Within(0.001f));
            Assert.That(status.SelfHitPower, Is.EqualTo(40));
        }

        [Test]
        public void ImmuneTypes_SetsImmuneTypeArray()
        {
            var status = Status.Define("Poison")
                .ImmuneTypes(PokemonType.Poison, PokemonType.Steel)
                .Build();

            Assert.That(status.ImmuneTypes, Contains.Item(PokemonType.Poison));
            Assert.That(status.ImmuneTypes, Contains.Item(PokemonType.Steel));
        }

        [Test]
        public void RestrictsMoveCategory_SetsRestriction()
        {
            var status = Status.Define("Taunt")
                .RestrictsMoveCategory(MoveCategory.Status)
                .Build();

            Assert.That(status.RestrictedMoveCategory, Is.EqualTo(MoveCategory.Status));
        }

        #endregion

        #region Helper Method Tests

        [Test]
        public void IsTypeImmune_ImmuneTyoe_ReturnsTrue()
        {
            var status = Status.Define("Test")
                .ImmuneTypes(PokemonType.Fire)
                .Build();

            Assert.That(status.IsTypeImmune(PokemonType.Fire), Is.True);
            Assert.That(status.IsTypeImmune(PokemonType.Water), Is.False);
        }

        [Test]
        public void CanBeCuredByMoveType_CuringType_ReturnsTrue()
        {
            var status = Status.Define("Freeze")
                .CuredByMoveTypes(PokemonType.Fire)
                .Build();

            Assert.That(status.CanBeCuredByMoveType(PokemonType.Fire), Is.True);
            Assert.That(status.CanBeCuredByMoveType(PokemonType.Water), Is.False);
        }

        [Test]
        public void GetRandomDuration_IndefiniteStatus_ReturnsZero()
        {
            var status = Status.Define("Test").Indefinite().Build();

            Assert.That(status.GetRandomDuration(), Is.EqualTo(0));
        }

        [Test]
        public void GetRandomDuration_FixedDuration_ReturnsFixedValue()
        {
            var status = Status.Define("Test").LastsTurns(3).Build();

            Assert.That(status.GetRandomDuration(), Is.EqualTo(3));
        }

        [Test]
        public void GetRandomDuration_RandomDuration_ReturnsWithinRange()
        {
            var status = Status.Define("Test").LastsTurns(1, 5).Build();
            var random = new System.Random(42);

            for (int i = 0; i < 100; i++)
            {
                int duration = status.GetRandomDuration(random);
                Assert.That(duration, Is.InRange(1, 5));
            }
        }

        [Test]
        public void GetEscalatingDamage_NotEscalating_ReturnsBaseDamage()
        {
            var status = Status.Define("Test")
                .DealsDamagePerTurn(0.125f)
                .Build();

            Assert.That(status.GetEscalatingDamage(1), Is.EqualTo(0.125f).Within(0.001f));
            Assert.That(status.GetEscalatingDamage(5), Is.EqualTo(0.125f).Within(0.001f));
        }

        [Test]
        public void GetEscalatingDamage_Escalating_ReturnsScaledDamage()
        {
            var status = Status.Define("Toxic")
                .DealsEscalatingDamage(0.0625f, 1)
                .Build();

            // Turn 1: 1/16 * 1 = 0.0625
            Assert.That(status.GetEscalatingDamage(1), Is.EqualTo(0.0625f).Within(0.001f));
            // Turn 2: 1/16 * 2 = 0.125
            Assert.That(status.GetEscalatingDamage(2), Is.EqualTo(0.125f).Within(0.001f));
            // Turn 3: 1/16 * 3 = 0.1875
            Assert.That(status.GetEscalatingDamage(3), Is.EqualTo(0.1875f).Within(0.001f));
        }

        #endregion

        #region Catalog Tests

        [Test]
        public void Catalog_Burn_HasCorrectProperties()
        {
            var burn = StatusCatalog.Burn;

            Assert.That(burn.PersistentStatus, Is.EqualTo(PersistentStatus.Burn));
            Assert.That(burn.EndOfTurnDamage, Is.EqualTo(1f / 16f).Within(0.001f));
            Assert.That(burn.AttackMultiplier, Is.EqualTo(0.5f));
            Assert.That(burn.AttackModifierIsPhysicalOnly, Is.True);
            Assert.That(burn.IsTypeImmune(PokemonType.Fire), Is.True);
        }

        [Test]
        public void Catalog_Paralysis_HasCorrectProperties()
        {
            var para = StatusCatalog.Paralysis;

            Assert.That(para.PersistentStatus, Is.EqualTo(PersistentStatus.Paralysis));
            Assert.That(para.MoveFailChance, Is.EqualTo(0.25f).Within(0.001f));
            Assert.That(para.SpeedMultiplier, Is.EqualTo(0.5f));
            Assert.That(para.IsTypeImmune(PokemonType.Electric), Is.True);
        }

        [Test]
        public void Catalog_Sleep_HasCorrectProperties()
        {
            var sleep = StatusCatalog.Sleep;

            Assert.That(sleep.PersistentStatus, Is.EqualTo(PersistentStatus.Sleep));
            Assert.That(sleep.PreventsAction, Is.True);
            Assert.That(sleep.MinTurns, Is.EqualTo(1));
            Assert.That(sleep.MaxTurns, Is.EqualTo(3));
        }

        [Test]
        public void Catalog_Poison_HasCorrectProperties()
        {
            var poison = StatusCatalog.Poison;

            Assert.That(poison.PersistentStatus, Is.EqualTo(PersistentStatus.Poison));
            Assert.That(poison.EndOfTurnDamage, Is.EqualTo(1f / 8f).Within(0.001f));
            Assert.That(poison.IsTypeImmune(PokemonType.Poison), Is.True);
            Assert.That(poison.IsTypeImmune(PokemonType.Steel), Is.True);
        }

        [Test]
        public void Catalog_BadlyPoisoned_HasEscalatingDamage()
        {
            var toxic = StatusCatalog.BadlyPoisoned;

            Assert.That(toxic.DamageEscalates, Is.True);
            Assert.That(toxic.GetEscalatingDamage(1), Is.EqualTo(1f / 16f).Within(0.001f));
            Assert.That(toxic.GetEscalatingDamage(5), Is.EqualTo(5f / 16f).Within(0.001f));
        }

        [Test]
        public void Catalog_Freeze_HasRecoveryAndCure()
        {
            var freeze = StatusCatalog.Freeze;

            Assert.That(freeze.PreventsAction, Is.True);
            Assert.That(freeze.RecoveryChancePerTurn, Is.EqualTo(0.20f).Within(0.001f));
            Assert.That(freeze.CanBeCuredByMoveType(PokemonType.Fire), Is.True);
            Assert.That(freeze.IsTypeImmune(PokemonType.Ice), Is.True);
        }

        [Test]
        public void Catalog_Confusion_HasSelfHitChance()
        {
            var confusion = StatusCatalog.Confusion;

            Assert.That(confusion.IsVolatile, Is.True);
            Assert.That(confusion.SelfHitChance, Is.EqualTo(0.33f).Within(0.01f));
            Assert.That(confusion.SelfHitPower, Is.EqualTo(40));
            Assert.That(confusion.MinTurns, Is.EqualTo(2));
            Assert.That(confusion.MaxTurns, Is.EqualTo(5));
        }

        [Test]
        public void Catalog_Taunt_RestrictsStatusMoves()
        {
            var taunt = StatusCatalog.Taunt;

            Assert.That(taunt.RestrictedMoveCategory, Is.EqualTo(MoveCategory.Status));
        }

        [Test]
        public void Catalog_LeechSeed_DrainsToOpponent()
        {
            var leechSeed = StatusCatalog.LeechSeed;

            Assert.That(leechSeed.DrainsToOpponent, Is.True);
            Assert.That(leechSeed.EndOfTurnDamage, Is.EqualTo(1f / 8f).Within(0.001f));
            Assert.That(leechSeed.IsTypeImmune(PokemonType.Grass), Is.True);
        }

        [Test]
        public void Catalog_GetByStatus_Persistent_ReturnsCorrectData()
        {
            Assert.That(StatusCatalog.GetByStatus(PersistentStatus.Burn), Is.EqualTo(StatusCatalog.Burn));
            Assert.That(StatusCatalog.GetByStatus(PersistentStatus.Paralysis), Is.EqualTo(StatusCatalog.Paralysis));
            Assert.That(StatusCatalog.GetByStatus(PersistentStatus.None), Is.Null);
        }

        [Test]
        public void Catalog_GetByStatus_Volatile_ReturnsCorrectData()
        {
            Assert.That(StatusCatalog.GetByStatus(VolatileStatus.Confusion), Is.EqualTo(StatusCatalog.Confusion));
            Assert.That(StatusCatalog.GetByStatus(VolatileStatus.Taunt), Is.EqualTo(StatusCatalog.Taunt));
            Assert.That(StatusCatalog.GetByStatus(VolatileStatus.None), Is.Null);
        }

        [Test]
        public void Catalog_All_ContainsAllStatuses()
        {
            Assert.That(StatusCatalog.All.Count, Is.EqualTo(15)); // 6 persistent + 9 volatile
        }

        #endregion
    }
}

