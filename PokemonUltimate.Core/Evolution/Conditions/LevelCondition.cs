using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Core.Evolution.Conditions
{
    /// <summary>
    /// Evolution condition: Pokemon must reach a minimum level.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.11: Evolution System
    /// **Documentation**: See `docs/features/1-game-data/1.11-evolution-system/README.md`
    /// </remarks>
    public class LevelCondition : IEvolutionCondition
    {
        public EvolutionConditionType ConditionType => EvolutionConditionType.Level;
        public string Description => $"Reach level {MinLevel}";

        /// <summary>
        /// Minimum level required for evolution.
        /// </summary>
        public int MinLevel { get; set; }

        public LevelCondition() { }

        public LevelCondition(int minLevel)
        {
            MinLevel = minLevel;
        }

        public bool IsMet(PokemonInstance pokemon)
        {
            return pokemon != null && pokemon.Level >= MinLevel;
        }
    }
}
