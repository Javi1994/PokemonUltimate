using PokemonUltimate.Localization.Constants;

namespace PokemonUltimate.Localization.Providers
{
    /// <summary>
    /// Partial class for item name and description translations.
    /// </summary>
    public static partial class LocalizationDataProvider
    {
        private static void InitializeItemNames()
        {
            // Held Items
            RegisterItemName("Leftovers", "Leftovers", "Restos", "Restes");
            RegisterItemName("Black Sludge", "Black Sludge", "Lodo Negro", "Glaçon Noir");
            RegisterItemName("Choice Band", "Choice Band", "Cinta Elegida", "Bandeau Choix");
            RegisterItemName("Choice Specs", "Choice Specs", "Gafas Elegidas", "Lunettes Choix");
            RegisterItemName("Choice Scarf", "Choice Scarf", "Pañuelo Elegido", "Mouchoir Choix");
            RegisterItemName("Life Orb", "Life Orb", "Bola Vida", "Orbe Vie");
            RegisterItemName("Expert Belt", "Expert Belt", "Cinta Experta", "Ceinture Pro");
            RegisterItemName("Charcoal", "Charcoal", "Carbón", "Charbon");
            RegisterItemName("Mystic Water", "Mystic Water", "Agua Mística", "Eau Mystique");
            RegisterItemName("Magnet", "Magnet", "Imán", "Aimant");
            RegisterItemName("Miracle Seed", "Miracle Seed", "Semilla Milagro", "Graine Miracle");
            RegisterItemName("Focus Sash", "Focus Sash", "Banda Concentración", "Ceinture Force");
            RegisterItemName("Eviolite", "Eviolite", "Mineral Evolución", "Évoluroche");
            RegisterItemName("Assault Vest", "Assault Vest", "Chal Salvavidas", "Mousse Protect");
            RegisterItemName("Rocky Helmet", "Rocky Helmet", "Casco Pétreo", "Casque Brut");

            // Berries
            RegisterItemName("Oran Berry", "Oran Berry", "Baya Zarzamora", "Baie Framby");
            RegisterItemName("Sitrus Berry", "Sitrus Berry", "Baya Zidra", "Baie Siam");
            RegisterItemName("Cheri Berry", "Cheri Berry", "Baya Cereza", "Baie Ceriz");
            RegisterItemName("Chesto Berry", "Chesto Berry", "Baya Caquic", "Baie Ceriz");
            RegisterItemName("Pecha Berry", "Pecha Berry", "Baya Melocotón", "Baie Pêcha");
            RegisterItemName("Rawst Berry", "Rawst Berry", "Baya Pera", "Baie Fraive");
            RegisterItemName("Aspear Berry", "Aspear Berry", "Baya Peraguac", "Baie Willia");
            RegisterItemName("Lum Berry", "Lum Berry", "Baya Zrezal", "Baie Prine");
        }

        private static void InitializeItemDescriptions()
        {
            // Held Items descriptions
            RegisterItemDescription("Leftovers", "Restores HP each turn.",
                "Restaura PS cada turno.",
                "Restaure des PV à chaque tour.");
            RegisterItemDescription("Black Sludge", "Restores HP for Poison types; damages others.",
                "Restaura PS para tipos Veneno; daña a otros.",
                "Restaure des PV pour les types Poison ; blesse les autres.");
            RegisterItemDescription("Choice Band", "Boosts Attack but locks to one move.",
                "Potencia el Ataque pero bloquea a un movimiento.",
                "Augmente l'Attaque mais bloque sur une capacité.");
            RegisterItemDescription("Choice Specs", "Boosts Sp. Attack but locks to one move.",
                "Potencia el At. Esp. pero bloquea a un movimiento.",
                "Augmente l'Atq. Spé. mais bloque sur une capacité.");
            RegisterItemDescription("Choice Scarf", "Boosts Speed but locks to one move.",
                "Potencia la Velocidad pero bloquea a un movimiento.",
                "Augmente la Vitesse mais bloque sur une capacité.");
            RegisterItemDescription("Life Orb", "Boosts damage but causes recoil.",
                "Potencia el daño pero causa retroceso.",
                "Augmente les dégâts mais cause des dégâts de recul.");
            RegisterItemDescription("Expert Belt", "Boosts super effective moves.",
                "Potencia los movimientos súper efectivos.",
                "Augmente les capacités super efficaces.");
            RegisterItemDescription("Charcoal", "Boosts Fire-type moves.",
                "Potencia los movimientos de tipo Fuego.",
                "Augmente les capacités de type Feu.");
            RegisterItemDescription("Mystic Water", "Boosts Water-type moves.",
                "Potencia los movimientos de tipo Agua.",
                "Augmente les capacités de type Eau.");
            RegisterItemDescription("Magnet", "Boosts Electric-type moves.",
                "Potencia los movimientos de tipo Eléctrico.",
                "Augmente les capacités de type Électrik.");
            RegisterItemDescription("Miracle Seed", "Boosts Grass-type moves.",
                "Potencia los movimientos de tipo Planta.",
                "Augmente les capacités de type Plante.");
            RegisterItemDescription("Focus Sash", "Survives one fatal hit at full HP.",
                "Sobrevive a un golpe fatal con PS completos.",
                "Survit à un coup fatal avec les PV au maximum.");
            RegisterItemDescription("Eviolite", "Boosts defenses of unevolved Pokémon.",
                "Potencia las defensas de Pokémon no evolucionados.",
                "Augmente les défenses des Pokémon non évolués.");
            RegisterItemDescription("Assault Vest", "Boosts Sp. Def but prevents status moves.",
                "Potencia la Def. Esp. pero previene movimientos de estado.",
                "Augmente la Déf. Spé. mais empêche les capacités de statut.");
            RegisterItemDescription("Rocky Helmet", "Damages attackers on contact.",
                "Daña a los atacantes al hacer contacto.",
                "Blesse les attaquants au contact.");

            // Berries descriptions
            RegisterItemDescription("Oran Berry", "Restores 10 HP when below half.",
                "Restaura 10 PS cuando está por debajo de la mitad.",
                "Restaure 10 PV lorsqu'il est en dessous de la moitié.");
            RegisterItemDescription("Sitrus Berry", "Restores 25% HP when below quarter.",
                "Restaura 25% de PS cuando está por debajo de un cuarto.",
                "Restaure 25% des PV lorsqu'il est en dessous d'un quart.");
            RegisterItemDescription("Cheri Berry", "Cures paralysis.",
                "Cura la parálisis.",
                "Guérit la paralysie.");
            RegisterItemDescription("Chesto Berry", "Cures sleep.",
                "Cura el sueño.",
                "Guérit le sommeil.");
            RegisterItemDescription("Pecha Berry", "Cures poison.",
                "Cura el veneno.",
                "Guérit le poison.");
            RegisterItemDescription("Rawst Berry", "Cures burn.",
                "Cura la quemadura.",
                "Guérit la brûlure.");
            RegisterItemDescription("Aspear Berry", "Cures freeze.",
                "Cura la congelación.",
                "Guérit le gel.");
            RegisterItemDescription("Lum Berry", "Cures any status condition.",
                "Cura cualquier condición de estado.",
                "Guérit toute altération de statut.");
        }
    }
}
