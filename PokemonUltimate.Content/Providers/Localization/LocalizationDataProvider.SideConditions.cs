using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.Content.Providers
{
    /// <summary>
    /// Partial class for side condition name and description translations.
    /// </summary>
    public static partial class LocalizationDataProvider
    {
        private static void InitializeSideConditionNames()
        {
            // Side condition names
            RegisterSideConditionName("Reflect", "Reflect", "Reflejo", "Mur Lumière");
            RegisterSideConditionName("Light Screen", "Light Screen", "Pantalla de Luz", "Voile Lumière");
            RegisterSideConditionName("Aurora Veil", "Aurora Veil", "Velo Aurora", "Voile Aurore");
            RegisterSideConditionName("Tailwind", "Tailwind", "Viento Afín", "Vent Arrière");
            RegisterSideConditionName("Safeguard", "Safeguard", "Velo Sagrado", "Rune Protect");
            RegisterSideConditionName("Mist", "Mist", "Neblina", "Brume");
            RegisterSideConditionName("Lucky Chant", "Lucky Chant", "Canto Vital", "Chant Chance");
            RegisterSideConditionName("Wide Guard", "Wide Guard", "Guardia Ampla", "Garde Large");
            RegisterSideConditionName("Quick Guard", "Quick Guard", "Guardia Rápida", "Garde Rapide");
            RegisterSideConditionName("Mat Block", "Mat Block", "Escudo Tatami", "Bloquage Tatami");

            // Side condition descriptions
            RegisterSideConditionDescription("Reflect", "A wondrous wall of light reduces physical damage.",
                "Una maravillosa pared de luz reduce el daño físico.",
                "Un mur de lumière merveilleux réduit les dégâts physiques.");
            RegisterSideConditionDescription("Light Screen", "A wondrous wall of light reduces special damage.",
                "Una maravillosa pared de luz reduce el daño especial.",
                "Un mur de lumière merveilleux réduit les dégâts spéciaux.");
            RegisterSideConditionDescription("Aurora Veil", "A veil of light reduces damage from physical and special moves.",
                "Un velo de luz reduce el daño de movimientos físicos y especiales.",
                "Un voile de lumière réduit les dégâts des capacités physiques et spéciales.");
            RegisterSideConditionDescription("Tailwind", "A turbulent wind blows, boosting the Speed of the team.",
                "Sopla un viento turbulento que aumenta la Velocidad del equipo.",
                "Un vent turbulent souffle, augmentant la Vitesse de l'équipe.");
            RegisterSideConditionDescription("Safeguard", "A mystical veil prevents status conditions.",
                "Un velo místico previene las condiciones de estado.",
                "Un voile mystique empêche les altérations de statut.");
            RegisterSideConditionDescription("Mist", "A mist prevents the team's stats from being lowered.",
                "Una neblina previene que las estadísticas del equipo se reduzcan.",
                "Une brume empêche les stats de l'équipe d'être réduites.");
            RegisterSideConditionDescription("Lucky Chant", "A lucky chant prevents the opposing team from landing critical hits.",
                "Un canto de la suerte previene que el equipo rival aseste golpes críticos.",
                "Un chant chanceux empêche l'équipe adverse de porter des coups critiques.");
            RegisterSideConditionDescription("Wide Guard", "Protects the team from wide-ranging attacks.",
                "Protege al equipo de ataques de amplio alcance.",
                "Protège l'équipe des attaques à large portée.");
            RegisterSideConditionDescription("Quick Guard", "Protects the team from priority moves.",
                "Protege al equipo de movimientos prioritarios.",
                "Protège l'équipe des capacités prioritaires.");
            RegisterSideConditionDescription("Mat Block", "Blocks damaging moves with a mat. Only works on first turn.",
                "Bloquea movimientos dañinos con una estera. Solo funciona en el primer turno.",
                "Bloque les capacités offensives avec un tatami. Ne fonctionne qu'au premier tour.");
        }
    }
}
