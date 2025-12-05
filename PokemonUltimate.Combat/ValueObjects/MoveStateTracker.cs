namespace PokemonUltimate.Combat.ValueObjects
{
    /// <summary>
    /// Value Object tracking all move-related states for a Pokemon in battle.
    /// Encapsulates semi-invulnerable states, charging states, and other move-specific tracking.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    public class MoveStateTracker
    {
        /// <summary>
        /// Creates a new MoveStateTracker instance with all states cleared.
        /// </summary>
        public MoveStateTracker()
        {
            SemiInvulnerableState = new SemiInvulnerableState();
            ChargingMoveState = new ChargingMoveState();
        }

        /// <summary>
        /// Creates a new MoveStateTracker instance with the specified states.
        /// </summary>
        /// <param name="semiInvulnerableState">The semi-invulnerable state.</param>
        /// <param name="chargingMoveState">The charging move state.</param>
        private MoveStateTracker(SemiInvulnerableState semiInvulnerableState, ChargingMoveState chargingMoveState)
        {
            SemiInvulnerableState = semiInvulnerableState ?? new SemiInvulnerableState();
            ChargingMoveState = chargingMoveState ?? new ChargingMoveState();
        }

        /// <summary>
        /// The semi-invulnerable move state (Dig, Dive, Fly, etc.).
        /// </summary>
        public SemiInvulnerableState SemiInvulnerableState { get; }

        /// <summary>
        /// The charging move state (Solar Beam, Sky Attack, etc.).
        /// </summary>
        public ChargingMoveState ChargingMoveState { get; }

        /// <summary>
        /// Creates a new MoveStateTracker instance with the semi-invulnerable state updated.
        /// </summary>
        /// <param name="semiInvulnerableState">The new semi-invulnerable state.</param>
        /// <returns>A new MoveStateTracker instance with the updated state.</returns>
        public MoveStateTracker WithSemiInvulnerableState(SemiInvulnerableState semiInvulnerableState)
        {
            return new MoveStateTracker(semiInvulnerableState, ChargingMoveState);
        }

        /// <summary>
        /// Creates a new MoveStateTracker instance with the charging move state updated.
        /// </summary>
        /// <param name="chargingMoveState">The new charging move state.</param>
        /// <returns>A new MoveStateTracker instance with the updated state.</returns>
        public MoveStateTracker WithChargingMoveState(ChargingMoveState chargingMoveState)
        {
            return new MoveStateTracker(SemiInvulnerableState, chargingMoveState);
        }

        /// <summary>
        /// Creates a new MoveStateTracker instance with all states reset.
        /// </summary>
        /// <returns>A new MoveStateTracker instance with all states cleared.</returns>
        public MoveStateTracker Reset()
        {
            return new MoveStateTracker();
        }
    }
}
