namespace PokemonUltimate.Core.Strategies.Nature.Definition
{
    /// <summary>
    /// Strategy interface for determining which nature boosts a specific stat.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.12: Factories & Calculators
    /// **Documentation**: See `docs/features/1-game-data/1.12-factories-calculators/README.md`
    /// </remarks>
    public interface INatureBoostingStrategy
    {
        /// <summary>
        /// Gets the nature that boosts the stat associated with this strategy.
        /// </summary>
        Data.Enums.Nature GetBoostingNature();
    }
}
