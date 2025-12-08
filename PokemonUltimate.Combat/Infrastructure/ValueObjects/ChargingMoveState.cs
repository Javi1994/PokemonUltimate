using System;

namespace PokemonUltimate.Combat.Infrastructure.ValueObjects
{
    /// <summary>
    /// Value Object representing a charging move state (e.g., Solar Beam, Sky Attack).
    /// Encapsulates the charging move name.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    public class ChargingMoveState
    {
        /// <summary>
        /// Creates a new ChargingMoveState instance with no active move.
        /// </summary>
        public ChargingMoveState()
        {
            MoveName = null;
        }

        /// <summary>
        /// Creates a new ChargingMoveState instance with the specified move name.
        /// </summary>
        /// <param name="moveName">The name of the charging move. Can be null.</param>
        private ChargingMoveState(string moveName)
        {
            MoveName = moveName;
        }

        /// <summary>
        /// The name of the charging move being used. Null if no move is active.
        /// </summary>
        public string MoveName { get; }

        /// <summary>
        /// True if a charging move is currently active.
        /// </summary>
        public bool IsActive => MoveName != null;

        /// <summary>
        /// Creates a new ChargingMoveState instance with the move set.
        /// </summary>
        /// <param name="moveName">The name of the charging move. Cannot be null or empty.</param>
        /// <returns>A new ChargingMoveState instance with the move set.</returns>
        /// <exception cref="ArgumentException">If moveName is null or empty.</exception>
        public ChargingMoveState SetMove(string moveName)
        {
            if (string.IsNullOrWhiteSpace(moveName))
                throw new ArgumentException("Move name cannot be null or empty.", nameof(moveName));

            return new ChargingMoveState(moveName);
        }

        /// <summary>
        /// Creates a new ChargingMoveState instance with the move cleared.
        /// </summary>
        /// <returns>A new ChargingMoveState instance with no active move.</returns>
        public ChargingMoveState Clear()
        {
            return new ChargingMoveState();
        }
    }
}
