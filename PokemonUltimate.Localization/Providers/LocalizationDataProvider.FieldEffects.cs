using PokemonUltimate.Localization.Constants;

namespace PokemonUltimate.Localization.Providers
{
    /// <summary>
    /// Partial class for field effect name and description translations.
    /// </summary>
    public static partial class LocalizationDataProvider
    {
        private static void InitializeFieldEffectNames()
        {
            // Field effect names
            RegisterFieldEffectName("Trick Room", "Trick Room", "Espacio Raro", "Distorsion");
            RegisterFieldEffectName("Magic Room", "Magic Room", "Espacio Mágico", "Zone Magique");
            RegisterFieldEffectName("Wonder Room", "Wonder Room", "Espacio Asombroso", "Zone Étrange");
            RegisterFieldEffectName("Gravity", "Gravity", "Gravedad", "Gravité");
            RegisterFieldEffectName("Ion Deluge", "Ion Deluge", "Cortina de Iones", "Déluge Plasmique");
            RegisterFieldEffectName("Fairy Lock", "Fairy Lock", "Cerrojo Feérico", "Verrou Enchanté");
            RegisterFieldEffectName("Mud Sport", "Mud Sport", "Rizo Lodo", "Lance-Boue");
            RegisterFieldEffectName("Water Sport", "Water Sport", "Rizo Agua", "Pistolet à O");

            // Field effect descriptions
            RegisterFieldEffectDescription("Trick Room", "A bizarre area where slower Pokemon move first.",
                "Un área extraña donde los Pokémon más lentos se mueven primero.",
                "Une zone bizarre où les Pokémon les plus lents se déplacent en premier.");
            RegisterFieldEffectDescription("Magic Room", "A bizarre area where held items lose their effects.",
                "Un área extraña donde los objetos equipados pierden sus efectos.",
                "Une zone bizarre où les objets tenus perdent leurs effets.");
            RegisterFieldEffectDescription("Wonder Room", "A bizarre area where Defense and Sp. Def are swapped.",
                "Un área extraña donde la Defensa y la Def. Esp. se intercambian.",
                "Une zone bizarre où la Défense et la Déf. Spé. sont échangées.");
            RegisterFieldEffectDescription("Gravity", "Intense gravity grounds all Pokemon and prevents flying moves.",
                "Una gravedad intensa hace que todos los Pokémon toquen el suelo y previene movimientos voladores.",
                "Une gravité intense fait atterrir tous les Pokémon et empêche les capacités volantes.");
            RegisterFieldEffectDescription("Ion Deluge", "Electric charge changes Normal moves to Electric type.",
                "Una carga eléctrica cambia los movimientos Normales a tipo Eléctrico.",
                "Une charge électrique change les capacités Normales en type Électrik.");
            RegisterFieldEffectDescription("Fairy Lock", "Locks down the battlefield, preventing escape.",
                "Bloquea el campo de batalla, impidiendo la huida.",
                "Verrouille le champ de bataille, empêchant la fuite.");
            RegisterFieldEffectDescription("Mud Sport", "Weakens Electric-type moves while in effect.",
                "Debilita los movimientos de tipo Eléctrico mientras está en efecto.",
                "Affaiblit les capacités de type Électrik tant qu'elle est active.");
            RegisterFieldEffectDescription("Water Sport", "Weakens Fire-type moves while in effect.",
                "Debilita los movimientos de tipo Fuego mientras está en efecto.",
                "Affaiblit les capacités de type Feu tant qu'elle est active.");
        }
    }
}
