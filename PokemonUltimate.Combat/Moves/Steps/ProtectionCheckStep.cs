using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Combat.Moves.Definition;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Moves.Steps
{
    /// <summary>
    /// Checks protection using Protection Handler.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class ProtectionCheckStep : IMoveExecutionStep
    {
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        /// <summary>
        /// Creates a new protection check step.
        /// </summary>
        /// <param name="handlerRegistry">The handler registry. Cannot be null.</param>
        public ProtectionCheckStep(CombatEffectHandlerRegistry handlerRegistry)
        {
            _handlerRegistry = handlerRegistry ?? throw new System.ArgumentNullException(nameof(handlerRegistry));
        }

        /// <summary>
        /// Gets the execution order.
        /// </summary>
        public int ExecutionOrder => 70;

        /// <summary>
        /// Checks protection.
        /// </summary>
        public MoveExecutionStepResult Process(MoveExecutionContext context)
        {
            var protectionHandler = _handlerRegistry.GetProtectionHandler();
            var protectionResult = protectionHandler.CheckProtection(context.Target, context.Move, context.CanBeBlocked);

            if (protectionResult.IsProtected)
            {
                context.Actions.Add(new MessageAction(protectionResult.ProtectionMessage));

                // Remove focusing status if move was blocked
                if (context.HasFocusPunchEffect)
                {
                    context.User.RemoveVolatileStatus(VolatileStatus.Focusing);
                }

                context.ShouldStop = true;
                return MoveExecutionStepResult.Stop;
            }

            return MoveExecutionStepResult.Continue;
        }
    }
}
