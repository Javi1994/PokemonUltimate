using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Actions
{
    /// <summary>
    /// Tests for HealAction - restores HP to a Pokemon.
    /// </summary>
    [TestFixture]
    public class HealActionTests
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
        public void ExecuteLogic_RestoresHP()
        {
            _pokemon.CurrentHP = 50;
            int healAmount = 30;
            int expectedHP = 80;

            var action = new HealAction(null, _targetSlot, healAmount);
            var reactions = action.ExecuteLogic(_field);

            Assert.That(_pokemon.CurrentHP, Is.EqualTo(expectedHP));
            Assert.That(reactions, Is.Empty);
        }

        [Test]
        public void ExecuteLogic_DoesNotOverheal()
        {
            _pokemon.CurrentHP = _pokemon.MaxHP - 10;
            int healAmount = 100; // More than needed

            var action = new HealAction(null, _targetSlot, healAmount);
            action.ExecuteLogic(_field);

            Assert.That(_pokemon.CurrentHP, Is.EqualTo(_pokemon.MaxHP));
        }

        [Test]
        public void ExecuteLogic_ZeroHeal_DoesNothing()
        {
            int initialHP = _pokemon.CurrentHP;

            var action = new HealAction(null, _targetSlot, 0);
            var reactions = action.ExecuteLogic(_field);

            Assert.That(_pokemon.CurrentHP, Is.EqualTo(initialHP));
            Assert.That(reactions, Is.Empty);
        }

        [Test]
        public void ExecuteLogic_EmptySlot_DoesNothing()
        {
            var emptySlot = new BattleSlot(0);
            var action = new HealAction(null, emptySlot, 50);

            var reactions = action.ExecuteLogic(_field);

            Assert.That(reactions, Is.Empty);
        }

        [Test]
        public void ExecuteLogic_FaintedPokemon_CanBeHealed()
        {
            _pokemon.CurrentHP = 0;
            int healAmount = 50;

            var action = new HealAction(null, _targetSlot, healAmount);
            action.ExecuteLogic(_field);

            Assert.That(_pokemon.CurrentHP, Is.EqualTo(healAmount));
            Assert.That(_pokemon.IsFainted, Is.False);
        }

        #endregion

        #region ExecuteVisual Tests

        [Test]
        public void ExecuteVisual_CallsUpdateHPBar()
        {
            var view = new MockHealBattleView();
            var action = new HealAction(null, _targetSlot, 30);

            action.ExecuteVisual(view).Wait();

            Assert.That(view.HPBarUpdateCalled, Is.True);
            Assert.That(view.HPBarUpdateSlot, Is.EqualTo(_targetSlot));
        }

        #endregion
    }

    /// <summary>
    /// Mock implementation of IBattleView for heal testing.
    /// </summary>
    internal class MockHealBattleView : IBattleView
    {
        public bool HPBarUpdateCalled { get; private set; }
        public BattleSlot HPBarUpdateSlot { get; private set; }

        public Task ShowMessage(string message) => Task.CompletedTask;

        public Task PlayDamageAnimation(BattleSlot slot) => Task.CompletedTask;

        public Task UpdateHPBar(BattleSlot slot)
        {
            HPBarUpdateCalled = true;
            HPBarUpdateSlot = slot;
            return Task.CompletedTask;
        }

        public Task PlayMoveAnimation(BattleSlot user, BattleSlot target, string moveId) => Task.CompletedTask;

        public Task PlayFaintAnimation(BattleSlot slot) => Task.CompletedTask;

        public Task PlayStatusAnimation(BattleSlot slot, string statusName) => Task.CompletedTask;

        public Task ShowStatChange(BattleSlot slot, string statName, int stages) => Task.CompletedTask;

        public Task PlaySwitchOutAnimation(BattleSlot slot) => Task.CompletedTask;

        public Task PlaySwitchInAnimation(BattleSlot slot) => Task.CompletedTask;
        public Task<BattleActionType> SelectActionType(BattleSlot slot) => Task.FromResult(BattleActionType.Fight);
        public Task<MoveInstance> SelectMove(IReadOnlyList<MoveInstance> moves) => Task.FromResult(moves?.FirstOrDefault());
        public Task<BattleSlot> SelectTarget(IReadOnlyList<BattleSlot> validTargets) => Task.FromResult(validTargets?.FirstOrDefault());
        public Task<PokemonInstance> SelectSwitch(IReadOnlyList<PokemonInstance> availablePokemon) => Task.FromResult(availablePokemon?.FirstOrDefault());
    }
}

