using System;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Engine.BattleFlow;
using PokemonUltimate.Combat.Engine.BattleFlow.Definition;

namespace PokemonUltimate.Combat.Engine.BattleFlow.Steps
{
    /// <summary>
    /// Step que valida el estado inicial del campo de batalla.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `BATTLE_FLOW_STEPS_PROPOSAL.md`
    /// </remarks>
    public class ValidateInitialStateStep : IBattleFlowStep
    {
        public string StepName => "Validate Initial State";

        public async Task<bool> ExecuteAsync(BattleFlowContext context)
        {
            if (context.Field == null)
                throw new InvalidOperationException("Field must be created before validation.");
            if (context.StateValidator == null)
            {
                context.Logger?.LogDebug("No state validator provided, skipping validation");
                return await Task.FromResult(true);
            }

            context.Logger?.LogDebug("Validating initial battle state");

            // Validate initial battle state
            var errors = context.StateValidator.ValidateField(context.Field);
            if (errors.Count > 0)
            {
                var errorMessage = "Battle state validation failed:\n" + string.Join("\n", errors);
                context.Logger?.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            return await Task.FromResult(true);
        }
    }
}
