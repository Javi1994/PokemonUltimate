using System.Threading.Tasks;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;

namespace PokemonUltimate.Tests.Combat
{
    /// <summary>
    /// Simple action provider for testing CombatEngine.
    /// Returns a predefined action or a default message action.
    /// </summary>
    public class TestActionProvider : IActionProvider
    {
        private readonly BattleAction _actionToReturn;

        /// <summary>
        /// Creates a test provider that returns a specific action.
        /// </summary>
        public TestActionProvider(BattleAction actionToReturn)
        {
            _actionToReturn = actionToReturn;
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
            // Return null if _actionToReturn is null (for testing null handling)
            return Task.FromResult(_actionToReturn);
        }
    }
}

