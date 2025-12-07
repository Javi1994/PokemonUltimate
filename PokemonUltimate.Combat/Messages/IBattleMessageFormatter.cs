using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Domain.Instances;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.Messages
{
    /// <summary>
    /// Interface for formatting battle messages.
    /// Centralizes message formatting logic and provides type-safe message creation.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    public interface IBattleMessageFormatter
    {
        /// <summary>
        /// Formats a message for when a Pokemon uses a move.
        /// </summary>
        /// <param name="pokemon">The Pokemon using the move.</param>
        /// <param name="move">The move being used.</param>
        /// <returns>The formatted message.</returns>
        string FormatMoveUsed(PokemonInstance pokemon, MoveData move);

        /// <summary>
        /// Formats a message for when a Pokemon uses a move while another is switching out.
        /// </summary>
        /// <param name="switchingPokemon">The Pokemon switching out.</param>
        /// <param name="attackingPokemon">The Pokemon using the move.</param>
        /// <param name="move">The move being used.</param>
        /// <returns>The formatted message.</returns>
        string FormatMoveUsedDuringSwitch(PokemonInstance switchingPokemon, PokemonInstance attackingPokemon, MoveData move);

        /// <summary>
        /// Formats a message using a GameMessages template.
        /// </summary>
        /// <param name="template">The message template.</param>
        /// <param name="args">The arguments for the template.</param>
        /// <returns>The formatted message.</returns>
        string Format(string template, params object[] args);
    }
}
