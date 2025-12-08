using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Combat.Engine.BattleFlow;
using PokemonUltimate.Combat.Engine.BattleFlow.Definition;
using PokemonUltimate.Combat.Infrastructure.Events;
using PokemonUltimate.Combat.Utilities;
using PokemonUltimate.Core.Data.Constants;

namespace PokemonUltimate.Combat.Engine.BattleFlow.Steps
{
    /// <summary>
    /// Step que procesa el inicio de la batalla.
    /// Publica eventos de inicio de batalla y cambios iniciales de Pokemon.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `BATTLE_FLOW_STEPS_PROPOSAL.md`
    /// </remarks>
    public class BattleStartFlowStep : IBattleFlowStep
    {
        public string StepName => "Battle Start";

        public async Task<bool> ExecuteAsync(BattleFlowContext context)
        {
            if (context.Field == null)
                throw new InvalidOperationException("Field must be created before battle start.");

            // Raise battle start event
            BattleEventManager.RaiseBattleStart(context.Field);

            return await Task.FromResult(true);
        }
    }
}
