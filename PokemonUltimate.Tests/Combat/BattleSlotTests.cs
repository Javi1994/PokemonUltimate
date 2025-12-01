using NUnit.Framework;
using PokemonUltimate.Core.Combat;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Combat
{
    /// <summary>
    /// Tests for BattleSlot - container for an active Pokemon in battle.
    /// </summary>
    [TestFixture]
    public class BattleSlotTests
    {
        private PokemonInstance _pokemon;

        [SetUp]
        public void SetUp()
        {
            _pokemon = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
        }

        #region Constructor Tests

        [Test]
        public void Constructor_WithSlotIndex_InitializesCorrectly()
        {
            var slot = new BattleSlot(0);

            Assert.That(slot.SlotIndex, Is.EqualTo(0));
            Assert.That(slot.IsEmpty, Is.True);
            Assert.That(slot.Pokemon, Is.Null);
        }

        [Test]
        public void Constructor_NegativeIndex_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new BattleSlot(-1));
        }

        #endregion

        #region Pokemon Management Tests

        [Test]
        public void SetPokemon_ValidPokemon_SetsPokemon()
        {
            var slot = new BattleSlot(0);

            slot.SetPokemon(_pokemon);

            Assert.That(slot.Pokemon, Is.EqualTo(_pokemon));
            Assert.That(slot.IsEmpty, Is.False);
        }

        [Test]
        public void SetPokemon_Null_ThrowsArgumentNullException()
        {
            var slot = new BattleSlot(0);

            Assert.Throws<ArgumentNullException>(() => slot.SetPokemon(null));
        }

        [Test]
        public void SetPokemon_ReplacesExistingPokemon()
        {
            var slot = new BattleSlot(0);
            var charmander = PokemonFactory.Create(PokemonCatalog.Charmander, 10);
            slot.SetPokemon(_pokemon);

            slot.SetPokemon(charmander);

            Assert.That(slot.Pokemon, Is.EqualTo(charmander));
        }

        [Test]
        public void ClearSlot_RemovesPokemon()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(_pokemon);

            slot.ClearSlot();

            Assert.That(slot.IsEmpty, Is.True);
            Assert.That(slot.Pokemon, Is.Null);
        }

        [Test]
        public void ClearSlot_EmptySlot_DoesNotThrow()
        {
            var slot = new BattleSlot(0);

            Assert.DoesNotThrow(() => slot.ClearSlot());
        }

        #endregion

        #region State Tests

        [Test]
        public void IsEmpty_NoPokemon_ReturnsTrue()
        {
            var slot = new BattleSlot(0);

            Assert.That(slot.IsEmpty, Is.True);
        }

        [Test]
        public void IsEmpty_WithPokemon_ReturnsFalse()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(_pokemon);

            Assert.That(slot.IsEmpty, Is.False);
        }

        [Test]
        public void HasFainted_HealthyPokemon_ReturnsFalse()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(_pokemon);

            Assert.That(slot.HasFainted, Is.False);
        }

        [Test]
        public void HasFainted_FaintedPokemon_ReturnsTrue()
        {
            var slot = new BattleSlot(0);
            _pokemon.TakeDamage(_pokemon.MaxHP);
            slot.SetPokemon(_pokemon);

            Assert.That(slot.HasFainted, Is.True);
        }

        [Test]
        public void HasFainted_EmptySlot_ReturnsFalse()
        {
            var slot = new BattleSlot(0);

            Assert.That(slot.HasFainted, Is.False);
        }

        #endregion

        #region Stat Stages Tests

        [Test]
        public void StatStages_Default_AllZero()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(_pokemon);

            Assert.That(slot.GetStatStage(Stat.Attack), Is.EqualTo(0));
            Assert.That(slot.GetStatStage(Stat.Defense), Is.EqualTo(0));
            Assert.That(slot.GetStatStage(Stat.SpAttack), Is.EqualTo(0));
            Assert.That(slot.GetStatStage(Stat.SpDefense), Is.EqualTo(0));
            Assert.That(slot.GetStatStage(Stat.Speed), Is.EqualTo(0));
            Assert.That(slot.GetStatStage(Stat.Accuracy), Is.EqualTo(0));
            Assert.That(slot.GetStatStage(Stat.Evasion), Is.EqualTo(0));
        }

        [Test]
        public void ModifyStatStage_Positive_IncreasesStage()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(_pokemon);

            var change = slot.ModifyStatStage(Stat.Attack, 2);

            Assert.That(slot.GetStatStage(Stat.Attack), Is.EqualTo(2));
            Assert.That(change, Is.EqualTo(2));
        }

        [Test]
        public void ModifyStatStage_Negative_DecreasesStage()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(_pokemon);

            var change = slot.ModifyStatStage(Stat.Defense, -2);

            Assert.That(slot.GetStatStage(Stat.Defense), Is.EqualTo(-2));
            Assert.That(change, Is.EqualTo(-2));
        }

        [Test]
        public void ModifyStatStage_ClampsToMax6()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(_pokemon);
            slot.ModifyStatStage(Stat.Attack, 5);

            var change = slot.ModifyStatStage(Stat.Attack, 3);

            Assert.That(slot.GetStatStage(Stat.Attack), Is.EqualTo(6));
            Assert.That(change, Is.EqualTo(1)); // Only went up by 1
        }

        [Test]
        public void ModifyStatStage_ClampsToMinNegative6()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(_pokemon);
            slot.ModifyStatStage(Stat.Speed, -5);

            var change = slot.ModifyStatStage(Stat.Speed, -3);

            Assert.That(slot.GetStatStage(Stat.Speed), Is.EqualTo(-6));
            Assert.That(change, Is.EqualTo(-1)); // Only went down by 1
        }

        [Test]
        public void ModifyStatStage_AtMax_ReturnsZeroChange()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(_pokemon);
            slot.ModifyStatStage(Stat.Attack, 6);

            var change = slot.ModifyStatStage(Stat.Attack, 2);

            Assert.That(change, Is.EqualTo(0));
        }

        [Test]
        public void ModifyStatStage_HP_ThrowsArgumentException()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(_pokemon);

            Assert.Throws<ArgumentException>(() => slot.ModifyStatStage(Stat.HP, 1));
        }

        #endregion

        #region Volatile Status Tests

        [Test]
        public void VolatileStatus_Default_IsNone()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(_pokemon);

            Assert.That(slot.VolatileStatus, Is.EqualTo(VolatileStatus.None));
        }

        [Test]
        public void AddVolatileStatus_AddsFlag()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(_pokemon);

            slot.AddVolatileStatus(VolatileStatus.Confusion);

            Assert.That(slot.HasVolatileStatus(VolatileStatus.Confusion), Is.True);
        }

        [Test]
        public void AddVolatileStatus_MultipleTimes_KeepsFlag()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(_pokemon);

            slot.AddVolatileStatus(VolatileStatus.Confusion);
            slot.AddVolatileStatus(VolatileStatus.Confusion);

            Assert.That(slot.HasVolatileStatus(VolatileStatus.Confusion), Is.True);
        }

        [Test]
        public void RemoveVolatileStatus_RemovesFlag()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(_pokemon);
            slot.AddVolatileStatus(VolatileStatus.Confusion);

            slot.RemoveVolatileStatus(VolatileStatus.Confusion);

            Assert.That(slot.HasVolatileStatus(VolatileStatus.Confusion), Is.False);
        }

        [Test]
        public void HasVolatileStatus_MultipleStatuses_ChecksCorrectly()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(_pokemon);
            slot.AddVolatileStatus(VolatileStatus.Confusion);
            slot.AddVolatileStatus(VolatileStatus.Flinch);

            Assert.That(slot.HasVolatileStatus(VolatileStatus.Confusion), Is.True);
            Assert.That(slot.HasVolatileStatus(VolatileStatus.Flinch), Is.True);
            Assert.That(slot.HasVolatileStatus(VolatileStatus.Attract), Is.False);
        }

        #endregion

        #region Reset Battle State Tests

        [Test]
        public void ResetBattleState_ClearsStatStages()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(_pokemon);
            slot.ModifyStatStage(Stat.Attack, 3);
            slot.ModifyStatStage(Stat.Defense, -2);

            slot.ResetBattleState();

            Assert.That(slot.GetStatStage(Stat.Attack), Is.EqualTo(0));
            Assert.That(slot.GetStatStage(Stat.Defense), Is.EqualTo(0));
        }

        [Test]
        public void ResetBattleState_ClearsVolatileStatus()
        {
            var slot = new BattleSlot(0);
            slot.SetPokemon(_pokemon);
            slot.AddVolatileStatus(VolatileStatus.Confusion);
            slot.AddVolatileStatus(VolatileStatus.Flinch);

            slot.ResetBattleState();

            Assert.That(slot.VolatileStatus, Is.EqualTo(VolatileStatus.None));
        }

        [Test]
        public void ResetBattleState_EmptySlot_DoesNotThrow()
        {
            var slot = new BattleSlot(0);

            Assert.DoesNotThrow(() => slot.ResetBattleState());
        }

        #endregion
    }
}

