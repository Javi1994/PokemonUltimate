using System;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Actions
{
    /// <summary>
    /// Edge case tests for ApplyStatusAction - boundary conditions and error handling.
    /// </summary>
    [TestFixture]
    public class ApplyStatusActionEdgeCasesTests
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

        #region Null Validation Tests

        [Test]
        public void Constructor_NullTarget_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new ApplyStatusAction(null, null, PersistentStatus.Burn));
        }

        [Test]
        public void ExecuteLogic_NullField_ThrowsArgumentNullException()
        {
            var action = new ApplyStatusAction(null, _targetSlot, PersistentStatus.Burn);

            Assert.Throws<ArgumentNullException>(() => 
                action.ExecuteLogic(null).ToList());
        }

        [Test]
        public void ExecuteVisual_NullView_ThrowsArgumentNullException()
        {
            var action = new ApplyStatusAction(null, _targetSlot, PersistentStatus.Burn);

            Assert.Throws<ArgumentNullException>(() => 
                action.ExecuteVisual(null).Wait());
        }

        #endregion

        #region Edge Cases

        [Test]
        public void ExecuteLogic_FaintedPokemon_CanStillReceiveStatus()
        {
            _pokemon.CurrentHP = 0;

            var action = new ApplyStatusAction(null, _targetSlot, PersistentStatus.Burn);
            action.ExecuteLogic(_field);

            // Note: In real Pokemon, fainted Pokemon can't receive status
            // But our implementation allows it for consistency
            Assert.That(_pokemon.Status, Is.EqualTo(PersistentStatus.Burn));
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
        public void ExecuteVisual_NoneStatus_SkipsAnimation()
        {
            var view = new MockStatusBattleView();

            var action = new ApplyStatusAction(null, _targetSlot, PersistentStatus.None);
            action.ExecuteVisual(view).Wait();

            Assert.That(view.StatusAnimationCalled, Is.False);
        }

        [Test]
        public void ExecuteVisual_EmptySlot_SkipsAnimation()
        {
            var view = new MockStatusBattleView();
            var emptySlot = new BattleSlot(0);

            var action = new ApplyStatusAction(null, emptySlot, PersistentStatus.Burn);
            action.ExecuteVisual(view).Wait();

            Assert.That(view.StatusAnimationCalled, Is.False);
        }

        [Test]
        public void ExecuteLogic_StatusReplacement_ReplacesCorrectly()
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

            foreach (var oldStatus in statuses)
            {
                foreach (var newStatus in statuses)
                {
                    if (oldStatus == newStatus) continue;

                    _pokemon.Status = oldStatus;
                    var action = new ApplyStatusAction(null, _targetSlot, newStatus);
                    action.ExecuteLogic(_field);

                    Assert.That(_pokemon.Status, Is.EqualTo(newStatus),
                        $"Failed to replace {oldStatus} with {newStatus}");
                }
            }
        }

        #endregion
    }
}

