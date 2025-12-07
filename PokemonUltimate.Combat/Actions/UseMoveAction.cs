using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Execution.Processors;
using PokemonUltimate.Combat.Extensions;
using PokemonUltimate.Combat.Foundation.Constants;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Combat.Infrastructure.Factories;
using PokemonUltimate.Combat.Infrastructure.Helpers;
using PokemonUltimate.Combat.Infrastructure.Messages;
using PokemonUltimate.Combat.Infrastructure.Providers;
using PokemonUltimate.Combat.Integration.View;
using PokemonUltimate.Combat.Systems.Damage;
using PokemonUltimate.Combat.Systems.Effects;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Data.Effects;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Domain.Instances;
using PokemonUltimate.Core.Domain.Instances.Move;
using PokemonUltimate.Core.Infrastructure.Localization;
using PokemonUltimate.Core.Services;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

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
        private readonly AccuracyChecker _accuracyChecker;
        private readonly IRandomProvider _randomProvider;
        private readonly IDamagePipeline _damagePipeline;
        private readonly MoveEffectProcessorRegistry _effectProcessorRegistry;
        private readonly IBattleMessageFormatter _messageFormatter;
        private readonly BeforeMoveProcessor _beforeMoveProcessor;
        private readonly AfterMoveProcessor _afterMoveProcessor;
        private readonly ITargetResolver _targetResolver;

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
            ITargetResolver targetResolver = null) : base(user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), ErrorMessages.PokemonCannotBeNull);
            Target = target ?? throw new ArgumentNullException(nameof(target), ErrorMessages.PokemonCannotBeNull);
            MoveInstance = moveInstance ?? throw new ArgumentNullException(nameof(moveInstance), ErrorMessages.MoveCannotBeNull);

            // Create RandomProvider if not provided (temporary until full DI refactoring)
            _randomProvider = randomProvider ?? new RandomProvider();

            // Create AccuracyChecker if not provided (temporary until full DI refactoring)
            _accuracyChecker = accuracyChecker ?? new AccuracyChecker(_randomProvider);

            // Create DamagePipeline if not provided (temporary until full DI refactoring)
            _damagePipeline = damagePipeline ?? new DamagePipeline(_randomProvider);

            // Create MoveEffectProcessorRegistry if not provided (temporary until full DI refactoring)
            var damageContextFactory = new DamageContextFactory();
            _effectProcessorRegistry = effectProcessorRegistry ?? new MoveEffectProcessorRegistry(_randomProvider, damageContextFactory);

            // Create BattleMessageFormatter if not provided
            _messageFormatter = messageFormatter ?? new BattleMessageFormatter();

            // Create TargetResolver if not provided
            _targetResolver = targetResolver ?? new TargetResolver();

            // Create processors if not provided (temporary until full DI refactoring)
            _beforeMoveProcessor = beforeMoveProcessor ?? new BeforeMoveProcessor();
            _afterMoveProcessor = afterMoveProcessor ?? new AfterMoveProcessor();
        }

        /// <summary>
        /// Executes the move logic.
        /// Checks PP, status conditions, accuracy, and generates child actions.
        /// </summary>
        public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            // User must be active to use a move
            if (!User.IsActive())
                return Enumerable.Empty<BattleAction>();

            // Target slot must not be empty (but can be fainted - move still executes, consumes PP, but deals no damage)
            if (Target.IsEmpty)
                return Enumerable.Empty<BattleAction>();

            var actions = new List<BattleAction>();

            // Cancel conflicting move states
            CancelConflictingMoveStates();

            // Process before-move effects (abilities and items)
            // This happens BEFORE validation so abilities can block the move
            var beforeMoveActions = _beforeMoveProcessor.ProcessBeforeMove(User, field);
            actions.AddRange(beforeMoveActions);

            // Check if any ability blocked the move (e.g., Truant)
            // Truant returns a TruantLoafing message - if present, block the move
            bool moveBlocked = beforeMoveActions.Any(action =>
                action is MessageAction msg &&
                msg.Message.Contains("loafing around"));

            if (moveBlocked)
            {
                return actions; // Block move execution (PP not consumed)
            }

            // Validate move execution (PP, Flinch, Status)
            var validationResult = ValidateMoveExecution(actions);
            if (validationResult != null)
                return validationResult;

            // Process Multi-Turn moves
            bool hasMultiTurnEffect = Move.Effects.Any(e => e is MultiTurnEffect);
            var multiTurnResult = ProcessMultiTurnMove(actions, hasMultiTurnEffect);
            if (multiTurnResult != null)
                return multiTurnResult;

            // Process Focus Punch moves
            bool hasFocusPunchEffect = Move.Effects.Any(e => e is FocusPunchEffect);
            var focusPunchResult = ProcessFocusPunchMove(actions, hasFocusPunchEffect);
            if (focusPunchResult != null)
                return focusPunchResult;

            // Deduct PP and generate move message
            MoveInstance.Use();
            actions.Add(new MessageAction(_messageFormatter.FormatMoveUsed(User.Pokemon, Move)));

            // Show focusing message for Focus Punch (if not already failed)
            if (hasFocusPunchEffect)
            {
                actions.Add(new MessageAction(_messageFormatter.Format(LocalizationKey.MoveFocusing, User.Pokemon.DisplayName)));
            }

            // Check protection and semi-invulnerable status
            var protectionResult = CheckProtection(actions, hasFocusPunchEffect);
            if (protectionResult != null)
                return protectionResult;

            var semiInvulnerableResult = CheckSemiInvulnerable(actions, hasFocusPunchEffect, hasMultiTurnEffect);
            if (semiInvulnerableResult != null)
                return semiInvulnerableResult;

            // Check accuracy (skip if target is fainted - move still executes but deals no damage)
            if (Target.IsActive())
            {
                var accuracyResult = CheckAccuracy(actions, field, hasFocusPunchEffect, hasMultiTurnEffect);
                if (accuracyResult != null)
                    return accuracyResult;
            }

            // Remove focusing status if Focus Punch succeeds
            if (hasFocusPunchEffect)
            {
                User.RemoveVolatileStatus(VolatileStatus.Focusing);
            }

            // 12. Process move effects
            ProcessEffects(field, actions);

            // Process after-move effects (abilities and items, e.g., Moxie, Life Orb recoil)
            // This happens AFTER all effects are processed
            // Note: DamageActions are in the actions list but haven't been executed yet.
            // Life Orb and Moxie will check damage trackers which are updated when DamageActions execute.
            var afterMoveActions = _afterMoveProcessor.ProcessAfterMove(User, field);
            actions.AddRange(afterMoveActions);

            return actions;
        }

        /// <summary>
        /// Processes Multi-Turn move effects (charge turn vs attack turn).
        /// </summary>
        /// <param name="actions">The actions list to add messages to.</param>
        /// <param name="hasMultiTurnEffect">Whether the move has Multi-Turn effect.</param>
        /// <returns>Null if execution should continue, or the actions list if charge turn (early return).</returns>
        private List<BattleAction> ProcessMultiTurnMove(List<BattleAction> actions, bool hasMultiTurnEffect)
        {
            if (!hasMultiTurnEffect)
                return null;

            if (User.HasVolatileStatus(VolatileStatus.Charging) && User.ChargingMoveName == Move.Name)
            {
                // This is the attack turn - clear charging and proceed with normal execution
                User.RemoveVolatileStatus(VolatileStatus.Charging);
                User.ClearChargingMove();
                return null;
            }

            // This is the charge turn - mark as charging and return early
            User.AddVolatileStatus(VolatileStatus.Charging);
            User.SetChargingMove(Move.Name);
            MoveInstance.Use(); // Deduct PP
            var multiTurnEffect = Move.Effects.OfType<MultiTurnEffect>().First();
            string chargeMessage = !string.IsNullOrEmpty(multiTurnEffect.ChargeMessage)
                ? $"{User.Pokemon.DisplayName} {multiTurnEffect.ChargeMessage}"
                : $"{User.Pokemon.DisplayName} is charging!";
            actions.Add(new MessageAction(chargeMessage));
            return actions; // Don't execute damage on charge turn
        }

        /// <summary>
        /// Processes Focus Punch move effects (focusing state and hit check).
        /// </summary>
        /// <param name="actions">The actions list to add messages to.</param>
        /// <param name="hasFocusPunchEffect">Whether the move has Focus Punch effect.</param>
        /// <returns>Null if execution should continue, or the actions list if focus was lost.</returns>
        private List<BattleAction> ProcessFocusPunchMove(List<BattleAction> actions, bool hasFocusPunchEffect)
        {
            if (!hasFocusPunchEffect)
                return null;

            // Mark user as focusing at start of turn (before checking if hit)
            // In a real battle, this would happen when actions are collected, but we do it here for simplicity
            User.AddVolatileStatus(VolatileStatus.Focusing);

            // Check if user was hit while focusing (must check after marking as focusing)
            if (User.WasHitWhileFocusing)
            {
                // Deduct PP even if move fails
                MoveInstance.Use();
                var provider = LocalizationService.Instance;
                actions.Add(new MessageAction(provider.GetString(LocalizationKey.MoveFocusLost, User.Pokemon.DisplayName)));
                User.RemoveVolatileStatus(VolatileStatus.Focusing);
                return actions;
            }

            return null;
        }

        /// <summary>
        /// Checks if target is protected and handles protection logic.
        /// </summary>
        /// <param name="actions">The actions list to add messages to.</param>
        /// <param name="hasFocusPunchEffect">Whether the move has Focus Punch effect.</param>
        /// <returns>Null if execution should continue, or the actions list if move was blocked.</returns>
        private List<BattleAction> CheckProtection(List<BattleAction> actions, bool hasFocusPunchEffect)
        {
            // Skip protection check if target is fainted (move still executes)
            if (!Target.IsActive())
                return null;

            if (!Target.HasVolatileStatus(VolatileStatus.Protected) || !CanBeBlocked)
                return null;

            // Check if move bypasses Protect (e.g., Feint)
            if (!Move.BypassesProtect)
            {
                actions.Add(new MessageAction(_messageFormatter.Format(LocalizationKey.BattleProtected, Target.Pokemon.DisplayName)));
                // Remove focusing status if move was blocked
                if (hasFocusPunchEffect)
                {
                    User.RemoveVolatileStatus(VolatileStatus.Focusing);
                }
                return actions;
            }

            return null;
        }

        /// <summary>
        /// Checks if target is semi-invulnerable and if this move can hit it.
        /// </summary>
        /// <param name="actions">The actions list to add messages to.</param>
        /// <param name="hasFocusPunchEffect">Whether the move has Focus Punch effect.</param>
        /// <param name="hasMultiTurnEffect">Whether the move has Multi-Turn effect.</param>
        /// <returns>Null if execution should continue, or the actions list if move missed.</returns>
        private List<BattleAction> CheckSemiInvulnerable(List<BattleAction> actions, bool hasFocusPunchEffect, bool hasMultiTurnEffect)
        {
            // Skip semi-invulnerable check if target is fainted (move still executes)
            if (!Target.IsActive())
                return null;

            if (!Target.HasVolatileStatus(VolatileStatus.SemiInvulnerable))
                return null;

            // Check if move can hit semi-invulnerable target
            bool canHit = CanHitSemiInvulnerable(Target.SemiInvulnerableMoveName);
            if (!canHit)
            {
                actions.Add(new MessageAction(_messageFormatter.Format(LocalizationKey.BattleMissed)));
                CleanupVolatileStatusesOnFailure(hasFocusPunchEffect, hasMultiTurnEffect);
                return actions;
            }

            return null;
        }

        /// <summary>
        /// Checks move accuracy and handles miss logic.
        /// </summary>
        /// <param name="actions">The actions list to add messages to.</param>
        /// <param name="field">The battlefield for accuracy calculations.</param>
        /// <param name="hasFocusPunchEffect">Whether the move has Focus Punch effect.</param>
        /// <param name="hasMultiTurnEffect">Whether the move has Multi-Turn effect.</param>
        /// <returns>Null if execution should continue, or the actions list if move missed.</returns>
        private List<BattleAction> CheckAccuracy(List<BattleAction> actions, BattleField field, bool hasFocusPunchEffect, bool hasMultiTurnEffect)
        {
            if (_accuracyChecker.CheckHit(User, Target, Move, field))
                return null;

            actions.Add(new MessageAction(_messageFormatter.Format(LocalizationKey.BattleMissed)));
            CleanupVolatileStatusesOnFailure(hasFocusPunchEffect, hasMultiTurnEffect);
            return actions;
        }

        /// <summary>
        /// Cleans up volatile statuses when a move fails (misses, blocked, etc.).
        /// </summary>
        /// <param name="hasFocusPunchEffect">Whether the move has Focus Punch effect.</param>
        /// <param name="hasMultiTurnEffect">Whether the move has Multi-Turn effect.</param>
        private void CleanupVolatileStatusesOnFailure(bool hasFocusPunchEffect, bool hasMultiTurnEffect)
        {
            // Remove focusing status if move failed
            if (hasFocusPunchEffect)
            {
                User.RemoveVolatileStatus(VolatileStatus.Focusing);
            }
            // Clear charging if move failed
            if (hasMultiTurnEffect)
            {
                User.RemoveVolatileStatus(VolatileStatus.Charging);
                User.ClearChargingMove();
            }
        }

        /// <summary>
        /// Checks if this move can hit a semi-invulnerable target.
        /// </summary>
        /// <param name="semiInvulnerableMoveName">The name of the semi-invulnerable move being used.</param>
        /// <returns>True if this move can hit the semi-invulnerable target, false otherwise.</returns>
        private bool CanHitSemiInvulnerable(string semiInvulnerableMoveName)
        {
            if (string.IsNullOrEmpty(semiInvulnerableMoveName))
                return false;

            // Always-hit moves can hit semi-invulnerable targets
            if (Move.NeverMisses)
                return true;

            // Check specific move combinations
            var moveNameLower = semiInvulnerableMoveName.ToLower();

            if (moveNameLower == "dig")
            {
                return MoveConstants.DigCounterMoveNames.Contains(Move.Name) ||
                       Move.NeverMisses;
            }

            if (moveNameLower == "dive")
            {
                return MoveConstants.DiveCounterMoveNames.Contains(Move.Name) ||
                       Move.NeverMisses;
            }

            if (moveNameLower == "fly" || moveNameLower == "bounce")
            {
                // Thunder hits Fly/Bounce in rain (handled by weather perfect accuracy)
                // For now, only always-hit moves can hit
                return Move.NeverMisses;
            }

            if (moveNameLower == "shadow force" || moveNameLower == "phantom force")
            {
                // Only always-hit moves can hit
                return Move.NeverMisses;
            }

            return false;
        }

        /// <summary>
        /// Cancels conflicting move states (charging different move, semi-invulnerable with different move).
        /// </summary>
        private void CancelConflictingMoveStates()
        {
            // Cancel charging if using a different move
            if (User.HasVolatileStatus(VolatileStatus.Charging) && User.ChargingMoveName != Move.Name)
            {
                User.RemoveVolatileStatus(VolatileStatus.Charging);
                User.ClearChargingMove();
            }

            // Cancel semi-invulnerable if using a different move
            // Note: If using the same move with SemiInvulnerableEffect, we don't cancel here
            // because ProcessEffects will handle it (either charge turn or attack turn)
            bool hasSemiInvulnerableEffect = Move.Effects.Any(e => e is SemiInvulnerableEffect);
            if (User.HasVolatileStatus(VolatileStatus.SemiInvulnerable) &&
                User.SemiInvulnerableMoveName != Move.Name)
            {
                User.RemoveVolatileStatus(VolatileStatus.SemiInvulnerable);
                User.ClearSemiInvulnerableMove();
            }
            // If using the same move with SemiInvulnerableEffect, don't cancel - ProcessEffects will handle it
        }

        /// <summary>
        /// Validates move execution (PP, Flinch, Status conditions).
        /// Returns null if validation passes, or a list of actions if validation fails.
        /// </summary>
        /// <param name="actions">The actions list to add validation failure messages to.</param>
        /// <returns>Null if validation passes, or the actions list if validation fails.</returns>
        private List<BattleAction> ValidateMoveExecution(List<BattleAction> actions)
        {
            // Check PP
            if (!MoveInstance.HasPP)
            {
                actions.Add(new MessageAction(_messageFormatter.Format(LocalizationKey.BattleNoPP, User.Pokemon.DisplayName)));
                return actions;
            }

            // Check Flinch (volatile status)
            if (User.HasVolatileStatus(VolatileStatus.Flinch))
            {
                actions.Add(new MessageAction(_messageFormatter.Format(LocalizationKey.BattleFlinched, User.Pokemon.DisplayName), User));
                User.RemoveVolatileStatus(VolatileStatus.Flinch); // Consume flinch
                return actions;
            }

            // Check persistent status conditions
            var statusCheckResult = CheckStatusConditions();
            if (statusCheckResult != null)
            {
                actions.Add(statusCheckResult);
                return actions;
            }

            return null;
        }

        /// <summary>
        /// Checks persistent status conditions (Sleep, Freeze, Paralysis).
        /// </summary>
        private MessageAction CheckStatusConditions()
        {
            var pokemon = User.Pokemon;

            switch (pokemon.Status)
            {
                case PersistentStatus.Sleep:
                    return new MessageAction(_messageFormatter.Format(LocalizationKey.BattleAsleep, pokemon.DisplayName), User);

                case PersistentStatus.Freeze:
                    var provider = LocalizationService.Instance;
                    return new MessageAction(provider.GetString(LocalizationKey.BattleFrozen, pokemon.DisplayName), User);

                case PersistentStatus.Paralysis:
                    // 25% chance to be fully paralyzed
                    if (_randomProvider.Next(100) < StatusConstants.ParalysisFullParalysisChance)
                    {
                        return new MessageAction(_messageFormatter.Format(LocalizationKey.BattleParalyzed, pokemon.DisplayName), User);
                    }
                    break;
            }

            return null;
        }

        /// <summary>
        /// Processes all move effects and adds corresponding actions.
        /// Processes damage first, then effects that depend on damage (recoil, drain), then other effects.
        /// </summary>
        private void ProcessEffects(BattleField field, List<BattleAction> actions)
        {
            int damageDealt = 0;

            // Check for Pursuit effect - doubles power if target is switching
            bool hasPursuitEffect = Move.Effects.Any(e => e is PursuitEffect);
            MoveData moveForDamage = Move;
            if (hasPursuitEffect && Target.HasVolatileStatus(VolatileStatus.SwitchingOut))
            {
                // Apply power multiplier using MoveModifier
                var pursuitModifier = MoveModifier.MultiplyPower(2.0f);
                moveForDamage = pursuitModifier.ApplyModifications(Move);
                actions.Add(new MessageAction(_messageFormatter.FormatMoveUsedDuringSwitch(Target.Pokemon, User.Pokemon, Move)));
            }

            // Check for Semi-Invulnerable effect - determine if charge or attack turn
            bool hasSemiInvulnerableEffect = Move.Effects.Any(e => e is SemiInvulnerableEffect);
            bool isSemiInvulnerableAttackTurn = false;
            if (hasSemiInvulnerableEffect)
            {
                // Check if user is already charging this move and ready to attack
                if (User.HasVolatileStatus(VolatileStatus.SemiInvulnerable) &&
                    User.SemiInvulnerableMoveName == Move.Name &&
                    !User.IsSemiInvulnerableCharging)
                {
                    isSemiInvulnerableAttackTurn = true;
                }
            }

            // Check for Multi-Hit effect
            bool hasMultiHitEffect = Move.Effects.Any(e => e is MultiHitEffect);
            MultiHitEffect multiHitEffect = null;
            if (hasMultiHitEffect)
            {
                multiHitEffect = Move.Effects.OfType<MultiHitEffect>().First();
            }

            // First pass: Process damage effect to get damageDealt
            // Skip damage on semi-invulnerable charge turn
            // For spread moves, check if there are any valid active targets
            var potentialTargets = _targetResolver.GetValidTargets(User, Move, field);
            bool hasActiveTargets = Target.IsActive() || potentialTargets.Any(t => t.IsActive());

            if (!(hasSemiInvulnerableEffect && !isSemiInvulnerableAttackTurn) && hasActiveTargets)
            {
                foreach (var effect in Move.Effects)
                {
                    if (effect is DamageEffect damageEffect)
                    {
                        if (hasMultiHitEffect && multiHitEffect != null)
                        {
                            // Multi-hit move: hit multiple times
                            int numHits = _randomProvider.Next(multiHitEffect.MinHits, multiHitEffect.MaxHits + 1);

                            for (int i = 0; i < numHits; i++)
                            {
                                // Each hit has independent accuracy and damage calculation
                                var context = _damagePipeline.Calculate(User, Target, moveForDamage, field);

                                if (context.FinalDamage > 0)
                                {
                                    var damageAction = new DamageAction(User, Target, context);
                                    actions.Add(damageAction);
                                    damageDealt += context.FinalDamage; // Accumulate total damage
                                }
                            }
                        }
                        else
                        {
                            // Check if this is a spread move (hits multiple targets)
                            var validTargets = _targetResolver.GetValidTargets(User, Move, field);
                            bool isSpreadMove = IsSpreadMove(Move, field, validTargets.Count);

                            if (isSpreadMove && validTargets.Count > 1)
                            {
                                // Spread move: hit all valid targets with 75% damage in doubles/triples
                                bool isMultiTargetFormat = field.Rules.PlayerSlots > 1 || field.Rules.EnemySlots > 1;
                                float spreadMultiplier = isMultiTargetFormat ? 0.75f : 1.0f;

                                int totalDamage = 0;
                                foreach (var target in validTargets)
                                {
                                    if (!target.IsActive())
                                        continue;

                                    var context = _damagePipeline.Calculate(User, target, moveForDamage, field);

                                    // Apply spread move damage reduction
                                    if (spreadMultiplier < 1.0f)
                                    {
                                        context.Multiplier *= spreadMultiplier;
                                    }

                                    if (context.FinalDamage > 0)
                                    {
                                        var damageAction = new DamageAction(User, target, context);
                                        actions.Add(damageAction);
                                        totalDamage += context.FinalDamage;
                                    }
                                }
                                damageDealt = totalDamage;
                            }
                            else
                            {
                                // Single-target move: normal damage calculation
                                var context = _damagePipeline.Calculate(User, Target, moveForDamage, field);

                                if (context.FinalDamage > 0)
                                {
                                    var damageAction = new DamageAction(User, Target, context);
                                    actions.Add(damageAction);
                                    damageDealt = context.FinalDamage;
                                }
                            }
                        }
                        break; // Only process first DamageEffect
                    }
                }
            }

            // Second pass: Process all other effects (including recoil/drain that depend on damageDealt)
            foreach (var effect in Move.Effects)
            {
                // Skip DamageEffect (already processed in first pass)
                if (effect is DamageEffect)
                    continue;

                // Process SemiInvulnerableEffect
                if (effect is SemiInvulnerableEffect semiInvulnerableEffect)
                {
                    // Check if this is the attack turn (user already charging this move)
                    if (User.HasVolatileStatus(VolatileStatus.SemiInvulnerable) &&
                        User.SemiInvulnerableMoveName == Move.Name &&
                        !User.IsSemiInvulnerableCharging)
                    {
                        // This is turn 2 - execute attack and clear semi-invulnerable status
                        User.RemoveVolatileStatus(VolatileStatus.SemiInvulnerable);
                        User.ClearSemiInvulnerableMove();
                        // Damage already processed in first pass
                        break;
                    }
                    else
                    {
                        // This is turn 1 - charge turn (no damage, just message)
                        User.AddVolatileStatus(VolatileStatus.SemiInvulnerable);
                        User.SetSemiInvulnerableMove(Move.Name, isCharging: true);
                        // Message varies by move (e.g., "flew up high!" for Fly)
                        string message = FormatSemiInvulnerableMessage(Move.Name, User.Pokemon);
                        actions.Add(new MessageAction(message));
                        // Charge turn handled - continue to process other effects if any
                        continue;
                    }
                }

                // Try to process effect using registry
                bool processed = _effectProcessorRegistry.Process(effect, User, Target, Move, field, damageDealt, actions);

                // If not processed by registry, it's an effect type we don't have a processor for yet
                // This allows for graceful handling of new effect types without breaking existing code
                if (!processed)
                {
                    // Unknown effect type - log or handle as needed
                    // For now, we silently skip it (could add logging in the future)
                }
            }
        }

        /// <summary>
        /// Determines if a move is a spread move (hits multiple targets).
        /// </summary>
        /// <param name="move">The move to check.</param>
        /// <param name="field">The battlefield.</param>
        /// <param name="validTargetCount">The number of valid targets.</param>
        /// <returns>True if the move is a spread move.</returns>
        private bool IsSpreadMove(MoveData move, BattleField field, int validTargetCount)
        {
            // A move is a spread move if:
            // 1. It can target multiple Pokemon (based on TargetScope)
            // 2. There are multiple valid targets
            if (validTargetCount <= 1)
                return false;

            switch (move.TargetScope)
            {
                case TargetScope.AllEnemies:
                case TargetScope.AllAdjacent:
                case TargetScope.AllAdjacentEnemies:
                case TargetScope.AllOthers:
                    return true;

                case TargetScope.AllAllies:
                    // Only spread if there are multiple allies (excluding self)
                    return validTargetCount > 1;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Formats a message for semi-invulnerable move charging.
        /// </summary>
        /// <param name="moveName">The name of the move.</param>
        /// <param name="pokemon">The Pokemon using the move.</param>
        /// <returns>The formatted message.</returns>
        private string FormatSemiInvulnerableMessage(string moveName, PokemonInstance pokemon)
        {
            string moveNameLower = moveName.ToLower();
            string action;

            if (moveNameLower == "fly")
                action = "flew up high!";
            else if (moveNameLower == "dig")
                action = "dug underground!";
            else if (moveNameLower == "dive")
                action = "dove underwater!";
            else if (moveNameLower == "bounce")
                action = "bounced up!";
            else if (moveNameLower == "shadow force" || moveNameLower == "phantom force")
                action = "vanished instantly!";
            else
                return _messageFormatter.FormatMoveUsed(pokemon, Move);

            return $"{pokemon.DisplayName} {action}";
        }

        /// <summary>
        /// Plays the move animation.
        /// </summary>
        public override Task ExecuteVisual(IBattleView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            // Don't play animation if user is not active (fainted, switched out, etc.)
            // This prevents showing animations for moves that were queued but can't execute
            if (!User.IsActive() || User.IsEmpty || Target.IsEmpty)
                return Task.CompletedTask;

            return view.PlayMoveAnimation(User, Target, Move.Id);
        }
    }
}

