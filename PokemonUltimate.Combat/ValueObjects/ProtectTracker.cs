namespace PokemonUltimate.Combat.ValueObjects
{
    /// <summary>
    /// Value Object tracking consecutive Protect uses for success rate calculation.
    /// Encapsulates the protect usage counter with validation.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    public class ProtectTracker
    {
        /// <summary>
        /// Creates a new ProtectTracker instance with zero consecutive uses.
        /// </summary>
        public ProtectTracker()
        {
            ConsecutiveUses = 0;
        }

        /// <summary>
        /// Creates a new ProtectTracker instance with the specified consecutive uses.
        /// </summary>
        /// <param name="consecutiveUses">The number of consecutive uses. Must be non-negative.</param>
        /// <exception cref="System.ArgumentException">If consecutiveUses is negative.</exception>
        private ProtectTracker(int consecutiveUses)
        {
            if (consecutiveUses < 0)
                throw new System.ArgumentException("Consecutive uses cannot be negative.", nameof(consecutiveUses));

            ConsecutiveUses = consecutiveUses;
        }

        /// <summary>
        /// The number of consecutive Protect uses.
        /// </summary>
        public int ConsecutiveUses { get; }

        /// <summary>
        /// Creates a new ProtectTracker instance with the consecutive uses incremented.
        /// </summary>
        /// <returns>A new ProtectTracker instance with ConsecutiveUses incremented by 1.</returns>
        public ProtectTracker Increment()
        {
            return new ProtectTracker(ConsecutiveUses + 1);
        }

        /// <summary>
        /// Creates a new ProtectTracker instance with consecutive uses reset to zero.
        /// </summary>
        /// <returns>A new ProtectTracker instance with ConsecutiveUses at 0.</returns>
        public ProtectTracker Reset()
        {
            return new ProtectTracker();
        }
    }
}
