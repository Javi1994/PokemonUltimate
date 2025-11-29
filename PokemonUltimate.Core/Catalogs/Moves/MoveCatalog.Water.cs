using PokemonUltimate.Core.Data;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Catalogs
{
    /// <summary>
    /// Water-type moves.
    /// </summary>
    public static partial class MoveCatalog
    {
        public static readonly MoveData WaterGun = new MoveData
        {
            Name = "Water Gun",
            Description = "The target is blasted with a forceful shot of water.",
            Type = PokemonType.Water,
            Category = MoveCategory.Special,
            Power = 40,
            Accuracy = 100,
            MaxPP = 25,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = { new DamageEffect() }
        };

        public static readonly MoveData Surf = new MoveData
        {
            Name = "Surf",
            Description = "The user attacks everything around it by swamping its surroundings with a giant wave.",
            Type = PokemonType.Water,
            Category = MoveCategory.Special,
            Power = 90,
            Accuracy = 100,
            MaxPP = 15,
            Priority = 0,
            TargetScope = TargetScope.AllEnemies,
            Effects = { new DamageEffect() }
        };

        public static readonly MoveData HydroPump = new MoveData
        {
            Name = "Hydro Pump",
            Description = "The target is blasted by a huge volume of water launched under great pressure.",
            Type = PokemonType.Water,
            Category = MoveCategory.Special,
            Power = 110,
            Accuracy = 80,
            MaxPP = 5,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = { new DamageEffect() }
        };

        static partial void RegisterWater()
        {
            _all.Add(WaterGun);
            _all.Add(Surf);
            _all.Add(HydroPump);
        }
    }
}

