using System;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Damage.Definition;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Combat.Engine.BattleFlow;
using PokemonUltimate.Combat.Engine.BattleFlow.Definition;
using PokemonUltimate.Combat.Engine.Service;
using PokemonUltimate.Combat.Handlers.Effects;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Combat.Infrastructure.Factories;
using PokemonUltimate.Combat.Infrastructure.Logging.Definition;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Combat.Utilities;

namespace PokemonUltimate.Combat.Engine.BattleFlow.Steps
{
    /// <summary>
    /// Step que crea todas las dependencias necesarias para la ejecución de turnos.
    /// Crea TurnExecutor y handlers de efectos.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `BATTLE_FLOW_STEPS_PROPOSAL.md`
    /// </remarks>
    public class CreateDependenciesStep : IBattleFlowStep
    {
        private readonly IRandomProvider _randomProvider;
        private readonly IDamagePipeline _damagePipeline;
        private readonly CombatEffectHandlerRegistry _handlerRegistry;
        private readonly AccuracyChecker _accuracyChecker;

        public string StepName => "Create Dependencies";

        public CreateDependenciesStep(
            IRandomProvider randomProvider,
            IDamagePipeline damagePipeline,
            CombatEffectHandlerRegistry handlerRegistry,
            AccuracyChecker accuracyChecker)
        {
            _randomProvider = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider));
            _damagePipeline = damagePipeline ?? throw new ArgumentNullException(nameof(damagePipeline));
            _handlerRegistry = handlerRegistry ?? throw new ArgumentNullException(nameof(handlerRegistry));
            _accuracyChecker = accuracyChecker ?? throw new ArgumentNullException(nameof(accuracyChecker));
        }

        public async Task<bool> ExecuteAsync(BattleFlowContext context)
        {
            if (context.Field == null || context.QueueService == null)
                throw new InvalidOperationException("Field and Queue must be created before creating dependencies.");

            context.Logger?.LogDebug("Creating turn execution dependencies");

            // Create shared dependencies for the battle
            var turnOrderResolver = new TurnOrderResolver(_randomProvider);
            var damageContextFactory = new DamageContextFactory();

            // Use TargetResolver from context (created in CombatEngine)
            var targetResolver = context.TargetResolver ?? throw new InvalidOperationException("TargetResolver must be set in BattleFlowContext before creating dependencies.");

            // Initialize handler registry if needed
            if (!_handlerRegistry.IsInitialized)
            {
                _handlerRegistry.Initialize(_randomProvider, damageContextFactory);
            }

            // Create end-of-turn effect handlers using factory
            var servicesFactory = new BattleServicesFactory(damageContextFactory, _handlerRegistry);
            var (statusDamageHandler, weatherDamageHandler, terrainHealingHandler, entryHazardHandler) = servicesFactory.CreateEndOfTurnHandlers();

            // Create TurnExecutor with all dependencies
            context.TurnEngine = new TurnEngine(
                context.QueueService,
                context.View,
                context.StateValidator,
                context.Logger,
                turnOrderResolver,
                targetResolver,
                _randomProvider,
                _damagePipeline,
                _accuracyChecker,
                _handlerRegistry,
                damageContextFactory,
                statusDamageHandler,
                weatherDamageHandler,
                terrainHealingHandler,
                entryHazardHandler);

            return await Task.FromResult(true);
        }
    }
}
