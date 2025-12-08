using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Damage.Definition;
using PokemonUltimate.Combat.Engine.Service;
using PokemonUltimate.Combat.Engine.TurnFlow;
using PokemonUltimate.Combat.Engine.TurnFlow.Definition;
using PokemonUltimate.Combat.Engine.TurnFlow.Steps;
using PokemonUltimate.Combat.Engine.Validation.Definition;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Combat.Infrastructure.Factories;
using PokemonUltimate.Combat.Infrastructure.Logging;
using PokemonUltimate.Combat.Infrastructure.Logging.Definition;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Combat.Utilities;
using PokemonUltimate.Combat.View.Definition;

namespace PokemonUltimate.Combat.Engine
{
    /// <summary>
    /// Executes a single turn of battle using a modular step-based system.
    /// Coordinates the execution of all phases in the correct order.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md` and `TURN_STEPS_PROPOSAL.md`
    /// </remarks>
    public class TurnEngine
    {
        private readonly TurnStepExecutor _stepExecutor;
        private readonly BattleQueueService _queueService;
        private readonly IBattleView _view;
        private readonly IBattleStateValidator _stateValidator;
        private readonly IBattleLogger _logger;
        private readonly TurnOrderResolver _turnOrderResolver;
        private readonly TargetResolver _targetResolver;

