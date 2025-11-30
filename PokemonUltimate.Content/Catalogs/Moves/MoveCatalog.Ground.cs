using PokemonUltimate.Content.Builders;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Catalogs.Moves
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
