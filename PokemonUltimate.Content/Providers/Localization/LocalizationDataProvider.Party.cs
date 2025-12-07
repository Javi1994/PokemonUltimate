using System.Collections.Generic;
using PokemonUltimate.Core.Infrastructure.Localization;

namespace PokemonUltimate.Content.Providers
{
    /// <summary>
    /// Partial class for party management message translations.
    /// </summary>
    public static partial class LocalizationDataProvider
    {
        private static void InitializeParty()
        {
            Register(LocalizationKey.PartyIsFull, new Dictionary<string, string>
            {
                { "en", "Party is full (maximum {0} Pokemon)" },
                { "es", "El equipo está lleno (máximo {0} Pokemon)" },
                { "fr", "L'équipe est pleine (maximum {0} Pokemon)" }
            });

            Register(LocalizationKey.PartyTooSmallForBattle, new Dictionary<string, string>
            {
                { "en", "Party must have at least {0} active Pokemon for battle" },
                { "es", "El equipo debe tener al menos {0} Pokemon activo para la batalla" },
                { "fr", "L'équipe doit avoir au moins {0} Pokemon actif pour la bataille" }
            });

            Register(LocalizationKey.CannotRemoveLastActivePokemon, new Dictionary<string, string>
            {
                { "en", "Cannot remove last active Pokemon during battle" },
                { "es", "No se puede eliminar el último Pokemon activo durante la batalla" },
                { "fr", "Impossible de retirer le dernier Pokemon actif pendant la bataille" }
            });

            Register(LocalizationKey.InvalidPartyIndex, new Dictionary<string, string>
            {
                { "en", "Party index {0} is invalid (party size: {1})" },
                { "es", "El índice {0} del equipo es inválido (tamaño del equipo: {1})" },
                { "fr", "L'index {0} de l'équipe est invalide (taille de l'équipe: {1})" }
            });
        }
    }
}
