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
    /// Tests for FaintAction - handles Pokemon fainting.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    [TestFixture]
    public class FaintActionTests
    {
        private BattleField _field;
        private BattleSlot _faintedSlot;
        private PokemonInstance _pokemon;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _faintedSlot = _field.PlayerSide.Slots[0];
            _pokemon = _faintedSlot.Pokemon;
            _pokemon.CurrentHP = 0; // Already fainted
        }

        #region ExecuteLogic Tests

        [Test]
        public void ExecuteLogic_MarksPokemonFainted()
        {
            Assert.That(_pokemon.IsFainted, Is.True);

            var action = new FaintAction(null, _faintedSlot);
            var reactions = action.ExecuteLogic(_field);

            // Pokemon should remain fainted (no change needed, already fainted)
            Assert.That(_pokemon.IsFainted, Is.True);
            Assert.That(reactions, Is.Empty);
        }

        [Test]
        public void ExecuteLogic_ChecksForBattleEnd()
        {
            // When all Pokemon on a side faint, battle should end
            // This will be handled by CombatEngine in Phase 2.6
            // For now, FaintAction just marks the Pokemon as fainted

            var action = new FaintAction(null, _faintedSlot);
            var reactions = action.ExecuteLogic(_field);

            Assert.That(_pokemon.IsFainted, Is.True);
            // Battle end check deferred to CombatEngine
            Assert.That(reactions, Is.Empty);
        }

        [Test]
        public void ExecuteLogic_EmptySlot_DoesNothing()
        {
            var emptySlot = new BattleSlot(0);
            var action = new FaintAction(null, emptySlot);

            var reactions = action.ExecuteLogic(_field);

            Assert.That(reactions, Is.Empty);
        }

        #endregion

        #region ExecuteVisual Tests

        [Test]
        public void ExecuteVisual_CallsFaintAnimation()
        {
            var view = new MockFaintBattleView();
            var action = new FaintAction(null, _faintedSlot);

            action.ExecuteVisual(view).Wait();

            Assert.That(view.FaintAnimationCalled, Is.True);
            Assert.That(view.FaintAnimationSlot, Is.EqualTo(_faintedSlot));
        }

        #endregion
    }

    /// <summary>
    /// Mock implementation of IBattleView for faint testing.
    /// </summary>
    internal class MockFaintBattleView : IBattleView
    {
        public bool FaintAnimationCalled { get; private set; }
        public BattleSlot FaintAnimationSlot { get; private set; }

        public Task ShowMessage(string message) => Task.CompletedTask;

        public Task PlayDamageAnimation(BattleSlot slot) => Task.CompletedTask;

        public Task UpdateHPBar(BattleSlot slot) => Task.CompletedTask;

        public Task PlayMoveAnimation(BattleSlot user, BattleSlot target, string moveId) => Task.CompletedTask;

        public Task PlayFaintAnimation(BattleSlot slot)
        {
            FaintAnimationCalled = true;
            FaintAnimationSlot = slot;
            return Task.CompletedTask;
        }

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

