using System;
using System.Linq;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Constants;
using PokemonUltimate.Core.Data.Constants;

namespace PokemonUltimate.Combat.Engine.Service
{
    /// <summary>
    /// Determines the outcome of a battle by inspecting the battlefield state.
    /// Stateless service that checks win/loss conditions.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public static class BattleArbiterService
    {
        /// <summary>
        /// Checks the current outcome of the battle.
        /// </summary>
        /// <param name="field">The battlefield to inspect. Cannot be null.</param>
        /// <returns>The current battle outcome.</returns>
        /// <exception cref="ArgumentNullException">If field is null.</exception>
        public static BattleOutcome CheckOutcome(BattleField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            // Check if sides have active Pokemon on the field
            bool playerHasActiveSlots = field.PlayerSide.HasActivePokemon();
            bool enemyHasActiveSlots = field.EnemySide.HasActivePokemon();

            // If a side has no active slots but has Pokemon in party that aren't fainted,
            // it means they can't switch (all Pokemon are either fainted or already active)
            // This is equivalent to being defeated
            bool playerDefeated = !playerHasActiveSlots &&
                (field.PlayerSide.IsDefeated() || !HasAvailableSwitches(field.PlayerSide));
            bool enemyDefeated = !enemyHasActiveSlots &&
                (field.EnemySide.IsDefeated() || !HasAvailableSwitches(field.EnemySide));

            // Both sides defeated simultaneously (e.g., Explosion double KO)
            if (playerDefeated && enemyDefeated)
                return BattleOutcome.Draw;

            // Player defeated
            if (playerDefeated)
                return BattleOutcome.Defeat;

            // Enemy defeated
            if (enemyDefeated)
                return BattleOutcome.Victory;

            // Battle continues
            return BattleOutcome.Ongoing;
        }

        /// <summary>
        /// Checks if a side has Pokemon available for switching.
        /// </summary>
        /// <param name="side">The side to check.</param>
        /// <returns>True if there are Pokemon available to switch in.</returns>
        private static bool HasAvailableSwitches(BattleSide side)
        {
            if (side == null || side.Party == null || side.Party.Count == 0)
                return false;

            // Check if there are any non-fainted Pokemon that are not currently in active slots (or are in slots but fainted)
            var activeNonFaintedPokemon = side.Slots
                .Where(s => !s.IsEmpty && !s.HasFainted)
                .Select(s => s.Pokemon)
                .ToHashSet();

            return side.Party.Any(p => !p.IsFainted && !activeNonFaintedPokemon.Contains(p));
        }
    }
}

