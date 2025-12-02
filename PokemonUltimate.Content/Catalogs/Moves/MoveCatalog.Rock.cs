using PokemonUltimate.Content.Builders;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Catalogs.Moves
{
    /// <summary>
    /// Rock-type moves.
    /// </summary>
    public static partial class MoveCatalog
    {
        public static readonly MoveData RockThrow = Move.Define("Rock Throw")
            .Description("The user picks up and throws a small rock at the target to attack.")
            .Type(PokemonType.Rock)
            .Physical(50, 90, 15)
            .WithEffects(e => e.Damage())
            .Build();

        public static readonly MoveData RockSlide = Move.Define("Rock Slide")
            .Description("Large boulders are hurled at opposing Pokémon. May also make them flinch.")
            .Type(PokemonType.Rock)
            .Physical(75, 90, 10)
            .TargetAllEnemies()
            .WithEffects(e => e
                .Damage()
                .MayFlinch(30))
            .Build();

        static partial void RegisterRock()
        {
            _all.Add(RockThrow);
            _all.Add(RockSlide);
        }
    }
}

