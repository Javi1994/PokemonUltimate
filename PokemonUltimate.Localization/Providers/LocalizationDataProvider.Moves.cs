using System.Collections.Generic;
using PokemonUltimate.Localization.Constants;

namespace PokemonUltimate.Localization.Providers
{
    /// <summary>
    /// Partial class for move name translations.
    /// </summary>
    public static partial class LocalizationDataProvider
    {
        private static void InitializeMoveNames()
        {
            // Electric moves
            RegisterMoveName("Thunder Shock", "Thunder Shock", "Impactrueno", "Éclair");
            RegisterMoveName("Thunderbolt", "Thunderbolt", "Rayo", "Tonnerre");
            RegisterMoveName("Thunder", "Thunder", "Trueno", "Fatal-Foudre");
            RegisterMoveName("Thunder Wave", "Thunder Wave", "Onda Trueno", "Cage-Éclair");

            // Fire moves
            RegisterMoveName("Ember", "Ember", "Ascuas", "Flammèche");
            RegisterMoveName("Flamethrower", "Flamethrower", "Lanzallamas", "Lance-Flammes");
            RegisterMoveName("Fire Blast", "Fire Blast", "Llamarada", "Déflagration");

            // Water moves
            RegisterMoveName("Water Gun", "Water Gun", "Pistola Agua", "Pistolet à O");
            RegisterMoveName("Surf", "Surf", "Surf", "Surf");
            RegisterMoveName("Hydro Pump", "Hydro Pump", "Hidrobomba", "Hydrocanon");

            // Grass moves
            RegisterMoveName("Vine Whip", "Vine Whip", "Látigo Cepa", "Fouet Lianes");
            RegisterMoveName("Razor Leaf", "Razor Leaf", "Hoja Afilada", "Tranch'Herbe");
            RegisterMoveName("Solar Beam", "Solar Beam", "Rayo Solar", "Lance-Soleil");

            // Normal moves
            RegisterMoveName("Tackle", "Tackle", "Placaje", "Charge");
            RegisterMoveName("Scratch", "Scratch", "Arañazo", "Griffe");
            RegisterMoveName("Quick Attack", "Quick Attack", "Ataque Rápido", "Vive-Attaque");
            RegisterMoveName("Hyper Beam", "Hyper Beam", "Hiperrayo", "Ultralaser");
            RegisterMoveName("Growl", "Growl", "Gruñido", "Rugissement");
            RegisterMoveName("Defense Curl", "Defense Curl", "Rizo Defensa", "Boul'Armure");
            RegisterMoveName("Splash", "Splash", "Salpicadura", "Trempette");

            // Water moves (additional)
            RegisterMoveName("Waterfall", "Waterfall", "Cascada", "Cascade");

            // Rock moves
            RegisterMoveName("Rock Throw", "Rock Throw", "Lanzarrocas", "Jet-Pierres");
            RegisterMoveName("Rock Slide", "Rock Slide", "Avalancha", "Éboulement");

            // Psychic moves
            RegisterMoveName("Psychic", "Psychic", "Psíquico", "Psykokwak");
            RegisterMoveName("Teleport", "Teleport", "Teletransporte", "Téléport");
            RegisterMoveName("Confusion", "Confusion", "Confusión", "Choc Mental");
            RegisterMoveName("Psybeam", "Psybeam", "Psicorrayo", "Rafale Psy");
            RegisterMoveName("Hypnosis", "Hypnosis", "Hipnosis", "Hypnose");

            // Poison moves
            RegisterMoveName("Poison Sting", "Poison Sting", "Picotazo Veneno", "Dard-Venin");
            RegisterMoveName("Sludge Bomb", "Sludge Bomb", "Bomba Lodo", "Bomb-Beurk");

            // Ground moves
            RegisterMoveName("Earthquake", "Earthquake", "Terremoto", "Séisme");

            // Ghost moves
            RegisterMoveName("Lick", "Lick", "Lengüetazo", "Léchouille");
            RegisterMoveName("Shadow Ball", "Shadow Ball", "Bola Sombra", "Ball'Ombre");

            // Flying moves
            RegisterMoveName("Wing Attack", "Wing Attack", "Ataque Ala", "Cru-Aile");
            RegisterMoveName("Fly", "Fly", "Volar", "Vol");

            // Dragon moves
            RegisterMoveName("Dragon Rage", "Dragon Rage", "Furia Dragón", "Colère Dragon");
        }

        private static void InitializeMoveCategories()
        {
            Register(LocalizationKey.MoveCategory_Physical, new Dictionary<string, string>
            {
                { "en", "Physical" },
                { "es", "Físico" },
                { "fr", "Physique" }
            });

            Register(LocalizationKey.MoveCategory_Special, new Dictionary<string, string>
            {
                { "en", "Special" },
                { "es", "Especial" },
                { "fr", "Spécial" }
            });

            Register(LocalizationKey.MoveCategory_Status, new Dictionary<string, string>
            {
                { "en", "Status" },
                { "es", "Estado" },
                { "fr", "Statut" }
            });
        }
    }
}
