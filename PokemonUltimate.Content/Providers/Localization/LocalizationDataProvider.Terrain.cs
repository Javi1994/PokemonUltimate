using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.Content.Providers
{
    /// <summary>
    /// Partial class for terrain name and description translations.
    /// </summary>
    public static partial class LocalizationDataProvider
    {
        private static void InitializeTerrainNames()
        {
            // Terrain names
            RegisterTerrainName("Grassy Terrain", "Grassy Terrain", "Campo de Hierba", "Champ Herbu");
            RegisterTerrainName("Electric Terrain", "Electric Terrain", "Campo Eléctrico", "Champ Électrifié");
            RegisterTerrainName("Psychic Terrain", "Psychic Terrain", "Campo Psíquico", "Champ Psychique");
            RegisterTerrainName("Misty Terrain", "Misty Terrain", "Campo de Niebla", "Champ Brumeux");

            // Terrain descriptions
            RegisterTerrainDescription("Grassy Terrain", "Grass grows on the battlefield. Grass moves are boosted and grounded Pokemon heal each turn.",
                "La hierba crece en el campo de batalla. Los movimientos de Planta se potencian y los Pokémon en el suelo se curan cada turno.",
                "L'herbe pousse sur le champ de bataille. Les capacités Plante sont renforcées et les Pokémon au sol sont soignés à chaque tour.");
            RegisterTerrainDescription("Electric Terrain", "Electricity crackles on the ground. Electric moves are boosted and grounded Pokemon cannot fall asleep.",
                "La electricidad cruje en el suelo. Los movimientos Eléctricos se potencian y los Pokémon en el suelo no pueden quedarse dormidos.",
                "L'électricité crépite sur le sol. Les capacités Électrik sont renforcées et les Pokémon au sol ne peuvent pas s'endormir.");
            RegisterTerrainDescription("Psychic Terrain", "Psychic energy covers the ground. Psychic moves are boosted and grounded Pokemon are protected from priority moves.",
                "La energía psíquica cubre el suelo. Los movimientos Psíquicos se potencian y los Pokémon en el suelo están protegidos de movimientos prioritarios.",
                "L'énergie psychique couvre le sol. Les capacités Psy sont renforcées et les Pokémon au sol sont protégés des capacités prioritaires.");
            RegisterTerrainDescription("Misty Terrain", "Mist covers the ground. Grounded Pokemon are protected from status conditions and Dragon damage is halved.",
                "La niebla cubre el suelo. Los Pokémon en el suelo están protegidos de condiciones de estado y el daño Dragón se reduce a la mitad.",
                "La brume couvre le sol. Les Pokémon au sol sont protégés des altérations de statut et les dégâts Dragon sont réduits de moitié.");
        }
    }
}
