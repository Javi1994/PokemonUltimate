using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// Affects the battlefield conditions (weather, terrain, hazards, screens, rooms).
    /// Used by Rain Dance, Stealth Rock, Reflect, Trick Room, etc.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.2: Move Data
    /// **Documentation**: See `docs/features/1-game-data/1.2-move-data/architecture.md`
    /// </remarks>
    public class FieldConditionEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.FieldCondition;
        public string Description => GetDescription();
        
        /// <summary>Type of field condition to set.</summary>
        public FieldConditionType ConditionType { get; set; }
        
        /// <summary>Weather to set (if ConditionType is Weather).</summary>
        public Weather? WeatherToSet { get; set; }
        
        /// <summary>Terrain to set (if ConditionType is Terrain).</summary>
        public Terrain? TerrainToSet { get; set; }
        
        /// <summary>Hazard to set (if ConditionType is Hazard).</summary>
        public HazardType? HazardToSet { get; set; }
        
        /// <summary>Side condition to set (if ConditionType is SideCondition).</summary>
        public SideCondition? SideConditionToSet { get; set; }
        
        /// <summary>Field effect to set (if ConditionType is FieldEffect).</summary>
        public FieldEffect? FieldEffectToSet { get; set; }
        
        /// <summary>Whether this targets the user's side (true) or opponent's side (false).</summary>
        public bool TargetsUserSide { get; set; } = false;
        
        /// <summary>Whether this removes conditions instead of setting them (Defog, Rapid Spin).</summary>
        public bool RemovesCondition { get; set; } = false;
        
        /// <summary>Which conditions to remove (for Defog, Rapid Spin).</summary>
        public FieldConditionType[] ConditionsToRemove { get; set; }
        
        public FieldConditionEffect() { }
        
        private string GetDescription()
        {
            if (RemovesCondition)
                return "Removes field conditions.";
            
            switch (ConditionType)
            {
                case FieldConditionType.Weather: return $"Sets {WeatherToSet}.";
                case FieldConditionType.Terrain: return $"Sets {TerrainToSet}.";
                case FieldConditionType.Hazard: return $"Sets {HazardToSet} on opponent's side.";
                case FieldConditionType.SideCondition: return $"Sets {SideConditionToSet}.";
                case FieldConditionType.FieldEffect: return $"Sets {FieldEffectToSet}.";
                default: return "Affects the field.";
            }
        }
    }
    
    /// <summary>
    /// Types of field conditions that can be set.
    /// </summary>
    public enum FieldConditionType
    {
        Weather,
        Terrain,
        Hazard,
        SideCondition,
        FieldEffect,
    }
}

