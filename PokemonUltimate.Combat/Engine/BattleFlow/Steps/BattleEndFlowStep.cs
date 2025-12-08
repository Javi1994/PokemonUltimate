using System;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Engine.BattleFlow;
using PokemonUltimate.Combat.Engine.BattleFlow.Definition;
using PokemonUltimate.Combat.Infrastructure.Events;
using PokemonUltimate.Core.Data.Constants;

namespace PokemonUltimate.Combat.Engine.BattleFlow.Steps
{
    /// <summary>
    /// Step que procesa el fin de la batalla.
    /// Publica eventos de fin de batalla con estadísticas.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `BATTLE_FLOW_STEPS_PROPOSAL.md`
    /// </remarks>
    public class BattleEndFlowStep : IBattleFlowStep
    {
        public string StepName => "Battle End";

        public async Task<bool> ExecuteAsync(BattleFlowContext context)
        {
            if (context.Field == null)
                throw new InvalidOperationException("Field must be created before battle end.");
            if (context.Result == null)
                throw new InvalidOperationException("BattleResult must be set before battle end step.");

            // Calculate team statistics for battle ended event
            int playerFainted = context.Field?.PlayerSide?.Party?.Count(p => p.IsFainted) ?? 0;
            int playerTotal = context.Field?.PlayerSide?.Party?.Count ?? 0;
            int enemyFainted = context.Field?.EnemySide?.Party?.Count(p => p.IsFainted) ?? 0;
            int enemyTotal = context.Field?.EnemySide?.Party?.Count ?? 0;

            // Raise battle end event
            BattleEventManager.RaiseBattleEnd(context.Outcome, context.Field);

            return await Task.FromResult(true);
        }
    }
}
