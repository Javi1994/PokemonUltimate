namespace PokemonUltimate.Core.Enums
{
    /// <summary>
    /// Pokemon nature. Affects stat calculation: +10% to one stat, -10% to another.
    /// 5 natures are neutral (same stat boosted and reduced).
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    public enum Nature
    {
        // Neutral natures (no stat change)
        Hardy,      // +Atk -Atk (neutral)
        Docile,     // +Def -Def (neutral)
        Serious,    // +Spe -Spe (neutral)
        Bashful,    // +SpA -SpA (neutral)
        Quirky,     // +SpD -SpD (neutral)

        // Attack boosting (+Atk)
        Lonely,     // +Atk -Def
        Brave,      // +Atk -Spe
        Adamant,    // +Atk -SpA
        Naughty,    // +Atk -SpD

        // Defense boosting (+Def)
        Bold,       // +Def -Atk
        Relaxed,    // +Def -Spe
        Impish,     // +Def -SpA
        Lax,        // +Def -SpD

        // Speed boosting (+Spe)
        Timid,      // +Spe -Atk
        Hasty,      // +Spe -Def
        Jolly,      // +Spe -SpA
        Naive,      // +Spe -SpD

        // Special Attack boosting (+SpA)
        Modest,     // +SpA -Atk
        Mild,       // +SpA -Def
        Quiet,      // +SpA -Spe
        Rash,       // +SpA -SpD

        // Special Defense boosting (+SpD)
        Calm,       // +SpD -Atk
        Gentle,     // +SpD -Def
        Sassy,      // +SpD -Spe
        Careful     // +SpD -SpA
    }
}

