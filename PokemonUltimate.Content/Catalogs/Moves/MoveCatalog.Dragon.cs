using PokemonUltimate.Content.Builders;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Catalogs.Moves
{
    /// <summary>
    /// Dragon-type moves.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.2: Move Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.2-move-expansion/architecture.md`
    /// </remarks>
    public static partial class MoveCatalog
    {
        public static readonly MoveData DragonRage = Move.Define("Dragon Rage")
            .Description("This attack hits the target with a shock wave of pure rage. This attack always inflicts 40 HP damage.")
            .Type(PokemonType.Dragon)
            .Special(0, 100, 10)
            .WithEffects(e => e.FixedDamage(40))
            .Build();

        static partial void RegisterDragon()
        {
            _all.Add(DragonRage);
        }
    }
}

