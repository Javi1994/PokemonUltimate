namespace PokemonUltimate.Core.Evolution
{
    /// <summary>
    /// Types of conditions that can trigger evolution.
    /// </summary>
    public enum EvolutionConditionType
    {
        /// <summary>Reach a minimum level.</summary>
        Level,
        
        /// <summary>Use a specific item (stone, etc.).</summary>
        UseItem,
        
        /// <summary>Trade with another player.</summary>
        Trade,
        
        /// <summary>High friendship value.</summary>
        Friendship,
        
        /// <summary>Specific time of day.</summary>
        TimeOfDay,
        
        /// <summary>Must know a specific move.</summary>
        KnowsMove,
        
        /// <summary>Must be a specific gender.</summary>
        Gender,
        
        /// <summary>Must be in a specific location.</summary>
        Location,
        
        /// <summary>Must be holding a specific item.</summary>
        HeldItem
    }
}

