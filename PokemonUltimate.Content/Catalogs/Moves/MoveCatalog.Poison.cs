using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Infrastructure.Builders;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Content.Catalogs.Moves
{
    /// <summary>
    /// Poison-type moves.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.2: Move Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.2-move-expansion/architecture.md`
    /// </remarks>
    public static partial class MoveCatalog
    {
        public static readonly MoveData PoisonSting = Move.Define("Poison Sting")
            .Description("The user stabs the target with a poisonous stinger. May also poison the target.")
            .Type(PokemonType.Poison)
            .Physical(15, 100, 35)
            .WithEffects(e => e
                .Damage()
                .MayPoison(30))
            .Build();

        public static readonly MoveData SludgeBomb = Move.Define("Sludge Bomb")
            .Description("Unsanitary sludge is hurled at the target. May also poison the target.")
            .Type(PokemonType.Poison)
            .Special(90, 100, 10)
            .WithEffects(e => e
                .Damage()
                .MayPoison(30))
            .Build();

        static partial void RegisterPoison()
        {
            _all.Add(PoisonSting);
            _all.Add(SludgeBomb);
        }
    }
}

