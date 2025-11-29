using PokemonUltimate.Core.Models;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Catalogs
{
    /// <summary>
    /// Electric-type moves.
    /// </summary>
    public static partial class MoveCatalog
    {
        public static readonly MoveData ThunderShock = new MoveData
        {
            Name = "Thunder Shock",
            Description = "A jolt of electricity crashes down on the target. May also cause paralysis.",
            Type = PokemonType.Electric,
            Category = MoveCategory.Special,
            Power = 40,
            Accuracy = 100,
            MaxPP = 30,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = 
            { 
                new DamageEffect(),
                new StatusEffect(PersistentStatus.Paralysis, 10)
            }
        };

        public static readonly MoveData Thunderbolt = new MoveData
        {
            Name = "Thunderbolt",
            Description = "A strong electric blast crashes down on the target. May also cause paralysis.",
            Type = PokemonType.Electric,
            Category = MoveCategory.Special,
            Power = 90,
            Accuracy = 100,
            MaxPP = 15,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = 
            { 
                new DamageEffect(),
                new StatusEffect(PersistentStatus.Paralysis, 10)
            }
        };

        public static readonly MoveData Thunder = new MoveData
        {
            Name = "Thunder",
            Description = "A wicked thunderbolt is dropped on the target. May also cause paralysis.",
            Type = PokemonType.Electric,
            Category = MoveCategory.Special,
            Power = 110,
            Accuracy = 70,
            MaxPP = 10,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = 
            { 
                new DamageEffect(),
                new StatusEffect(PersistentStatus.Paralysis, 30)
            }
        };

        public static readonly MoveData ThunderWave = new MoveData
        {
            Name = "Thunder Wave",
            Description = "The user launches a weak jolt of electricity that paralyzes the target.",
            Type = PokemonType.Electric,
            Category = MoveCategory.Status,
            Power = 0,
            Accuracy = 90,
            MaxPP = 20,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = { new StatusEffect(PersistentStatus.Paralysis, 100) }
        };

        static partial void RegisterElectric()
        {
            _all.Add(ThunderShock);
            _all.Add(Thunderbolt);
            _all.Add(Thunder);
            _all.Add(ThunderWave);
        }
    }
}

