using System;
using System.Threading.Tasks;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;

namespace PokemonUltimate.Tests.Systems.Combat.Engine
{
    /// <summary>
    /// Simple action provider for testing CombatEngine.
    /// Returns a predefined action or a default message action.
    /// </summary>
    public class TestActionProvider : IActionProvider
    {
        private readonly BattleAction _actionToReturn;
        private readonly Func<BattleField, BattleSlot, BattleAction> _actionFactory;

        /// <summary>
        /// Creates a test provider that returns a specific action.
        /// </summary>
        public TestActionProvider(BattleAction actionToReturn)
        {
            _actionToReturn = actionToReturn;
        }

        /// <summary>
        /// Creates a test provider that uses a factory function to create actions dynamically.
        /// </summary>
        public TestActionProvider(Func<BattleField, BattleSlot, BattleAction> actionFactory)
        {
            _actionFactory = actionFactory ?? throw new ArgumentNullException(nameof(actionFactory));
        }

        /// <summary>
        /// Creates a test provider that returns a message action.
        /// </summary>
        public TestActionProvider(string message = "Test action")
        {
            _actionToReturn = new MessageAction(message);
        }

        public Task<BattleAction> GetAction(BattleField field, BattleSlot mySlot)
        {
            if (_actionFactory != null)
            {
                return Task.FromResult(_actionFactory(field, mySlot));
            }
            // Return null if _actionToReturn is null (for testing null handling)
            return Task.FromResult(_actionToReturn);
        }
    }
}

