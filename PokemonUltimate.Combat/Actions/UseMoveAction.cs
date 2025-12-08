using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions.Registry;
using PokemonUltimate.Combat.Actions.Validation;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Damage.Definition;
using PokemonUltimate.Combat.Effects.Registry;
using PokemonUltimate.Combat.Engine.Processors;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Factories;
using PokemonUltimate.Combat.Infrastructure.Messages;
using PokemonUltimate.Combat.Infrastructure.Messages.Definition;
using PokemonUltimate.Combat.Infrastructure.Providers;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Combat.Moves.Definition;
using PokemonUltimate.Combat.Moves.Orchestrator;
using PokemonUltimate.Combat.Moves.Steps;
using PokemonUltimate.Combat.Utilities;
using PokemonUltimate.Combat.Utilities.Definition;
using PokemonUltimate.Combat.View.Definition;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Domain.Instances.Move;

namespace PokemonUltimate.Combat.Actions
{
    /// <summary>
    /// Executes a move in battle.
    /// Handles PP checking, status checks, accuracy, damage calculation, and effect application.
    /// Generates child actions for the battle queue.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class UseMoveAction : BattleAction
    {
        private readonly MoveExecutionOrchestrator _executionOrchestrator;
        private readonly BehaviorCheckerRegistry _behaviorRegistry;
        private readonly BeforeMoveProcessor _beforeMoveProcessor;
        private readonly AfterMoveProcessor _afterMoveProcessor;
        private readonly IBattleMessageFormatter _messageFormatter;

        /// <summary>
        /// The target slot for this move.
        /// </summary>
        public BattleSlot Target { get; }

        /// <summary>
        /// The move instance being used.
        /// </summary>
        public MoveInstance MoveInstance { get; }

        /// <summary>
        /// The move data blueprint.
        /// </summary>
        public MoveData Move => MoveInstance.Move;

        /// <summary>
        /// Priority override from the move data.
        /// </summary>
        public override int Priority => Move.Priority;

        /// <summary>
        /// Moves can be blocked by effects like Protect.
        /// </summary>
        public override bool CanBeBlocked => true;

        /// <summary>
        /// Creates a new use move action.
        /// </summary>
        /// <param name="user">The slot using the move. Cannot be null.</param>
        /// <param name="target">The target slot. Cannot be null.</param>
        /// <param name="moveInstance">The move instance to use. Cannot be null.</param>
        /// <param name="randomProvider">The random provider. If null, creates a temporary one.</param>
        /// <param name="accuracyChecker">The accuracy checker. If null, creates a temporary one.</param>
        /// <param name="damagePipeline">The damage pipeline. If null, creates a temporary one.</param>
        /// <param name="effectProcessorRegistry">The effect processor registry. If null, creates a temporary one.</param>
        /// <param name="messageFormatter">The message formatter. If null, creates a default one.</param>
        /// <param name="beforeMoveProcessor">The before move processor. If null, creates a temporary one.</param>
        /// <param name="afterMoveProcessor">The after move processor. If null, creates a temporary one.</param>
        /// <param name="targetResolver">The target resolver. If null, creates a temporary one.</param>
        /// <param name="behaviorRegistry">The behavior checker registry. If null, creates a default one.</param>
        /// <param name="executionOrchestrator">The move execution orchestrator. If null, creates one with default steps.</param>
        /// <exception cref="ArgumentNullException">If user, target, or moveInstance is null.</exception>
        public UseMoveAction(
            BattleSlot user,
            BattleSlot target,
            MoveInstance moveInstance,
            IRandomProvider randomProvider = null,
            AccuracyChecker accuracyChecker = null,
            IDamagePipeline damagePipeline = null,
            MoveEffectProcessorRegistry effectProcessorRegistry = null,
            IBattleMessageFormatter messageFormatter = null,
            BeforeMoveProcessor beforeMoveProcessor = null,
            AfterMoveProcessor afterMoveProcessor = null,
            ITargetResolver targetResolver = null,
            BehaviorCheckerRegistry behaviorRegistry = null,
            MoveExecutionOrchestrator executionOrchestrator = null) : base(user)
        {
            ActionValidators.ValidateUser(user, nameof(user));
            ActionValidators.ValidateTargetNotNull(target, nameof(target));
            ActionValidators.ValidateMoveInstance(moveInstance, nameof(moveInstance));

            Target = target;
            MoveInstance = moveInstance;

            // Create dependencies if not provided (temporary until full DI refactoring)
            var randomProviderInstance = randomProvider ?? new RandomProvider();
            var accuracyCheckerInstance = accuracyChecker ?? new AccuracyChecker(randomProviderInstance);
            var damagePipelineInstance = damagePipeline ?? new DamagePipeline(randomProviderInstance);

            var damageContextFactory = new DamageContextFactory();
            var effectProcessorRegistryInstance = effectProcessorRegistry ?? new MoveEffectProcessorRegistry(randomProviderInstance, damageContextFactory);
            _messageFormatter = messageFormatter ?? new BattleMessageFormatter();
            var targetResolverInstance = targetResolver ?? new TargetResolver();
            _beforeMoveProcessor = beforeMoveProcessor ?? new BeforeMoveProcessor();
            _afterMoveProcessor = afterMoveProcessor ?? new AfterMoveProcessor();
            _behaviorRegistry = behaviorRegistry ?? new BehaviorCheckerRegistry();

            // Initialize processors in registry (they need dependencies)
            // This creates the MoveEffectOrchestrator internally
            _behaviorRegistry.InitializeProcessors(damagePipelineInstance, randomProviderInstance, targetResolverInstance, effectProcessorRegistryInstance, accuracyCheckerInstance, _messageFormatter);

            // Get move effect orchestrator from registry
            var moveEffectOrchestrator = _behaviorRegistry.GetMoveEffectOrchestrator();

            // Create execution orchestrator if not provided
            _executionOrchestrator = executionOrchestrator ?? CreateDefaultOrchestrator(moveEffectOrchestrator);
        }

        /// <summary>
        /// Creates the default execution orchestrator with all standard steps.
        /// </summary>
        private MoveExecutionOrchestrator CreateDefaultOrchestrator(MoveEffectOrchestrator moveEffectOrchestrator)
        {
            var steps = new List<IMoveExecutionStep>
            {
                new InitialValidationStep(),
                new CancelConflictingStatesStep(_behaviorRegistry),
                new BeforeMoveProcessingStep(_beforeMoveProcessor),
                new MoveExecutionValidationStep(_behaviorRegistry),
                new SpecialBehaviorProcessingStep(_behaviorRegistry),
                new PPConsumptionStep(_messageFormatter),
                new ProtectionCheckStep(_behaviorRegistry),
                new SemiInvulnerableCheckStep(_behaviorRegistry),
                new AccuracyCheckStep(_behaviorRegistry),
                new EffectProcessingStep(moveEffectOrchestrator),
                new AfterMoveProcessingStep(_afterMoveProcessor)
            };

            return new MoveExecutionOrchestrator(steps);
        }

        /// <summary>
        /// Executes the move logic using the execution orchestrator.
        /// The orchestrator handles all steps: validation, checks, effects, etc.
        /// </summary>
        public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            ActionValidators.ValidateField(field);

            // Early validation: If user is not active (fainted or empty), don't execute the move at all
            // This prevents moves from executing when Pokemon faint during the same turn
            if (!ActionValidators.ValidateUserActive(User))
            {
                // Return empty - no actions should be generated for a move used by a fainted Pokemon
                return new List<BattleAction>();
            }

            // Execute all steps using the orchestrator
            var actions = _executionOrchestrator.Execute(User, Target, MoveInstance, field, CanBeBlocked);

            return actions;
        }

        /// <summary>
        /// Plays the move animation.
        /// </summary>
        public override Task ExecuteVisual(IBattleView view)
        {
            ActionValidators.ValidateView(view);

            // Don't play animation if user or target are not valid/active
            // This prevents showing animations for moves that were queued but can't execute
            if (!ActionValidators.ValidateUserAndTargetForVisual(User, Target))
                return Task.CompletedTask;

            return view.PlayMoveAnimation(User, Target, Move.Id);
        }
    }
}

