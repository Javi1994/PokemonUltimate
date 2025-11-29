using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// Base interface for all move effects.
    /// Effects describe WHAT a move does. Execution logic will be added later with BattleActions.
    /// Following Composition Pattern: moves are composed of multiple effects.
    /// </summary>
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
