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

            // 4. Deduct PP
            MoveInstance.Use();

            // 5. Generate "X used Y!" message
            actions.Add(new MessageAction($"{User.Pokemon.DisplayName} used {Move.Name}!"));

            // 6. Accuracy check
            if (!AccuracyChecker.CheckHit(User, Target, Move))
            {
                actions.Add(new MessageAction(GameMessages.MoveMissed));
                return actions;
            }

            // 7. Process move effects
            ProcessEffects(field, actions);

            return actions;
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

            // First pass: Process damage effect to get damageDealt
            foreach (var effect in Move.Effects)
            {
                if (effect is DamageEffect damageEffect)
                {
                    // Calculate and apply damage
                    var pipeline = new DamagePipeline();
                    var context = pipeline.Calculate(User, Target, Move, field);
                    
                    if (context.FinalDamage > 0)
                    {
                        var damageAction = new DamageAction(User, Target, context);
                        actions.Add(damageAction);
                        damageDealt = context.FinalDamage;
                    }
                    break; // Only process first DamageEffect
                }
            }

            // Second pass: Process all other effects (including recoil/drain that depend on damageDealt)
            foreach (var effect in Move.Effects)
            {
                // Skip DamageEffect (already processed in first pass)
                if (effect is DamageEffect)
                    continue;

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

