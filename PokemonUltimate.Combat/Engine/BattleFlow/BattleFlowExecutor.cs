using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Engine.BattleFlow.Definition;
using PokemonUltimate.Combat.Engine.BattleFlow.Steps;
using PokemonUltimate.Combat.Engine.Validation.Definition;
using PokemonUltimate.Combat.Infrastructure.Constants;
using PokemonUltimate.Combat.Infrastructure.Events;
using PokemonUltimate.Combat.Infrastructure.Logging.Definition;

namespace PokemonUltimate.Combat.Engine.BattleFlow
{
    /// <summary>
    /// Ejecuta el flujo completo de batalla usando un sistema de steps modulares.
    /// Similar a TurnStepExecutor pero para toda la batalla (setup, ejecución, cleanup).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `BATTLE_FLOW_STEPS_PROPOSAL.md`
    /// </remarks>
    public class BattleFlowExecutor
    {
        private readonly List<IBattleFlowStep> _steps;
        private readonly IBattleStateValidator _stateValidator;
        private readonly IBattleLogger _logger;

        /// <summary>
        /// Crea un nuevo BattleFlowExecutor con los steps especificados.
        /// </summary>
        /// <param name="steps">Los steps a ejecutar en orden. No puede ser null.</param>
        /// <param name="stateValidator">El validador de estado de batalla. Si es null, no valida.</param>
        /// <param name="logger">El logger de batalla. Si es null, usa NullLogger.</param>
        public BattleFlowExecutor(
            IEnumerable<IBattleFlowStep> steps,
            IBattleStateValidator stateValidator = null,
            IBattleLogger logger = null)
        {
            if (steps == null)
                throw new ArgumentNullException(nameof(steps));

            _steps = new List<IBattleFlowStep>(steps);
            _stateValidator = stateValidator;
            _logger = logger ?? new Infrastructure.Logging.NullBattleLogger();
        }

        /// <summary>
        /// Ejecuta todos los steps del flujo de batalla en orden.
        /// </summary>
        /// <param name="context">El contexto del flujo de batalla. No puede ser null.</param>
        /// <exception cref="ArgumentNullException">Si context es null.</exception>
        public async Task ExecuteFlow(BattleFlowContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Set debug mode for events
            BattleEventManager.IsDebugMode = context.IsDebugMode;

            foreach (var step in _steps)
            {
                _logger.LogDebug($"Executing battle flow step: {step.StepName}");

                var stopwatch = Stopwatch.StartNew();
                BattleEventManager.RaiseStepStarted(step.StepName, StepType.BattleFlow, context.Field);

                var shouldContinue = await step.ExecuteAsync(context).ConfigureAwait(false);

                stopwatch.Stop();
                BattleEventManager.RaiseStepFinished(step.StepName, StepType.BattleFlow, context.Field, stopwatch.Elapsed, shouldContinue);
                BattleEventManager.RaiseStepExecuted(step.StepName, StepType.BattleFlow, context.Field, stopwatch.Elapsed);

                if (!shouldContinue)
                {
                    _logger.LogDebug($"Step {step.StepName} requested to stop execution.");
                    break;
                }

                // Check if battle is complete (for execution steps)
                if (context.IsBattleComplete)
                {
                    _logger.LogDebug($"Battle completed, stopping flow execution at step: {step.StepName}");
                    break;
                }
            }
        }
    }
}
