namespace PokemonUltimate.Core.Blueprints
{
    /// <summary>
    /// Base stats for a Pokemon species. These are the foundation for calculating
    /// actual stats based on level, IVs, EVs, and nature.
    /// Values typically range from 1-255 in official games.
    /// </summary>
    public class BaseStats
    {
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int SpAttack { get; set; }
        public int SpDefense { get; set; }
        public int Speed { get; set; }

        /// <summary>
        /// Base Stat Total - sum of all stats, used for comparing Pokemon power.
        /// </summary>
        public int Total => HP + Attack + Defense + SpAttack + SpDefense + Speed;

        public BaseStats() { }

        public BaseStats(int hp, int attack, int defense, int spAttack, int spDefense, int speed)
        {
            HP = hp;
            Attack = attack;
            Defense = defense;
            SpAttack = spAttack;
            SpDefense = spDefense;
            Speed = speed;
        }
    }
}

