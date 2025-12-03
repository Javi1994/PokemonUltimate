using System;
using System.Collections.Generic;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Builders
{
    /// <summary>
    /// Fluent builder for creating StatusEffectData.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.11: Builders
    /// **Documentation**: See `docs/features/1-game-data/1.11-builders/architecture.md`
    /// </remarks>
    public class StatusEffectBuilder
    {
        private readonly string _id;
        private readonly string _name;
        private string _description = string.Empty;
        private PersistentStatus _persistentStatus = PersistentStatus.None;
        private VolatileStatus _volatileStatus = VolatileStatus.None;
        private int _minTurns;
        private int _maxTurns;
        private float _endOfTurnDamage;
        private bool _damageEscalates;
        private int _escalatingDamageStart = 1;
        private bool _drainsToOpponent;
        private float _moveFailChance;
        private float _recoveryChancePerTurn;
        private float _speedMultiplier = 1.0f;
        private float _attackMultiplier = 1.0f;
        private bool _attackModifierIsPhysicalOnly;
        private float _selfHitChance;
        private int _selfHitPower;
        private List<PokemonType> _immuneTypes = new List<PokemonType>();
        private List<PokemonType> _curedByMoveTypes = new List<PokemonType>();
        private bool _preventsSwitching;
        private bool _forcesMove;
        private MoveCategory? _restrictedMoveCategory;

        private StatusEffectBuilder(string name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _id = name.ToLowerInvariant().Replace(" ", "-");
        }

        /// <summary>
        /// Start defining a new status effect.
        /// </summary>
        public static StatusEffectBuilder Define(string name) => new StatusEffectBuilder(name);

        #region Identity

        public StatusEffectBuilder Description(string description)
        {
            _description = description;
            return this;
        }

        /// <summary>
        /// Set as a persistent status (Burn, Paralysis, etc.).
        /// </summary>
        public StatusEffectBuilder Persistent(PersistentStatus status)
        {
            _persistentStatus = status;
            _volatileStatus = VolatileStatus.None;
            return this;
        }

        /// <summary>
        /// Set as a volatile status (Confusion, Taunt, etc.).
        /// </summary>
        public StatusEffectBuilder Volatile(VolatileStatus status)
        {
            _volatileStatus = status;
            _persistentStatus = PersistentStatus.None;
            return this;
        }

        #endregion

        #region Duration

        /// <summary>
        /// Status lasts indefinitely until cured.
        /// </summary>
        public StatusEffectBuilder Indefinite()
        {
            _minTurns = 0;
            _maxTurns = 0;
            return this;
        }

        /// <summary>
        /// Status lasts a fixed number of turns.
        /// </summary>
        public StatusEffectBuilder LastsTurns(int turns)
        {
            _minTurns = turns;
            _maxTurns = turns;
            return this;
        }

        /// <summary>
        /// Status lasts between min and max turns (randomly).
        /// </summary>
        public StatusEffectBuilder LastsTurns(int min, int max)
        {
            _minTurns = min;
            _maxTurns = max;
            return this;
        }

        #endregion

        #region End of Turn Effects

        /// <summary>
        /// Deals damage at end of turn as fraction of max HP (1/16 = 0.0625).
        /// </summary>
        public StatusEffectBuilder DealsDamagePerTurn(float fraction)
        {
            _endOfTurnDamage = fraction;
            _damageEscalates = false;
            return this;
        }

        /// <summary>
        /// Deals escalating damage (Toxic: 1/16, 2/16, 3/16...).
        /// </summary>
        public StatusEffectBuilder DealsEscalatingDamage(float baseFraction, int startMultiplier = 1)
        {
            _endOfTurnDamage = baseFraction;
            _damageEscalates = true;
            _escalatingDamageStart = startMultiplier;
            return this;
        }

        /// <summary>
        /// Heals HP at end of turn as fraction of max HP.
        /// </summary>
        public StatusEffectBuilder HealsPerTurn(float fraction)
        {
            _endOfTurnDamage = -fraction;
            return this;
        }

        /// <summary>
        /// Drains HP to the opponent (Leech Seed).
        /// </summary>
        public StatusEffectBuilder DrainsToOpponent(float fraction)
        {
            _endOfTurnDamage = fraction;
            _drainsToOpponent = true;
            return this;
        }

        #endregion

        #region Action Prevention

        /// <summary>
        /// Chance to fail to move each turn.
        /// </summary>
        public StatusEffectBuilder FailsToMove(float chance)
        {
            _moveFailChance = chance;
            return this;
        }

        /// <summary>
        /// Completely prevents action (Sleep, Freeze).
        /// </summary>
        public StatusEffectBuilder PreventsAction()
        {
            _moveFailChance = 1.0f;
            return this;
        }

        /// <summary>
        /// Chance to recover naturally each turn.
        /// </summary>
        public StatusEffectBuilder RecoveryChance(float chancePerTurn)
        {
            _recoveryChancePerTurn = chancePerTurn;
            return this;
        }

        #endregion

        #region Stat Modifiers

        /// <summary>
        /// Modifies Speed stat.
        /// </summary>
        public StatusEffectBuilder SpeedModifier(float multiplier)
        {
            _speedMultiplier = multiplier;
            return this;
        }

        /// <summary>
        /// Halves Speed.
        /// </summary>
        public StatusEffectBuilder HalvesSpeed()
        {
            _speedMultiplier = 0.5f;
            return this;
        }

        /// <summary>
        /// Modifies Attack stat (optionally only for physical moves).
        /// </summary>
        public StatusEffectBuilder AttackModifier(float multiplier, bool physicalOnly = false)
        {
            _attackMultiplier = multiplier;
            _attackModifierIsPhysicalOnly = physicalOnly;
            return this;
        }

        /// <summary>
        /// Halves physical Attack (Burn).
        /// </summary>
        public StatusEffectBuilder HalvesPhysicalAttack()
        {
            _attackMultiplier = 0.5f;
            _attackModifierIsPhysicalOnly = true;
            return this;
        }

        #endregion

        #region Self-Damage

        /// <summary>
        /// Chance to hit self instead of target (Confusion).
        /// </summary>
        public StatusEffectBuilder SelfHitChance(float chance, int power = 40)
        {
            _selfHitChance = chance;
            _selfHitPower = power;
            return this;
        }

        #endregion

        #region Type Interactions

        /// <summary>
        /// Types that are immune to this status.
        /// </summary>
        public StatusEffectBuilder ImmuneTypes(params PokemonType[] types)
        {
            _immuneTypes.AddRange(types);
            return this;
        }

        /// <summary>
        /// Move types that can cure this status when used.
        /// </summary>
        public StatusEffectBuilder CuredByMoveTypes(params PokemonType[] types)
        {
            _curedByMoveTypes.AddRange(types);
            return this;
        }

        #endregion

        #region Special Effects

        /// <summary>
        /// Prevents switching out.
        /// </summary>
        public StatusEffectBuilder PreventsSwitching()
        {
            _preventsSwitching = true;
            return this;
        }

        /// <summary>
        /// Forces the use of a specific move (Encore).
        /// </summary>
        public StatusEffectBuilder ForcesMove()
        {
            _forcesMove = true;
            return this;
        }

        /// <summary>
        /// Restricts a category of moves (Taunt = no Status moves).
        /// </summary>
        public StatusEffectBuilder RestrictsMoveCategory(MoveCategory category)
        {
            _restrictedMoveCategory = category;
            return this;
        }

        #endregion

        #region Build

        public StatusEffectData Build()
        {
            return new StatusEffectData(
                _id,
                _name,
                _description,
                _persistentStatus,
                _volatileStatus,
                _minTurns,
                _maxTurns,
                _endOfTurnDamage,
                _damageEscalates,
                _escalatingDamageStart,
                _drainsToOpponent,
                _moveFailChance,
                _recoveryChancePerTurn,
                _speedMultiplier,
                _attackMultiplier,
                _attackModifierIsPhysicalOnly,
                _selfHitChance,
                _selfHitPower,
                _immuneTypes.ToArray(),
                _curedByMoveTypes.ToArray(),
                _preventsSwitching,
                _forcesMove,
                _restrictedMoveCategory);
        }

        #endregion
    }

    /// <summary>
    /// Alias for StatusEffectBuilder for cleaner syntax.
    /// </summary>
    public static class Status
    {
        public static StatusEffectBuilder Define(string name) => StatusEffectBuilder.Define(name);
    }
}

