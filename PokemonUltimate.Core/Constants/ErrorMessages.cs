namespace PokemonUltimate.Core.Constants
{
    /// <summary>
    /// Centralized error messages for validation and exceptions.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    public static class ErrorMessages
    {
        #region Numeric Validation

        public const string AmountCannotBeNegative = "Amount cannot be negative";
        public const string LevelMustBeBetween1And100 = "Level must be between 1 and 100";
        public const string ExperienceCannotBeNegative = "Experience cannot be negative";
        public const string BaseStatCannotBeNegative = "Base stat cannot be negative";
        public const string HPCannotBeNegative = "HP cannot be negative";
        public const string PercentMustBeBetween0And1 = "Percent must be between 0.0 and 1.0";
        public const string FriendshipMustBeBetween0And255 = "Friendship must be between 0 and 255";
        public const string MoveCountMustBeBetween1And4 = "Move count must be between 1 and 4";
        public const string MinLevelCannotBeGreaterThanMaxLevel = "minLevel cannot be greater than maxLevel";
        public const string StatCannotBeNegative = "Stat cannot be negative";
        public const string IVMustBeBetween = "IV must be between 0 and {0}";
        public const string EVMustBeBetween = "EV must be between 0 and {0}";

        #endregion

        #region Gender Validation

        public const string SpeciesIsGenderless = "{0} is genderless";
        public const string SpeciesIsMaleOnly = "{0} is male-only";
        public const string SpeciesIsFemaleOnly = "{0} is female-only";
        public const string SpeciesCannotBeGenderless = "{0} cannot be genderless";

        #endregion

        #region Null Validation

        public const string MoveCannotBeNull = "Move cannot be null";
        public const string SpeciesCannotBeNull = "Species cannot be null";
        public const string PokemonCannotBeNull = "Pokemon cannot be null";
        public const string TargetSpeciesCannotBeNull = "Target species cannot be null";
        public const string EvolutionTargetNotValid = "{0} cannot evolve to {1}";
        public const string FieldCannotBeNull = "Battle field cannot be null";
        public const string ViewCannotBeNull = "Battle view cannot be null";

        #endregion

        #region Not Found

        public const string ItemNotFound = "Item with ID {0} not found.";
        public const string PokemonNotFoundByPokedex = "Pokemon with Pokedex Number {0} not found.";

        #endregion

        #region Combat

        public const string SlotIndexCannotBeNegative = "Slot index cannot be negative";
        public const string CannotModifyHPStatStage = "Cannot modify HP stat stage";
        public const string SlotCountMustBePositive = "Slot count must be at least 1";
        public const string PartyCannotBeNull = "Party cannot be null";
        public const string PartyCannotBeEmpty = "Party cannot be empty";
        public const string BattleQueueInfiniteLoop = "Battle queue exceeded maximum iterations (1000). Possible infinite loop.";
        public const string SlotCannotBeNull = "Battle slot cannot be null";
        public const string AbilityCannotBeNull = "Ability cannot be null";
        public const string ItemCannotBeNull = "Item cannot be null";
        public const string ContextCannotBeNull = "Damage context cannot be null";

        #endregion

        #region Helpers

        /// <summary>
        /// Formats a message template with arguments.
        /// </summary>
        public static string Format(string template, params object[] args)
        {
            return string.Format(template, args);
        }

        #endregion
    }
}

