using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Infrastructure.Builders;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Content.Catalogs.Moves
{
    /// <summary>
    /// Ground-type moves.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.2: Move Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.2-move-expansion/architecture.md`
    /// </remarks>
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
