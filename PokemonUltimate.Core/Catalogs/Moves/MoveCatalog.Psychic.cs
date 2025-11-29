using PokemonUltimate.Core.Builders;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Models;

namespace PokemonUltimate.Core.Catalogs
{
    /// <summary>
    /// Psychic-type moves.
    /// </summary>
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

        static partial void RegisterPsychic()
        {
            _all.Add(Psychic);
        }
    }
}
