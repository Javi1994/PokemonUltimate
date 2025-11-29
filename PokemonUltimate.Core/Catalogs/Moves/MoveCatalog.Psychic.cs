using PokemonUltimate.Core.Models;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Catalogs
{
    /// <summary>
    /// Psychic-type moves.
    /// </summary>
    public static partial class MoveCatalog
    {
        public static readonly MoveData Psychic = new MoveData
        {
            Name = "Psychic",
            Description = "The target is hit by a strong telekinetic force. May also lower the target's Sp. Def.",
            Type = PokemonType.Psychic,
            Category = MoveCategory.Special,
            Power = 90,
            Accuracy = 100,
            MaxPP = 10,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = 
            { 
                new DamageEffect(),
                new StatChangeEffect(Stat.SpDefense, -1, targetSelf: false, chancePercent: 10)
            }
        };

        static partial void RegisterPsychic()
        {
            _all.Add(Psychic);
        }
    }
}

