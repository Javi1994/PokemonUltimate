using System;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Combat.Messages
{
    /// <summary>
    /// Default implementation of IBattleMessageFormatter.
    /// Formats battle messages using GameMessages templates.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    public class BattleMessageFormatter : IBattleMessageFormatter
    {
        /// <summary>
        /// Formats a message for when a Pokemon uses a move.
        /// </summary>
        public string FormatMoveUsed(PokemonInstance pokemon, MoveData move)
        {
            if (pokemon == null)
                throw new ArgumentNullException(nameof(pokemon));
            if (move == null)
                throw new ArgumentNullException(nameof(move));

            return $"{pokemon.DisplayName} used {move.Name}!";
        }

        /// <summary>
        /// Formats a message for when a Pokemon uses a move while another is switching out.
        /// </summary>
        public string FormatMoveUsedDuringSwitch(PokemonInstance switchingPokemon, PokemonInstance attackingPokemon, MoveData move)
        {
            if (switchingPokemon == null)
                throw new ArgumentNullException(nameof(switchingPokemon));
            if (attackingPokemon == null)
                throw new ArgumentNullException(nameof(attackingPokemon));
            if (move == null)
                throw new ArgumentNullException(nameof(move));

            return $"{switchingPokemon.DisplayName} is switching out! {attackingPokemon.DisplayName} used {move.Name}!";
        }

        /// <summary>
        /// Formats a message using a GameMessages template.
        /// </summary>
        public string Format(string template, params object[] args)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));

            if (args == null || args.Length == 0)
                return template;

            return string.Format(template, args);
        }
    }
}
