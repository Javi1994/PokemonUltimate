using System.Collections.Generic;
using PokemonUltimate.Combat.Engine.Service;
using PokemonUltimate.Combat.Engine.Validation.Definition;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Constants;
using PokemonUltimate.Combat.Infrastructure.Logging.Definition;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Combat.View.Definition;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.Engine.BattleFlow
{
    /// <summary>
    /// Contexto para el flujo completo de batalla (setup, ejecución, cleanup).
    /// Similar a TurnContext pero para toda la batalla.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `BATTLE_FLOW_STEPS_PROPOSAL.md`
    /// </remarks>
    public class BattleFlowContext
    {
        // Input parameters (set at start)
        public BattleRules Rules { get; set; }
        public IReadOnlyList<PokemonInstance> PlayerParty { get; set; }
        public IReadOnlyList<PokemonInstance> EnemyParty { get; set; }
        public IActionProvider PlayerProvider { get; set; }
        public IActionProvider EnemyProvider { get; set; }
        public IBattleView View { get; set; }

        // Created during flow (populated by steps)
        public BattleField Field { get; set; }
        public BattleQueueService QueueService { get; set; }
        public BattleResult Result { get; set; }
        public BattleOutcome Outcome { get; set; } = BattleOutcome.Ongoing;

        // Dependencies (set at start or created during flow)
        public TurnEngine TurnEngine { get; set; }
        public IBattleStateValidator StateValidator { get; set; }
        public IBattleLogger Logger { get; set; }
        public CombatEngine Engine { get; set; }
        public Utilities.TargetResolver TargetResolver { get; set; }

        // Debug mode
        public bool IsDebugMode { get; set; } = false;

        // Helper properties
        public bool IsInitialized => Field != null && QueueService != null;
        public bool IsBattleComplete => Outcome != BattleOutcome.Ongoing;
    }
}
