using System.Collections.Generic;
using PokemonUltimate.Combat.Foundation.Field;

namespace PokemonUltimate.Combat.Infrastructure.Validation
{
    /// <summary>
    /// Interface for validating battle state invariants and consistency.
    /// Ensures the battle state remains valid throughout execution.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    public interface IBattleStateValidator
    {
        /// <summary>
        /// Validates that a slot's state is consistent.
        /// Checks stat stages, volatile status, and other state invariants.
        /// </summary>
        /// <param name="slot">The slot to validate.</param>
        /// <returns>List of validation errors. Empty if valid.</returns>
        IReadOnlyList<string> ValidateSlot(BattleSlot slot);

        /// <summary>
        /// Validates that a side's state is consistent.
        /// Checks party consistency, active slots, and side conditions.
        /// </summary>
        /// <param name="side">The side to validate.</param>
        /// <returns>List of validation errors. Empty if valid.</returns>
        IReadOnlyList<string> ValidateSide(BattleSide side);

        /// <summary>
        /// Validates that the battlefield state is consistent.
        /// Checks weather, terrain, and overall field state.
        /// </summary>
        /// <param name="field">The battlefield to validate.</param>
        /// <returns>List of validation errors. Empty if valid.</returns>
        IReadOnlyList<string> ValidateField(BattleField field);

        /// <summary>
        /// Validates stat stages are within valid range (-6 to +6).
        /// </summary>
        /// <param name="slot">The slot to validate.</param>
        /// <returns>List of validation errors. Empty if valid.</returns>
        IReadOnlyList<string> ValidateStatStages(BattleSlot slot);

        /// <summary>
        /// Validates that state counters are within valid ranges.
        /// Checks protect consecutive uses, damage tracking, etc.
        /// </summary>
        /// <param name="slot">The slot to validate.</param>
        /// <returns>List of validation errors. Empty if valid.</returns>
        IReadOnlyList<string> ValidateStateCounters(BattleSlot slot);

        /// <summary>
        /// Validates that party consistency is maintained.
        /// Ensures active Pokemon are in the party and party Pokemon are tracked correctly.
        /// </summary>
        /// <param name="side">The side to validate.</param>
        /// <returns>List of validation errors. Empty if valid.</returns>
        IReadOnlyList<string> ValidatePartyConsistency(BattleSide side);
    }
}
