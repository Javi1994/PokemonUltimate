using PokemonUltimate.Core.Data.Blueprints;

namespace PokemonUltimate.Combat.Systems.Effects
{
    /// <summary>
    /// Interface for modifying move properties temporarily during battle.
    /// Used for effects like Pursuit (doubles power), Helping Hand (boosts power), etc.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public interface IMoveModifier
    {
        /// <summary>
        /// Applies modifications to a move's properties.
        /// Returns a new MoveData instance with modifications applied, or the original if no changes.
        /// </summary>
        /// <param name="originalMove">The original move data. Cannot be null.</param>
        /// <returns>A MoveData instance with modifications applied.</returns>
        MoveData ApplyModifications(MoveData originalMove);
    }
}
