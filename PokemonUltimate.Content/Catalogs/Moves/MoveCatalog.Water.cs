using PokemonUltimate.Content.Builders;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Catalogs.Moves
{
    /// <summary>
    /// Water-type moves.
    /// </summary>
    public static partial class MoveCatalog
    {
        public static readonly MoveData WaterGun = Move.Define("Water Gun")
            .Description("The target is blasted with a forceful shot of water.")
            .Type(PokemonType.Water)
            .Special(40, 100, 25)
            .WithEffects(e => e.Damage())
            .Build();

        public static readonly MoveData Surf = Move.Define("Surf")
            .Description("The user attacks everything around it by swamping its surroundings with a giant wave.")
            .Type(PokemonType.Water)
            .Special(90, 100, 15)
            .TargetAllEnemies()
            .WithEffects(e => e.Damage())
            .Build();

        public static readonly MoveData HydroPump = Move.Define("Hydro Pump")
            .Description("The target is blasted by a huge volume of water launched under great pressure.")
            .Type(PokemonType.Water)
            .Special(110, 80, 5)
            .WithEffects(e => e.Damage())
            .Build();

        static partial void RegisterWater()
        {
            _all.Add(WaterGun);
            _all.Add(Surf);
            _all.Add(HydroPump);
        }
    }
}
