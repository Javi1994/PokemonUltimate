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
    /// Tests for StatChangeAction - modifies stat stages of a Pokemon.
    /// </summary>
    [TestFixture]
    public class StatChangeActionTests
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
        public void ExecuteLogic_IncreasesStatStage()
        {
            int initialStage = _targetSlot.GetStatStage(Stat.Attack);
            int change = 2;

            var action = new StatChangeAction(null, _targetSlot, Stat.Attack, change);
            var reactions = action.ExecuteLogic(_field);

            Assert.That(_targetSlot.GetStatStage(Stat.Attack), Is.EqualTo(initialStage + change));
            Assert.That(reactions, Is.Empty);
        }

        [Test]
        public void ExecuteLogic_DecreasesStatStage()
        {
            _targetSlot.ModifyStatStage(Stat.Defense, 3);
            int change = -2;

            var action = new StatChangeAction(null, _targetSlot, Stat.Defense, change);
            action.ExecuteLogic(_field);

            Assert.That(_targetSlot.GetStatStage(Stat.Defense), Is.EqualTo(1));
        }

        [Test]
        public void ExecuteLogic_ClampsToMaxStage()
        {
            _targetSlot.ModifyStatStage(Stat.Attack, 5);
            int change = 5; // Would exceed +6

            var action = new StatChangeAction(null, _targetSlot, Stat.Attack, change);
            action.ExecuteLogic(_field);

            Assert.That(_targetSlot.GetStatStage(Stat.Attack), Is.EqualTo(6));
        }

        [Test]
        public void ExecuteLogic_ClampsToMinStage()
        {
            _targetSlot.ModifyStatStage(Stat.Defense, -5);
            int change = -5; // Would exceed -6

            var action = new StatChangeAction(null, _targetSlot, Stat.Defense, change);
            action.ExecuteLogic(_field);

            Assert.That(_targetSlot.GetStatStage(Stat.Defense), Is.EqualTo(-6));
        }

        [Test]
        public void ExecuteLogic_ZeroChange_DoesNothing()
        {
            int initialStage = _targetSlot.GetStatStage(Stat.Speed);

            var action = new StatChangeAction(null, _targetSlot, Stat.Speed, 0);
            action.ExecuteLogic(_field);

            Assert.That(_targetSlot.GetStatStage(Stat.Speed), Is.EqualTo(initialStage));
        }

        [Test]
        public void ExecuteLogic_EmptySlot_DoesNothing()
        {
            var emptySlot = new BattleSlot(0);
            var action = new StatChangeAction(null, emptySlot, Stat.Attack, 2);

            var reactions = action.ExecuteLogic(_field);

            Assert.That(reactions, Is.Empty);
        }

        #endregion

        #region ExecuteVisual Tests

        [Test]
        public void ExecuteVisual_CallsShowStatChange()
        {
            var view = new MockStatChangeBattleView();
            var action = new StatChangeAction(null, _targetSlot, Stat.Attack, 2);

            action.ExecuteVisual(view).Wait();

            Assert.That(view.StatChangeCalled, Is.True);
            Assert.That(view.StatChangeSlot, Is.EqualTo(_targetSlot));
            Assert.That(view.StatChangeStatName, Is.EqualTo("Attack"));
            Assert.That(view.StatChangeStages, Is.EqualTo(2));
        }

        [Test]
        public void ExecuteVisual_NegativeChange_ShowsNegativeStages()
        {
            var view = new MockStatChangeBattleView();
            var action = new StatChangeAction(null, _targetSlot, Stat.Defense, -1);

            action.ExecuteVisual(view).Wait();

            Assert.That(view.StatChangeStages, Is.EqualTo(-1));
        }

        #endregion
    }

    /// <summary>
    /// Mock implementation of IBattleView for stat change testing.
    /// </summary>
    internal class MockStatChangeBattleView : IBattleView
    {
        public bool StatChangeCalled { get; private set; }
        public BattleSlot StatChangeSlot { get; private set; }
        public string StatChangeStatName { get; private set; }
        public int StatChangeStages { get; private set; }

        public Task ShowMessage(string message) => Task.CompletedTask;

        public Task PlayDamageAnimation(BattleSlot slot) => Task.CompletedTask;

        public Task UpdateHPBar(BattleSlot slot) => Task.CompletedTask;

        public Task PlayMoveAnimation(BattleSlot user, BattleSlot target, string moveId) => Task.CompletedTask;

        public Task PlayFaintAnimation(BattleSlot slot) => Task.CompletedTask;

        public Task PlayStatusAnimation(BattleSlot slot, string statusName) => Task.CompletedTask;

        public Task ShowStatChange(BattleSlot slot, string statName, int stages)
        {
            StatChangeCalled = true;
            StatChangeSlot = slot;
            StatChangeStatName = statName;
            StatChangeStages = stages;
            return Task.CompletedTask;
        }

        public Task PlaySwitchOutAnimation(BattleSlot slot) => Task.CompletedTask;

        public Task PlaySwitchInAnimation(BattleSlot slot) => Task.CompletedTask;
        public Task<BattleActionType> SelectActionType(BattleSlot slot) => Task.FromResult(BattleActionType.Fight);
        public Task<MoveInstance> SelectMove(IReadOnlyList<MoveInstance> moves) => Task.FromResult(moves?.FirstOrDefault());
        public Task<BattleSlot> SelectTarget(IReadOnlyList<BattleSlot> validTargets) => Task.FromResult(validTargets?.FirstOrDefault());
        public Task<PokemonInstance> SelectSwitch(IReadOnlyList<PokemonInstance> availablePokemon) => Task.FromResult(availablePokemon?.FirstOrDefault());
    }
}

