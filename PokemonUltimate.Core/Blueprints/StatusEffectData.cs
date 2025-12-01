using System;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Blueprints
{
    /// <summary>
    /// Immutable data defining a status effect's behavior.
    /// Covers both persistent and volatile status conditions.
    /// </summary>
    public sealed class StatusEffectData
    {
        #region Identity

        /// <summary>
        /// Internal identifier for the status effect.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Display name of the status effect.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Description of what the status does.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The persistent status this data represents (None if volatile only).
        /// </summary>
        public PersistentStatus PersistentStatus { get; }

        /// <summary>
        /// The volatile status this data represents (None if persistent).
        /// </summary>
        public VolatileStatus VolatileStatus { get; }

        /// <summary>
        /// Whether this is a persistent status (vs volatile).
        /// </summary>
        public bool IsPersistent => PersistentStatus != PersistentStatus.None;

        /// <summary>
        /// Whether this is a volatile status (vs persistent).
        /// </summary>
        public bool IsVolatile => VolatileStatus != VolatileStatus.None;

        #endregion

        #region Turn Duration

        /// <summary>
        /// Minimum turns this status lasts (0 = indefinite until cured).
        /// </summary>
        public int MinTurns { get; }

        /// <summary>
        /// Maximum turns this status lasts (0 = indefinite until cured).
        /// </summary>
        public int MaxTurns { get; }

        /// <summary>
        /// Whether this status has a random duration.
        /// </summary>
        public bool HasRandomDuration => MinTurns > 0 && MaxTurns > MinTurns;

        /// <summary>
        /// Whether this status lasts indefinitely until cured.
        /// </summary>
        public bool IsIndefinite => MinTurns == 0 && MaxTurns == 0;

        #endregion

        #region End of Turn Effects

        /// <summary>
        /// HP damage dealt at end of turn as fraction of max HP (1/16 = 0.0625).
        /// Negative values mean healing.
        /// </summary>
        public float EndOfTurnDamage { get; }

        /// <summary>
        /// Whether damage escalates each turn (like Toxic).
        /// </summary>
        public bool DamageEscalates { get; }

        /// <summary>
        /// Starting multiplier for escalating damage.
        /// </summary>
        public int EscalatingDamageStart { get; }

        /// <summary>
        /// Whether this status drains HP to another Pokemon (Leech Seed).
        /// </summary>
        public bool DrainsToOpponent { get; }

        #endregion

        #region Action Prevention

        /// <summary>
        /// Chance (0.0-1.0) that the Pokemon cannot move this turn.
        /// </summary>
        public float MoveFailChance { get; }

        /// <summary>
        /// Whether this status completely prevents action until cured (Sleep, Freeze).
        /// </summary>
        public bool PreventsAction => MoveFailChance >= 1.0f;

        /// <summary>
        /// Chance (0.0-1.0) to recover from this status each turn.
        /// </summary>
        public float RecoveryChancePerTurn { get; }

        #endregion

        #region Stat Modifiers

        /// <summary>
        /// Multiplier applied to Speed stat (Paralysis = 0.5).
        /// </summary>
        public float SpeedMultiplier { get; }

        /// <summary>
        /// Multiplier applied to Attack stat (Burn = 0.5 for physical moves).
        /// </summary>
        public float AttackMultiplier { get; }

        /// <summary>
        /// Whether the attack multiplier only applies to physical moves.
        /// </summary>
        public bool AttackModifierIsPhysicalOnly { get; }

        #endregion

        #region Self-Damage

        /// <summary>
        /// Chance (0.0-1.0) to hit self instead of target (Confusion).
        /// </summary>
        public float SelfHitChance { get; }

        /// <summary>
        /// Base power of self-hit damage (Confusion = 40).
        /// </summary>
        public int SelfHitPower { get; }

        #endregion

        #region Special Flags

        /// <summary>
        /// Types that are immune to this status.
        /// </summary>
        public PokemonType[] ImmuneTypes { get; }

        /// <summary>
        /// Move types that can cure this status when used by the affected Pokemon.
        /// </summary>
        public PokemonType[] CuredByMoveTypes { get; }

        /// <summary>
        /// Whether this status prevents switching (trapping).
        /// </summary>
        public bool PreventsSwitching { get; }

        /// <summary>
        /// Whether this status forces a specific move (Encore).
        /// </summary>
        public bool ForcesMove { get; }

        /// <summary>
        /// Whether this status restricts move category (Taunt = no status moves).
        /// </summary>
        public MoveCategory? RestrictedMoveCategory { get; }

        #endregion

        #region Constructor

        internal StatusEffectData(
            string id,
            string name,
            string description,
            PersistentStatus persistentStatus,
            VolatileStatus volatileStatus,
            int minTurns,
            int maxTurns,
            float endOfTurnDamage,
            bool damageEscalates,
            int escalatingDamageStart,
            bool drainsToOpponent,
            float moveFailChance,
            float recoveryChancePerTurn,
            float speedMultiplier,
            float attackMultiplier,
            bool attackModifierIsPhysicalOnly,
            float selfHitChance,
            int selfHitPower,
            PokemonType[] immuneTypes,
            PokemonType[] curedByMoveTypes,
            bool preventsSwitching,
            bool forcesMove,
            MoveCategory? restrictedMoveCategory)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? string.Empty;
            PersistentStatus = persistentStatus;
            VolatileStatus = volatileStatus;
            MinTurns = minTurns;
            MaxTurns = maxTurns;
            EndOfTurnDamage = endOfTurnDamage;
            DamageEscalates = damageEscalates;
            EscalatingDamageStart = escalatingDamageStart;
            DrainsToOpponent = drainsToOpponent;
            MoveFailChance = moveFailChance;
            RecoveryChancePerTurn = recoveryChancePerTurn;
            SpeedMultiplier = speedMultiplier;
            AttackMultiplier = attackMultiplier;
            AttackModifierIsPhysicalOnly = attackModifierIsPhysicalOnly;
            SelfHitChance = selfHitChance;
            SelfHitPower = selfHitPower;
            ImmuneTypes = immuneTypes ?? Array.Empty<PokemonType>();
            CuredByMoveTypes = curedByMoveTypes ?? Array.Empty<PokemonType>();
            PreventsSwitching = preventsSwitching;
            ForcesMove = forcesMove;
            RestrictedMoveCategory = restrictedMoveCategory;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Checks if a Pokemon type is immune to this status.
        /// </summary>
        public bool IsTypeImmune(PokemonType type)
        {
            foreach (var immuneType in ImmuneTypes)
            {
                if (immuneType == type) return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if a move type can cure this status.
        /// </summary>
        public bool CanBeCuredByMoveType(PokemonType moveType)
        {
            foreach (var cureType in CuredByMoveTypes)
            {
                if (cureType == moveType) return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a random duration for this status.
        /// </summary>
        public int GetRandomDuration(Random random = null)
        {
            if (IsIndefinite) return 0;
            if (MinTurns == MaxTurns) return MinTurns;
            
            random = random ?? new Random();
            return random.Next(MinTurns, MaxTurns + 1);
        }

        /// <summary>
        /// Calculates damage for escalating status effects.
        /// </summary>
        public float GetEscalatingDamage(int turnCount)
        {
            if (!DamageEscalates) return EndOfTurnDamage;
            
            int multiplier = EscalatingDamageStart + turnCount - 1;
            return EndOfTurnDamage * multiplier;
        }

        public override string ToString() => Name;

        #endregion
    }
}

