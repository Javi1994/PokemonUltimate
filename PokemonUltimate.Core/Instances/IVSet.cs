using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Instances
{
    /// <summary>
    /// Individual Values (IVs) for a Pokemon instance.
    /// IVs are genetic values that range from 0-31 for each stat.
    /// They are determined when a Pokemon is created and cannot be changed (except through breeding).
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.1: Pokemon Data (Phase 4: IVs/EVs System)
    /// **Documentation**: See `docs/features/1-game-data/roadmap.md#phase-4-optional-enhancements-low-priority`
    /// </remarks>
    public class IVSet
    {
        /// <summary>
        /// Minimum Individual Value (0).
        /// </summary>
        public const int MinIV = 0;

        /// <summary>
        /// Maximum Individual Value (31).
        /// </summary>
        public const int MaxIV = CoreConstants.MaxIV;

        /// <summary>
        /// HP Individual Value (0-31).
        /// </summary>
        public int HP { get; set; }

        /// <summary>
        /// Attack Individual Value (0-31).
        /// </summary>
        public int Attack { get; set; }

        /// <summary>
        /// Defense Individual Value (0-31).
        /// </summary>
        public int Defense { get; set; }

        /// <summary>
        /// Special Attack Individual Value (0-31).
        /// </summary>
        public int SpAttack { get; set; }

        /// <summary>
        /// Special Defense Individual Value (0-31).
        /// </summary>
        public int SpDefense { get; set; }

        /// <summary>
        /// Speed Individual Value (0-31).
        /// </summary>
        public int Speed { get; set; }

        #region Constructors

        /// <summary>
        /// Creates an IVSet with all values set to maximum (31).
        /// </summary>
        public IVSet() : this(MaxIV, MaxIV, MaxIV, MaxIV, MaxIV, MaxIV)
        {
        }

        /// <summary>
        /// Creates an IVSet with specified values for each stat.
        /// </summary>
        public IVSet(int hp, int attack, int defense, int spAttack, int spDefense, int speed)
        {
            CoreValidators.ValidateIV(hp, nameof(hp));
            CoreValidators.ValidateIV(attack, nameof(attack));
            CoreValidators.ValidateIV(defense, nameof(defense));
            CoreValidators.ValidateIV(spAttack, nameof(spAttack));
            CoreValidators.ValidateIV(spDefense, nameof(spDefense));
            CoreValidators.ValidateIV(speed, nameof(speed));

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
        /// Returns true if all IVs are within valid range (0-31).
        /// </summary>
        public bool IsValid =>
            HP >= MinIV && HP <= MaxIV &&
            Attack >= MinIV && Attack <= MaxIV &&
            Defense >= MinIV && Defense <= MaxIV &&
            SpAttack >= MinIV && SpAttack <= MaxIV &&
            SpDefense >= MinIV && SpDefense <= MaxIV &&
            Speed >= MinIV && Speed <= MaxIV;

        /// <summary>
        /// Total IV sum (0-186).
        /// </summary>
        public int Total => HP + Attack + Defense + SpAttack + SpDefense + Speed;

        #endregion

        #region Methods

        /// <summary>
        /// Gets the IV value for a specific stat.
        /// </summary>
        public int GetIV(Stat stat)
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
                    return 0; // Accuracy and Evasion don't have IVs
            }
        }

        /// <summary>
        /// Creates a perfect IVSet (all 31).
        /// </summary>
        public static IVSet Perfect()
        {
            return new IVSet(MaxIV, MaxIV, MaxIV, MaxIV, MaxIV, MaxIV);
        }

        /// <summary>
        /// Creates an IVSet with all zeros.
        /// </summary>
        public static IVSet Zero()
        {
            return new IVSet(0, 0, 0, 0, 0, 0);
        }

        #endregion
    }
}

