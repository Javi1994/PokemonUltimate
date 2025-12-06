using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Instances
{
    /// <summary>
    /// Individual Values for a Pokemon instance (0-31 per stat).
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.1: Pokemon Data
    /// **Documentation**: See `docs/features/1-game-data/1.1-pokemon-data/architecture.md`
    /// </remarks>
    public class IVSet
    {
        public IVSet(int hp, int attack, int defense, int spAttack, int spDefense, int speed)
        {
            CoreValidators.ValidateIV(hp, nameof(hp));
            CoreValidators.ValidateIV(attack, nameof(attack));
            CoreValidators.ValidateIV(defense, nameof(defense));
            CoreValidators.ValidateIV(spAttack, nameof(spAttack));
            CoreValidators.ValidateIV(spDefense, nameof(spDefense));
            CoreValidators.ValidateIV(speed, nameof(speed));

            HP = hp;
            Attack = attack;
            Defense = defense;
            SpAttack = spAttack;
            SpDefense = spDefense;
            Speed = speed;
        }

        public int HP { get; }
        public int Attack { get; }
        public int Defense { get; }
        public int SpAttack { get; }
        public int SpDefense { get; }
        public int Speed { get; }

        public static IVSet Perfect => new IVSet(
            CoreConstants.MaxIV,
            CoreConstants.MaxIV,
            CoreConstants.MaxIV,
            CoreConstants.MaxIV,
            CoreConstants.MaxIV,
            CoreConstants.MaxIV);
    }
}

