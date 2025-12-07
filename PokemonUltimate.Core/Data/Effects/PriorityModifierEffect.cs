using PokemonUltimate.Core.Data.Effects.Definition;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Strategies.Effect;

namespace PokemonUltimate.Core.Data.Effects
{
    /// <summary>
    /// Modifies the move's priority bracket.
    /// Note: This is typically set on MoveData.Priority, but this effect handles conditional priority.
    /// Used by Grassy Glide (+1 in Grassy Terrain), Gale Wings (+1 for Flying at full HP).
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.2: Move Data
    /// **Documentation**: See `docs/features/1-game-data/1.2-move-data/architecture.md`
    /// </remarks>
    public class PriorityModifierEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.PriorityModifier;
        public string Description => GetDescription();

        /// <summary>Priority change when condition is met.</summary>
        public int PriorityChange { get; set; } = 1;

        /// <summary>Condition type that activates this modifier.</summary>
        public PriorityCondition Condition { get; set; } = PriorityCondition.Always;

        /// <summary>Required terrain for TerrainBased condition.</summary>
        public Terrain? RequiredTerrain { get; set; }

        /// <summary>Required weather for WeatherBased condition.</summary>
        public Weather? RequiredWeather { get; set; }

        /// <summary>HP threshold for HPBased condition (1.0 = full HP).</summary>
        public float HPThreshold { get; set; } = 1.0f;

        public PriorityModifierEffect() { }

        private string GetDescription()
        {
            return EffectDescriptionRegistries.GetPriorityModifierDescription(this);
        }
    }

    /// <summary>
    /// Conditions for priority modification.
    /// </summary>
    public enum PriorityCondition
    {
        /// <summary>Always applies (Quick Attack, Extreme Speed).</summary>
        Always,
        /// <summary>Applies in specific terrain (Grassy Glide).</summary>
        TerrainBased,
        /// <summary>Applies in specific weather.</summary>
        WeatherBased,
        /// <summary>Applies at full HP (Gale Wings).</summary>
        FullHP,
        /// <summary>Applies below HP threshold.</summary>
        LowHP,
    }
}

