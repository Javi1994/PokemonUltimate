using PokemonUltimate.Content.Builders;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Catalogs.Moves
{
    /// <summary>
    /// Normal-type moves.
    /// </summary>
    public static partial class MoveCatalog
    {
        public static readonly MoveData Tackle = Move.Define("Tackle")
            .Description("A physical attack in which the user charges and slams into the target.")
            .Type(PokemonType.Normal)
            .Physical(40, 100, 35)
            .WithEffects(e => e.Damage())
            .Build();

        public static readonly MoveData Scratch = Move.Define("Scratch")
            .Description("Hard, pointed, sharp claws rake the target to inflict damage.")
            .Type(PokemonType.Normal)
            .Physical(40, 100, 35)
            .WithEffects(e => e.Damage())
            .Build();

        public static readonly MoveData QuickAttack = Move.Define("Quick Attack")
            .Description("The user lunges at the target at a speed that makes it almost invisible.")
            .Type(PokemonType.Normal)
            .Physical(40, 100, 30)
            .Priority(1)
            .WithEffects(e => e.Damage())
            .Build();

        public static readonly MoveData HyperBeam = Move.Define("Hyper Beam")
            .Description("The target is attacked with a powerful beam. The user can't move on the next turn.")
            .Type(PokemonType.Normal)
            .Special(150, 90, 5)
            .WithEffects(e => e.Damage())
            .Build();

        public static readonly MoveData Growl = Move.Define("Growl")
            .Description("The user growls in an endearing way, lowering the opposing Pokémon's Attack stat.")
            .Type(PokemonType.Normal)
            .Status(100, 40)
            .TargetAllEnemies()
            .WithEffects(e => e.LowerAttack())
            .Build();

        public static readonly MoveData DefenseCurl = Move.Define("Defense Curl")
            .Description("The user curls up to conceal weak spots and raise its Defense stat.")
            .Type(PokemonType.Normal)
            .Status(100, 40)
            .WithEffects(e => e.RaiseDefense())
            .Build();

        public static readonly MoveData Splash = Move.Define("Splash")
            .Description("The user just flops and splashes around to no effect at all...")
            .Type(PokemonType.Normal)
            .Status(100, 40)
            .WithEffects(e => { }) // No effects - does nothing
            .Build();

        static partial void RegisterNormal()
        {
            _all.Add(Tackle);
            _all.Add(Scratch);
            _all.Add(QuickAttack);
            _all.Add(HyperBeam);
            _all.Add(Growl);
            _all.Add(DefenseCurl);
            _all.Add(Splash);
        }
    }
}
