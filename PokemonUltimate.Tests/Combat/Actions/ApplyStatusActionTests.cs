using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Combat.Actions
{
    /// <summary>
    /// Tests for ApplyStatusAction - applies a persistent status condition to a Pokemon.
    /// </summary>
    [TestFixture]
    public class ApplyStatusActionTests
    {
        private BattleField _field;
        private BattleSlot _targetSlot;
        private PokemonInstance _pokemon;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _targetSlot = _field.PlayerSide.Slots[0];
            _pokemon = _targetSlot.Pokemon;
        }

        #region ExecuteLogic Tests

        [Test]
        public void ExecuteLogic_AppliesStatus()
        {
            Assert.That(_pokemon.Status, Is.EqualTo(PersistentStatus.None));

            var action = new ApplyStatusAction(null, _targetSlot, PersistentStatus.Burn);
            var reactions = action.ExecuteLogic(_field);

            Assert.That(_pokemon.Status, Is.EqualTo(PersistentStatus.Burn));
            Assert.That(reactions, Is.Empty);
        }

        [Test]
        public void ExecuteLogic_ReplacesExistingStatus()
        {
            _pokemon.Status = PersistentStatus.Paralysis;

            var action = new ApplyStatusAction(null, _targetSlot, PersistentStatus.Burn);
            action.ExecuteLogic(_field);

            Assert.That(_pokemon.Status, Is.EqualTo(PersistentStatus.Burn));
        }

        [Test]
        public void ExecuteLogic_NoneStatus_ClearsStatus()
        {
            _pokemon.Status = PersistentStatus.Burn;

            var action = new ApplyStatusAction(null, _targetSlot, PersistentStatus.None);
            action.ExecuteLogic(_field);

            Assert.That(_pokemon.Status, Is.EqualTo(PersistentStatus.None));
        }

        [Test]
        public void ExecuteLogic_EmptySlot_DoesNothing()
        {
            var emptySlot = new BattleSlot(0);
            var action = new ApplyStatusAction(null, emptySlot, PersistentStatus.Burn);

            var reactions = action.ExecuteLogic(_field);

            Assert.That(reactions, Is.Empty);
        }

        [Test]
        public void ExecuteLogic_AllStatusTypes_CanBeApplied()
        {
            var statuses = new[]
            {
                PersistentStatus.Burn,
                PersistentStatus.Paralysis,
                PersistentStatus.Sleep,
                PersistentStatus.Poison,
                PersistentStatus.BadlyPoisoned,
                PersistentStatus.Freeze
            };

            foreach (var status in statuses)
            {
                _pokemon.Status = PersistentStatus.None;
                var action = new ApplyStatusAction(null, _targetSlot, status);
                action.ExecuteLogic(_field);
                Assert.That(_pokemon.Status, Is.EqualTo(status), $"Failed to apply {status}");
            }
        }

        #endregion

        #region ExecuteVisual Tests

        [Test]
        public void ExecuteVisual_CallsPlayStatusAnimation()
        {
            var view = new MockStatusBattleView();
            var action = new ApplyStatusAction(null, _targetSlot, PersistentStatus.Burn);

            action.ExecuteVisual(view).Wait();

            Assert.That(view.StatusAnimationCalled, Is.True);
            Assert.That(view.StatusAnimationSlot, Is.EqualTo(_targetSlot));
            Assert.That(view.StatusAnimationName, Is.EqualTo("Burn"));
        }

        [Test]
        public void ExecuteVisual_NoneStatus_DoesNotCallAnimation()
        {
            var view = new MockStatusBattleView();
            var action = new ApplyStatusAction(null, _targetSlot, PersistentStatus.None);

            action.ExecuteVisual(view).Wait();

            Assert.That(view.StatusAnimationCalled, Is.False);
        }

        #endregion
    }

    /// <summary>
    /// Mock implementation of IBattleView for status testing.
    /// </summary>
    internal class MockStatusBattleView : IBattleView
    {
        public bool StatusAnimationCalled { get; private set; }
        public BattleSlot StatusAnimationSlot { get; private set; }
        public string StatusAnimationName { get; private set; }

        public Task ShowMessage(string message) => Task.CompletedTask;

        public Task PlayDamageAnimation(BattleSlot slot) => Task.CompletedTask;

        public Task UpdateHPBar(BattleSlot slot) => Task.CompletedTask;

        public Task PlayMoveAnimation(BattleSlot user, BattleSlot target, string moveId) => Task.CompletedTask;

        public Task PlayFaintAnimation(BattleSlot slot) => Task.CompletedTask;

        public Task PlayStatusAnimation(BattleSlot slot, string statusName)
        {
            StatusAnimationCalled = true;
            StatusAnimationSlot = slot;
            StatusAnimationName = statusName;
            return Task.CompletedTask;
        }

        public Task ShowStatChange(BattleSlot slot, string statName, int stages) => Task.CompletedTask;

        public Task PlaySwitchOutAnimation(BattleSlot slot) => Task.CompletedTask;

        public Task PlaySwitchInAnimation(BattleSlot slot) => Task.CompletedTask;
    }
}

