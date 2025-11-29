namespace PokemonUltimate.Core.Enums
{
    // Major status conditions that persist outside of battle.
    // Only one can be active at a time on a Pokemon.
    public enum PersistentStatus
    {
        None = 0,
        Burn = 1,
        Paralysis = 2,
        Sleep = 3,
        Poison = 4,
        BadlyPoisoned = 5,
        Freeze = 6
    }
}

