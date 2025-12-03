using PokemonUltimate.Content.Builders;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Catalogs.Moves
{
    /// <summary>
    /// Electric-type moves.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.2: Move Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.2-move-expansion/architecture.md`
    /// </remarks>
    public static partial class MoveCatalog
    {
        public static readonly MoveData ThunderShock = Move.Define("Thunder Shock")
            .Description("A jolt of electricity crashes down on the target. May also cause paralysis.")
            .Type(PokemonType.Electric)
            .Special(40, 100, 30)
            .WithEffects(e => e
                .Damage()
                .MayParalyze(10))
            .Build();

        public static readonly MoveData Thunderbolt = Move.Define("Thunderbolt")
            .Description("A strong electric blast crashes down on the target. May also cause paralysis.")
            .Type(PokemonType.Electric)
            .Special(90, 100, 15)
            .WithEffects(e => e
                .Damage()
                .MayParalyze(10))
            .Build();

        public static readonly MoveData Thunder = Move.Define("Thunder")
            .Description("A wicked thunderbolt is dropped on the target. May also cause paralysis.")
            .Type(PokemonType.Electric)
            .Special(110, 70, 10)
            .WithEffects(e => e
                .Damage()
                .MayParalyze(30))
            .Build();

        public static readonly MoveData ThunderWave = Move.Define("Thunder Wave")
            .Description("The user launches a weak jolt of electricity that paralyzes the target.")
            .Type(PokemonType.Electric)
            .Status(90, 20)
            .WithEffects(e => e.MayParalyze())
            .Build();

        static partial void RegisterElectric()
        {
            _all.Add(ThunderShock);
            _all.Add(Thunderbolt);
            _all.Add(Thunder);
            _all.Add(ThunderWave);
        }
    }
}
