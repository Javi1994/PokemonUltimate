using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;

namespace PokemonUltimate.Tests.Combat.Actions
{
    /// <summary>
    /// Tests for BattleAction and MessageAction.
    /// </summary>
    [TestFixture]
    public class BattleActionTests
    {
        #region MessageAction Tests

        [Test]
        public void MessageAction_Constructor_StoresMessage()
        {
            var action = new MessageAction("Hello!");

            Assert.That(action.Message, Is.EqualTo("Hello!"));
        }

        [Test]
        public void MessageAction_Constructor_NullMessage_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MessageAction(null));
        }

        [Test]
        public void MessageAction_User_IsNull()
        {
            var action = new MessageAction("Test");

            Assert.That(action.User, Is.Null);
        }

        [Test]
        public void MessageAction_Priority_IsZero()
        {
            var action = new MessageAction("Test");

            Assert.That(action.Priority, Is.EqualTo(0));
        }

        [Test]
        public void MessageAction_CanBeBlocked_IsFalse()
        {
            var action = new MessageAction("Test");

            Assert.That(action.CanBeBlocked, Is.False);
        }

        [Test]
        public void MessageAction_ExecuteLogic_ReturnsEmpty()
        {
            var action = new MessageAction("Test");

            var reactions = action.ExecuteLogic(null);

            Assert.That(reactions, Is.Empty);
        }

        [Test]
        public async Task MessageAction_ExecuteVisual_CallsShowMessage()
        {
            var action = new MessageAction("Test message");
            var mockView = new MockBattleView();

            await action.ExecuteVisual(mockView);

            Assert.That(mockView.LastMessage, Is.EqualTo("Test message"));
            Assert.That(mockView.ShowMessageCalled, Is.True);
        }

        #endregion

        #region Mock Battle View

        /// <summary>
        /// Simple mock for testing visual calls.
        /// </summary>
        private class MockBattleView : IBattleView
        {
            public bool ShowMessageCalled { get; private set; }
            public string LastMessage { get; private set; }

            public Task ShowMessage(string message)
            {
                ShowMessageCalled = true;
                LastMessage = message;
                return Task.CompletedTask;
            }

            public Task PlayDamageAnimation(BattleSlot slot) => Task.CompletedTask;
            public Task UpdateHPBar(BattleSlot slot) => Task.CompletedTask;
            public Task PlayMoveAnimation(BattleSlot user, BattleSlot target, string moveId) => Task.CompletedTask;
            public Task PlayFaintAnimation(BattleSlot slot) => Task.CompletedTask;
            public Task PlayStatusAnimation(BattleSlot slot, string statusName) => Task.CompletedTask;
            public Task ShowStatChange(BattleSlot slot, string statName, int stages) => Task.CompletedTask;
            public Task PlaySwitchOutAnimation(BattleSlot slot) => Task.CompletedTask;
            public Task PlaySwitchInAnimation(BattleSlot slot) => Task.CompletedTask;
        }

        #endregion
    }
}

