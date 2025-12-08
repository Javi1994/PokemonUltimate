using System;
using PokemonUltimate.Combat.Infrastructure.Messages.Definition;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Domain.Instances.Pokemon;
using PokemonUltimate.Core.Utilities.Extensions;
using PokemonUltimate.Localization;
using PokemonUltimate.Localization.Constants;
using PokemonUltimate.Localization.Extensions;
using PokemonUltimate.Localization.Providers;
using PokemonUltimate.Localization.Providers.Definition;

namespace PokemonUltimate.Combat.Infrastructure.Messages
{
    /// <summary>
    /// Default implementation of IBattleMessageFormatter.
    /// Formats battle messages using ILocalizationProvider for multi-language support.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// **Integration**: Uses Feature 4.9: Localization System
    /// </remarks>
    public class BattleMessageFormatter : IBattleMessageFormatter
    {
        private readonly ILocalizationProvider _localizationProvider;

        /// <summary>
        /// Initializes a new instance of BattleMessageFormatter with default LocalizationProvider.
        /// </summary>
        public BattleMessageFormatter() : this(new LocalizationProvider())
        {
        }

        /// <summary>
        /// Initializes a new instance of BattleMessageFormatter with the specified LocalizationProvider.
        /// </summary>
        /// <param name="localizationProvider">The localization provider to use.</param>
        public BattleMessageFormatter(ILocalizationProvider localizationProvider)
        {
            _localizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
        }

        /// <summary>
        /// Formats a message for when a Pokemon uses a move.
        /// </summary>
        public string FormatMoveUsed(PokemonInstance pokemon, MoveData move)
        {
            if (pokemon == null)
                throw new ArgumentNullException(nameof(pokemon));
            if (move == null)
                throw new ArgumentNullException(nameof(move));

            return _localizationProvider.GetString(
                LocalizationKey.BattleUsedMove,
                pokemon.Species.GetDisplayName(_localizationProvider),
                move.GetDisplayName(_localizationProvider)
            );
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

            var switchMessage = _localizationProvider.GetString(
                LocalizationKey.BattleSwitchingOut,
                switchingPokemon.Species.GetDisplayName(_localizationProvider)
            );
            var moveMessage = _localizationProvider.GetString(
                LocalizationKey.BattleUsedMove,
                attackingPokemon.Species.GetDisplayName(_localizationProvider),
                move.GetDisplayName(_localizationProvider)
            );
            return $"{switchMessage} {moveMessage}";
        }

        /// <summary>
        /// Formats a message using a localization key or template.
        /// Supports both LocalizationKey constants and legacy GameMessages templates.
        /// </summary>
        public string Format(string template, params object[] args)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));

            // Check if template is a localization key (starts with known prefixes)
            if (template.StartsWith("battle_") ||
                template.StartsWith("type_") ||
                template.StartsWith("status_") ||
                template.StartsWith("weather_") ||
                template.StartsWith("terrain_") ||
                template.StartsWith("ability_") ||
                template.StartsWith("item_") ||
                template.StartsWith("hazard_") ||
                template.StartsWith("move_") ||
                template.StartsWith("hits_") ||
                template.StartsWith("truant_"))
            {
                // Treat as localization key
                return _localizationProvider.GetString(template, args);
            }

            // Legacy support: treat as format string (backward compatibility)
            if (args == null || args.Length == 0)
                return template;

            return string.Format(template, args);
        }
    }
}