        /// <summary>
        /// Creates a new TurnExecutor with all required dependencies.
        /// </summary>
        public TurnEngine(
            BattleQueueService queueService,
            IBattleView view,
            IBattleStateValidator stateValidator,
            IBattleLogger logger,
            TurnOrderResolver turnOrderResolver,
            TargetResolver targetResolver,
            IRandomProvider randomProvider,
            IDamagePipeline damagePipeline,
            AccuracyChecker accuracyChecker,
            CombatEffectHandlerRegistry handlerRegistry,
            DamageContextFactory damageContextFactory,
            Handlers.Effects.StatusDamageHandler statusDamageHandler,
            Handlers.Effects.WeatherDamageHandler weatherDamageHandler,
            Handlers.Effects.TerrainHealingHandler terrainHealingHandler,
            Handlers.Effects.EntryHazardHandler entryHazardHandler)
        {
            if (queueService == null)
                throw new ArgumentNullException(nameof(queueService));
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            if (stateValidator == null)
                throw new ArgumentNullException(nameof(stateValidator));
            if (turnOrderResolver == null)
                throw new ArgumentNullException(nameof(turnOrderResolver));
            if (targetResolver == null)
                throw new ArgumentNullException(nameof(targetResolver));
            if (randomProvider == null)
                throw new ArgumentNullException(nameof(randomProvider));
            if (damagePipeline == null)
                throw new ArgumentNullException(nameof(damagePipeline));
            if (accuracyChecker == null)
                throw new ArgumentNullException(nameof(accuracyChecker));
            if (handlerRegistry == null)
                throw new ArgumentNullException(nameof(handlerRegistry));
            if (damageContextFactory == null)
                throw new ArgumentNullException(nameof(damageContextFactory));
            if (statusDamageHandler == null)
                throw new ArgumentNullException(nameof(statusDamageHandler));
            if (weatherDamageHandler == null)
                throw new ArgumentNullException(nameof(weatherDamageHandler));
            if (terrainHealingHandler == null)
                throw new ArgumentNullException(nameof(terrainHealingHandler));
            if (entryHazardHandler == null)
                throw new ArgumentNullException(nameof(entryHazardHandler));

            _queueService = queueService;
            _view = view;
            _stateValidator = stateValidator;
            _logger = logger ?? new NullBattleLogger();
            _turnOrderResolver = turnOrderResolver;
            _targetResolver = targetResolver;

            // Create steps for the turn pipeline with explicit decoupled steps (no processors)
            var steps = new List<ITurnStep>
            {
                // === FASE 1: PREPARACIÓN ===
                new TurnStartStep(),
                new ActionCollectionStep(randomProvider),
                new TargetResolutionStep(), // Resolver targets y aplicar redirecciones
                new ActionSortingStep(turnOrderResolver),

                // === FASE 2: VALIDACIÓN DE MOVIMIENTOS ===
                new MoveValidationStep(handlerRegistry, randomProvider),
                new MoveProtectionCheckStep(handlerRegistry),
                new MoveAccuracyCheckStep(handlerRegistry, accuracyChecker),

                // === FASE 3: EFECTOS ANTES DEL MOVIMIENTO ===
                new BeforeMoveEffectsStep(handlerRegistry),
                new ProcessGeneratedActionsStep(), // Procesar acciones generadas

                // === FASE 4: CÁLCULO DE DAÑO ===
                new MoveDamageCalculationStep(damagePipeline),

                // === FASE 5: APLICACIÓN DE DAÑO ===
                new MoveDamageApplicationStep(handlerRegistry),
                new ProcessGeneratedActionsStep(), // Procesar DamageActions

                // === FASE 5.5: ANIMACIONES DE MOVIMIENTOS ===
                new MoveAnimationStep(), // Ejecutar animaciones de movimientos

                // === FASE 6: EFECTOS REACTIVOS ===
                new DamageTakenEffectsStep(handlerRegistry),
                new ContactReceivedEffectsStep(handlerRegistry),
                new ProcessGeneratedActionsStep(), // Procesar reacciones

                // === FASE 7: EFECTOS DEL MOVIMIENTO ===
                new MoveEffectProcessingStep(handlerRegistry),
                new ProcessGeneratedActionsStep(), // Procesar efectos del movimiento

                // === FASE 8: EFECTOS DESPUÉS DEL MOVIMIENTO ===
                new AfterMoveEffectsStep(handlerRegistry),
                new ProcessGeneratedActionsStep(), // Procesar efectos después

                // === FASE 9: OTRAS ACCIONES ===
                new SwitchActionsStep(),
                new ProcessGeneratedActionsStep(), // Procesar cambios
                new SwitchInEffectsStep(entryHazardHandler, handlerRegistry), // Procesar efectos de entrada
                new ProcessGeneratedActionsStep(), // Procesar efectos de entrada
                new StatusActionsStep(),
                new ProcessGeneratedActionsStep(), // Procesar acciones de status

                // === FASE 10: VERIFICACIÓN DE DEBILITADOS ===
                new FaintedPokemonCheckStep(randomProvider, logger),

                // === FASE 11: FIN DE TURNO ===
                new EndOfTurnEffectsStep(statusDamageHandler, weatherDamageHandler, terrainHealingHandler),
                new ProcessGeneratedActionsStep(), // Procesar efectos de fin de turno
                new FaintedPokemonCheckStep(randomProvider, logger),
                new DurationDecrementStep(),
                new TurnEndStep(handlerRegistry),
                new ProcessGeneratedActionsStep(), // Procesar efectos de fin de turno
                new FaintedPokemonCheckStep(randomProvider, logger)
            };

            // Create step executor
            _stepExecutor = new TurnStepExecutor(steps, stateValidator, _logger);
        }

        /// <summary>
        /// Executes a single turn of battle using the step-based pipeline.
        /// </summary>
        /// <param name="field">The battlefield. Cannot be null.</param>
        /// <param name="turnNumber">The current turn number (for event publishing).</param>
        /// <exception cref="ArgumentNullException">If field is null.</exception>
        public async Task ExecuteTurn(BattleField field, int turnNumber = 0)
        {
            if (field == null)
                throw new InvalidOperationException("BattleField must be initialized before running turn.");

            // Create turn context
            var context = new TurnContext
            {
                Field = field,
                QueueService = _queueService,
                View = _view,
                TurnNumber = turnNumber,
                StateValidator = _stateValidator,
                Logger = _logger,
                TurnOrderResolver = _turnOrderResolver,
                TargetResolver = _targetResolver
            };

            await _stepExecutor.ExecuteTurn(context);
        }
    }
}
