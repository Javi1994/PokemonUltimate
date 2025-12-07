using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.Content.Providers
{
    /// <summary>
    /// Partial class for status effect name and description translations.
    /// </summary>
    public static partial class LocalizationDataProvider
    {
        private static void InitializeStatusEffectNames()
        {
            // Status effect names (some already exist, but adding descriptions)
            RegisterStatusEffectName("Burn", "Burn", "Quemadura", "Brûlure");
            RegisterStatusEffectName("Paralysis", "Paralysis", "Parálisis", "Paralysie");
            RegisterStatusEffectName("Sleep", "Sleep", "Sueño", "Sommeil");
            RegisterStatusEffectName("Poison", "Poison", "Veneno", "Poison");
            RegisterStatusEffectName("Badly Poisoned", "Badly Poisoned", "Envenenado Grave", "Gravement Empoisonné");
            RegisterStatusEffectName("Freeze", "Freeze", "Congelación", "Gel");
            RegisterStatusEffectName("Confusion", "Confusion", "Confusión", "Confusion");
            RegisterStatusEffectName("Attract", "Attract", "Atracción", "Attraction");
            RegisterStatusEffectName("Flinch", "Flinch", "Retroceso", "Recul");
            RegisterStatusEffectName("Leech Seed", "Leech Seed", "Drenadoras", "Vampigraine");
            RegisterStatusEffectName("Curse", "Curse", "Maldición", "Malédiction");
            RegisterStatusEffectName("Encore", "Encore", "Danza Amiga", "Encore");
            RegisterStatusEffectName("Taunt", "Taunt", "Mofa", "Provoc");
            RegisterStatusEffectName("Torment", "Torment", "Tormento", "Tourmente");
            RegisterStatusEffectName("Disable", "Disable", "Anulación", "Entrave");

            // Status effect descriptions
            RegisterStatusEffectDescription("Burn", "The Pokémon is burned. It takes damage each turn and its Attack is halved.",
                "El Pokémon está quemado. Recibe daño cada turno y su Ataque se reduce a la mitad.",
                "Le Pokémon est brûlé. Il subit des dégâts à chaque tour et son Attaque est réduite de moitié.");
            RegisterStatusEffectDescription("Paralysis", "The Pokémon is paralyzed. It may be unable to move and its Speed is halved.",
                "El Pokémon está paralizado. Puede ser incapaz de moverse y su Velocidad se reduce a la mitad.",
                "Le Pokémon est paralysé. Il peut être incapable de bouger et sa Vitesse est réduite de moitié.");
            RegisterStatusEffectDescription("Sleep", "The Pokémon is asleep and cannot move.",
                "El Pokémon está dormido y no puede moverse.",
                "Le Pokémon est endormi et ne peut pas bouger.");
            RegisterStatusEffectDescription("Poison", "The Pokémon is poisoned and takes damage each turn.",
                "El Pokémon está envenenado y recibe daño cada turno.",
                "Le Pokémon est empoisonné et subit des dégâts à chaque tour.");
            RegisterStatusEffectDescription("Badly Poisoned", "The Pokémon is badly poisoned. Damage increases each turn.",
                "El Pokémon está gravemente envenenado. El daño aumenta cada turno.",
                "Le Pokémon est gravement empoisonné. Les dégâts augmentent à chaque tour.");
            RegisterStatusEffectDescription("Freeze", "The Pokémon is frozen solid and cannot move.",
                "El Pokémon está congelado y no puede moverse.",
                "Le Pokémon est gelé et ne peut pas bouger.");
            RegisterStatusEffectDescription("Confusion", "The Pokémon is confused and may hurt itself.",
                "El Pokémon está confundido y puede lastimarse a sí mismo.",
                "Le Pokémon est confus et peut se blesser lui-même.");
            RegisterStatusEffectDescription("Attract", "The Pokémon is infatuated and may be unable to attack.",
                "El Pokémon está enamorado y puede ser incapaz de atacar.",
                "Le Pokémon est amoureux et peut être incapable d'attaquer.");
            RegisterStatusEffectDescription("Flinch", "The Pokémon flinched and couldn't move.",
                "El Pokémon retrocedió y no pudo moverse.",
                "Le Pokémon a reculé et n'a pas pu bouger.");
            RegisterStatusEffectDescription("Leech Seed", "The Pokémon's HP is drained to the opponent each turn.",
                "Los PS del Pokémon se drenan al oponente cada turno.",
                "Les PV du Pokémon sont drainés vers l'adversaire à chaque tour.");
            RegisterStatusEffectDescription("Curse", "The Pokémon is cursed and loses HP each turn.",
                "El Pokémon está maldito y pierde PS cada turno.",
                "Le Pokémon est maudit et perd des PV à chaque tour.");
            RegisterStatusEffectDescription("Encore", "The Pokémon is forced to repeat its last move.",
                "El Pokémon está obligado a repetir su último movimiento.",
                "Le Pokémon est forcé de répéter sa dernière capacité.");
            RegisterStatusEffectDescription("Taunt", "The Pokémon cannot use status moves.",
                "El Pokémon no puede usar movimientos de estado.",
                "Le Pokémon ne peut pas utiliser de capacités de statut.");
            RegisterStatusEffectDescription("Torment", "The Pokémon cannot use the same move consecutively.",
                "El Pokémon no puede usar el mismo movimiento consecutivamente.",
                "Le Pokémon ne peut pas utiliser la même capacité consécutivement.");
            RegisterStatusEffectDescription("Disable", "One of the Pokémon's moves is disabled.",
                "Uno de los movimientos del Pokémon está deshabilitado.",
                "Une des capacités du Pokémon est désactivée.");
        }

        private static void InitializeUI()
        {
        }
    }
}
