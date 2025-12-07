namespace PokemonUltimate.Core.Data.Blueprints
{
    /// <summary>
    /// Contract for any object that has a unique String ID.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.9: Interfaces Base
    /// **Documentation**: See `docs/features/1-game-data/1.9-interfaces-base/README.md`
    /// </remarks>
    public interface IIdentifiable
    {
        string Id { get; }
    }
}

