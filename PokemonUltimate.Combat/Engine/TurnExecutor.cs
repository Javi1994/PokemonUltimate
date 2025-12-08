using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Engine.Processors;
using PokemonUltimate.Combat.Engine.Processors.Observer;
using PokemonUltimate.Combat.Engine.TurnSteps;
using PokemonUltimate.Combat.Engine.TurnSteps.Steps;
using PokemonUltimate.Combat.Engine.Validation.Definition;
using PokemonUltimate.Combat.Events.Definition;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Logging;
using PokemonUltimate.Combat.Infrastructure.Logging.Definition;
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
    public class TurnExecutor
    {
        private readonly TurnStepExecutor _stepExecutor;
        private readonly ActionProcessorObserver _actionProcessorObserver;
        private readonly BattleQueue _queue;
        private readonly IBattleView _view;
        private readonly IBattleEventBus _eventBus;
        private readonly IBattleStateValidator _stateValidator;
        private readonly IBattleLogger _logger;
        private readonly TurnOrderResolver _turnOrderResolver;

        /// <summary>
        /// Creates a new TurnExecutor with all required processors.
        /// </summary>
        public TurnExecutor(
            TurnStartProcessor turnStartProcessor,
            ActionCollectionProcessor actionCollectionProcessor,
            ActionSortingProcessor actionSortingProcessor,
            FaintedPokemonSwitchingProcessor faintedSwitchingProcessor,
            EndOfTurnEffectsProcessor endOfTurnEffectsProcessor,
            DurationDecrementProcessor durationDecrementProcessor,
            TurnEndProcessor turnEndProcessor,
            IBattleEventBus eventBus,
            BattleQueue queue,
            IBattleView view,
            IBattleStateValidator stateValidator,
            IBattleLogger logger,
            TurnOrderResolver turnOrderResolver,
            DamageTakenProcessor damageTakenProcessor = null,
            ContactReceivedProcessor contactReceivedProcessor = null,
            WeatherChangeProcessor weatherChangeProcessor = null,
            SwitchInProcessor switchInProcessor = null)
        {
            if (turnStartProcessor == null)
                throw new ArgumentNullException(nameof(turnStartProcessor));
            if (actionCollectionProcessor == null)
                throw new ArgumentNullException(nameof(actionCollectionProcessor));
            if (actionSortingProcessor == null)
                throw new ArgumentNullException(nameof(actionSortingProcessor));
            if (faintedSwitchingProcessor == null)
                throw new ArgumentNullException(nameof(faintedSwitchingProcessor));
            if (endOfTurnEffectsProcessor == null)
                throw new ArgumentNullException(nameof(endOfTurnEffectsProcessor));
            if (durationDecrementProcessor == null)
                throw new ArgumentNullException(nameof(durationDecrementProcessor));
            if (turnEndProcessor == null)
                throw new ArgumentNullException(nameof(turnEndProcessor));
            if (eventBus == null)
                throw new ArgumentNullException(nameof(eventBus));
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            if (stateValidator == null)
                throw new ArgumentNullException(nameof(stateValidator));
            if (turnOrderResolver == null)
                throw new ArgumentNullException(nameof(turnOrderResolver));

            _queue = queue;
            _view = view;
            _eventBus = eventBus;
            _stateValidator = stateValidator;
            _logger = logger ?? new NullBattleLogger();
            _turnOrderResolver = turnOrderResolver;

            // Create and register ActionProcessorObserver to handle processor calls
            _actionProcessorObserver = new ActionProcessorObserver(
                queue,
                damageTakenProcessor,
                contactReceivedProcessor,
                weatherChangeProcessor,
                switchInProcessor);
            queue.AddObserver(_actionProcessorObserver);

            // Create steps for the turn pipeline
            var steps = new List<ITurnStep>
            {
                new TurnStartStep(turnStartProcessor),
                new ActionCollectionStep(actionCollectionProcessor),
                new ActionSortingStep(actionSortingProcessor),
                new ActionExecutionStep(),
                new FaintedPokemonCheckStep(faintedSwitchingProcessor),
                new EndOfTurnEffectsStep(endOfTurnEffectsProcessor),
                new FaintedPokemonCheckStep(faintedSwitchingProcessor),
                new DurationDecrementStep(durationDecrementProcessor),
                new TurnEndStep(turnEndProcessor),
                new FaintedPokemonCheckStep(faintedSwitchingProcessor)
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
                Queue = _queue,
                View = _view,
                TurnNumber = turnNumber,
                EventBus = _eventBus,
                StateValidator = _stateValidator,
                Logger = _logger,
                TurnOrderResolver = _turnOrderResolver
            };

            await _stepExecutor.ExecuteTurn(context);
        }
    }
}
