using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.BattleSimulator.Helpers
{
    /// <summary>
    /// Helper class to map Pokemon InstanceIds to readable names with team information.
    /// Used for displaying unique identifiers in logs and statistics when multiple instances
    /// of the same Pokemon species exist.
    /// </summary>
    public static class PokemonNameMapper
    {
        /// <summary>
        /// Creates a mapping from InstanceId to readable name with team info.
        /// Format: "SpeciesName #N (Team)" or "SpeciesName (Team)" if only one instance.
        /// </summary>
        /// <param name="field">The battle field containing both parties. Can be null.</param>
        /// <returns>Dictionary mapping InstanceId to display name.</returns>
        public static Dictionary<string, string> CreateNameMapping(BattleField? field)
        {
            var mapping = new Dictionary<string, string>();

            if (field == null)
                return mapping;

            // Process player party
            if (field.PlayerSide?.Party != null)
            {
                var playerPokemon = field.PlayerSide.Party.ToList();
                var playerSpeciesGroups = playerPokemon.GroupBy(p => p.Species.Name).ToList();

                foreach (var speciesGroup in playerSpeciesGroups)
                {
                    var speciesName = speciesGroup.Key;
                    var pokemonList = speciesGroup.ToList();

                    if (pokemonList.Count == 1)
                    {
                        mapping[pokemonList[0].InstanceId] = $"{speciesName} (Jugador)";
                    }
                    else
                    {
                        for (int i = 0; i < pokemonList.Count; i++)
                        {
                            mapping[pokemonList[i].InstanceId] = $"{speciesName} #{i + 1} (Jugador)";
                        }
                    }
                }
            }

            // Process enemy party
            if (field.EnemySide?.Party != null)
            {
                var enemyPokemon = field.EnemySide.Party.ToList();
                var enemySpeciesGroups = enemyPokemon.GroupBy(p => p.Species.Name).ToList();

                foreach (var speciesGroup in enemySpeciesGroups)
                {
                    var speciesName = speciesGroup.Key;
                    var pokemonList = speciesGroup.ToList();

                    if (pokemonList.Count == 1)
                    {
                        mapping[pokemonList[0].InstanceId] = $"{speciesName} (Enemigo)";
                    }
                    else
                    {
                        for (int i = 0; i < pokemonList.Count; i++)
                        {
                            mapping[pokemonList[i].InstanceId] = $"{speciesName} #{i + 1} (Enemigo)";
                        }
                    }
                }
            }

            return mapping;
        }

        /// <summary>
        /// Gets the display name for a Pokemon instance, using the mapping if available.
        /// Falls back to DisplayName if mapping is not available.
        /// </summary>
        /// <param name="pokemon">The Pokemon instance.</param>
        /// <param name="mapping">The name mapping dictionary. Can be null.</param>
        /// <returns>The display name with unique identifier.</returns>
        public static string GetDisplayName(PokemonInstance pokemon, Dictionary<string, string>? mapping)
        {
            if (pokemon == null)
                return "Unknown";

            if (mapping != null && mapping.TryGetValue(pokemon.InstanceId, out var mappedName))
                return mappedName;

            return pokemon.DisplayName;
        }
    }
}
