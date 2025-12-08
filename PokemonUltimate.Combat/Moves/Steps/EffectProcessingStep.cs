using PokemonUltimate.Combat.Moves.Definition;
using PokemonUltimate.Combat.Moves.Orchestrator;

namespace PokemonUltimate.Combat.Moves.Steps
{
    /// <summary>
    /// Processes move effects (damage and other effects).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class EffectProcessingStep : IMoveExecutionStep
    {
        private readonly MoveEffectOrchestrator _effectOrchestrator;

        /// <summary>
        /// Creates a new effect processing step.
        /// </summary>
        /// <param name="effectOrchestrator">The move effect orchestrator. Cannot be null.</param>
        public EffectProcessingStep(MoveEffectOrchestrator effectOrchestrator)
        {
            _effectOrchestrator = effectOrchestrator ?? throw new System.ArgumentNullException(nameof(effectOrchestrator));
        }

        /// <summary>
        /// Gets the execution order.
        /// </summary>
        public int ExecutionOrder => 100;

        /// <summary>
        /// Processes move effects.
        /// </summary>
        public MoveExecutionStepResult Process(MoveExecutionContext context)
        {
            // Process all effects (damage and other effects) using orchestrator
            var result = _effectOrchestrator.ProcessAllEffects(
                context.User,
                context.Target,
                context.Move,
                context.Field,
                context.Actions);

            // Set damage dealt in context
            context.DamageDealt = result.TotalDamage;

            // If processing should stop (e.g., semi-invulnerable charge turn), stop execution
            if (result.ShouldStop)
            {
                context.ShouldStop = true;
                return MoveExecutionStepResult.Stop;
            }

            return MoveExecutionStepResult.Continue;
        }
    }
}
