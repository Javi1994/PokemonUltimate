using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Infrastructure.Builders;
using PokemonInstance = PokemonUltimate.Core.Domain.Instances.Pokemon.PokemonInstance;

namespace PokemonUltimate.Core.Infrastructure.Factories
{
    /// <summary>
    /// Static factory for quick Pokemon creation.
    /// For more control, use Pokemon.Create() builder pattern instead.
    ///
    /// Quick usage:
    ///   var pokemon = PokemonFactory.Create(species, level);
    ///
    /// Builder usage (recommended):
    ///   var pokemon = Pokemon.Create(species, level)
    ///       .WithNature(Nature.Jolly)
    ///       .Named("Sparky")
    ///       .Build();
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.12: Factories & Calculators
    /// **Documentation**: See `docs/features/1-game-data/1.12-factories-calculators/README.md`
    /// </remarks>
    public static class PokemonFactory
    {
        /// <summary>
        /// Creates a Pokemon instance with random nature and gender.
        /// </summary>
        public static PokemonInstance Create(PokemonSpeciesData species, int level)
        {
            return PokemonInstanceBuilder.Create(species, level).Build();
        }

        /// <summary>
        /// Creates a Pokemon instance with specific nature and random gender.
        /// </summary>
        public static PokemonInstance Create(PokemonSpeciesData species, int level, Nature nature)
        {
            return PokemonInstanceBuilder.Create(species, level)
                .WithNature(nature)
                .Build();
        }

        /// <summary>
        /// Creates a Pokemon instance with specific nature and gender.
        /// </summary>
        public static PokemonInstance Create(PokemonSpeciesData species, int level, Nature nature, Gender gender)
        {
            return PokemonInstanceBuilder.Create(species, level)
                .WithNature(nature)
                .WithGender(gender)
                .Build();
        }

        /// <summary>
        /// Sets the random seed for deterministic generation.
        /// </summary>
        public static void SetSeed(int seed) => PokemonInstanceBuilder.SetSeed(seed);

        /// <summary>
        /// Resets to a new random instance.
        /// </summary>
        public static void ResetRandom() => PokemonInstanceBuilder.ResetRandom();
    }
}
