namespace PokemonUltimate.Core.Strategies.Effect.Definition
{
    /// <summary>
    /// Strategy interface for generating descriptions for move restriction effects.
    /// </summary>
    public interface IMoveRestrictionDescriptionStrategy
    {
        /// <summary>
        /// Gets the description for the move restriction effect.
        /// </summary>
        /// <param name="duration">Duration in turns (0 = indefinite).</param>
        string GetDescription(int duration);
    }
}
