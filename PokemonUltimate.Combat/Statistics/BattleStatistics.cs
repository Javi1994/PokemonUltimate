using System.Collections.Generic;

namespace PokemonUltimate.Combat.Statistics
{
    /// <summary>
    /// Comprehensive battle statistics collected during battle execution.
    /// Designed to be easily extensible - just add new properties as needed.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.20: Statistics System
    /// **Documentation**: See `docs/features/2-combat-system/2.20-statistics-system/architecture.md`
    /// </remarks>
    public class BattleStatistics
    {
        // ===== BATTLE OUTCOMES =====
        public int PlayerWins { get; set; }
        public int EnemyWins { get; set; }
        public int Draws { get; set; }
        
        // ===== MOVE STATISTICS =====
        // Move usage (Pokemon -> Move -> Count)
        public Dictionary<string, Dictionary<string, int>> MoveUsageStats { get; set; } = new Dictionary<string, Dictionary<string, int>>();
        
        // ===== STATUS EFFECTS =====
        // Persistent status (Pokemon -> Status -> Count)
        public Dictionary<string, Dictionary<string, int>> StatusEffectStats { get; set; } = new Dictionary<string, Dictionary<string, int>>();
        
        // Volatile status (Pokemon -> VolatileStatus -> Count)
        public Dictionary<string, Dictionary<string, int>> VolatileStatusStats { get; set; } = new Dictionary<string, Dictionary<string, int>>();
        
        // ===== DAMAGE STATISTICS =====
        // Damage values (Pokemon -> List of damage values)
        public Dictionary<string, List<int>> DamageStats { get; set; } = new Dictionary<string, List<int>>();
        
        // Critical hits
        public int CriticalHits { get; set; }
        
        // Misses
        public int Misses { get; set; }
        
        // ===== FIELD EFFECTS =====
        // Weather changes (Weather -> Count)
        public Dictionary<string, int> WeatherChanges { get; set; } = new Dictionary<string, int>();
        
        // Terrain changes (Terrain -> Count)
        public Dictionary<string, int> TerrainChanges { get; set; } = new Dictionary<string, int>();
        
        // Side conditions (Side -> Condition -> Count)
        public Dictionary<string, Dictionary<string, int>> SideConditionStats { get; set; } = new Dictionary<string, Dictionary<string, int>>();
        
        // Hazards (Side -> Hazard -> Count)
        public Dictionary<string, Dictionary<string, int>> HazardStats { get; set; } = new Dictionary<string, Dictionary<string, int>>();
        
        // ===== STAT CHANGES =====
        // Stat changes (Pokemon -> Stat -> List of stage changes)
        public Dictionary<string, Dictionary<string, List<int>>> StatChangeStats { get; set; } = new Dictionary<string, Dictionary<string, List<int>>>();
        
        // ===== HEALING =====
        // Healing (Pokemon -> List of heal amounts)
        public Dictionary<string, List<int>> HealingStats { get; set; } = new Dictionary<string, List<int>>();
        
        // ===== ACTION STATISTICS =====
        // Action type counts (ActionType -> Count)
        public Dictionary<string, int> ActionTypeStats { get; set; } = new Dictionary<string, int>();
        
        // Effects generated (Move -> EffectType -> Count)
        public Dictionary<string, Dictionary<string, int>> EffectGenerationStats { get; set; } = new Dictionary<string, Dictionary<string, int>>();
        
        // ===== TURN STATISTICS =====
        public int TotalTurns { get; set; }
        public List<int> TurnDurations { get; set; } = new List<int>();
        
        // ===== ABILITY & ITEM STATISTICS =====
        // Ability activations (Pokemon -> Ability -> Count)
        public Dictionary<string, Dictionary<string, int>> AbilityActivationStats { get; set; } = new Dictionary<string, Dictionary<string, int>>();
        
        // Item activations (Pokemon -> Item -> Count)
        public Dictionary<string, Dictionary<string, int>> ItemActivationStats { get; set; } = new Dictionary<string, Dictionary<string, int>>();
    }
}

