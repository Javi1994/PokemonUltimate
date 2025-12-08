using System;

namespace PokemonUltimate.Combat.Infrastructure.ValueObjects
{
    /// <summary>
    /// Value Object representing the semi-invulnerable move state (Dig, Dive, Fly, etc.).
    /// Encapsulates the move name and charging phase state.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    public class SemiInvulnerableState
    {
        /// <summary>
        /// Creates a new SemiInvulnerableState instance with no active move.
        /// </summary>
        public SemiInvulnerableState()
        {
            MoveName = null;
            IsCharging = false;
        }

        /// <summary>
        /// Creates a new SemiInvulnerableState instance with the specified values.
        /// </summary>
        /// <param name="moveName">The name of the semi-invulnerable move. Can be null.</param>
        /// <param name="isCharging">True if charging (turn 1), false if attacking (turn 2).</param>
        private SemiInvulnerableState(string moveName, bool isCharging)
        {
            MoveName = moveName;
            IsCharging = isCharging;
        }

        /// <summary>
        /// The name of the semi-invulnerable move being used. Null if no move is active.
        /// </summary>
        public string MoveName { get; }

        /// <summary>
        /// True if the Pokemon is in the charging phase (turn 1), false if attacking (turn 2).
        /// </summary>
        public bool IsCharging { get; }

        /// <summary>
        /// True if a semi-invulnerable move is currently active.
        /// </summary>
        public bool IsActive => MoveName != null;

        /// <summary>
        /// Creates a new SemiInvulnerableState instance with the move set.
        /// </summary>
        /// <param name="moveName">The name of the semi-invulnerable move. Cannot be null or empty.</param>
        /// <param name="isCharging">True if charging (turn 1), false if attacking (turn 2).</param>
        /// <returns>A new SemiInvulnerableState instance with the move set.</returns>
        /// <exception cref="ArgumentException">If moveName is null or empty.</exception>
        public SemiInvulnerableState SetMove(string moveName, bool isCharging = true)
        {
            if (string.IsNullOrWhiteSpace(moveName))
                throw new ArgumentException("Move name cannot be null or empty.", nameof(moveName));

            return new SemiInvulnerableState(moveName, isCharging);
        }

        /// <summary>
        /// Creates a new SemiInvulnerableState instance with the move cleared.
        /// </summary>
        /// <returns>A new SemiInvulnerableState instance with no active move.</returns>
        public SemiInvulnerableState Clear()
        {
            return new SemiInvulnerableState();
        }

        /// <summary>
        /// Creates a new SemiInvulnerableState instance with IsCharging set to false (ready to attack).
        /// </summary>
        /// <returns>A new SemiInvulnerableState instance with IsCharging = false.</returns>
        public SemiInvulnerableState SetReady()
        {
            if (!IsActive)
                return this;

            return new SemiInvulnerableState(MoveName, false);
        }
    }
}
