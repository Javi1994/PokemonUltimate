using PokemonUltimate.Core.Infrastructure.Localization;

namespace PokemonUltimate.Content.Providers
{
    /// <summary>
    /// Partial class for Pokemon name translations.
    /// </summary>
    public static partial class LocalizationDataProvider
    {
        private static void InitializePokemonNames()
        {
            // Starter lines
            RegisterPokemonName("Bulbasaur", "Bulbasaur", "Bulbasaur", "Bulbizarre");
            RegisterPokemonName("Ivysaur", "Ivysaur", "Ivysaur", "Herbizarre");
            RegisterPokemonName("Venusaur", "Venusaur", "Venusaur", "Florizarre");

            RegisterPokemonName("Charmander", "Charmander", "Charmander", "Salamèche");
            RegisterPokemonName("Charmeleon", "Charmeleon", "Charmeleon", "Reptincel");
            RegisterPokemonName("Charizard", "Charizard", "Charizard", "Dracaufeu");

            RegisterPokemonName("Squirtle", "Squirtle", "Squirtle", "Carapuce");
            RegisterPokemonName("Wartortle", "Wartortle", "Wartortle", "Carabaffe");
            RegisterPokemonName("Blastoise", "Blastoise", "Blastoise", "Tortank");

            // Electric Pokemon
            RegisterPokemonName("Pikachu", "Pikachu", "Pikachu", "Pikachu");
            RegisterPokemonName("Raichu", "Raichu", "Raichu", "Raichu");

            // Normal Pokemon
            RegisterPokemonName("Eevee", "Eevee", "Eevee", "Évoli");

            // Psychic Pokemon
            RegisterPokemonName("Mewtwo", "Mewtwo", "Mewtwo", "Mewtwo");
            RegisterPokemonName("Mew", "Mew", "Mew", "Mew");

            // Ghost/Poison line
            RegisterPokemonName("Gastly", "Gastly", "Gastly", "Fantominus");
            RegisterPokemonName("Haunter", "Haunter", "Haunter", "Spectrum");
            RegisterPokemonName("Gengar", "Gengar", "Gengar", "Ectoplasma");

            // Rock/Ground line
            RegisterPokemonName("Geodude", "Geodude", "Geodude", "Racaillou");
            RegisterPokemonName("Graveler", "Graveler", "Graveler", "Gravalanch");
            RegisterPokemonName("Golem", "Golem", "Golem", "Grolem");

            // Normal Pokemon (additional)
            RegisterPokemonName("Snorlax", "Snorlax", "Snorlax", "Ronflex");

            // Water/Flying Pokemon
            RegisterPokemonName("Magikarp", "Magikarp", "Magikarp", "Magicarpe");
            RegisterPokemonName("Gyarados", "Gyarados", "Gyarados", "Léviator");

            // Psychic Pokemon line
            RegisterPokemonName("Abra", "Abra", "Abra", "Abra");
            RegisterPokemonName("Kadabra", "Kadabra", "Kadabra", "Kadabra");
            RegisterPokemonName("Alakazam", "Alakazam", "Alakazam", "Alakazam");

            // Gen 3 Pokemon
            RegisterPokemonName("Slakoth", "Slakoth", "Slakoth", "Parecool");
            RegisterPokemonName("Vigoroth", "Vigoroth", "Vigoroth", "Vigoroth");
            RegisterPokemonName("Slaking", "Slaking", "Slaking", "Monaflèmit");
            RegisterPokemonName("Carvanha", "Carvanha", "Carvanha", "Carvanha");
            RegisterPokemonName("Sharpedo", "Sharpedo", "Sharpedo", "Sharpedo");

            // Gen 4 Pokemon
            RegisterPokemonName("Snover", "Snover", "Snover", "Blizzi");
            RegisterPokemonName("Abomasnow", "Abomasnow", "Abomasnow", "Blizzaroi");

            // Gen 5 Pokemon
            RegisterPokemonName("Ferroseed", "Ferroseed", "Ferroseed", "Grindur");
            RegisterPokemonName("Ferrothorn", "Ferrothorn", "Ferrothorn", "Noacier");
        }
    }
}
