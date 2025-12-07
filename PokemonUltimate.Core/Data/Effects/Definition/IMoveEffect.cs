using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Core.Data.Effects.Definition
{
    /// <summary>
    /// Base interface for all move effects.
    /// Effects describe WHAT a move does. Execution logic will be added later with BattleActions.
    /// Following Composition Pattern: moves are composed of multiple effects.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.2: Move Data
    /// **Documentation**: See `docs/features/1-game-data/1.2-move-data/architecture.md`
    /// </remarks>
    public interface IMoveEffect
    {
        /// <summary>
        /// The type of this effect, used for identification and filtering.
        /// </summary>
        EffectType EffectType { get; }

        /// <summary>
        /// Human-readable description of what this effect does.
        /// </summary>
        string Description { get; }
    }
}
