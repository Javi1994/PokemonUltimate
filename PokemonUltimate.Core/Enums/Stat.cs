namespace PokemonUltimate.Core.Enums
{
    // Pokemon stats used for damage calculation and stat modifications.
    public enum Stat
    {
        HP,
        Attack,
        Defense,
        SpAttack,
        SpDefense,
        Speed,
        
        // Battle-only stats (not stored in base stats)
        Accuracy,
        Evasion
    }
}

