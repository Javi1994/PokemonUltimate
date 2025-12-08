using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Combat.Integration.View;
using PokemonUltimate.Combat.Integration.View.Definition;
using PokemonUltimate.Content.Catalogs.Status;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Actions
{
    /// <summary>
    /// Applies a persistent status condition to a Pokemon.
    /// Can also clear status by applying PersistentStatus.None.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class ApplyStatusAction : BattleAction
    {
        /// <summary>
        /// The slot receiving the status condition.
        /// </summary>
        public BattleSlot Target { get; }

        /// <summary>
        /// The status condition to apply.
        /// </summary>
        public PersistentStatus Status { get; }

        /// <summary>
        /// Creates a new apply status action.
        /// </summary>
        /// <param name="user">The slot that initiated this status application. Can be null for system actions.</param>
        /// <param name="target">The slot receiving the status. Cannot be null.</param>
        /// <param name="status">The status condition to apply.</param>
        /// <exception cref="ArgumentNullException">If target is null.</exception>
        public ApplyStatusAction(BattleSlot user, BattleSlot target, PersistentStatus status) : base(user)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target), ErrorMessages.PokemonCannotBeNull);
            Status = status;
        }

        /// <summary>
        /// Applies the status condition to the target Pokemon.
        /// If status is None, clears any existing status.
        /// Safeguard prevents status application (except for clearing status).
        /// </summary>
        public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (Target.IsEmpty)
                return Enumerable.Empty<BattleAction>();

            // Check if clearing status (Status.None)
            if (Status == PersistentStatus.None)
            {
                Target.Pokemon.Status = Status;
                return Enumerable.Empty<BattleAction>();
            }

            // Check if Pokemon already has a status
            if (Target.Pokemon.Status != PersistentStatus.None)
            {
                // Pokemon already has a status, cannot apply another
                return Enumerable.Empty<BattleAction>();
            }

            // Get status effect data to check immunities
            var statusData = StatusCatalog.GetByStatus(Status);
            if (statusData != null)
            {
                // Check type immunities (e.g., Fire types immune to Burn)
                var pokemon = Target.Pokemon;
                if (statusData.IsTypeImmune(pokemon.Species.PrimaryType) ||
                    (pokemon.Species.SecondaryType.HasValue && statusData.IsTypeImmune(pokemon.Species.SecondaryType.Value)))
                {
                    // Pokemon is immune to this status - don't apply it
                    return Enumerable.Empty<BattleAction>();
                }
            }

            // Safeguard prevents status application
            if (Target.Side.HasSideCondition(SideCondition.Safeguard))
            {
                var safeguardData = Target.Side.GetSideConditionData(SideCondition.Safeguard);
                if (safeguardData != null && safeguardData.PreventsStatus)
                {
                    // Status is blocked by Safeguard
                    return Enumerable.Empty<BattleAction>();
                }
            }

            // Apply the status
            Target.Pokemon.Status = Status;

            return Enumerable.Empty<BattleAction>();
        }

        /// <summary>
        /// Plays the status application animation.
        /// Skips animation if status is None (clearing status).
        /// </summary>
        public override Task ExecuteVisual(IBattleView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            if (Target.IsEmpty || Status == PersistentStatus.None)
                return Task.CompletedTask;

            string statusName = Status.ToString();
            return view.PlayStatusAnimation(Target, statusName);
        }
    }
}

