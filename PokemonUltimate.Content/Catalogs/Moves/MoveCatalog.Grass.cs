using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Infrastructure.Builders;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Content.Catalogs.Moves
{
    /// <summary>
    /// Grass-type moves.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.2: Move Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.2-move-expansion/architecture.md`
    /// </remarks>
    public static partial class MoveCatalog
    {
        public static readonly MoveData VineWhip = Move.Define("Vine Whip")
            .Description("The target is struck with slender, whiplike vines to inflict damage.")
            .Type(PokemonType.Grass)
            .Physical(45, 100, 25)
            .WithEffects(e => e.Damage())
            .Build();

        public static readonly MoveData RazorLeaf = Move.Define("Razor Leaf")
            .Description("Sharp-edged leaves are launched to slash at opposing Pokémon. Critical hits land more easily.")
            .Type(PokemonType.Grass)
            .Physical(55, 95, 25)
            .TargetAllEnemies()
            .WithEffects(e => e.DamageHighCrit())
            .Build();

        public static readonly MoveData SolarBeam = Move.Define("Solar Beam")
            .Description("In this two-turn attack, the user gathers light, then blasts a bundled beam on the next turn.")
            .Type(PokemonType.Grass)
            .Special(120, 100, 10)
            .WithEffects(e => e.Damage())
            .Build();

        static partial void RegisterGrass()
        {
            _all.Add(VineWhip);
            _all.Add(RazorLeaf);
            _all.Add(SolarBeam);
        }
    }
}
