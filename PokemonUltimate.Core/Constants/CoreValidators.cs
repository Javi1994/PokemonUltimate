using PokemonUltimate.Core.Constants;

namespace PokemonUltimate.Core.Constants
{
    /// <summary>
    /// Centralized validation methods for Core module.
    /// Provides consistent validation across all classes.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    public static class CoreValidators
    {
        /// <summary>
        /// Validates that level is between 1 and 100.
        /// </summary>
        /// <param name="level">The level to validate.</param>
        /// <param name="paramName">The parameter name for error messages.</param>
        /// <exception cref="ArgumentException">Thrown when level is out of valid range.</exception>
        public static void ValidateLevel(int level, string paramName = "level")
        {
            if (level < 1 || level > 100)
                throw new System.ArgumentException(ErrorMessages.LevelMustBeBetween1And100, paramName);
        }

        /// <summary>
        /// Validates that friendship is between 0 and 255.
        /// </summary>
        /// <param name="friendship">The friendship value to validate.</param>
        /// <param name="paramName">The parameter name for error messages.</param>
        /// <exception cref="ArgumentException">Thrown when friendship is out of valid range.</exception>
        public static void ValidateFriendship(int friendship, string paramName = "friendship")
        {
            if (friendship < 0 || friendship > CoreConstants.MaxFriendship)
                throw new System.ArgumentException(ErrorMessages.FriendshipMustBeBetween0And255, paramName);
        }

        /// <summary>
        /// Validates that stat stage is between -6 and +6.
        /// </summary>
        /// <param name="stage">The stat stage to validate.</param>
        /// <param name="paramName">The parameter name for error messages.</param>
        /// <exception cref="ArgumentException">Thrown when stat stage is out of valid range.</exception>
        public static void ValidateStatStage(int stage, string paramName = "stage")
        {
            if (stage < CoreConstants.MinStatStage || stage > CoreConstants.MaxStatStage)
                throw new System.ArgumentException(
                    ErrorMessages.Format("Stat stage must be between {0} and {1}",
                        CoreConstants.MinStatStage, CoreConstants.MaxStatStage),
                    paramName);
        }

        /// <summary>
        /// Validates that IV is between 0 and 31.
        /// </summary>
        /// <param name="iv">The IV value to validate.</param>
        /// <param name="paramName">The parameter name for error messages.</param>
        /// <exception cref="ArgumentException">Thrown when IV is out of valid range.</exception>
        public static void ValidateIV(int iv, string paramName = "iv")
        {
            if (iv < 0 || iv > CoreConstants.MaxIV)
                throw new System.ArgumentException(
                    ErrorMessages.Format(ErrorMessages.IVMustBeBetween, CoreConstants.MaxIV),
                    paramName);
        }

        /// <summary>
        /// Validates that EV is between 0 and 252.
        /// </summary>
        /// <param name="ev">The EV value to validate.</param>
        /// <param name="paramName">The parameter name for error messages.</param>
        /// <exception cref="ArgumentException">Thrown when EV is out of valid range.</exception>
        public static void ValidateEV(int ev, string paramName = "ev")
        {
            if (ev < 0 || ev > CoreConstants.MaxEV)
                throw new System.ArgumentException(
                    ErrorMessages.Format(ErrorMessages.EVMustBeBetween, CoreConstants.MaxEV),
                    paramName);
        }
    }
}
