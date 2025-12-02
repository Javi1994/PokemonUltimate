using PokemonUltimate.Content.Builders;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Catalogs.Moves
{
    /// <summary>
    /// Ghost-type moves.
    /// </summary>
    public static partial class MoveCatalog
    {
        public static readonly MoveData Lick = Move.Define("Lick")
            .Description("The target is licked with a long tongue, causing damage. May also cause paralysis.")
            .Type(PokemonType.Ghost)
            .Physical(30, 100, 30)
            .WithEffects(e => e
                .Damage()
                .MayParalyze(30))
            .Build();

        public static readonly MoveData ShadowBall = Move.Define("Shadow Ball")
            .Description("The user hurls a shadowy blob at the target. May also lower the target's Sp. Def.")
            .Type(PokemonType.Ghost)
            .Special(80, 100, 15)
            .WithEffects(e => e
                .Damage()
                .LowerSpDefense(1, 20))
            .Build();

        static partial void RegisterGhost()
        {
            _all.Add(Lick);
            _all.Add(ShadowBall);
        }
    }
}

