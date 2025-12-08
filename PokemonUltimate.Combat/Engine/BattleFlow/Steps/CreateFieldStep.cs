using System;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Engine.BattleFlow;
using PokemonUltimate.Combat.Engine.BattleFlow.Definition;
using PokemonUltimate.Combat.Infrastructure.Factories.Definition;
using PokemonUltimate.Core.Data.Constants;

namespace PokemonUltimate.Combat.Engine.BattleFlow.Steps
{
    /// <summary>
    /// Step que crea el campo de batalla usando el factory.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `BATTLE_FLOW_STEPS_PROPOSAL.md`
    /// </remarks>
    public class CreateFieldStep : IBattleFlowStep
    {
        private readonly IBattleFieldFactory _battleFieldFactory;

        public string StepName => "Create Battle Field";

        public CreateFieldStep(IBattleFieldFactory battleFieldFactory)
        {
            _battleFieldFactory = battleFieldFactory ?? throw new ArgumentNullException(nameof(battleFieldFactory));
        }

        public async Task<bool> ExecuteAsync(BattleFlowContext context)
        {
            if (context.Rules == null)
                throw new InvalidOperationException("BattleRules must be set in context before creating field.");
            if (context.PlayerParty == null)
                throw new InvalidOperationException("PlayerParty must be set in context before creating field.");
            if (context.EnemyParty == null)
                throw new InvalidOperationException("EnemyParty must be set in context before creating field.");

            context.Logger?.LogDebug($"Creating battle field: {context.Rules.PlayerSlots}v{context.Rules.EnemySlots}");

            // Create BattleField using factory
            context.Field = _battleFieldFactory.Create(
                context.Rules,
                context.PlayerParty,
                context.EnemyParty);

            return await Task.FromResult(true);
        }
    }
}
