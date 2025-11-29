using PokemonUltimate.Core.Models;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Catalogs
{
    /// <summary>
    /// Fire-type moves.
    /// </summary>
    public static partial class MoveCatalog
    {
        public static readonly MoveData Ember = new MoveData
        {
            Name = "Ember",
            Description = "The target is attacked with small flames. May also leave the target with a burn.",
            Type = PokemonType.Fire,
            Category = MoveCategory.Special,
            Power = 40,
            Accuracy = 100,
            MaxPP = 25,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = 
            { 
                new DamageEffect(),
                new StatusEffect(PersistentStatus.Burn, 10)
            }
        };

        public static readonly MoveData Flamethrower = new MoveData
        {
            Name = "Flamethrower",
            Description = "The target is scorched with an intense blast of fire. May also leave the target with a burn.",
            Type = PokemonType.Fire,
            Category = MoveCategory.Special,
            Power = 90,
            Accuracy = 100,
            MaxPP = 15,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = 
            { 
                new DamageEffect(),
                new StatusEffect(PersistentStatus.Burn, 10)
            }
        };

        public static readonly MoveData FireBlast = new MoveData
        {
            Name = "Fire Blast",
            Description = "The target is attacked with an intense blast of all-consuming fire. May also burn.",
            Type = PokemonType.Fire,
            Category = MoveCategory.Special,
            Power = 110,
            Accuracy = 85,
            MaxPP = 5,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = 
            { 
                new DamageEffect(),
                new StatusEffect(PersistentStatus.Burn, 10)
            }
        };

        static partial void RegisterFire()
        {
            _all.Add(Ember);
            _all.Add(Flamethrower);
            _all.Add(FireBlast);
        }
    }
}

