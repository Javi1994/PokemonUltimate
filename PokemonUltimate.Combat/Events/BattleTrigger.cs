namespace PokemonUltimate.Combat.Events
{
    /// <summary>
    /// Defines key battle events that trigger abilities and items.
    /// These are the main phases where passive effects activate.
    /// </summary>
    public enum BattleTrigger
    {
        /// <summary>When a Pokemon switches into battle.</summary>
        OnSwitchIn,
        
        /// <summary>Before a Pokemon uses a move.</summary>
        OnBeforeMove,
        
        /// <summary>After a Pokemon uses a move.</summary>
        OnAfterMove,
        
        /// <summary>When a Pokemon takes damage.</summary>
        OnDamageTaken,
        
        /// <summary>At the end of each turn.</summary>
        OnTurnEnd,
        
        /// <summary>When weather changes.</summary>
        OnWeatherChange
    }
}

