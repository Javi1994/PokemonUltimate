using System;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Engine.BattleFlow;
using PokemonUltimate.Combat.Engine.BattleFlow.Definition;
using PokemonUltimate.Combat.Infrastructure.Factories.Definition;

namespace PokemonUltimate.Combat.Engine.BattleFlow.Steps
{
    /// <summary>
    /// Step que crea la cola de batalla usando el factory.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `BATTLE_FLOW_STEPS_PROPOSAL.md`
    /// </remarks>
    public class CreateQueueStep : IBattleFlowStep
    {
        private readonly IBattleQueueFactory _battleQueueFactory;

        public string StepName => "Create Battle Queue";

        public CreateQueueStep(IBattleQueueFactory battleQueueFactory)
        {
            _battleQueueFactory = battleQueueFactory ?? throw new ArgumentNullException(nameof(battleQueueFactory));
        }

        public async Task<bool> ExecuteAsync(BattleFlowContext context)
        {
            context.Logger?.LogDebug("Creating battle queue");

            // Create BattleQueue using factory
            context.QueueService = _battleQueueFactory.Create();

            return await Task.FromResult(true);
        }
    }
}
