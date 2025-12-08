using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine.TurnFlow.Definition;
using PokemonUltimate.Combat.Handlers.Registry;

namespace PokemonUltimate.Combat.Engine.TurnFlow.Steps
{
    /// <summary>
    /// Step que verifica protección (Protect, Detect) para movimientos.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `DECOUPLED_STEPS_PROPOSAL.md`
    /// </remarks>
    public class MoveProtectionCheckStep : ITurnStep
    {
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        public string StepName => "Move Protection Check";
        public bool ExecuteEvenIfFainted => false;

        public MoveProtectionCheckStep(CombatEffectHandlerRegistry handlerRegistry)
        {
            _handlerRegistry = handlerRegistry ?? throw new ArgumentNullException(nameof(handlerRegistry));
        }

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            if (context.ProtectionChecks == null)
                context.ProtectionChecks = new System.Collections.Generic.Dictionary<UseMoveAction, bool>();

            var moveActions = context.SortedActions.OfType<UseMoveAction>()
                .Where(ma => context.MoveValidations?.GetValueOrDefault(ma, true) == true);

            foreach (var moveAction in moveActions)
            {
                context.Logger?.LogDebug(
                    $"Checking protection for {moveAction.Move.Name} against {moveAction.Target.Pokemon?.DisplayName}");

                var protectionHandler = _handlerRegistry.GetProtectionHandler();
                var protectionResult = protectionHandler.CheckProtection(
                    moveAction.Target,
                    moveAction.Move,
                    moveAction.CanBeBlocked);

                bool isProtected = protectionResult.IsProtected;
                context.ProtectionChecks[moveAction] = isProtected;

                if (isProtected)
                {
                    context.Logger?.LogDebug(
                        $"Move {moveAction.Move.Name} was protected: {protectionResult.ProtectionMessage}");

                    context.GeneratedActions.Add(
                        new MessageAction(protectionResult.ProtectionMessage));
                }
            }

            return await Task.FromResult(true);
        }
    }
}
