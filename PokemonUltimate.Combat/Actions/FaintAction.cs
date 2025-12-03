using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Core.Constants;

namespace PokemonUltimate.Combat.Actions
{
    /// <summary>
    /// Handles a Pokemon fainting (HP reaching zero).
    /// Plays faint animation and marks the Pokemon as fainted.
    /// Battle end checking is handled by CombatEngine (Phase 2.6).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class FaintAction : BattleAction
    {
        /// <summary>
        /// The slot containing the fainted Pokemon.
        /// </summary>
        public BattleSlot Target { get; }

        /// <summary>
        /// Creates a new faint action.
        /// </summary>
        /// <param name="user">The slot that caused the faint (attacker). Can be null for system actions.</param>
        /// <param name="target">The slot containing the fainted Pokemon. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If target is null.</exception>
        public FaintAction(BattleSlot user, BattleSlot target) : base(user)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target), ErrorMessages.PokemonCannotBeNull);
        }

        /// <summary>
        /// Marks the Pokemon as fainted.
        /// Battle end checking is deferred to CombatEngine.
        /// </summary>
        public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (Target.IsEmpty)
                return Enumerable.Empty<BattleAction>();

            // Pokemon should already be at 0 HP (set by DamageAction)
            // This action just handles the visual and any cleanup
            // Battle end check will be handled by CombatEngine

            return Enumerable.Empty<BattleAction>();
        }

        /// <summary>
        /// Plays the faint animation.
        /// </summary>
        public override Task ExecuteVisual(IBattleView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            if (Target.IsEmpty)
                return Task.CompletedTask;

            return view.PlayFaintAnimation(Target);
        }
    }
}

