using PokemonUltimate.Core.Builders;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Models;

namespace PokemonUltimate.Core.Catalogs
{
    /// <summary>
    /// Fire-type moves.
    /// </summary>
    public static partial class MoveCatalog
    {
        public static readonly MoveData Ember = Move.Define("Ember")
            .Description("The target is attacked with small flames. May also leave the target with a burn.")
            .Type(PokemonType.Fire)
            .Special(40, 100, 25)
            .WithEffects(e => e
                .Damage()
                .MayBurn(10))
            .Build();

        public static readonly MoveData Flamethrower = Move.Define("Flamethrower")
            .Description("The target is scorched with an intense blast of fire. May also leave the target with a burn.")
            .Type(PokemonType.Fire)
            .Special(90, 100, 15)
            .WithEffects(e => e
                .Damage()
                .MayBurn(10))
            .Build();

        public static readonly MoveData FireBlast = Move.Define("Fire Blast")
            .Description("The target is attacked with an intense blast of all-consuming fire. May also burn.")
            .Type(PokemonType.Fire)
            .Special(110, 85, 5)
            .WithEffects(e => e
                .Damage()
                .MayBurn(10))
            .Build();

        static partial void RegisterFire()
        {
            _all.Add(Ember);
            _all.Add(Flamethrower);
            _all.Add(FireBlast);
        }
    }
}
