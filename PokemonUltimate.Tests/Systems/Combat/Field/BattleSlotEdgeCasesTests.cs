using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Field
{
    /// <summary>
    /// Edge case tests for BattleSlot - boundaries, limits, and unusual scenarios.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    [TestFixture]
    public class BattleSlotEdgeCasesTests
    {
        #region Stat Stage Boundaries

        [Test]
        public void ModifyStatStage_ExactlyAtMax6_ReturnsZeroChange()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(PokemonFactory.Create(PokemonCatalog.Pikachu, 25));
            slot.ModifyStatStage(Stat.Attack, 6);

            var change = slot.ModifyStatStage(Stat.Attack, 1);

            Assert.That(change, Is.EqualTo(0));
            Assert.That(slot.GetStatStage(Stat.Attack), Is.EqualTo(6));
        }

        [Test]
        public void ModifyStatStage_ExactlyAtMinNeg6_ReturnsZeroChange()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(PokemonFactory.Create(PokemonCatalog.Pikachu, 25));
            slot.ModifyStatStage(Stat.Defense, -6);

            var change = slot.ModifyStatStage(Stat.Defense, -1);

            Assert.That(change, Is.EqualTo(0));
            Assert.That(slot.GetStatStage(Stat.Defense), Is.EqualTo(-6));
        }

        [Test]
        public void ModifyStatStage_LargePositiveValue_ClampsToMax()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(PokemonFactory.Create(PokemonCatalog.Pikachu, 25));

            var change = slot.ModifyStatStage(Stat.Speed, 100);

            Assert.That(slot.GetStatStage(Stat.Speed), Is.EqualTo(6));
            Assert.That(change, Is.EqualTo(6));
        }

        [Test]
        public void ModifyStatStage_LargeNegativeValue_ClampsToMin()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(PokemonFactory.Create(PokemonCatalog.Pikachu, 25));

            var change = slot.ModifyStatStage(Stat.SpAttack, -100);

            Assert.That(slot.GetStatStage(Stat.SpAttack), Is.EqualTo(-6));
            Assert.That(change, Is.EqualTo(-6));
        }

        [Test]
        public void ModifyStatStage_ZeroChange_ReturnsZero()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(PokemonFactory.Create(PokemonCatalog.Pikachu, 25));

            var change = slot.ModifyStatStage(Stat.Attack, 0);

            Assert.That(change, Is.EqualTo(0));
            Assert.That(slot.GetStatStage(Stat.Attack), Is.EqualTo(0));
        }

        [Test]
        public void ModifyStatStage_AllStatsIndependently()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(PokemonFactory.Create(PokemonCatalog.Pikachu, 25));

            slot.ModifyStatStage(Stat.Attack, 2);
            slot.ModifyStatStage(Stat.Defense, -2);
            slot.ModifyStatStage(Stat.SpAttack, 3);
            slot.ModifyStatStage(Stat.SpDefense, -3);
            slot.ModifyStatStage(Stat.Speed, 1);
            slot.ModifyStatStage(Stat.Accuracy, 4);
            slot.ModifyStatStage(Stat.Evasion, -4);

            Assert.That(slot.GetStatStage(Stat.Attack), Is.EqualTo(2));
            Assert.That(slot.GetStatStage(Stat.Defense), Is.EqualTo(-2));
            Assert.That(slot.GetStatStage(Stat.SpAttack), Is.EqualTo(3));
            Assert.That(slot.GetStatStage(Stat.SpDefense), Is.EqualTo(-3));
            Assert.That(slot.GetStatStage(Stat.Speed), Is.EqualTo(1));
            Assert.That(slot.GetStatStage(Stat.Accuracy), Is.EqualTo(4));
            Assert.That(slot.GetStatStage(Stat.Evasion), Is.EqualTo(-4));
        }

        [Test]
        public void GetStatStage_UnknownStat_ReturnsZero()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(PokemonFactory.Create(PokemonCatalog.Pikachu, 25));

            // HP is not a modifiable stat stage, should return 0
            Assert.That(slot.GetStatStage(Stat.HP), Is.EqualTo(0));
        }

        #endregion

        #region Volatile Status Edge Cases

        [Test]
        public void AddVolatileStatus_AllStatusesCombined()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(PokemonFactory.Create(PokemonCatalog.Pikachu, 25));

            slot.AddVolatileStatus(VolatileStatus.Confusion);
            slot.AddVolatileStatus(VolatileStatus.Flinch);
            slot.AddVolatileStatus(VolatileStatus.LeechSeed);
            slot.AddVolatileStatus(VolatileStatus.Attract);

            Assert.That(slot.HasVolatileStatus(VolatileStatus.Confusion), Is.True);
            Assert.That(slot.HasVolatileStatus(VolatileStatus.Flinch), Is.True);
            Assert.That(slot.HasVolatileStatus(VolatileStatus.LeechSeed), Is.True);
            Assert.That(slot.HasVolatileStatus(VolatileStatus.Attract), Is.True);
        }

        [Test]
        public void RemoveVolatileStatus_OnlyRemovesSpecified()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(PokemonFactory.Create(PokemonCatalog.Pikachu, 25));
            slot.AddVolatileStatus(VolatileStatus.Confusion);
            slot.AddVolatileStatus(VolatileStatus.Flinch);

            slot.RemoveVolatileStatus(VolatileStatus.Confusion);

            Assert.That(slot.HasVolatileStatus(VolatileStatus.Confusion), Is.False);
            Assert.That(slot.HasVolatileStatus(VolatileStatus.Flinch), Is.True);
        }

        [Test]
        public void RemoveVolatileStatus_NotPresent_DoesNothing()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(PokemonFactory.Create(PokemonCatalog.Pikachu, 25));
            slot.AddVolatileStatus(VolatileStatus.Flinch);

            Assert.DoesNotThrow(() => slot.RemoveVolatileStatus(VolatileStatus.Confusion));
            Assert.That(slot.HasVolatileStatus(VolatileStatus.Flinch), Is.True);
        }

        [Test]
        public void HasVolatileStatus_None_ReturnsFalse()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(PokemonFactory.Create(PokemonCatalog.Pikachu, 25));

            Assert.That(slot.HasVolatileStatus(VolatileStatus.None), Is.True); // None is always present
        }

        #endregion

        #region Slot Index Edge Cases

        [Test]
        public void Constructor_LargeSlotIndex_Allowed()
        {
            var slot = new BattleSlot(999);

            Assert.That(slot.SlotIndex, Is.EqualTo(999));
        }

        [Test]
        public void Constructor_ZeroIndex_Allowed()
        {
            var slot = new BattleSlot(0);

            Assert.That(slot.SlotIndex, Is.EqualTo(0));
        }

        #endregion

        #region Pokemon Replacement Edge Cases

        [Test]
        public void SetPokemon_MultipleTimes_KeepsLatest()
        {
            var slot = new BattleSlot(0);
            var pika = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
            var char1 = PokemonFactory.Create(PokemonCatalog.Charmander, 20);
            var bulba = PokemonFactory.Create(PokemonCatalog.Bulbasaur, 15);

            slot.SetPokemon(pika);
            slot.SetPokemon(char1);
            slot.SetPokemon(bulba);

            Assert.That(slot.Pokemon, Is.EqualTo(bulba));
        }

        [Test]
        public void SetPokemon_ResetsStatStages()
        {
            var slot = new BattleSlot(0);
            var pika = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
            slot.SetPokemon(pika);
            slot.ModifyStatStage(Stat.Attack, 3);

            var char1 = PokemonFactory.Create(PokemonCatalog.Charmander, 20);
            slot.SetPokemon(char1);

            Assert.That(slot.GetStatStage(Stat.Attack), Is.EqualTo(0));
        }

        [Test]
        public void SetPokemon_ResetsVolatileStatus()
        {
            var slot = new BattleSlot(0);
            var pika = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
            slot.SetPokemon(pika);
            slot.AddVolatileStatus(VolatileStatus.Confusion);

            var char1 = PokemonFactory.Create(PokemonCatalog.Charmander, 20);
            slot.SetPokemon(char1);

            Assert.That(slot.VolatileStatus, Is.EqualTo(VolatileStatus.None));
        }

        [Test]
        public void ClearSlot_ThenSetPokemon_WorksCorrectly()
        {
            var slot = new BattleSlot(0);
            var pika = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
            slot.SetPokemon(pika);
            slot.ClearSlot();

            var char1 = PokemonFactory.Create(PokemonCatalog.Charmander, 20);
            slot.SetPokemon(char1);

            Assert.That(slot.Pokemon, Is.EqualTo(char1));
            Assert.That(slot.IsEmpty, Is.False);
        }

        #endregion

        #region Fainted Pokemon Edge Cases

        [Test]
        public void HasFainted_PokemonAt1HP_ReturnsFalse()
        {
            var slot = new BattleSlot(0);
            var pika = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
            pika.TakeDamage(pika.MaxHP - 1); // Leave at 1 HP
            slot.SetPokemon(pika);

            Assert.That(slot.HasFainted, Is.False);
            Assert.That(pika.CurrentHP, Is.EqualTo(1));
        }

        [Test]
        public void HasFainted_PokemonAtExactly0HP_ReturnsTrue()
        {
            var slot = new BattleSlot(0);
            var pika = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
            pika.TakeDamage(pika.MaxHP);
            slot.SetPokemon(pika);

            Assert.That(slot.HasFainted, Is.True);
            Assert.That(pika.CurrentHP, Is.EqualTo(0));
        }

        [Test]
        public void HasFainted_PokemonTakesOverkillDamage_ReturnsTrue()
        {
            var slot = new BattleSlot(0);
            var pika = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
            pika.TakeDamage(9999); // Overkill
            slot.SetPokemon(pika);

            Assert.That(slot.HasFainted, Is.True);
            Assert.That(pika.CurrentHP, Is.EqualTo(0));
        }

        #endregion
    }
}

