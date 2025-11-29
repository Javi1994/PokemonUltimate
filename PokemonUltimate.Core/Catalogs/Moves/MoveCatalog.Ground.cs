using PokemonUltimate.Core.Builders;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Models;

namespace PokemonUltimate.Core.Catalogs
{
    /// <summary>
    /// Ground-type moves.
    /// </summary>
    public static partial class MoveCatalog
    {
        public static readonly MoveData Earthquake = Move.Define("Earthquake")
            .Description("The user sets off an earthquake that strikes every Pokémon around it.")
            .Type(PokemonType.Ground)
            .Physical(100, 100, 10)
            .Target(TargetScope.AllOthers)
            .WithEffects(e => e.Damage())
            .Build();

        static partial void RegisterGround()
        {
            _all.Add(Earthquake);
        }
    }
}
