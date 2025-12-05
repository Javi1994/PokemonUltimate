using System;
using System.Collections.Generic;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Combat
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
        private const int MinStatStage = -6;
        private const int MaxStatStage = 6;

        private readonly Dictionary<Stat, int> _statStages;
        private PokemonInstance _pokemon;
        private VolatileStatus _volatileStatus;
        private int _protectConsecutiveUses;
        private int _physicalDamageTakenThisTurn;
        private int _specialDamageTakenThisTurn;
        private bool _wasHitWhileFocusing;
        private string _semiInvulnerableMoveName;
        private bool _isSemiInvulnerableCharging; // True if charging (turn 1), false if attacking (turn 2)
        private string _chargingMoveName;

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
            _statStages = new Dictionary<Stat, int>
            {
                { Stat.Attack, 0 },
                { Stat.Defense, 0 },
                { Stat.SpAttack, 0 },
                { Stat.SpDefense, 0 },
                { Stat.Speed, 0 },
                { Stat.Accuracy, 0 },
                { Stat.Evasion, 0 }
            };
            _volatileStatus = VolatileStatus.None;
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
            if (stat == Stat.HP)
                return 0;

            return _statStages.TryGetValue(stat, out var stage) ? stage : 0;
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
            if (stat == Stat.HP)
                throw new ArgumentException(ErrorMessages.CannotModifyHPStatStage, nameof(stat));

            if (!_statStages.ContainsKey(stat))
                return 0;

            var oldStage = _statStages[stat];
            var newStage = Math.Max(MinStatStage, Math.Min(MaxStatStage, oldStage + change));
            _statStages[stat] = newStage;

            return newStage - oldStage;
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
        public int ProtectConsecutiveUses => _protectConsecutiveUses;

        /// <summary>
        /// Increments the consecutive Protect uses counter.
        /// </summary>
        public void IncrementProtectUses()
        {
            _protectConsecutiveUses++;
        }

        /// <summary>
        /// Resets the consecutive Protect uses counter.
        /// </summary>
        public void ResetProtectUses()
        {
            _protectConsecutiveUses = 0;
        }

        /// <summary>
        /// Records physical damage taken this turn (for Counter).
        /// </summary>
        public void RecordPhysicalDamage(int damage)
        {
            _physicalDamageTakenThisTurn += damage;
        }

        /// <summary>
        /// Records special damage taken this turn (for Mirror Coat).
        /// </summary>
        public void RecordSpecialDamage(int damage)
        {
            _specialDamageTakenThisTurn += damage;
        }

        /// <summary>
        /// Gets the physical damage taken this turn.
        /// </summary>
        public int PhysicalDamageTakenThisTurn => _physicalDamageTakenThisTurn;

        /// <summary>
        /// Gets the special damage taken this turn.
        /// </summary>
        public int SpecialDamageTakenThisTurn => _specialDamageTakenThisTurn;

        /// <summary>
        /// Gets whether the Pokemon was hit while focusing (for Focus Punch).
        /// </summary>
        public bool WasHitWhileFocusing => _wasHitWhileFocusing;

        /// <summary>
        /// Marks that the Pokemon was hit while focusing.
        /// </summary>
        public void MarkHitWhileFocusing()
        {
            if (HasVolatileStatus(VolatileStatus.Focusing))
            {
                _wasHitWhileFocusing = true;
            }
        }

        /// <summary>
        /// Gets the name of the semi-invulnerable move being used.
        /// </summary>
        public string SemiInvulnerableMoveName => _semiInvulnerableMoveName;

        /// <summary>
        /// Gets whether the Pokemon is in the charging phase of a semi-invulnerable move (turn 1).
        /// </summary>
        public bool IsSemiInvulnerableCharging => _isSemiInvulnerableCharging;

        /// <summary>
        /// Sets the semi-invulnerable move name and charging state.
        /// </summary>
        public void SetSemiInvulnerableMove(string moveName, bool isCharging = true)
        {
            _semiInvulnerableMoveName = moveName;
            _isSemiInvulnerableCharging = isCharging;
        }

        /// <summary>
        /// Clears the semi-invulnerable move name and state.
        /// </summary>
        public void ClearSemiInvulnerableMove()
        {
            _semiInvulnerableMoveName = null;
            _isSemiInvulnerableCharging = false;
        }

        /// <summary>
        /// Marks that the semi-invulnerable move is ready to attack (turn 2).
        /// </summary>
        public void SetSemiInvulnerableReady()
        {
            _isSemiInvulnerableCharging = false;
        }

        /// <summary>
        /// Gets the name of the charging move being used.
        /// </summary>
        public string ChargingMoveName => _chargingMoveName;

        /// <summary>
        /// Sets the charging move name.
        /// </summary>
        public void SetChargingMove(string moveName)
        {
            _chargingMoveName = moveName;
        }

        /// <summary>
        /// Clears the charging move name.
        /// </summary>
        public void ClearChargingMove()
        {
            _chargingMoveName = null;
        }

        /// <summary>
        /// Resets damage tracking for Counter/Mirror Coat at end of turn.
        /// </summary>
        public void ResetDamageTracking()
        {
            _physicalDamageTakenThisTurn = 0;
            _specialDamageTakenThisTurn = 0;
            _wasHitWhileFocusing = false;
        }

        /// <summary>
        /// Resets all battle-specific state (stat stages, volatile status).
        /// Called when a Pokemon switches in or out.
        /// </summary>
        public void ResetBattleState()
        {
            _statStages[Stat.Attack] = 0;
            _statStages[Stat.Defense] = 0;
            _statStages[Stat.SpAttack] = 0;
            _statStages[Stat.SpDefense] = 0;
            _statStages[Stat.Speed] = 0;
            _statStages[Stat.Accuracy] = 0;
            _statStages[Stat.Evasion] = 0;

            _volatileStatus = VolatileStatus.None;
            _protectConsecutiveUses = 0;
            _physicalDamageTakenThisTurn = 0;
            _specialDamageTakenThisTurn = 0;
            _wasHitWhileFocusing = false;
            _semiInvulnerableMoveName = null;
            _isSemiInvulnerableCharging = false;
            _chargingMoveName = null;
        }
    }
}

