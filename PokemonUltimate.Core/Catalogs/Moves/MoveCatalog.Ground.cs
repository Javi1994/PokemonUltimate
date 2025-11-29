using PokemonUltimate.Core.Data;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Catalogs
{
    /// <summary>
    /// Ground-type moves.
    /// </summary>
    public static partial class MoveCatalog
    {
        public static readonly MoveData Earthquake = new MoveData
        {
            Name = "Earthquake",
            Description = "The user sets off an earthquake that strikes every Pokémon around it.",
            Type = PokemonType.Ground,
            Category = MoveCategory.Physical,
            Power = 100,
            Accuracy = 100,
            MaxPP = 10,
            Priority = 0,
            TargetScope = TargetScope.AllOthers,
            Effects = { new DamageEffect() }
        };

        static partial void RegisterGround()
        {
            _all.Add(Earthquake);
        }
    }
}

