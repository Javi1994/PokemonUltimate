using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Engine.Service;
using PokemonUltimate.Combat.Engine.TurnFlow.Definition;
using PokemonUltimate.Combat.Engine.Validation.Definition;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Constants;
using PokemonUltimate.Combat.Infrastructure.Events;
using PokemonUltimate.Combat.Infrastructure.Logging.Definition;

namespace PokemonUltimate.Combat.Engine.TurnFlow
{
    /// <summary>
    /// Ejecuta un turno completo usando un sistema de steps modulares.
    /// Similar al DamagePipeline, cada step procesa el contexto en orden.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class TurnStepExecutor
    {
        private readonly List<ITurnStep> _steps;
        private readonly IBattleStateValidator _stateValidator;
        private readonly IBattleLogger _logger;

        /// <summary>
        /// Crea un nuevo TurnStepExecutor con los steps especificados.
        /// </summary>
        /// <param name="steps">Los steps a ejecutar en orden. No puede ser null.</param>
        /// <param name="stateValidator">El validador de estado de batalla. Si es null, no valida.</param>
        /// <param name="logger">El logger de batalla. Si es null, usa NullLogger.</param>
        public TurnStepExecutor(
            IEnumerable<ITurnStep> steps,
            IBattleStateValidator stateValidator = null,
            IBattleLogger logger = null)
        {
            if (steps == null)
                throw new ArgumentNullException(nameof(steps));

            _steps = new List<ITurnStep>(steps);
            _stateValidator = stateValidator;
            _logger = logger ?? new Infrastructure.Logging.NullBattleLogger();
        }

        /// <summary>
        /// Ejecuta todos los steps del turno en orden.
        /// </summary>
        /// <param name="context">El contexto del turno. No puede ser null.</param>
        /// <exception cref="ArgumentNullException">Si context es null.</exception>
        public async Task ExecuteTurn(TurnContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.Field == null)
                throw new InvalidOperationException("TurnContext.Field must be initialized.");

            foreach (var step in _steps)
            {
                // Skip step if there are fainted Pokemon and step doesn't handle them
                if (context.HasFaintedPokemon && !step.ExecuteEvenIfFainted)
                {
                    // Check if battle ended
                    if (IsBattleOver(context.Field))
                    {
                        _logger.LogInfo($"Battle ended, stopping turn execution at step: {step.StepName}");
                        break;
                    }
                }

                _logger.LogDebug($"Executing step: {step.StepName}");

                var stopwatch = Stopwatch.StartNew();
                BattleEventManager.RaiseStepStarted(step.StepName, StepType.Turn, context.Field);

                var shouldContinue = await step.ExecuteAsync(context).ConfigureAwait(false);

                stopwatch.Stop();
                BattleEventManager.RaiseStepFinished(step.StepName, StepType.Turn, context.Field, stopwatch.Elapsed, shouldContinue);
                BattleEventManager.RaiseStepExecuted(step.StepName, StepType.Turn, context.Field, stopwatch.Elapsed);

                // Check if battle ended after each step (especially after FaintedPokemonCheckStep)
                if (IsBattleOver(context.Field))
                {
                    _logger.LogInfo($"Battle ended, stopping turn execution at step: {step.StepName}");
                    break;
                }

                if (!shouldContinue)
                {
                    _logger.LogDebug($"Step {step.StepName} requested to stop execution.");
                    break;
                }
            }

            // Final validation
            ValidateBattleState(context);
        }

        /// <summary>
        /// Valida el estado de batalla después del turno.
        /// </summary>
        private void ValidateBattleState(TurnContext context)
        {
            if (_stateValidator == null)
                return;

            var errors = _stateValidator.ValidateField(context.Field);
            if (errors.Count > 0)
            {
                var errorMessage = "Battle state validation failed:\n" + string.Join("\n", errors);
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
        }

        /// <summary>
        /// Verifica si la batalla ha terminado.
        /// Uses the same logic as BattleArbiterService to ensure consistency.
        /// </summary>
        private bool IsBattleOver(BattleField field)
        {
            if (field == null)
                return false;

            // Use BattleArbiterService to check outcome consistently
            var outcome = BattleArbiterService.CheckOutcome(field);
            return outcome != BattleOutcome.Ongoing;
        }
    }
}
