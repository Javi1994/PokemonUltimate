using PokemonUltimate.Content.Builders;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Catalogs.Moves
{
    /// <summary>
    /// Flying-type moves.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.2: Move Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.2-move-expansion/architecture.md`
    /// </remarks>
    public static partial class MoveCatalog
    {
        public static readonly MoveData WingAttack = Move.Define("Wing Attack")
            .Description("The target is struck with large, imposing wings spread wide to inflict damage.")
            .Type(PokemonType.Flying)
            .Physical(60, 100, 35)
            .WithEffects(e => e.Damage())
            .Build();

        public static readonly MoveData Fly = Move.Define("Fly")
            .Description("The user soars and then strikes its target on the next turn.")
            .Type(PokemonType.Flying)
            .Physical(90, 95, 15)
            .WithEffects(e => e.Damage())
            .Build();

        static partial void RegisterFlying()
        {
            _all.Add(WingAttack);
            _all.Add(Fly);
        }
    }
}

