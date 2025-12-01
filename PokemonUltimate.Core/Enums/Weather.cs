namespace PokemonUltimate.Core.Enums
{
    /// <summary>
    /// Weather conditions that can affect battle.
    /// </summary>
    public enum Weather
    {
        /// <summary>No weather effect.</summary>
        None,
        
        /// <summary>Rain - Boosts Water, weakens Fire, enables Thunder/Hurricane.</summary>
        Rain,
        
        /// <summary>Harsh Sunlight - Boosts Fire, weakens Water, enables Solar Beam.</summary>
        Sun,
        
        /// <summary>Sandstorm - Damages non-Rock/Ground/Steel, boosts Rock SpDef.</summary>
        Sandstorm,
        
        /// <summary>Hail - Damages non-Ice types, enables Blizzard.</summary>
        Hail,
        
        /// <summary>Snow - Boosts Ice Defense (Gen 9+).</summary>
        Snow,
        
        /// <summary>Heavy Rain - Nullifies Fire moves (Primordial Sea).</summary>
        HeavyRain,
        
        /// <summary>Extremely Harsh Sunlight - Nullifies Water moves (Desolate Land).</summary>
        ExtremelyHarshSunlight,
        
        /// <summary>Strong Winds - Reduces Flying weaknesses (Delta Stream).</summary>
        StrongWinds,
        
        /// <summary>Fog - Reduces accuracy (Gen 4 only).</summary>
        Fog,
    }
}

