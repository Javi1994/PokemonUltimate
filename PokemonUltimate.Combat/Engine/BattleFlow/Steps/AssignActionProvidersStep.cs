using System;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Engine.BattleFlow;
using PokemonUltimate.Combat.Engine.BattleFlow.Definition;

namespace PokemonUltimate.Combat.Engine.BattleFlow.Steps
{
    /// <summary>
    /// Step que asigna los ActionProviders a cada slot del campo.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `BATTLE_FLOW_STEPS_PROPOSAL.md`
    /// </remarks>
    public class AssignActionProvidersStep : IBattleFlowStep
    {
        public string StepName => "Assign Action Providers";

        public async Task<bool> ExecuteAsync(BattleFlowContext context)
        {
            if (context.Field == null)
                throw new InvalidOperationException("Field must be created before assigning action providers.");

            context.Logger?.LogDebug("Assigning action providers to slots");

            // Assign action providers to slots
            foreach (var slot in context.Field.PlayerSide.Slots)
            {
                slot.ActionProvider = context.PlayerProvider;
            }

            foreach (var slot in context.Field.EnemySide.Slots)
            {
                slot.ActionProvider = context.EnemyProvider;
            }

            return await Task.FromResult(true);
        }
    }
}
