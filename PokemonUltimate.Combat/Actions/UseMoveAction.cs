using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Helpers;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Instances;

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
        /// <exception cref="ArgumentNullException">If user, target, or moveInstance is null.</exception>
        public UseMoveAction(BattleSlot user, BattleSlot target, MoveInstance moveInstance) : base(user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), ErrorMessages.PokemonCannotBeNull);
            Target = target ?? throw new ArgumentNullException(nameof(target), ErrorMessages.PokemonCannotBeNull);
            MoveInstance = moveInstance ?? throw new ArgumentNullException(nameof(moveInstance), ErrorMessages.MoveCannotBeNull);
        }

        /// <summary>
        /// Executes the move logic.
        /// Checks PP, status conditions, accuracy, and generates child actions.
        /// </summary>
        public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (User.IsEmpty || User.HasFainted || Target.IsEmpty)
                return Enumerable.Empty<BattleAction>();

            var actions = new List<BattleAction>();

            // 0. Check if user is charging a different move - cancel it
            if (User.HasVolatileStatus(VolatileStatus.Charging) && User.ChargingMoveName != Move.Name)
            {
                User.RemoveVolatileStatus(VolatileStatus.Charging);
                User.ClearChargingMove();
            }

            // 0.5. Check if user is semi-invulnerable
            bool hasSemiInvulnerableEffect = Move.Effects.Any(e => e is SemiInvulnerableEffect);
            // Only cancel if using a different move (not the same move on attack turn)
            if (User.HasVolatileStatus(VolatileStatus.SemiInvulnerable) && 
                User.SemiInvulnerableMoveName != Move.Name)
            {
                // Using different move - cancel semi-invulnerable
                User.RemoveVolatileStatus(VolatileStatus.SemiInvulnerable);
                User.ClearSemiInvulnerableMove();
            }

            // 1. Check PP
            if (!MoveInstance.HasPP)
            {
                actions.Add(new MessageAction(string.Format(GameMessages.MoveNoPP, User.Pokemon.DisplayName)));
                return actions;
            }

            // 2. Check Flinch (volatile status)
            if (User.HasVolatileStatus(VolatileStatus.Flinch))
            {
                actions.Add(new MessageAction(string.Format(GameMessages.MoveFlinched, User.Pokemon.DisplayName)));
                User.RemoveVolatileStatus(VolatileStatus.Flinch); // Consume flinch
                return actions;
            }

            // 3. Check persistent status conditions
            var statusCheckResult = CheckStatusConditions();
            if (statusCheckResult != null)
            {
                actions.Add(statusCheckResult);
                return actions;
            }

            // 4. Check Multi-Turn effect - if user is already charging this move, execute attack
            bool hasMultiTurnEffect = Move.Effects.Any(e => e is MultiTurnEffect);
            if (hasMultiTurnEffect && User.HasVolatileStatus(VolatileStatus.Charging) && User.ChargingMoveName == Move.Name)
            {
                // This is the attack turn - clear charging and proceed with normal execution
                User.RemoveVolatileStatus(VolatileStatus.Charging);
                User.ClearChargingMove();
            }
            else if (hasMultiTurnEffect)
            {
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

            // 5. Check Focus Punch effect
            bool hasFocusPunchEffect = Move.Effects.Any(e => e is FocusPunchEffect);
            
            // Mark user as focusing at start of turn (before checking if hit)
            // In a real battle, this would happen when actions are collected, but we do it here for simplicity
            if (hasFocusPunchEffect)
            {
                User.AddVolatileStatus(VolatileStatus.Focusing);
            }

            // Check if user was hit while focusing (must check after marking as focusing)
            if (hasFocusPunchEffect && User.WasHitWhileFocusing)
            {
                // Deduct PP even if move fails
                MoveInstance.Use();
                actions.Add(new MessageAction(string.Format(GameMessages.MoveFocusLost, User.Pokemon.DisplayName)));
                User.RemoveVolatileStatus(VolatileStatus.Focusing);
                return actions;
            }

            // 6. Deduct PP
            MoveInstance.Use();

            // 7. Generate "X used Y!" message
            actions.Add(new MessageAction($"{User.Pokemon.DisplayName} used {Move.Name}!"));

            // 8. Show focusing message for Focus Punch (if not already failed)
            if (hasFocusPunchEffect)
            {
                actions.Add(new MessageAction(string.Format(GameMessages.MoveFocusing, User.Pokemon.DisplayName)));
            }

            // 9. Check if target is protected (before accuracy check)
            if (Target.HasVolatileStatus(VolatileStatus.Protected) && CanBeBlocked)
            {
                // Check if move bypasses Protect (e.g., Feint)
                if (!Move.BypassesProtect)
                {
                    actions.Add(new MessageAction(string.Format(GameMessages.MoveProtected, Target.Pokemon.DisplayName)));
                    // Remove focusing status if move was blocked
                    if (hasFocusPunchEffect)
                    {
                        User.RemoveVolatileStatus(VolatileStatus.Focusing);
                    }
                    return actions;
                }
            }

            // 9.5. Check if target is semi-invulnerable (before accuracy check)
            if (Target.HasVolatileStatus(VolatileStatus.SemiInvulnerable))
            {
                // Check if move can hit semi-invulnerable target
                bool canHit = CanHitSemiInvulnerable(Target.SemiInvulnerableMoveName);
                if (!canHit)
                {
                    actions.Add(new MessageAction(GameMessages.MoveMissed));
                    CleanupVolatileStatusesOnFailure(hasFocusPunchEffect, hasMultiTurnEffect);
                    return actions;
                }
            }

            // 10. Accuracy check
            if (!AccuracyChecker.CheckHit(User, Target, Move, field))
            {
                actions.Add(new MessageAction(GameMessages.MoveMissed));
                CleanupVolatileStatusesOnFailure(hasFocusPunchEffect, hasMultiTurnEffect);
                return actions;
            }

            // 11. Remove focusing status if Focus Punch succeeds
            if (hasFocusPunchEffect)
            {
                User.RemoveVolatileStatus(VolatileStatus.Focusing);
            }

            // 12. Process move effects
            ProcessEffects(field, actions);

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
            switch (semiInvulnerableMoveName.ToLower())
            {
                case "dig":
                    return Move.Name.Equals("Earthquake", StringComparison.OrdinalIgnoreCase) ||
                           Move.Name.Equals("Magnitude", StringComparison.OrdinalIgnoreCase) ||
                           Move.NeverMisses;
                case "dive":
                    return Move.Name.Equals("Surf", StringComparison.OrdinalIgnoreCase) ||
                           Move.Name.Equals("Whirlpool", StringComparison.OrdinalIgnoreCase) ||
                           Move.NeverMisses;
                case "fly":
                case "bounce":
                    // Thunder hits Fly/Bounce in rain (handled by weather perfect accuracy)
                    // For now, only always-hit moves can hit
                    return Move.NeverMisses;
                case "shadow force":
                case "phantom force":
                    // Only always-hit moves can hit
                    return Move.NeverMisses;
                default:
                    return false;
            }
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
                    return new MessageAction(string.Format(GameMessages.MoveAsleep, pokemon.DisplayName));

                case PersistentStatus.Freeze:
                    return new MessageAction(string.Format(GameMessages.MoveFrozen, pokemon.DisplayName));

                case PersistentStatus.Paralysis:
                    // 25% chance to be fully paralyzed
                    var random = new Random();
                    if (random.Next(100) < 25)
                    {
                        return new MessageAction(string.Format(GameMessages.MoveParalyzed, pokemon.DisplayName));
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
            var random = new Random();
            int damageDealt = 0;

            // Check for Pursuit effect - doubles power if target is switching
            bool hasPursuitEffect = Move.Effects.Any(e => e is PursuitEffect);
            MoveData moveForDamage = Move;
            if (hasPursuitEffect && Target.HasVolatileStatus(VolatileStatus.SwitchingOut))
            {
                // Create temporary MoveData with doubled power
                moveForDamage = new MoveData
                {
                    Name = Move.Name,
                    Power = Move.Power * 2,
                    Accuracy = Move.Accuracy,
                    Type = Move.Type,
                    Category = Move.Category,
                    MaxPP = Move.MaxPP,
                    Priority = Move.Priority,
                    TargetScope = Move.TargetScope,
                    Effects = Move.Effects
                };
                actions.Add(new MessageAction($"{Target.Pokemon.DisplayName} is switching out! {User.Pokemon.DisplayName} used {Move.Name}!"));
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
            if (!(hasSemiInvulnerableEffect && !isSemiInvulnerableAttackTurn))
            {
                foreach (var effect in Move.Effects)
                {
                    if (effect is DamageEffect damageEffect)
                    {
                        if (hasMultiHitEffect && multiHitEffect != null)
                        {
                            // Multi-hit move: hit multiple times
                            int numHits = random.Next(multiHitEffect.MinHits, multiHitEffect.MaxHits + 1);
                            
                            for (int i = 0; i < numHits; i++)
                            {
                                // Each hit has independent accuracy and damage calculation
                                var pipeline = new DamagePipeline();
                                var context = pipeline.Calculate(User, Target, moveForDamage, field);
                                
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
                            // Single-hit move: normal damage calculation
                            var pipeline = new DamagePipeline();
                            var context = pipeline.Calculate(User, Target, moveForDamage, field);
                            
                            if (context.FinalDamage > 0)
                            {
                                var damageAction = new DamageAction(User, Target, context);
                                actions.Add(damageAction);
                                damageDealt = context.FinalDamage;
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
                        string message;
                        string moveNameLower = Move.Name.ToLower();
                        if (moveNameLower == "fly")
                            message = $"{User.Pokemon.DisplayName} flew up high!";
                        else if (moveNameLower == "dig")
                            message = $"{User.Pokemon.DisplayName} dug underground!";
                        else if (moveNameLower == "dive")
                            message = $"{User.Pokemon.DisplayName} dove underwater!";
                        else if (moveNameLower == "bounce")
                            message = $"{User.Pokemon.DisplayName} bounced up!";
                        else if (moveNameLower == "shadow force" || moveNameLower == "phantom force")
                            message = $"{User.Pokemon.DisplayName} vanished instantly!";
                        else
                            message = $"{User.Pokemon.DisplayName} used {Move.Name}!";
                        actions.Add(new MessageAction(message));
                        // Charge turn handled - continue to process other effects if any
                        continue;
                    }
                }

                switch (effect)
                {

                    case StatusEffect statusEffect:
                        // Check chance
                        if (random.Next(100) < statusEffect.ChancePercent)
                        {
                            var targetSlot = statusEffect.TargetSelf ? User : Target;
                            actions.Add(new ApplyStatusAction(User, targetSlot, statusEffect.Status));
                        }
                        break;

                    case StatChangeEffect statChangeEffect:
                        // Check chance
                        if (random.Next(100) < statChangeEffect.ChancePercent)
                        {
                            var targetSlot = statChangeEffect.TargetSelf ? User : Target;
                            actions.Add(new StatChangeAction(User, targetSlot, statChangeEffect.TargetStat, statChangeEffect.Stages));
                        }
                        break;

                    case HealEffect healEffect:
                        // Heal user by percentage of max HP
                        var healAmount = (int)(User.Pokemon.MaxHP * healEffect.HealPercent / 100f);
                        actions.Add(new HealAction(User, User, healAmount));
                        break;

                    case RecoilEffect recoilEffect:
                        // Apply recoil damage to user (if damage was dealt)
                        // Recoil always deals at least 1 HP if damage was dealt
                        if (damageDealt > 0)
                        {
                            int recoilDamage = (int)(damageDealt * recoilEffect.RecoilPercent / 100f);
                            recoilDamage = System.Math.Max(1, recoilDamage); // At least 1 HP
                            
                            // Create a simple damage context for recoil
                            var recoilContext = new DamageContext(User, User, Move, field)
                            {
                                BaseDamage = recoilDamage,
                                Multiplier = 1.0f,
                                TypeEffectiveness = 1.0f
                            };
                            actions.Add(new DamageAction(User, User, recoilContext));
                        }
                        break;

                    case DrainEffect drainEffect:
                        // Heal user by percentage of damage dealt (if damage was dealt)
                        // Drain always heals at least 1 HP if damage was dealt
                        if (damageDealt > 0)
                        {
                            int drainHealAmount = (int)(damageDealt * drainEffect.DrainPercent / 100f);
                            drainHealAmount = System.Math.Max(1, drainHealAmount); // At least 1 HP
                            
                            actions.Add(new HealAction(User, User, drainHealAmount));
                        }
                        break;

                    case FlinchEffect flinchEffect:
                        // Apply flinch to target (if chance succeeds)
                        if (random.Next(100) < flinchEffect.ChancePercent)
                        {
                            Target.AddVolatileStatus(VolatileStatus.Flinch);
                        }
                        break;

                    case ProtectEffect protectEffect:
                        // Apply Protect: success rate halves with consecutive use (100%, 50%, 25%, 12.5%...)
                        // Counter increments regardless of success (for tracking consecutive uses)
                        int consecutiveUses = User.ProtectConsecutiveUses;
                        int successRate = 100;
                        for (int i = 0; i < consecutiveUses; i++)
                        {
                            successRate /= 2; // Halve each time
                        }
                        
                        // Increment counter before checking success (tracks consecutive uses)
                        User.IncrementProtectUses();
                        
                        if (random.Next(100) < successRate)
                        {
                            User.AddVolatileStatus(VolatileStatus.Protected);
                            actions.Add(new MessageAction(string.Format(GameMessages.MoveProtected, User.Pokemon.DisplayName)));
                        }
                        else
                        {
                            actions.Add(new MessageAction(string.Format(GameMessages.MoveProtectFailed, User.Pokemon.DisplayName)));
                        }
                        break;

                    case CounterEffect counterEffect:
                        // Counter/Mirror Coat: returns 2x damage taken this turn
                        int damageToReturn = 0;
                        if (counterEffect.IsPhysicalCounter)
                        {
                            damageToReturn = User.PhysicalDamageTakenThisTurn * 2;
                        }
                        else
                        {
                            damageToReturn = User.SpecialDamageTakenThisTurn * 2;
                        }

                        if (damageToReturn > 0)
                        {
                            // Create damage context for counter damage
                            var counterContext = new DamageContext(User, Target, Move, field)
                            {
                                BaseDamage = damageToReturn,
                                Multiplier = 1.0f,
                                TypeEffectiveness = 1.0f
                            };
                            actions.Add(new DamageAction(User, Target, counterContext));
                            actions.Add(new MessageAction(string.Format(GameMessages.MoveCountered, User.Pokemon.DisplayName)));
                        }
                        // If no damage taken, Counter fails silently (no message)
                        break;
                }
            }
        }

        /// <summary>
        /// Plays the move animation.
        /// </summary>
        public override Task ExecuteVisual(IBattleView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            if (User.IsEmpty || Target.IsEmpty)
                return Task.CompletedTask;

            return view.PlayMoveAnimation(User, Target, Move.Id);
        }
    }
}

