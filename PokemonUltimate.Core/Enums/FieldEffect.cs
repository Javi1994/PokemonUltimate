namespace PokemonUltimate.Core.Enums
{
    /// <summary>
    /// Effects that affect the entire battlefield.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    public enum FieldEffect
    {
        /// <summary>No effect.</summary>
        None,
        
        /// <summary>
        /// Trick Room - Slower Pokemon move first.
        /// Duration: 5 turns. Priority -7.
        /// </summary>
        TrickRoom,
        
        /// <summary>
        /// Magic Room - Held items have no effect.
        /// Duration: 5 turns.
        /// </summary>
        MagicRoom,
        
        /// <summary>
        /// Wonder Room - Swaps Defense and Special Defense.
        /// Duration: 5 turns.
        /// </summary>
        WonderRoom,
        
        /// <summary>
        /// Gravity - Grounds all Pokemon, disables flying moves.
        /// Duration: 5 turns. Ground moves hit Flying.
        /// </summary>
        Gravity,
        
        /// <summary>
        /// Ion Deluge - Normal moves become Electric type.
        /// Duration: 1 turn.
        /// </summary>
        IonDeluge,
        
        /// <summary>
        /// Fairy Lock - No Pokemon can flee or switch next turn.
        /// Duration: 1 turn.
        /// </summary>
        FairyLock,
        
        /// <summary>
        /// Mud Sport - Reduces Electric move power by 67%.
        /// Duration: 5 turns.
        /// </summary>
        MudSport,
        
        /// <summary>
        /// Water Sport - Reduces Fire move power by 67%.
        /// Duration: 5 turns.
        /// </summary>
        WaterSport,
    }
}

