using System;
using System.Collections.Generic;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Instances
{
    /// <summary>
    /// Runtime instance of a Pokemon with mutable state.
    /// Created from a PokemonSpeciesData blueprint via PokemonFactory.
    /// Tracks level, HP, stats, moves, status, and battle state.
    /// 
    /// This is a partial class split across multiple files:
    /// - PokemonInstance.cs: Core properties and constructor
    /// - PokemonInstance.Battle.cs: Battle-related methods
    /// - PokemonInstance.LevelUp.cs: Level up and move learning
    /// - PokemonInstance.Evolution.cs: Evolution system
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.1: Pokemon Data
    /// **Documentation**: See `docs/features/1-game-data/1.1-pokemon-data/architecture.md`
    /// </remarks>
    public partial class PokemonInstance
    {
        #region Identity

        /// <summary>
        /// Reference to the species blueprint (immutable data).
        /// </summary>
        public PokemonSpeciesData Species { get; private set; }

        /// <summary>
        /// Unique identifier for this specific instance.
        /// </summary>
        public string InstanceId { get; }

        /// <summary>
        /// Optional nickname (defaults to species name if null).
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Display name (nickname if set, otherwise species name).
        /// </summary>
        public string DisplayName => Nickname ?? Species.Name;

        #endregion

        #region Level & Experience

        /// <summary>
        /// Current level (1-100).
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// Current experience points.
        /// </summary>
        public int CurrentExp { get; set; }

        #endregion

        #region Stats

        /// <summary>
        /// Maximum HP (calculated from base stats, level, and nature).
        /// Recalculated on level up.
        /// </summary>
        public int MaxHP { get; private set; }

        /// <summary>
        /// Current HP (0 = fainted).
        /// </summary>
        public int CurrentHP { get; set; }

        /// <summary>
        /// Attack stat (calculated from base stats, level, and nature).
        /// </summary>
        public int Attack { get; private set; }

        /// <summary>
        /// Defense stat (calculated from base stats, level, and nature).
        /// </summary>
        public int Defense { get; private set; }

        /// <summary>
        /// Special Attack stat (calculated from base stats, level, and nature).
        /// </summary>
        public int SpAttack { get; private set; }

        /// <summary>
        /// Special Defense stat (calculated from base stats, level, and nature).
        /// </summary>
        public int SpDefense { get; private set; }

        /// <summary>
        /// Speed stat (calculated from base stats, level, and nature).
        /// </summary>
        public int Speed { get; private set; }

        #endregion

        #region Personal Characteristics

        /// <summary>
        /// Pokemon's gender.
        /// </summary>
        public Gender Gender { get; }

        /// <summary>
        /// Pokemon's nature (affects stat calculation).
        /// </summary>
        public Nature Nature { get; }

        /// <summary>
        /// Friendship/Happiness value (0-255).
        /// Affects certain evolutions and moves like Return/Frustration.
        /// Default is 70 for wild Pokemon, 120 for Pokemon from eggs.
        /// </summary>
        public int Friendship { get; set; }

        /// <summary>
        /// Whether this Pokemon is shiny (alternate coloration).
        /// Approximately 1/4096 chance naturally.
        /// </summary>
        public bool IsShiny { get; }

        #endregion

        #region Moves

        /// <summary>
        /// Current moveset (max 4 moves).
        /// </summary>
        public List<MoveInstance> Moves { get; }

        #endregion

        #region Ability & Item

        /// <summary>
        /// The Pokemon's active ability.
        /// Can be primary, secondary, or hidden ability from the species.
        /// </summary>
        public AbilityData Ability { get; private set; }

        /// <summary>
        /// The held item (null if not holding anything).
        /// </summary>
        public ItemData HeldItem { get; set; }

        /// <summary>
        /// Returns true if this Pokemon is holding an item.
        /// </summary>
        public bool HasHeldItem => HeldItem != null;

        /// <summary>
        /// Returns true if this Pokemon has an ability assigned.
        /// </summary>
        public bool HasAbility => Ability != null;

        /// <summary>
        /// Returns true if this Pokemon has the specified ability.
        /// </summary>
        public bool HasAbilityNamed(string abilityName) =>
            Ability != null && Ability.Name.Equals(abilityName, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Returns true if this Pokemon is using its hidden ability.
        /// </summary>
        public bool IsUsingHiddenAbility =>
            Ability != null && Species.HiddenAbility != null && Ability == Species.HiddenAbility;

        /// <summary>
        /// Changes the Pokemon's ability (used by moves like Skill Swap).
        /// </summary>
        public void SetAbility(AbilityData ability)
        {
            Ability = ability;
        }

        /// <summary>
        /// Gives an item to the Pokemon.
        /// </summary>
        public void GiveItem(ItemData item)
        {
            HeldItem = item;
        }

        /// <summary>
        /// Removes and returns the held item.
        /// </summary>
        public ItemData TakeItem()
        {
            var item = HeldItem;
            HeldItem = null;
            return item;
        }

        #endregion

        #region Battle State

        /// <summary>
        /// Persistent status condition (Burn, Paralysis, etc.).
        /// Persists between battles.
        /// </summary>
        public PersistentStatus Status { get; set; }

        /// <summary>
        /// Volatile status condition (Confusion, Flinch, etc.).
        /// Reset after battle.
        /// </summary>
        public VolatileStatus VolatileStatus { get; set; }

        /// <summary>
        /// Turn counter for status effects (sleep duration, toxic damage, etc.).
        /// </summary>
        public int StatusTurnCounter { get; set; }

        /// <summary>
        /// Stat stages for battle (-6 to +6).
        /// Reset after battle.
        /// </summary>
        public Dictionary<Stat, int> StatStages { get; }

        #endregion

        #region Computed Properties

        /// <summary>
        /// Returns true if this Pokemon has fainted (HP <= 0).
        /// </summary>
        public bool IsFainted => CurrentHP <= 0;

        /// <summary>
        /// Returns HP as a percentage (0.0 to 1.0).
        /// </summary>
        public float HPPercentage => MaxHP > 0 ? (float)CurrentHP / MaxHP : 0f;

        /// <summary>
        /// Returns true if this Pokemon has a non-None persistent status.
        /// </summary>
        public bool HasStatus => Status != PersistentStatus.None;

        /// <summary>
        /// Returns true if friendship is high enough for friendship-based evolutions (>= 220).
        /// </summary>
        public bool HasHighFriendship => Friendship >= 220;

        /// <summary>
        /// Returns true if friendship is at maximum (255).
        /// </summary>
        public bool HasMaxFriendship => Friendship >= 255;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new Pokemon instance. 
        /// Prefer using PokemonFactory.Create() for normal usage.
        /// </summary>
        public PokemonInstance(
            PokemonSpeciesData species,
            int level,
            int maxHP,
            int attack,
            int defense,
            int spAttack,
            int spDefense,
            int speed,
            Nature nature,
            Gender gender,
            List<MoveInstance> moves,
            int friendship = 70,
            bool isShiny = false,
            AbilityData ability = null,
            ItemData heldItem = null)
        {
            Species = species ?? throw new ArgumentNullException(nameof(species));
            InstanceId = Guid.NewGuid().ToString();
            Level = level;

            // Stats
            MaxHP = maxHP;
            CurrentHP = maxHP; // Start at full HP
            Attack = attack;
            Defense = defense;
            SpAttack = spAttack;
            SpDefense = spDefense;
            Speed = speed;

            // Personal
            Nature = nature;
            Gender = gender;
            Friendship = Math.Max(0, Math.Min(255, friendship));
            IsShiny = isShiny;

            // Moves
            Moves = moves ?? new List<MoveInstance>();

            // Ability & Item
            Ability = ability ?? species.Ability1;
            HeldItem = heldItem;

            // Battle state - initialize to defaults
            Status = PersistentStatus.None;
            VolatileStatus = VolatileStatus.None;
            StatusTurnCounter = 0;

            // Initialize stat stages to 0
            StatStages = new Dictionary<Stat, int>
            {
                { Stat.Attack, 0 },
                { Stat.Defense, 0 },
                { Stat.SpAttack, 0 },
                { Stat.SpDefense, 0 },
                { Stat.Speed, 0 },
                { Stat.Accuracy, 0 },
                { Stat.Evasion, 0 }
            };

            // Initialize move check level to current level
            _lastMoveCheckLevel = level;
        }

        #endregion

        #region Private Helpers

        private int GetBaseStat(Stat stat)
        {
            switch (stat)
            {
                case Stat.HP: return MaxHP;
                case Stat.Attack: return Attack;
                case Stat.Defense: return Defense;
                case Stat.SpAttack: return SpAttack;
                case Stat.SpDefense: return SpDefense;
                case Stat.Speed: return Speed;
                case Stat.Accuracy: return 100; // Base accuracy is 100%
                case Stat.Evasion: return 100;  // Base evasion is 100%
                default: return 0;
            }
        }

        #endregion
    }
}
