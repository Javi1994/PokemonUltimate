using PokemonUltimate.Core.Constants;

namespace PokemonUltimate.Core.Instances
{
    /// <summary>
    /// Effort Values for a Pokemon instance (0-252 per stat).
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.1: Pokemon Data
    /// **Documentation**: See `docs/features/1-game-data/1.1-pokemon-data/architecture.md`
    /// </remarks>
    public class EVSet
    {
        public EVSet(int hp, int attack, int defense, int spAttack, int spDefense, int speed)
        {
            CoreValidators.ValidateEV(hp, nameof(hp));
            CoreValidators.ValidateEV(attack, nameof(attack));
            CoreValidators.ValidateEV(defense, nameof(defense));
            CoreValidators.ValidateEV(spAttack, nameof(spAttack));
            CoreValidators.ValidateEV(spDefense, nameof(spDefense));
            CoreValidators.ValidateEV(speed, nameof(speed));

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

        public static EVSet Max => new EVSet(
            CoreConstants.DefaultEV,
            CoreConstants.DefaultEV,
            CoreConstants.DefaultEV,
            CoreConstants.DefaultEV,
            CoreConstants.DefaultEV,
            CoreConstants.DefaultEV);
    }
}

