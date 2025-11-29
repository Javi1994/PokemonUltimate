using PokemonUltimate.Core.Data;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Catalogs
{
    /// <summary>
    /// Normal-type moves.
    /// </summary>
    public static partial class MoveCatalog
    {
        public static readonly MoveData Tackle = new MoveData
        {
            Name = "Tackle",
            Description = "A physical attack in which the user charges and slams into the target.",
            Type = PokemonType.Normal,
            Category = MoveCategory.Physical,
            Power = 40,
            Accuracy = 100,
            MaxPP = 35,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = { new DamageEffect() }
        };

        public static readonly MoveData Scratch = new MoveData
        {
            Name = "Scratch",
            Description = "Hard, pointed, sharp claws rake the target to inflict damage.",
            Type = PokemonType.Normal,
            Category = MoveCategory.Physical,
            Power = 40,
            Accuracy = 100,
            MaxPP = 35,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = { new DamageEffect() }
        };

        public static readonly MoveData QuickAttack = new MoveData
        {
            Name = "Quick Attack",
            Description = "The user lunges at the target at a speed that makes it almost invisible. Always goes first.",
            Type = PokemonType.Normal,
            Category = MoveCategory.Physical,
            Power = 40,
            Accuracy = 100,
            MaxPP = 30,
            Priority = 1,
            TargetScope = TargetScope.SingleEnemy,
            Effects = { new DamageEffect() }
        };

        public static readonly MoveData HyperBeam = new MoveData
        {
            Name = "Hyper Beam",
            Description = "The target is attacked with a powerful beam. The user can't move on the next turn.",
            Type = PokemonType.Normal,
            Category = MoveCategory.Special,
            Power = 150,
            Accuracy = 90,
            MaxPP = 5,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = { new DamageEffect() }
        };

        public static readonly MoveData Growl = new MoveData
        {
            Name = "Growl",
            Description = "The user growls in an endearing way, lowering the opposing Pokémon's Attack stat.",
            Type = PokemonType.Normal,
            Category = MoveCategory.Status,
            Power = 0,
            Accuracy = 100,
            MaxPP = 40,
            Priority = 0,
            TargetScope = TargetScope.AllEnemies,
            Effects = { new StatChangeEffect(Stat.Attack, -1, targetSelf: false) }
        };

        static partial void RegisterNormal()
        {
            _all.Add(Tackle);
            _all.Add(Scratch);
            _all.Add(QuickAttack);
            _all.Add(HyperBeam);
            _all.Add(Growl);
        }
    }
}

