namespace PokemonUltimate.Combat.ValueObjects
{
    /// <summary>
    /// Value Object tracking damage taken during a turn for Counter/Mirror Coat and Focus Punch.
    /// Encapsulates physical damage, special damage, and focus interruption tracking.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    public class DamageTracker
    {
        /// <summary>
        /// Creates a new DamageTracker instance with all values at zero.
        /// </summary>
        public DamageTracker()
        {
            PhysicalDamage = 0;
            SpecialDamage = 0;
            WasHitWhileFocusing = false;
        }

        /// <summary>
        /// Creates a new DamageTracker instance with the specified values.
        /// </summary>
        /// <param name="physicalDamage">Physical damage taken this turn.</param>
        /// <param name="specialDamage">Special damage taken this turn.</param>
        /// <param name="wasHitWhileFocusing">Whether the Pokemon was hit while focusing.</param>
        private DamageTracker(int physicalDamage, int specialDamage, bool wasHitWhileFocusing)
        {
            PhysicalDamage = physicalDamage;
            SpecialDamage = specialDamage;
            WasHitWhileFocusing = wasHitWhileFocusing;
        }

        /// <summary>
        /// Physical damage taken this turn (for Counter).
        /// </summary>
        public int PhysicalDamage { get; }

        /// <summary>
        /// Special damage taken this turn (for Mirror Coat).
        /// </summary>
        public int SpecialDamage { get; }

        /// <summary>
        /// Whether the Pokemon was hit while focusing (for Focus Punch).
        /// </summary>
        public bool WasHitWhileFocusing { get; }

        /// <summary>
        /// Creates a new DamageTracker instance with physical damage added.
        /// </summary>
        /// <param name="damage">The physical damage to add.</param>
        /// <returns>A new DamageTracker instance with the damage added.</returns>
        public DamageTracker AddPhysicalDamage(int damage)
        {
            return new DamageTracker(PhysicalDamage + damage, SpecialDamage, WasHitWhileFocusing);
        }

        /// <summary>
        /// Creates a new DamageTracker instance with special damage added.
        /// </summary>
        /// <param name="damage">The special damage to add.</param>
        /// <returns>A new DamageTracker instance with the damage added.</returns>
        public DamageTracker AddSpecialDamage(int damage)
        {
            return new DamageTracker(PhysicalDamage, SpecialDamage + damage, WasHitWhileFocusing);
        }

        /// <summary>
        /// Creates a new DamageTracker instance with the focus hit flag set.
        /// </summary>
        /// <param name="wasHit">Whether the Pokemon was hit while focusing.</param>
        /// <returns>A new DamageTracker instance with the flag set.</returns>
        public DamageTracker SetHitWhileFocusing(bool wasHit)
        {
            return new DamageTracker(PhysicalDamage, SpecialDamage, wasHit);
        }

        /// <summary>
        /// Creates a new DamageTracker instance with all values reset to zero.
        /// </summary>
        /// <returns>A new DamageTracker instance with all values at zero.</returns>
        public DamageTracker Reset()
        {
            return new DamageTracker();
        }
    }
}
