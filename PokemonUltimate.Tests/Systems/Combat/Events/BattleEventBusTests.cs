using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Events;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Systems.Combat.Events
{
    /// <summary>
    /// Tests for BattleEventBus - manages subscriptions and event publishing.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.9: Abilities & Items
    /// **Documentation**: See `docs/features/2-combat-system/2.9-abilities-items/architecture.md`
    /// </remarks>
    [TestFixture]
    public class BattleEventBusTests
    {
        private IBattleEventBus _eventBus;
        private BattleField _field;
        private TestListener _listener1;
        private TestListener _listener2;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new BattleEventBus();
            _field = new BattleField();
            _field.Initialize(BattleRules.Singles,
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) });
            _listener1 = new TestListener("Listener1");
            _listener2 = new TestListener("Listener2");
        }

        #region Subscribe Tests

        [Test]
        public void Subscribe_NewTrigger_CreatesSubscription()
        {
            // Act
            _eventBus.Subscribe(BattleTrigger.OnSwitchIn, _listener1);

            // Assert - No exception thrown
            Assert.Pass();
        }

        [Test]
        public void Subscribe_NullListener_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _eventBus.Subscribe(BattleTrigger.OnSwitchIn, null));
        }

        [Test]
        public void Subscribe_SameListenerTwice_OnlyAddsOnce()
        {
            // Arrange
            _eventBus.Subscribe(BattleTrigger.OnSwitchIn, _listener1);

            // Act
            _eventBus.Subscribe(BattleTrigger.OnSwitchIn, _listener1);

            // Assert - Should not throw, listener should only be subscribed once
            var actions = _eventBus.Publish(BattleTrigger.OnSwitchIn, _field).ToList();
            Assert.That(actions, Has.Count.EqualTo(1)); // Only one action from listener1
        }

        [Test]
        public void Subscribe_MultipleListeners_SubscribesAll()
        {
            // Arrange
            _eventBus.Subscribe(BattleTrigger.OnSwitchIn, _listener1);
            _eventBus.Subscribe(BattleTrigger.OnSwitchIn, _listener2);

            // Act
            var actions = _eventBus.Publish(BattleTrigger.OnSwitchIn, _field).ToList();

            // Assert
            Assert.That(actions, Has.Count.EqualTo(2));
        }

        #endregion

        #region Unsubscribe Tests

        [Test]
        public void Unsubscribe_SubscribedListener_RemovesSubscription()
        {
            // Arrange
            _eventBus.Subscribe(BattleTrigger.OnSwitchIn, _listener1);

            // Act
            _eventBus.Unsubscribe(BattleTrigger.OnSwitchIn, _listener1);

            // Assert
            var actions = _eventBus.Publish(BattleTrigger.OnSwitchIn, _field).ToList();
            Assert.That(actions, Is.Empty);
        }

        [Test]
        public void Unsubscribe_NullListener_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _eventBus.Unsubscribe(BattleTrigger.OnSwitchIn, null));
        }

        [Test]
        public void Unsubscribe_NotSubscribedListener_DoesNotThrow()
        {
            // Act & Assert - Should not throw
            Assert.DoesNotThrow(() =>
                _eventBus.Unsubscribe(BattleTrigger.OnSwitchIn, _listener1));
        }

        #endregion

        #region UnsubscribeAll Tests

        [Test]
        public void UnsubscribeAll_SubscribedToMultipleTriggers_RemovesAll()
        {
            // Arrange
            _eventBus.Subscribe(BattleTrigger.OnSwitchIn, _listener1);
            _eventBus.Subscribe(BattleTrigger.OnTurnEnd, _listener1);

            // Act
            _eventBus.UnsubscribeAll(_listener1);

            // Assert
            var actions1 = _eventBus.Publish(BattleTrigger.OnSwitchIn, _field).ToList();
            var actions2 = _eventBus.Publish(BattleTrigger.OnTurnEnd, _field).ToList();
            Assert.Multiple(() =>
            {
                Assert.That(actions1, Is.Empty);
                Assert.That(actions2, Is.Empty);
            });
        }

        [Test]
        public void UnsubscribeAll_NullListener_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _eventBus.UnsubscribeAll(null));
        }

        #endregion

        #region Publish Tests

        [Test]
        public void Publish_WithSubscribedListener_ReturnsActions()
        {
            // Arrange
            _eventBus.Subscribe(BattleTrigger.OnSwitchIn, _listener1);

            // Act
            var actions = _eventBus.Publish(BattleTrigger.OnSwitchIn, _field).ToList();

            // Assert
            Assert.That(actions, Has.Count.EqualTo(1));
            Assert.That(actions[0], Is.InstanceOf<MessageAction>());
        }

        [Test]
        public void Publish_NoSubscribers_ReturnsEmpty()
        {
            // Act
            var actions = _eventBus.Publish(BattleTrigger.OnSwitchIn, _field).ToList();

            // Assert
            Assert.That(actions, Is.Empty);
        }

        [Test]
        public void Publish_NullField_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _eventBus.Publish(BattleTrigger.OnSwitchIn, null));
        }

        [Test]
        public void Publish_MultipleListeners_ReturnsAllActions()
        {
            // Arrange
            _eventBus.Subscribe(BattleTrigger.OnSwitchIn, _listener1);
            _eventBus.Subscribe(BattleTrigger.OnSwitchIn, _listener2);

            // Act
            var actions = _eventBus.Publish(BattleTrigger.OnSwitchIn, _field).ToList();

            // Assert
            Assert.That(actions, Has.Count.EqualTo(2));
        }

        [Test]
        public void Publish_DifferentTrigger_DoesNotTrigger()
        {
            // Arrange
            _eventBus.Subscribe(BattleTrigger.OnSwitchIn, _listener1);

            // Act
            var actions = _eventBus.Publish(BattleTrigger.OnTurnEnd, _field).ToList();

            // Assert
            Assert.That(actions, Is.Empty);
        }

        #endregion

        #region Helper Classes

        private class TestListener : IBattleListener
        {
            private readonly string _name;

            public TestListener(string name)
            {
                _name = name;
            }

            public IEnumerable<BattleAction> OnTrigger(BattleTrigger trigger, BattleSlot holder, BattleField field)
            {
                return OnTrigger(trigger, holder, field, attacker: null);
            }

            public IEnumerable<BattleAction> OnTrigger(BattleTrigger trigger, BattleSlot holder, BattleField field, BattleSlot attacker)
            {
                yield return new MessageAction($"{_name} triggered on {trigger}");
            }
        }

        #endregion
    }
}
