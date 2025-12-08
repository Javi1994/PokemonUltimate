using System;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Combat.Infrastructure.ValueObjects;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.Field
{
    /// <summary>
    /// Represents a slot on the battlefield that can hold an active Pokemon.
    /// Contains battle-specific state (stat stages, volatile status) that resets when Pokemon switches out.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    public class BattleSlot
    {
        private PokemonInstance _pokemon;
        private VolatileStatus _volatileStatus;
        private StatStages _statStages;
        private ProtectTracker _protectTracker;
        private DamageTakenTracker _damageTakenTracker;
        private MoveStateTracker _moveStateTracker;

        /// <summary>
        /// The Pokemon currently in this slot. Null if empty.
        /// </summary>
        public PokemonInstance Pokemon => _pokemon;

        /// <summary>
        /// The index of this slot on its side (0-based).
        /// </summary>
        public int SlotIndex { get; }

        /// <summary>
        /// The side this slot belongs to. Null if created standalone (for testing).
        /// </summary>
        public BattleSide Side { get; }

        /// <summary>
        /// True if no Pokemon is currently in this slot.
        /// </summary>
        public bool IsEmpty => _pokemon == null;

        /// <summary>
        /// True if the Pokemon in this slot has fainted. False if empty.
        /// </summary>
        public bool HasFainted => _pokemon != null && _pokemon.IsFainted;

        /// <summary>
        /// Current volatile status flags for the Pokemon in this slot.
        /// </summary>
        public VolatileStatus VolatileStatus => _volatileStatus;

        /// <summary>
        /// The action provider for this slot (player input, AI, etc.).
        /// Can be null if not assigned yet.
        /// </summary>
        public IActionProvider ActionProvider { get; set; }

        /// <summary>
        /// Creates a new battle slot without a side reference (for testing).
        /// </summary>
        /// <param name="slotIndex">The index of this slot (0-based, cannot be negative).</param>
        /// <exception cref="ArgumentException">If slotIndex is negative.</exception>
        public BattleSlot(int slotIndex) : this(slotIndex, null)
        {
        }

        /// <summary>
        /// Creates a new battle slot with a side reference.
        /// </summary>
        /// <param name="slotIndex">The index of this slot (0-based, cannot be negative).</param>
        /// <param name="side">The side this slot belongs to. Can be null for standalone slots.</param>
        /// <exception cref="ArgumentException">If slotIndex is negative.</exception>
        public BattleSlot(int slotIndex, BattleSide side)
        {
            if (slotIndex < 0)
                throw new ArgumentException(ErrorMessages.SlotIndexCannotBeNegative, nameof(slotIndex));

            SlotIndex = slotIndex;
            Side = side;
            _statStages = new StatStages();
            _volatileStatus = VolatileStatus.None;
            _protectTracker = new ProtectTracker();
            _damageTakenTracker = new DamageTakenTracker();
            _moveStateTracker = new MoveStateTracker();
        }

        /// <summary>
        /// Places a Pokemon in this slot.
        /// </summary>
        /// <param name="pokemon">The Pokemon to place. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If pokemon is null.</exception>
        public void SetPokemon(PokemonInstance pokemon)
        {
            if (pokemon == null)
                throw new ArgumentNullException(nameof(pokemon), ErrorMessages.PokemonCannotBeNull);

            _pokemon = pokemon;
            ResetBattleState();
        }

        /// <summary>
        /// Removes the Pokemon from this slot.
        /// </summary>
        public void ClearSlot()
        {
            _pokemon = null;
            ResetBattleState();
        }

        /// <summary>
        /// Gets the current stat stage for a stat (-6 to +6).
        /// </summary>
        /// <param name="stat">The stat to query.</param>
        /// <returns>The current stage (-6 to +6).</returns>
        public int GetStatStage(Stat stat)
        {
            return _statStages.GetStage(stat);
        }

        /// <summary>
        /// Modifies a stat stage by the specified amount.
        /// </summary>
        /// <param name="stat">The stat to modify. Cannot be HP.</param>
        /// <param name="change">The amount to change (+/-).</param>
        /// <returns>The actual change applied (may be less if clamped).</returns>
        /// <exception cref="ArgumentException">If stat is HP.</exception>
        public int ModifyStatStage(Stat stat, int change)
        {
            _statStages = _statStages.ModifyStage(stat, change, out var actualChange);
            return actualChange;
        }

        /// <summary>
        /// Checks if the Pokemon has a specific volatile status.
        /// </summary>
        /// <param name="status">The status to check.</param>
        /// <returns>True if the Pokemon has the status.</returns>
        public bool HasVolatileStatus(VolatileStatus status)
        {
            return (_volatileStatus & status) == status;
        }

        /// <summary>
        /// Adds a volatile status to the Pokemon.
        /// </summary>
        /// <param name="status">The status to add.</param>
        public void AddVolatileStatus(VolatileStatus status)
        {
            _volatileStatus |= status;
        }

        /// <summary>
        /// Removes a volatile status from the Pokemon.
        /// </summary>
        /// <param name="status">The status to remove.</param>
        public void RemoveVolatileStatus(VolatileStatus status)
        {
            _volatileStatus &= ~status;
        }

        /// <summary>
        /// Gets the number of consecutive Protect uses (for success rate calculation).
        /// </summary>
        public int ProtectConsecutiveUses => _protectTracker.ConsecutiveUses;

        /// <summary>
        /// Increments the consecutive Protect uses counter.
        /// </summary>
        public void IncrementProtectUses()
        {
            _protectTracker = _protectTracker.Increment();
        }

        /// <summary>
        /// Resets the consecutive Protect uses counter.
        /// </summary>
        public void ResetProtectUses()
        {
            _protectTracker = _protectTracker.Reset();
        }

        /// <summary>
        /// Records physical damage taken this turn (for Counter).
        /// </summary>
        public void RecordPhysicalDamage(int damage)
        {
            _damageTakenTracker = _damageTakenTracker.AddPhysicalDamage(damage);
        }

        /// <summary>
        /// Records special damage taken this turn (for Mirror Coat).
        /// </summary>
        public void RecordSpecialDamage(int damage)
        {
            _damageTakenTracker = _damageTakenTracker.AddSpecialDamage(damage);
        }

        /// <summary>
        /// Gets the physical damage taken this turn.
        /// </summary>
        public int PhysicalDamageTakenThisTurn => _damageTakenTracker.PhysicalDamage;

        /// <summary>
        /// Gets the special damage taken this turn.
        /// </summary>
        public int SpecialDamageTakenThisTurn => _damageTakenTracker.SpecialDamage;

        /// <summary>
        /// Gets whether the Pokemon was hit while focusing (for Focus Punch).
        /// </summary>
        public bool WasHitWhileFocusing => _damageTakenTracker.WasHitWhileFocusing;

        /// <summary>
        /// Marks that the Pokemon was hit while focusing.
        /// </summary>
        public void MarkHitWhileFocusing()
        {
            if (HasVolatileStatus(VolatileStatus.Focusing))
            {
                _damageTakenTracker = _damageTakenTracker.SetHitWhileFocusing(true);
            }
        }

        /// <summary>
        /// Gets the name of the semi-invulnerable move being used.
        /// </summary>
        public string SemiInvulnerableMoveName => _moveStateTracker.SemiInvulnerableState.MoveName;

        /// <summary>
        /// Gets whether the Pokemon is in the charging phase of a semi-invulnerable move (turn 1).
        /// </summary>
        public bool IsSemiInvulnerableCharging => _moveStateTracker.SemiInvulnerableState.IsCharging;

        /// <summary>
        /// Sets the semi-invulnerable move name and charging state.
        /// </summary>
        public void SetSemiInvulnerableMove(string moveName, bool isCharging = true)
        {
            var newState = _moveStateTracker.SemiInvulnerableState.SetMove(moveName, isCharging);
            _moveStateTracker = _moveStateTracker.WithSemiInvulnerableState(newState);
        }

        /// <summary>
        /// Clears the semi-invulnerable move name and state.
        /// </summary>
        public void ClearSemiInvulnerableMove()
        {
            var newState = _moveStateTracker.SemiInvulnerableState.Clear();
            _moveStateTracker = _moveStateTracker.WithSemiInvulnerableState(newState);
        }

        /// <summary>
        /// Marks that the semi-invulnerable move is ready to attack (turn 2).
        /// </summary>
        public void SetSemiInvulnerableReady()
        {
            var newState = _moveStateTracker.SemiInvulnerableState.SetReady();
            _moveStateTracker = _moveStateTracker.WithSemiInvulnerableState(newState);
        }

        /// <summary>
        /// Gets the name of the charging move being used.
        /// </summary>
        public string ChargingMoveName => _moveStateTracker.ChargingMoveState.MoveName;

        /// <summary>
        /// Sets the charging move name.
        /// </summary>
        public void SetChargingMove(string moveName)
        {
            var newState = _moveStateTracker.ChargingMoveState.SetMove(moveName);
            _moveStateTracker = _moveStateTracker.WithChargingMoveState(newState);
        }

        /// <summary>
        /// Clears the charging move name.
        /// </summary>
        public void ClearChargingMove()
        {
            var newState = _moveStateTracker.ChargingMoveState.Clear();
            _moveStateTracker = _moveStateTracker.WithChargingMoveState(newState);
        }

        /// <summary>
        /// Resets damage tracking for Counter/Mirror Coat at end of turn.
        /// </summary>
        public void ResetDamageTracking()
        {
            _damageTakenTracker = _damageTakenTracker.Reset();
        }

        /// <summary>
        /// Resets all battle-specific state (stat stages, volatile status).
        /// Called when a Pokemon switches in or out.
        /// </summary>
        public void ResetBattleState()
        {
            _statStages = _statStages.Reset();
            _volatileStatus = VolatileStatus.None;
            _protectTracker = _protectTracker.Reset();
            _damageTakenTracker = _damageTakenTracker.Reset();
            _moveStateTracker = _moveStateTracker.Reset();
        }
    }
}

