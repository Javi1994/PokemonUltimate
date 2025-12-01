using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonUltimate.Combat.Actions
{
    /// <summary>
    /// A simple action that displays a message.
    /// Used for battle text like "Pikachu used Thunderbolt!" or "It's super effective!".
    /// </summary>
    public class MessageAction : BattleAction
    {
        /// <summary>
        /// The message to display.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Creates a message action.
        /// </summary>
        /// <param name="message">The message to display. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If message is null.</exception>
        public MessageAction(string message) : base(null)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        /// <summary>
        /// Message actions don't modify game state.
        /// </summary>
        public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            return Enumerable.Empty<BattleAction>();
        }

        /// <summary>
        /// Shows the message to the player.
        /// </summary>
        public override Task ExecuteVisual(IBattleView view)
        {
            return view.ShowMessage(Message);
        }
    }
}

