using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Infrastructure.Messages.Definition;
using PokemonUltimate.Combat.Moves.Definition;

namespace PokemonUltimate.Combat.Moves.Steps
{
    /// <summary>
    /// Deducts PP and generates move message.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class PPConsumptionStep : IMoveExecutionStep
    {
        private readonly IBattleMessageFormatter _messageFormatter;

        /// <summary>
        /// Creates a new PP consumption step.
        /// </summary>
        /// <param name="messageFormatter">The message formatter. Cannot be null.</param>
        public PPConsumptionStep(IBattleMessageFormatter messageFormatter)
        {
            _messageFormatter = messageFormatter ?? throw new System.ArgumentNullException(nameof(messageFormatter));
        }

        /// <summary>
        /// Gets the execution order.
        /// </summary>
        public int ExecutionOrder => 60;

        /// <summary>
        /// Deducts PP and generates move message.
        /// </summary>
        public MoveExecutionStepResult Process(MoveExecutionContext context)
        {
            // Deduct PP and generate move message
            context.MoveInstance.Use();
            context.PPConsumed = true;
            context.Actions.Add(new MessageAction(_messageFormatter.FormatMoveUsed(context.User.Pokemon, context.Move)));

            // Show focusing message for Focus Punch (if not already failed)
            if (context.HasFocusPunchEffect)
            {
                context.Actions.Add(new MessageAction(_messageFormatter.Format(
                    Localization.Constants.LocalizationKey.MoveFocusing,
                    context.User.Pokemon.DisplayName)));
            }

            return MoveExecutionStepResult.Continue;
        }
    }
}
