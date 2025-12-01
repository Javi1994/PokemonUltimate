using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Core.Factories
{
    /// <summary>
    /// Fluent builder for creating PokemonInstance objects.
    /// Provides a clean, readable API for generating Pokemon with full control.
    /// 
    /// Usage:
    ///   var pokemon = Pokemon.Create(PokemonCatalog.Pikachu, 25)
    ///       .WithNature(Nature.Jolly)
    ///       .Named("Sparky")
    ///       .Build();
    /// </summary>
    public class PokemonInstanceBuilder
    {
        private static Random _random = new Random();

        private readonly PokemonSpeciesData _species;
        private readonly int _level;

        // Optional configurations
        private Nature? _nature;
        private Gender? _gender;
        private string _nickname;
        private List<MoveData> _specificMoves;
        private int? _currentHP;
        private float? _hpPercent;
        private PersistentStatus _status = PersistentStatus.None;
        private int _experience;
        private bool _useRandomMoves;
        private bool _prioritizeStab;
        private bool _prioritizePower;
        private int _moveCount = 4;

        // Friendship and Shiny
        private int _friendship = 70; // Default for wild Pokemon
        private bool? _isShiny;
        private const int ShinyOdds = 4096; // 1/4096 chance

        // Ability and Item
        private AbilityData _ability;
        private bool _useHiddenAbility;
        private ItemData _heldItem;

        // For testing/special cases
        private int? _overrideMaxHP;
        private int? _overrideAttack;
        private int? _overrideDefense;
        private int? _overrideSpAttack;
        private int? _overrideSpDefense;
        private int? _overrideSpeed;

        #region Constructor

        private PokemonInstanceBuilder(PokemonSpeciesData species, int level)
        {
            _species = species ?? throw new ArgumentNullException(nameof(species));
            
            if (level < 1 || level > 100)
                throw new ArgumentException(ErrorMessages.LevelMustBeBetween1And100, nameof(level));
            
            _level = level;
        }

        #endregion

        #region Static Entry Points

        /// <summary>
        /// Start building a Pokemon instance.
        /// </summary>
        public static PokemonInstanceBuilder Create(PokemonSpeciesData species, int level)
        {
            return new PokemonInstanceBuilder(species, level);
        }

        /// <summary>
        /// Create a Pokemon with random level in range.
        /// </summary>
        public static PokemonInstanceBuilder CreateWithRandomLevel(PokemonSpeciesData species, int minLevel, int maxLevel)
        {
            if (minLevel > maxLevel)
                throw new ArgumentException(ErrorMessages.MinLevelCannotBeGreaterThanMaxLevel);
            
            int level = _random.Next(minLevel, maxLevel + 1);
            return new PokemonInstanceBuilder(species, level);
        }

        /// <summary>
        /// Quick creation with all random values.
        /// </summary>
        public static PokemonInstance CreateRandom(PokemonSpeciesData species, int level)
        {
            return new PokemonInstanceBuilder(species, level).Build();
        }

        /// <summary>
        /// Quick creation with specific nature.
        /// </summary>
        public static PokemonInstance CreateWithNature(PokemonSpeciesData species, int level, Nature nature)
        {
            return new PokemonInstanceBuilder(species, level)
                .WithNature(nature)
                .Build();
        }

        /// <summary>
        /// Quick creation with hidden ability.
        /// </summary>
        public static PokemonInstance CreateWithHiddenAbility(PokemonSpeciesData species, int level)
        {
            return new PokemonInstanceBuilder(species, level)
                .WithHiddenAbility()
                .Build();
        }

        /// <summary>
        /// Quick creation with specific held item.
        /// </summary>
        public static PokemonInstance CreateWithItem(PokemonSpeciesData species, int level, ItemData item)
        {
            return new PokemonInstanceBuilder(species, level)
                .Holding(item)
                .Build();
        }

        /// <summary>
        /// Sets the random seed for deterministic generation.
        /// </summary>
        public static void SetSeed(int seed)
        {
            _random = new Random(seed);
        }

        /// <summary>
        /// Resets to a new random instance.
        /// </summary>
        public static void ResetRandom()
        {
            _random = new Random();
        }

        #endregion

        #region Nature Configuration

        /// <summary>
        /// Set a specific nature.
        /// </summary>
        public PokemonInstanceBuilder WithNature(Nature nature)
        {
            _nature = nature;
            return this;
        }

        /// <summary>
        /// Use a random nature (default behavior).
        /// </summary>
        public PokemonInstanceBuilder WithRandomNature()
        {
            _nature = null;
            return this;
        }

        /// <summary>
        /// Set a nature that boosts the specified stat.
        /// </summary>
        public PokemonInstanceBuilder WithNatureBoosting(Stat stat)
        {
            _nature = GetNatureBoostingStat(stat);
            return this;
        }

        /// <summary>
        /// Set a neutral nature (no stat changes).
        /// </summary>
        public PokemonInstanceBuilder WithNeutralNature()
        {
            _nature = Nature.Hardy;
            return this;
        }

        #endregion

        #region Gender Configuration

        /// <summary>
        /// Set a specific gender.
        /// </summary>
        public PokemonInstanceBuilder WithGender(Gender gender)
        {
            _gender = gender;
            return this;
        }

        /// <summary>
        /// Force male gender (throws if species is female-only or genderless).
        /// </summary>
        public PokemonInstanceBuilder Male()
        {
            _gender = Gender.Male;
            return this;
        }

        /// <summary>
        /// Force female gender (throws if species is male-only or genderless).
        /// </summary>
        public PokemonInstanceBuilder Female()
        {
            _gender = Gender.Female;
            return this;
        }

        #endregion

        #region Nickname Configuration

        /// <summary>
        /// Set a nickname for the Pokemon.
        /// </summary>
        public PokemonInstanceBuilder Named(string nickname)
        {
            _nickname = nickname;
            return this;
        }

        /// <summary>
        /// Set a nickname for the Pokemon (alias for Named).
        /// </summary>
        public PokemonInstanceBuilder WithNickname(string nickname)
        {
            return Named(nickname);
        }

        #endregion

        #region Move Configuration

        /// <summary>
        /// Set specific moves for the Pokemon (takes first 4 if more provided).
        /// </summary>
        public PokemonInstanceBuilder WithMoves(params MoveData[] moves)
        {
            if (moves == null || moves.Length == 0)
            {
                _specificMoves = null;
                return this;
            }

            _specificMoves = moves.Where(m => m != null).Take(4).ToList();
            return this;
        }

        /// <summary>
        /// Set specific moves for the Pokemon from a list.
        /// </summary>
        public PokemonInstanceBuilder WithMoves(IEnumerable<MoveData> moves)
        {
            return WithMoves(moves?.ToArray() ?? new MoveData[0]);
        }

        /// <summary>
        /// Auto-select moves from learnset (default behavior).
        /// </summary>
        public PokemonInstanceBuilder WithLearnsetMoves()
        {
            _specificMoves = null;
            _useRandomMoves = false;
            return this;
        }

        /// <summary>
        /// Select random moves from available learnset.
        /// </summary>
        public PokemonInstanceBuilder WithRandomMoves()
        {
            _specificMoves = null;
            _useRandomMoves = true;
            return this;
        }

        /// <summary>
        /// Limit the number of moves (1-4).
        /// </summary>
        public PokemonInstanceBuilder WithMoveCount(int count)
        {
            if (count < 1 || count > 4)
                throw new ArgumentException(ErrorMessages.MoveCountMustBeBetween1And4);
            
            _moveCount = count;
            return this;
        }

        /// <summary>
        /// Create with only one move.
        /// </summary>
        public PokemonInstanceBuilder WithSingleMove()
        {
            _moveCount = 1;
            return this;
        }

        /// <summary>
        /// Create with no moves.
        /// </summary>
        public PokemonInstanceBuilder WithNoMoves()
        {
            _specificMoves = new List<MoveData>();
            return this;
        }

        /// <summary>
        /// Prioritize STAB moves (Same Type Attack Bonus).
        /// Selects moves that match the Pokemon's types first.
        /// </summary>
        public PokemonInstanceBuilder WithStabMoves()
        {
            _specificMoves = null;
            _useRandomMoves = false;
            _prioritizeStab = true;
            _prioritizePower = false;
            return this;
        }

        /// <summary>
        /// Prioritize high-power moves.
        /// Selects moves with highest base power.
        /// </summary>
        public PokemonInstanceBuilder WithStrongMoves()
        {
            _specificMoves = null;
            _useRandomMoves = false;
            _prioritizeStab = false;
            _prioritizePower = true;
            return this;
        }

        /// <summary>
        /// Prioritize both STAB and high power.
        /// Best competitive moveset selection.
        /// </summary>
        public PokemonInstanceBuilder WithOptimalMoves()
        {
            _specificMoves = null;
            _useRandomMoves = false;
            _prioritizeStab = true;
            _prioritizePower = true;
            return this;
        }

        #endregion

        #region HP Configuration

        /// <summary>
        /// Start at full HP (default).
        /// </summary>
        public PokemonInstanceBuilder AtFullHealth()
        {
            _currentHP = null;
            _hpPercent = null;
            return this;
        }

        /// <summary>
        /// Start at specific HP value.
        /// </summary>
        public PokemonInstanceBuilder AtHealth(int currentHP)
        {
            if (currentHP < 0)
                throw new ArgumentException(ErrorMessages.HPCannotBeNegative);
            
            _currentHP = currentHP;
            _hpPercent = null;
            return this;
        }

        /// <summary>
        /// Start at specific HP percentage (0.0 to 1.0).
        /// </summary>
        public PokemonInstanceBuilder AtHealthPercent(float percent)
        {
            if (percent < 0 || percent > 1)
                throw new ArgumentException(ErrorMessages.PercentMustBeBetween0And1);
            
            _hpPercent = percent;
            _currentHP = null;
            return this;
        }

        /// <summary>
        /// Start at half HP.
        /// </summary>
        public PokemonInstanceBuilder AtHalfHealth()
        {
            return AtHealthPercent(0.5f);
        }

        /// <summary>
        /// Start at 1 HP (clutch scenarios).
        /// </summary>
        public PokemonInstanceBuilder AtOneHP()
        {
            return AtHealth(1);
        }

        /// <summary>
        /// Start fainted (0 HP).
        /// </summary>
        public PokemonInstanceBuilder Fainted()
        {
            return AtHealth(0);
        }

        #endregion

        #region Status Configuration

        /// <summary>
        /// Start with a status condition.
        /// </summary>
        public PokemonInstanceBuilder WithStatus(PersistentStatus status)
        {
            _status = status;
            return this;
        }

        /// <summary>
        /// Start burned.
        /// </summary>
        public PokemonInstanceBuilder Burned()
        {
            return WithStatus(PersistentStatus.Burn);
        }

        /// <summary>
        /// Start paralyzed.
        /// </summary>
        public PokemonInstanceBuilder Paralyzed()
        {
            return WithStatus(PersistentStatus.Paralysis);
        }

        /// <summary>
        /// Start poisoned.
        /// </summary>
        public PokemonInstanceBuilder Poisoned()
        {
            return WithStatus(PersistentStatus.Poison);
        }

        /// <summary>
        /// Start badly poisoned (toxic).
        /// </summary>
        public PokemonInstanceBuilder BadlyPoisoned()
        {
            return WithStatus(PersistentStatus.BadlyPoisoned);
        }

        /// <summary>
        /// Start asleep.
        /// </summary>
        public PokemonInstanceBuilder Asleep()
        {
            return WithStatus(PersistentStatus.Sleep);
        }

        /// <summary>
        /// Start frozen.
        /// </summary>
        public PokemonInstanceBuilder Frozen()
        {
            return WithStatus(PersistentStatus.Freeze);
        }

        #endregion

        #region Experience Configuration

        /// <summary>
        /// Set initial experience points.
        /// </summary>
        public PokemonInstanceBuilder WithExperience(int exp)
        {
            if (exp < 0)
                throw new ArgumentException(ErrorMessages.ExperienceCannotBeNegative);
            
            _experience = exp;
            return this;
        }

        #endregion

        #region Friendship Configuration

        /// <summary>
        /// Set friendship value (0-255). Default is 70.
        /// </summary>
        public PokemonInstanceBuilder WithFriendship(int friendship)
        {
            if (friendship < 0 || friendship > 255)
                throw new ArgumentException(ErrorMessages.FriendshipMustBeBetween0And255);
            
            _friendship = friendship;
            return this;
        }

        /// <summary>
        /// Set high friendship (220) - enough for friendship evolutions.
        /// </summary>
        public PokemonInstanceBuilder WithHighFriendship()
        {
            _friendship = 220;
            return this;
        }

        /// <summary>
        /// Set maximum friendship (255).
        /// </summary>
        public PokemonInstanceBuilder WithMaxFriendship()
        {
            _friendship = 255;
            return this;
        }

        /// <summary>
        /// Set low friendship (0).
        /// </summary>
        public PokemonInstanceBuilder WithLowFriendship()
        {
            _friendship = 0;
            return this;
        }

        /// <summary>
        /// Set friendship for hatched Pokemon (120).
        /// </summary>
        public PokemonInstanceBuilder AsHatched()
        {
            _friendship = 120;
            return this;
        }

        #endregion

        #region Shiny Configuration

        /// <summary>
        /// Make this Pokemon shiny.
        /// </summary>
        public PokemonInstanceBuilder Shiny()
        {
            _isShiny = true;
            return this;
        }

        /// <summary>
        /// Make this Pokemon not shiny (default).
        /// </summary>
        public PokemonInstanceBuilder NotShiny()
        {
            _isShiny = false;
            return this;
        }

        /// <summary>
        /// Roll for shiny with natural odds (1/4096).
        /// </summary>
        public PokemonInstanceBuilder WithShinyChance()
        {
            _isShiny = null; // Will be rolled in Build()
            return this;
        }

        #endregion

        #region Ability Configuration

        /// <summary>
        /// Set a specific ability.
        /// </summary>
        public PokemonInstanceBuilder WithAbility(AbilityData ability)
        {
            _ability = ability;
            _useHiddenAbility = false;
            return this;
        }

        /// <summary>
        /// Use the species' hidden ability.
        /// </summary>
        public PokemonInstanceBuilder WithHiddenAbility()
        {
            _useHiddenAbility = true;
            _ability = null;
            return this;
        }

        /// <summary>
        /// Use a random normal ability from the species (Ability1 or Ability2).
        /// </summary>
        public PokemonInstanceBuilder WithRandomAbility()
        {
            _ability = null;
            _useHiddenAbility = false;
            return this;
        }

        #endregion

        #region Held Item Configuration

        /// <summary>
        /// Give this Pokemon a held item.
        /// </summary>
        public PokemonInstanceBuilder Holding(ItemData item)
        {
            _heldItem = item;
            return this;
        }

        /// <summary>
        /// Alias for Holding.
        /// </summary>
        public PokemonInstanceBuilder WithItem(ItemData item)
        {
            return Holding(item);
        }

        /// <summary>
        /// Clear any held item.
        /// </summary>
        public PokemonInstanceBuilder NoItem()
        {
            _heldItem = null;
            return this;
        }

        #endregion

        #region Stat Overrides (for testing)

        /// <summary>
        /// Override all stats (for testing purposes).
        /// </summary>
        public PokemonInstanceBuilder WithStats(int hp, int atk, int def, int spAtk, int spDef, int speed)
        {
            _overrideMaxHP = hp;
            _overrideAttack = atk;
            _overrideDefense = def;
            _overrideSpAttack = spAtk;
            _overrideSpDefense = spDef;
            _overrideSpeed = speed;
            return this;
        }

        /// <summary>
        /// Override max HP (for testing purposes).
        /// </summary>
        public PokemonInstanceBuilder WithMaxHP(int hp)
        {
            _overrideMaxHP = hp;
            return this;
        }

        /// <summary>
        /// Override attack stat (for testing purposes).
        /// </summary>
        public PokemonInstanceBuilder WithAttack(int attack)
        {
            _overrideAttack = attack;
            return this;
        }

        /// <summary>
        /// Override speed stat (for testing purposes).
        /// </summary>
        public PokemonInstanceBuilder WithSpeed(int speed)
        {
            _overrideSpeed = speed;
            return this;
        }

        #endregion

        #region Build

        /// <summary>
        /// Build the Pokemon instance.
        /// </summary>
        public PokemonInstance Build()
        {
            // Determine nature
            Nature nature = _nature ?? GetRandomNature();

            // Determine gender
            Gender gender = DetermineGender();

            // Calculate stats
            int hp = _overrideMaxHP ?? StatCalculator.CalculateHP(_species.BaseStats.HP, _level);
            int attack = _overrideAttack ?? StatCalculator.CalculateStat(_species.BaseStats.Attack, _level, nature, Stat.Attack);
            int defense = _overrideDefense ?? StatCalculator.CalculateStat(_species.BaseStats.Defense, _level, nature, Stat.Defense);
            int spAttack = _overrideSpAttack ?? StatCalculator.CalculateStat(_species.BaseStats.SpAttack, _level, nature, Stat.SpAttack);
            int spDefense = _overrideSpDefense ?? StatCalculator.CalculateStat(_species.BaseStats.SpDefense, _level, nature, Stat.SpDefense);
            int speed = _overrideSpeed ?? StatCalculator.CalculateStat(_species.BaseStats.Speed, _level, nature, Stat.Speed);

            // Select moves
            List<MoveInstance> moves = SelectMoves();

            // Determine shiny
            bool isShiny = _isShiny ?? (_random.Next(ShinyOdds) == 0);

            // Determine ability
            AbilityData ability = DetermineAbility();

            // Create instance
            var pokemon = new PokemonInstance(
                _species, _level, hp, attack, defense,
                spAttack, spDefense, speed, nature, gender, moves,
                _friendship, isShiny, ability, _heldItem);

            // Apply optional configurations
            if (_nickname != null)
                pokemon.Nickname = _nickname;

            if (_currentHP.HasValue)
                pokemon.CurrentHP = Math.Min(_currentHP.Value, pokemon.MaxHP);
            else if (_hpPercent.HasValue)
                pokemon.CurrentHP = (int)(pokemon.MaxHP * _hpPercent.Value);

            if (_status != PersistentStatus.None)
                pokemon.Status = _status;

            if (_experience > 0)
                pokemon.CurrentExp = _experience;

            return pokemon;
        }

        #endregion

        #region Private Helpers

        private Gender DetermineGender()
        {
            if (_gender.HasValue)
            {
                ValidateGender(_gender.Value);
                return _gender.Value;
            }

            if (_species.IsGenderless)
                return Gender.Genderless;
            if (_species.IsMaleOnly)
                return Gender.Male;
            if (_species.IsFemaleOnly)
                return Gender.Female;

            // Random based on ratio
            double roll = _random.NextDouble() * 100;
            return roll < _species.GenderRatio ? Gender.Male : Gender.Female;
        }

        private void ValidateGender(Gender gender)
        {
            if (_species.IsGenderless && gender != Gender.Genderless)
                throw new ArgumentException(ErrorMessages.Format(ErrorMessages.SpeciesIsGenderless, _species.Name));

            if (_species.IsMaleOnly && gender != Gender.Male)
                throw new ArgumentException(ErrorMessages.Format(ErrorMessages.SpeciesIsMaleOnly, _species.Name));

            if (_species.IsFemaleOnly && gender != Gender.Female)
                throw new ArgumentException(ErrorMessages.Format(ErrorMessages.SpeciesIsFemaleOnly, _species.Name));

            if (!_species.IsGenderless && gender == Gender.Genderless)
                throw new ArgumentException(ErrorMessages.Format(ErrorMessages.SpeciesCannotBeGenderless, _species.Name));
        }

        private List<MoveInstance> SelectMoves()
        {
            // Specific moves provided
            if (_specificMoves != null)
            {
                return _specificMoves
                    .Take(4)
                    .Select(m => new MoveInstance(m))
                    .ToList();
            }

            // No learnset
            if (_species.Learnset == null || _species.Learnset.Count == 0)
                return new List<MoveInstance>();

            // Get available moves
            var availableMoves = _species.GetMovesUpToLevel(_level).ToList();

            if (availableMoves.Count == 0)
                return new List<MoveInstance>();

            IEnumerable<LearnableMove> selectedMoves;

            if (_useRandomMoves)
            {
                // Shuffle and take random moves
                selectedMoves = availableMoves
                    .OrderBy(x => _random.Next())
                    .Take(_moveCount);
            }
            else if (_prioritizeStab || _prioritizePower)
            {
                // Apply smart move selection
                selectedMoves = SelectSmartMoves(availableMoves);
            }
            else
            {
                // Default: highest level first
                selectedMoves = availableMoves
                    .OrderByDescending(m => m.Level)
                    .ThenBy(m => m.Move.Name)
                    .Take(_moveCount);
            }

            return selectedMoves
                .Select(m => new MoveInstance(m.Move))
                .ToList();
        }

        private IEnumerable<LearnableMove> SelectSmartMoves(List<LearnableMove> availableMoves)
        {
            var scored = availableMoves.Select(m => new
            {
                Move = m,
                Score = CalculateMoveScore(m.Move)
            });

            return scored
                .OrderByDescending(x => x.Score)
                .Take(_moveCount)
                .Select(x => x.Move);
        }

        private int CalculateMoveScore(MoveData move)
        {
            int score = 0;

            // Base power score (0-150 range mapped to 0-150)
            if (_prioritizePower)
            {
                score += move.Power;
            }

            // STAB bonus
            if (_prioritizeStab)
            {
                bool isStab = move.Type == _species.PrimaryType || 
                              move.Type == _species.SecondaryType;
                if (isStab)
                {
                    score += 100; // Significant STAB bonus
                }
            }

            // Small bonus for accuracy (higher is better, 0 means always hits)
            if (move.Accuracy > 0)
            {
                score += move.Accuracy / 10;
            }

            // Bonus for damaging moves when prioritizing power
            if (_prioritizePower && move.Category != MoveCategory.Status)
            {
                score += 50;
            }

            return score;
        }

        private static Nature GetRandomNature()
        {
            var natures = (Nature[])Enum.GetValues(typeof(Nature));
            return natures[_random.Next(natures.Length)];
        }

        private static Nature GetNatureBoostingStat(Stat stat)
        {
            switch (stat)
            {
                case Stat.Attack: return Nature.Adamant;
                case Stat.Defense: return Nature.Bold;
                case Stat.SpAttack: return Nature.Modest;
                case Stat.SpDefense: return Nature.Calm;
                case Stat.Speed: return Nature.Jolly;
                default: return Nature.Hardy;
            }
        }

        private AbilityData DetermineAbility()
        {
            // Specific ability provided
            if (_ability != null)
                return _ability;

            // Hidden ability requested
            if (_useHiddenAbility && _species.HiddenAbility != null)
                return _species.HiddenAbility;

            // Random ability from species
            return _species.GetRandomAbility(_random);
        }

        #endregion
    }

    /// <summary>
    /// Static helper class for quick Pokemon creation.
    /// Alias for PokemonInstanceBuilder for cleaner syntax.
    /// </summary>
    public static class Pokemon
    {
        /// <summary>
        /// Start building a Pokemon instance.
        /// Usage: Pokemon.Create(species, level).WithNature(Nature.Jolly).Build()
        /// </summary>
        public static PokemonInstanceBuilder Create(PokemonSpeciesData species, int level)
        {
            return PokemonInstanceBuilder.Create(species, level);
        }

        /// <summary>
        /// Create a Pokemon with random level in range.
        /// </summary>
        public static PokemonInstanceBuilder CreateInLevelRange(PokemonSpeciesData species, int minLevel, int maxLevel)
        {
            return PokemonInstanceBuilder.CreateWithRandomLevel(species, minLevel, maxLevel);
        }

        /// <summary>
        /// Quick creation with all random values.
        /// </summary>
        public static PokemonInstance Random(PokemonSpeciesData species, int level)
        {
            return PokemonInstanceBuilder.CreateRandom(species, level);
        }

        /// <summary>
        /// Quick creation with hidden ability.
        /// </summary>
        public static PokemonInstance WithHiddenAbility(PokemonSpeciesData species, int level)
        {
            return PokemonInstanceBuilder.CreateWithHiddenAbility(species, level);
        }

        /// <summary>
        /// Quick creation with held item.
        /// </summary>
        public static PokemonInstance WithItem(PokemonSpeciesData species, int level, ItemData item)
        {
            return PokemonInstanceBuilder.CreateWithItem(species, level, item);
        }

        /// <summary>
        /// Sets the random seed for deterministic generation.
        /// </summary>
        public static void SetSeed(int seed)
        {
            PokemonInstanceBuilder.SetSeed(seed);
        }

        /// <summary>
        /// Resets to a new random instance.
        /// </summary>
        public static void ResetRandom()
        {
            PokemonInstanceBuilder.ResetRandom();
        }
    }
}

