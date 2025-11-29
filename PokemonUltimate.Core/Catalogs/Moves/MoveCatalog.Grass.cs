using PokemonUltimate.Core.Data;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Catalogs
{
    /// <summary>
    /// Grass-type moves.
    /// </summary>
    public static partial class MoveCatalog
    {
        public static readonly MoveData VineWhip = new MoveData
        {
            Name = "Vine Whip",
            Description = "The target is struck with slender, whiplike vines to inflict damage.",
            Type = PokemonType.Grass,
            Category = MoveCategory.Physical,
            Power = 45,
            Accuracy = 100,
            MaxPP = 25,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = { new DamageEffect() }
        };

        public static readonly MoveData RazorLeaf = new MoveData
        {
            Name = "Razor Leaf",
            Description = "Sharp-edged leaves are launched to slash at opposing Pokémon. Critical hits land more easily.",
            Type = PokemonType.Grass,
            Category = MoveCategory.Physical,
            Power = 55,
            Accuracy = 95,
            MaxPP = 25,
            Priority = 0,
            TargetScope = TargetScope.AllEnemies,
            Effects = { new DamageEffect { CritStages = 1 } }
        };

        public static readonly MoveData SolarBeam = new MoveData
        {
            Name = "Solar Beam",
            Description = "In this two-turn attack, the user gathers light, then blasts a bundled beam on the next turn.",
            Type = PokemonType.Grass,
            Category = MoveCategory.Special,
            Power = 120,
            Accuracy = 100,
            MaxPP = 10,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = { new DamageEffect() }
        };

        static partial void RegisterGrass()
        {
            _all.Add(VineWhip);
            _all.Add(RazorLeaf);
            _all.Add(SolarBeam);
        }
    }
}

