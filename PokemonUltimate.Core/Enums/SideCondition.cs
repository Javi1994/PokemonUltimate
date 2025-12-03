namespace PokemonUltimate.Core.Enums
{
    /// <summary>
    /// Conditions that affect one side of the battlefield.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    public enum SideCondition
    {
        /// <summary>No condition.</summary>
        None,
        
        /// <summary>
        /// Reflect - Reduces physical damage by 50% (Singles) or 33% (Doubles).
        /// Duration: 5 turns (8 with Light Clay).
        /// </summary>
        Reflect,
        
        /// <summary>
        /// Light Screen - Reduces special damage by 50% (Singles) or 33% (Doubles).
        /// Duration: 5 turns (8 with Light Clay).
        /// </summary>
        LightScreen,
        
        /// <summary>
        /// Aurora Veil - Reduces both physical and special damage.
        /// Can only be set in Hail/Snow. Duration: 5 turns (8 with Light Clay).
        /// </summary>
        AuroraVeil,
        
        /// <summary>
        /// Tailwind - Doubles Speed for the side.
        /// Duration: 4 turns.
        /// </summary>
        Tailwind,
        
        /// <summary>
        /// Safeguard - Prevents status conditions.
        /// Duration: 5 turns.
        /// </summary>
        Safeguard,
        
        /// <summary>
        /// Mist - Prevents stat reductions from opponents.
        /// Duration: 5 turns.
        /// </summary>
        Mist,
        
        /// <summary>
        /// Lucky Chant - Prevents critical hits against the side.
        /// Duration: 5 turns.
        /// </summary>
        LuckyChant,
        
        /// <summary>
        /// Wide Guard - Protects from spread moves for the turn.
        /// Single turn.
        /// </summary>
        WideGuard,
        
        /// <summary>
        /// Quick Guard - Protects from priority moves for the turn.
        /// Single turn.
        /// </summary>
        QuickGuard,
        
        /// <summary>
        /// Mat Block - Protects from damaging moves for the turn.
        /// Single turn, only works on first turn out.
        /// </summary>
        MatBlock,
    }
}

