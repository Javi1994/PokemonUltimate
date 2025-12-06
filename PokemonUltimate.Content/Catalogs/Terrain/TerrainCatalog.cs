using System.Collections.Generic;
using PokemonUltimate.Content.Builders;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Catalogs.Terrain
{
    /// <summary>
    /// Central catalog of all terrain condition definitions.
    /// All terrains last 5 turns by default (8 with Terrain Extender item).
    /// Terrains only affect "grounded" Pokemon.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.6: Field Conditions Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.6-field-conditions-expansion/README.md`
    /// </remarks>
    public static class TerrainCatalog
    {
        private static readonly List<TerrainData> _all = new List<TerrainData>();

        /// <summary>
        /// Gets all registered terrains.
        /// </summary>
        public static IReadOnlyList<TerrainData> All => _all;

        #region Terrain Definitions

        /// <summary>
        /// Grassy Terrain (Gen 6+):
        /// - Boosts Grass moves by 1.3x (Gen 7+) / 1.5x (Gen 6)
        /// - Heals grounded Pokemon 1/16 HP at end of turn
        /// - Halves Earthquake, Bulldoze, and Magnitude damage
        /// - Nature Power becomes Energy Ball
        /// </summary>
        public static readonly TerrainData Grassy = TerrainEffect.Define("Grassy Terrain")
            .Description("Grass grows on the battlefield. Grass moves are boosted and grounded Pokemon heal each turn.")
            .Type(Core.Enums.Terrain.Grassy)
            .Duration(5)
            .Boosts(PokemonType.Grass, 1.3f)
            .HealsEachTurn(1f / 16f)
            .HalvesDamageFrom("Earthquake", "Bulldoze", "Magnitude")
            .NaturePowerBecomes("Energy Ball")
            .CamouflageChangesTo(PokemonType.Grass)
            .SecretPowerCauses("Sleep")
            .SetByAbilities("Grassy Surge")
            .BenefitsAbilities("Grass Pelt")
            .Build();

        /// <summary>
        /// Electric Terrain (Gen 6+):
        /// - Boosts Electric moves by 1.3x (Gen 7+) / 1.5x (Gen 6)
        /// - Prevents Sleep for grounded Pokemon
        /// - Nature Power becomes Thunderbolt
        /// </summary>
        public static readonly TerrainData Electric = TerrainEffect.Define("Electric Terrain")
            .Description("Electricity crackles on the ground. Electric moves are boosted and grounded Pokemon cannot fall asleep.")
            .Type(Core.Enums.Terrain.Electric)
            .Duration(5)
            .Boosts(PokemonType.Electric, 1.3f)
            .PreventsSleep()
            .NaturePowerBecomes("Thunderbolt")
            .CamouflageChangesTo(PokemonType.Electric)
            .SecretPowerCauses("Paralysis")
            .SetByAbilities("Electric Surge")
            .BenefitsAbilities("Surge Surfer")
            .Build();

        /// <summary>
        /// Psychic Terrain (Gen 7+):
        /// - Boosts Psychic moves by 1.3x
        /// - Blocks priority moves against grounded Pokemon
        /// - Nature Power becomes Psychic
        /// </summary>
        public static readonly TerrainData Psychic = TerrainEffect.Define("Psychic Terrain")
            .Description("Psychic energy covers the ground. Psychic moves are boosted and grounded Pokemon are protected from priority moves.")
            .Type(Core.Enums.Terrain.Psychic)
            .Duration(5)
            .Boosts(PokemonType.Psychic, 1.3f)
            .BlocksPriorityMoves()
            .NaturePowerBecomes("Psychic")
            .CamouflageChangesTo(PokemonType.Psychic)
            .SecretPowerCauses("SpeedDrop")
            .SetByAbilities("Psychic Surge")
            .Build();

        /// <summary>
        /// Misty Terrain (Gen 6+):
        /// - Halves Dragon-type damage to grounded Pokemon
        /// - Prevents all status conditions for grounded Pokemon
        /// - Nature Power becomes Moonblast
        /// </summary>
        public static readonly TerrainData Misty = TerrainEffect.Define("Misty Terrain")
            .Description("Mist covers the ground. Grounded Pokemon are protected from status conditions and Dragon damage is halved.")
            .Type(Core.Enums.Terrain.Misty)
            .Duration(5)
            .ReducesDamageFrom(PokemonType.Dragon, 0.5f)
            .PreventsAllStatuses()
            .NaturePowerBecomes("Moonblast")
            .CamouflageChangesTo(PokemonType.Fairy)
            .SecretPowerCauses("SpAttackDrop")
            .SetByAbilities("Misty Surge")
            .Build();

        #endregion

        #region Lookup Methods

        /// <summary>
        /// Gets terrain data by terrain enum.
        /// </summary>
        public static TerrainData GetByTerrain(Core.Enums.Terrain terrain)
        {
            switch (terrain)
            {
                case Core.Enums.Terrain.Grassy: return Grassy;
                case Core.Enums.Terrain.Electric: return Electric;
                case Core.Enums.Terrain.Psychic: return Psychic;
                case Core.Enums.Terrain.Misty: return Misty;
                default: return null;
            }
        }

        /// <summary>
        /// Gets terrain data by name.
        /// </summary>
        public static TerrainData GetByName(string name)
        {
            return _all.Find(t => t.Name == name);
        }

        /// <summary>
        /// Gets all terrains that boost a specific type.
        /// </summary>
        public static IEnumerable<TerrainData> GetTerrainsByBoostedType(PokemonType type)
        {
            foreach (var terrain in _all)
            {
                if (terrain.BoostedType == type) yield return terrain;
            }
        }

        /// <summary>
        /// Gets all terrains that prevent status conditions.
        /// </summary>
        public static IEnumerable<TerrainData> GetStatusPreventingTerrains()
        {
            foreach (var terrain in _all)
            {
                if (terrain.PreventsStatuses) yield return terrain;
            }
        }

        #endregion

        #region Static Constructor

        static TerrainCatalog()
        {
            _all.Add(Grassy);
            _all.Add(Electric);
            _all.Add(Psychic);
            _all.Add(Misty);
        }

        #endregion
    }
}

