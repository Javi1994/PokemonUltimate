using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Instances
{
    /// <summary>
    /// Effort Values (EVs) for a Pokemon instance.
    /// EVs are gained through battles and range from 0-252 per stat, with a maximum total of 510.
    /// In this game, EVs are always set to maximum (252 per stat) for a roguelike experience.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.1: Pokemon Data (Phase 4: IVs/EVs System)
    /// **Documentation**: See `docs/features/1-game-data/roadmap.md#phase-4-optional-enhancements-low-priority`
    /// </remarks>
    public class EVSet
    {
        /// <summary>
        /// Minimum Effort Value (0).
        /// </summary>
        public const int MinEV = 0;

        /// <summary>
        /// Maximum Effort Value per stat (252).
        /// </summary>
        public const int MaxEV = CoreConstants.MaxEV;

        /// <summary>
        /// Maximum total EVs across all stats (510).
        /// </summary>
        public const int MaxTotalEV = CoreConstants.MaxTotalEV;

        /// <summary>
        /// HP Effort Value (0-252).
        /// </summary>
        public int HP { get; set; }

        /// <summary>
        /// Attack Effort Value (0-252).
        /// </summary>
        public int Attack { get; set; }

        /// <summary>
        /// Defense Effort Value (0-252).
        /// </summary>
        public int Defense { get; set; }

        /// <summary>
        /// Special Attack Effort Value (0-252).
        /// </summary>
        public int SpAttack { get; set; }

        /// <summary>
        /// Special Defense Effort Value (0-252).
        /// </summary>
        public int SpDefense { get; set; }

        /// <summary>
        /// Speed Effort Value (0-252).
        /// </summary>
        public int Speed { get; set; }

        #region Constructors

        /// <summary>
        /// Creates an EVSet with optimal distribution (252 HP, 252 Attack, 4 Defense, 0 others).
        /// This is a common competitive distribution that maximizes two stats.
        /// Total: 508 (within 510 limit).
        /// </summary>
        public EVSet() : this(MaxEV, MaxEV, 4, 0, 0, 0)
        {
        }

        /// <summary>
        /// Creates an EVSet with specified values for each stat.
        /// Validates that total does not exceed MaxTotalEV (510).
        /// </summary>
        public EVSet(int hp, int attack, int defense, int spAttack, int spDefense, int speed)
        {
            CoreValidators.ValidateEV(hp, nameof(hp));
            CoreValidators.ValidateEV(attack, nameof(attack));
            CoreValidators.ValidateEV(defense, nameof(defense));
            CoreValidators.ValidateEV(spAttack, nameof(spAttack));
            CoreValidators.ValidateEV(spDefense, nameof(spDefense));
            CoreValidators.ValidateEV(speed, nameof(speed));

            int total = hp + attack + defense + spAttack + spDefense + speed;
            if (total > MaxTotalEV)
            {
                throw new System.ArgumentException(
                    ErrorMessages.Format(ErrorMessages.TotalEVsCannotExceed, MaxTotalEV),
                    nameof(total));
            }

            HP = hp;
            Attack = attack;
            Defense = defense;
            SpAttack = spAttack;
            SpDefense = spDefense;
            Speed = speed;
        }

        #endregion

        #region Computed Properties

        /// <summary>
        /// Returns true if all EVs are within valid range (0-252) and total does not exceed 510.
        /// </summary>
        public bool IsValid =>
            HP >= MinEV && HP <= MaxEV &&
            Attack >= MinEV && Attack <= MaxEV &&
            Defense >= MinEV && Defense <= MaxEV &&
            SpAttack >= MinEV && SpAttack <= MaxEV &&
            SpDefense >= MinEV && SpDefense <= MaxEV &&
            Speed >= MinEV && Speed <= MaxEV &&
            Total <= MaxTotalEV;

        /// <summary>
        /// Total EV sum (0-510).
        /// </summary>
        public int Total => HP + Attack + Defense + SpAttack + SpDefense + Speed;

        #endregion

        #region Methods

        /// <summary>
        /// Gets the EV value for a specific stat.
        /// </summary>
        public int GetEV(Stat stat)
        {
            switch (stat)
            {
                case Stat.HP:
                    return HP;
                case Stat.Attack:
                    return Attack;
                case Stat.Defense:
                    return Defense;
                case Stat.SpAttack:
                    return SpAttack;
                case Stat.SpDefense:
                    return SpDefense;
                case Stat.Speed:
                    return Speed;
                default:
                    return 0; // Accuracy and Evasion don't have EVs
            }
        }

        /// <summary>
        /// Creates a maximum EVSet with optimal distribution (252 HP, 252 Attack, 4 Defense, 0 others).
        /// This is a common competitive distribution that maximizes two stats.
        /// Total: 508 (within 510 limit).
        /// </summary>
        public static EVSet Maximum()
        {
            return new EVSet(MaxEV, MaxEV, 4, 0, 0, 0);
        }

        /// <summary>
        /// Creates an EVSet with all zeros.
        /// </summary>
        public static EVSet Zero()
        {
            return new EVSet(0, 0, 0, 0, 0, 0);
        }

        #endregion
    }
}

