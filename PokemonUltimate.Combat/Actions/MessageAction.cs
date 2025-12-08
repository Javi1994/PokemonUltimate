using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions.Validation;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.View.Definition;

namespace PokemonUltimate.Combat.Actions
{
    /// <summary>
    /// A simple action that displays a message.
    /// Used for battle text like "Pikachu used Thunderbolt!" or "It's super effective!".
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
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
        /// <param name="user">Optional slot that initiated this message (for logging purposes).</param>
        /// <exception cref="ArgumentNullException">If message is null.</exception>
        public MessageAction(string message, BattleSlot user = null) : base(user)
        {
            ActionValidators.ValidateMessage(message, nameof(message));
            Message = message;
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
            ActionValidators.ValidateView(view);
            return view.ShowMessage(Message);
        }
    }
}

