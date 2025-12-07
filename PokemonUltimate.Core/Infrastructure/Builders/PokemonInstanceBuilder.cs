using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Data.Effects;
using PokemonUltimate.Core.Data.Effects.Definition;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Domain.Instances.Move;
using PokemonUltimate.Core.Domain.Instances.Stats;
using PokemonUltimate.Core.Infrastructure.Providers.Definition;
using PokemonUltimate.Core.Services;
using PokemonUltimate.Core.Strategies.Move;
using PokemonUltimate.Core.Strategies.Nature;
using PokemonInstance = PokemonUltimate.Core.Domain.Instances.Pokemon.PokemonInstance;

namespace PokemonUltimate.Core.Infrastructure.Builders
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
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.12: Factories & Calculators
    /// **Documentation**: See `docs/features/1-game-data/1.12-factories-calculators/README.md`
    /// </remarks>
    public class PokemonInstanceBuilder
    {
        private static IRandomProvider _defaultRandomProvider = new Providers.RandomProvider();

        private readonly PokemonSpeciesData _species;
        private readonly int _level;
        private readonly IRandomProvider _randomProvider;

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
        private int _friendship = CoreConstants.DefaultWildFriendship; // Default for wild Pokemon
        private bool? _isShiny;

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

        // IVs and EVs
        private IVSet _ivs;
        private EVSet _evs;

        #region Constructor

        private PokemonInstanceBuilder(PokemonSpeciesData species, int level, IRandomProvider randomProvider = null)
        {
            _species = species ?? throw new ArgumentNullException(nameof(species));

            CoreValidators.ValidateLevel(level);

            _level = level;
            _randomProvider = randomProvider ?? _defaultRandomProvider;
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

            int level = _defaultRandomProvider.Next(minLevel, maxLevel + 1);
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
            _defaultRandomProvider = new Providers.RandomProvider(seed);
        }

        /// <summary>
        /// Resets to a new random instance.
        /// </summary>
        public static void ResetRandom()
        {
            _defaultRandomProvider = new Providers.RandomProvider();
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
            _currentHP = currentHP;
            _hpPercent = null;
            return this;
        }

        /// <summary>
        /// Start at specific HP percentage (0.0 to 1.0).
        /// </summary>
        public PokemonInstanceBuilder AtHealthPercent(float percent)
        {
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
        /// Set current HP to a specific value (alias for AtHealth).
        /// </summary>
        public PokemonInstanceBuilder WithCurrentHP(int currentHP)
        {
            return AtHealth(currentHP);
        }

        /// <summary>
        /// Set current HP as a percentage (0.0 to 1.0) (alias for AtHealthPercent).
        /// </summary>
        public PokemonInstanceBuilder WithHPPercent(float hpPercent)
        {
            return AtHealthPercent(hpPercent);
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
            CoreValidators.ValidateFriendship(friendship);

            _friendship = friendship;
            return this;
        }

        /// <summary>
        /// Set high friendship (220) - enough for friendship evolutions.
        /// </summary>
        public PokemonInstanceBuilder WithHighFriendship()
        {
            _friendship = CoreConstants.HighFriendshipThreshold;
            return this;
        }

        /// <summary>
        /// Set maximum friendship (255).
        /// </summary>
        public PokemonInstanceBuilder WithMaxFriendship()
        {
            _friendship = CoreConstants.MaxFriendship;
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
            _friendship = CoreConstants.HatchedFriendship;
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

        /// <summary>
        /// Override defense stat (for testing purposes).
        /// </summary>
        public PokemonInstanceBuilder WithDefense(int defense)
        {
            _overrideDefense = defense;
            return this;
        }

        /// <summary>
        /// Override special attack stat (for testing purposes).
        /// </summary>
        public PokemonInstanceBuilder WithSpAttack(int spAttack)
        {
            _overrideSpAttack = spAttack;
            return this;
        }

        /// <summary>
        /// Override special defense stat (for testing purposes).
        /// </summary>
        public PokemonInstanceBuilder WithSpDefense(int spDefense)
        {
            _overrideSpDefense = spDefense;
            return this;
        }

        #endregion

        #region IVs and EVs Configuration

        /// <summary>
        /// Set specific IVs for the Pokemon.
        /// </summary>
        public PokemonInstanceBuilder WithIVs(IVSet ivs)
        {
            _ivs = ivs ?? throw new ArgumentNullException(nameof(ivs));
            return this;
        }

        /// <summary>
        /// Set specific IVs for each stat.
        /// </summary>
        public PokemonInstanceBuilder WithIVs(int hp, int attack, int defense, int spAttack, int spDefense, int speed)
        {
            _ivs = new IVSet(hp, attack, defense, spAttack, spDefense, speed);
            return this;
        }

        /// <summary>
        /// Set perfect IVs (all 31).
        /// </summary>
        public PokemonInstanceBuilder WithPerfectIVs()
        {
            _ivs = IVSet.Perfect();
            return this;
        }

        /// <summary>
        /// Set zero IVs (all 0).
        /// </summary>
        public PokemonInstanceBuilder WithZeroIVs()
        {
            _ivs = IVSet.Zero();
            return this;
        }

        /// <summary>
        /// Set specific EVs for the Pokemon.
        /// Note: In this game, EVs are always maximum by default for roguelike experience.
        /// </summary>
        public PokemonInstanceBuilder WithEVs(EVSet evs)
        {
            _evs = evs ?? throw new ArgumentNullException(nameof(evs));
            return this;
        }

        /// <summary>
        /// Set specific EVs for each stat.
        /// Note: Total must not exceed 510.
        /// </summary>
        public PokemonInstanceBuilder WithEVs(int hp, int attack, int defense, int spAttack, int spDefense, int speed)
        {
            _evs = new EVSet(hp, attack, defense, spAttack, spDefense, speed);
            return this;
        }

        /// <summary>
        /// Set maximum EVs (all 252).
        /// This is the default for this game.
        /// </summary>
        public PokemonInstanceBuilder WithMaximumEVs()
        {
            _evs = EVSet.Maximum();
            return this;
        }

        /// <summary>
        /// Set zero EVs (all 0).
        /// </summary>
        public PokemonInstanceBuilder WithZeroEVs()
        {
            _evs = EVSet.Zero();
            return this;
        }

        #endregion

        #region Build

        /// <summary>
        /// Build the Pokemon instance.
        /// </summary>
        public PokemonInstance Build()
        {
            // Validate configuration before building
            ValidateConfiguration();

            // Determine nature
            Nature nature = _nature ?? GetRandomNature(_randomProvider);

            // Determine gender
            Gender gender = DetermineGender();

            // Generate IVs and EVs if not provided
            IVSet ivs = _ivs ?? GenerateRandomIVs();
            EVSet evs = _evs ?? EVSet.Maximum(); // Always maximum EVs for roguelike experience

            // Calculate stats using IVs and EVs
            var stats = CalculateStats(nature, ivs, evs);

            // Select moves
            List<MoveInstance> moves = SelectMoves();

            // Determine shiny
            bool isShiny = DetermineShiny();

            // Determine ability
            AbilityData ability = DetermineAbility();

            // Create instance
            var pokemon = new PokemonInstance(
                _species, _level, stats.HP, stats.Attack, stats.Defense,
                stats.SpAttack, stats.SpDefense, stats.Speed, nature, gender, moves,
                _friendship, isShiny, ability, _heldItem, ivs, evs);

            // Apply optional configurations
            ApplyOptionalConfigurations(pokemon);

            return pokemon;
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Validates the builder configuration before building the Pokemon instance.
        /// </summary>
        private void ValidateConfiguration()
        {
            // Validate stat overrides
            if (_overrideMaxHP.HasValue && _overrideMaxHP.Value < 0)
                throw new ArgumentException(ErrorMessages.StatOverrideCannotBeNegative, nameof(_overrideMaxHP));

            if (_overrideAttack.HasValue && _overrideAttack.Value < 0)
                throw new ArgumentException(ErrorMessages.StatOverrideCannotBeNegative, nameof(_overrideAttack));

            if (_overrideDefense.HasValue && _overrideDefense.Value < 0)
                throw new ArgumentException(ErrorMessages.StatOverrideCannotBeNegative, nameof(_overrideDefense));

            if (_overrideSpAttack.HasValue && _overrideSpAttack.Value < 0)
                throw new ArgumentException(ErrorMessages.StatOverrideCannotBeNegative, nameof(_overrideSpAttack));

            if (_overrideSpDefense.HasValue && _overrideSpDefense.Value < 0)
                throw new ArgumentException(ErrorMessages.StatOverrideCannotBeNegative, nameof(_overrideSpDefense));

            if (_overrideSpeed.HasValue && _overrideSpeed.Value < 0)
                throw new ArgumentException(ErrorMessages.StatOverrideCannotBeNegative, nameof(_overrideSpeed));

            // Validate HP percent
            if (_hpPercent.HasValue)
            {
                if (_hpPercent.Value < 0.0f || _hpPercent.Value > 1.0f)
                    throw new ArgumentException(ErrorMessages.InvalidHPPercent, nameof(_hpPercent));
            }

            // Validate current HP
            if (_currentHP.HasValue && _currentHP.Value < 0)
                throw new ArgumentException(ErrorMessages.HPCannotBeNegative, nameof(_currentHP));

            // Validate experience
            if (_experience < 0)
                throw new ArgumentException(ErrorMessages.ExperienceCannotBeNegative, nameof(_experience));

            // Validate ability (if explicitly set)
            if (_ability != null)
            {
                ValidateAbility();
            }

            // Validate specific moves (if provided)
            if (_specificMoves != null && _specificMoves.Count > 0)
            {
                ValidateMoves();
            }
        }

        /// <summary>
        /// Validates that the ability is valid for the species.
        /// </summary>
        private void ValidateAbility()
        {
            bool isValid = _species.Ability1 == _ability ||
                          _species.Ability2 == _ability ||
                          _species.HiddenAbility == _ability;

            if (!isValid)
            {
                throw new ArgumentException(
                    ErrorMessages.Format(ErrorMessages.AbilityNotValidForSpecies, _ability.Name, _species.Name),
                    nameof(_ability));
            }
        }

        /// <summary>
        /// Validates that all specified moves are in the species' learnset.
        /// </summary>
        private void ValidateMoves()
        {
            if (_species.Learnset == null || _species.Learnset.Count == 0)
            {
                // If species has no learnset, we can't validate moves
                // This is acceptable - moves will be empty
                return;
            }

            var learnsetMoveIds = _species.Learnset.Select(m => m.Move.Id).ToHashSet();

            foreach (var move in _specificMoves)
            {
                if (move == null)
                    continue;

                if (!learnsetMoveIds.Contains(move.Id))
                {
                    throw new ArgumentException(
                        ErrorMessages.Format(ErrorMessages.MoveNotInLearnset, move.Name, _species.Name),
                        nameof(_specificMoves));
                }
            }
        }

        /// <summary>
        /// Calculates all stats for the Pokemon instance using IVs and EVs.
        /// </summary>
        private (int HP, int Attack, int Defense, int SpAttack, int SpDefense, int Speed) CalculateStats(Nature nature, IVSet ivs, EVSet evs)
        {
            var calculator = StatCalculatorService.Default;

            int hp = _overrideMaxHP ?? calculator.CalculateHP(_species.BaseStats.HP, _level, ivs, evs);
            int attack = _overrideAttack ?? calculator.CalculateStat(_species.BaseStats.Attack, _level, nature, Stat.Attack, ivs, evs);
            int defense = _overrideDefense ?? calculator.CalculateStat(_species.BaseStats.Defense, _level, nature, Stat.Defense, ivs, evs);
            int spAttack = _overrideSpAttack ?? calculator.CalculateStat(_species.BaseStats.SpAttack, _level, nature, Stat.SpAttack, ivs, evs);
            int spDefense = _overrideSpDefense ?? calculator.CalculateStat(_species.BaseStats.SpDefense, _level, nature, Stat.SpDefense, ivs, evs);
            int speed = _overrideSpeed ?? calculator.CalculateStat(_species.BaseStats.Speed, _level, nature, Stat.Speed, ivs, evs);

            return (hp, attack, defense, spAttack, spDefense, speed);
        }

        /// <summary>
        /// Generates random IVs for each stat (0-31).
        /// </summary>
        private IVSet GenerateRandomIVs()
        {
            return new IVSet(
                _randomProvider.Next(IVSet.MinIV, IVSet.MaxIV + 1), // HP
                _randomProvider.Next(IVSet.MinIV, IVSet.MaxIV + 1), // Attack
                _randomProvider.Next(IVSet.MinIV, IVSet.MaxIV + 1), // Defense
                _randomProvider.Next(IVSet.MinIV, IVSet.MaxIV + 1), // SpAttack
                _randomProvider.Next(IVSet.MinIV, IVSet.MaxIV + 1), // SpDefense
                _randomProvider.Next(IVSet.MinIV, IVSet.MaxIV + 1)  // Speed
            );
        }

        /// <summary>
        /// Determines if the Pokemon should be shiny.
        /// </summary>
        private bool DetermineShiny()
        {
            if (_isShiny.HasValue)
                return _isShiny.Value;

            return _randomProvider.Next(CoreConstants.ShinyOdds) == 0;
        }

        /// <summary>
        /// Applies optional configurations to the Pokemon instance.
        /// </summary>
        private void ApplyOptionalConfigurations(PokemonInstance pokemon)
        {
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
        }

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
            double roll = _randomProvider.NextDouble() * 100;
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
            List<MoveInstance> moves;

            // Specific moves provided
            if (_specificMoves != null)
            {
                moves = SelectSpecificMoves();
                // If explicitly set to empty (WithNoMoves()), don't assign default Tackle
                if (moves.Count == 0 && _specificMoves.Count == 0)
                {
                    return moves; // Return empty list as requested
                }
            }
            else
            {
                // Select from learnset
                moves = SelectFromLearnset();
            }

            // If no moves available and not explicitly set to empty, assign Tackle as default
            if (moves.Count == 0)
            {
                moves = new List<MoveInstance> { new MoveInstance(CreateDefaultTackle()) };
            }

            return moves;
        }

        /// <summary>
        /// Selects specific moves provided by the builder.
        /// </summary>
        private List<MoveInstance> SelectSpecificMoves()
        {
            return _specificMoves
                .Take(4)
                .Select(m => new MoveInstance(m))
                .ToList();
        }

        /// <summary>
        /// Selects moves from the Pokemon's learnset based on configuration.
        /// </summary>
        private List<MoveInstance> SelectFromLearnset()
        {
            // No learnset
            if (_species.Learnset == null || _species.Learnset.Count == 0)
                return new List<MoveInstance>();

            // Get available moves
            var availableMoves = _species.GetMovesUpToLevel(_level).ToList();

            if (availableMoves.Count == 0)
                return new List<MoveInstance>();

            // Create appropriate move selector based on configuration
            IMoveSelector selector = CreateMoveSelector();

            // Select moves using the selector
            var selectedMoves = selector.SelectMoves(availableMoves, _moveCount);

            return selectedMoves
                .Select(m => new MoveInstance(m.Move))
                .ToList();
        }

        /// <summary>
        /// Creates a default Tackle move for Pokemon without available moves.
        /// This prevents infinite battles when Pokemon have no moves.
        /// </summary>
        private static MoveData CreateDefaultTackle()
        {
            return new MoveData
            {
                Name = "Tackle",
                Description = "A physical attack in which the user charges and slams into the target.",
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                Power = 40,
                Accuracy = 100,
                MaxPP = 35,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                MakesContact = true,
                Effects = new List<IMoveEffect> { new DamageEffect() }
            };
        }

        /// <summary>
        /// Creates the appropriate move selector based on builder configuration.
        /// </summary>
        private IMoveSelector CreateMoveSelector()
        {
            if (_useRandomMoves)
            {
                return MoveSelector.CreateRandom(_randomProvider);
            }

            if (_prioritizeStab && _prioritizePower)
            {
                // Both STAB and Power - use Optimal strategy
                return MoveSelector.CreateOptimal(_species.PrimaryType, _species.SecondaryType, _randomProvider);
            }

            if (_prioritizeStab)
            {
                return MoveSelector.CreateStab(_species.PrimaryType, _species.SecondaryType);
            }

            if (_prioritizePower)
            {
                return MoveSelector.CreatePower();
            }

            // Default strategy
            return MoveSelector.CreateDefault();
        }


        private static Nature GetRandomNature(IRandomProvider randomProvider)
        {
            var natures = (Nature[])Enum.GetValues(typeof(Nature));
            return natures[randomProvider.Next(natures.Length)];
        }

        private static Nature GetNatureBoostingStat(Stat stat)
        {
            return NatureBoostingRegistry.GetBoostingNature(stat);
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
            return _species.GetRandomAbility(_randomProvider);
        }

        #endregion
    }
}

