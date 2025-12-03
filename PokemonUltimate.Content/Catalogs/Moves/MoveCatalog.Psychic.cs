using PokemonUltimate.Content.Builders;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Catalogs.Moves
{
    /// <summary>
    /// Psychic-type moves.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.2: Move Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.2-move-expansion/architecture.md`
    /// </remarks>
    public static partial class MoveCatalog
    {
        public static readonly MoveData Psychic = Move.Define("Psychic")
            .Description("The target is hit by a strong telekinetic force. May also lower the target's Sp. Def.")
            .Type(PokemonType.Psychic)
            .Special(90, 100, 10)
            .WithEffects(e => e
                .Damage()
                .LowerSpDefense(1, 10))
            .Build();

        public static readonly MoveData Teleport = Move.Define("Teleport")
            .Description("Use it to flee from any wild Pokémon. It can also warp to the last Pokémon Center visited.")
            .Type(PokemonType.Psychic)
            .Status(100, 20)
            .WithEffects(e => { }) // No effects - switching move (deferred)
            .Build();

        public static readonly MoveData Confusion = Move.Define("Confusion")
            .Description("The target is hit by a weak telekinetic force. May also leave the target confused.")
            .Type(PokemonType.Psychic)
            .Special(50, 100, 25)
            .WithEffects(e => e.Damage())
            .Build();

        public static readonly MoveData Psybeam = Move.Define("Psybeam")
            .Description("The target is attacked with a peculiar ray. May also leave the target confused.")
            .Type(PokemonType.Psychic)
            .Special(65, 100, 20)
            .WithEffects(e => e.Damage())
            .Build();

        public static readonly MoveData Hypnosis = Move.Define("Hypnosis")
            .Description("The user employs hypnotic suggestion to make the target fall into a deep sleep.")
            .Type(PokemonType.Psychic)
            .Status(60, 20)
            .WithEffects(e => e.MaySleep())
            .Build();

        static partial void RegisterPsychic()
        {
            _all.Add(Psychic);
            _all.Add(Teleport);
            _all.Add(Confusion);
            _all.Add(Psybeam);
            _all.Add(Hypnosis);
        }
    }
}
