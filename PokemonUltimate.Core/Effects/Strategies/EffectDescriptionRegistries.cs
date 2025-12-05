using System.Collections.Generic;
using PokemonUltimate.Core.Effects;

namespace PokemonUltimate.Core.Effects.Strategies
{
    /// <summary>
    /// Centralized registries for effect descriptions using dictionary-based approach.
    /// More efficient than Strategy Pattern for simple text descriptions.
    /// </summary>
    public static class EffectDescriptionRegistries
    {
        #region FieldConditionEffect Descriptions

        private static readonly Dictionary<FieldConditionType, string> _fieldConditionDescriptions = new Dictionary<FieldConditionType, string>
        {
            { FieldConditionType.Weather, "Sets weather." },
            { FieldConditionType.Terrain, "Sets terrain." },
            { FieldConditionType.Hazard, "Sets hazard on opponent's side." },
            { FieldConditionType.SideCondition, "Sets side condition." },
            { FieldConditionType.FieldEffect, "Sets field effect." }
        };

        public static string GetFieldConditionDescription(FieldConditionEffect effect)
        {
            if (effect.RemovesCondition)
                return "Removes field conditions.";

            if (_fieldConditionDescriptions.TryGetValue(effect.ConditionType, out var baseDescription))
            {
                switch (effect.ConditionType)
                {
                    case FieldConditionType.Weather:
                        return $"Sets {effect.WeatherToSet}.";
                    case FieldConditionType.Terrain:
                        return $"Sets {effect.TerrainToSet}.";
                    case FieldConditionType.Hazard:
                        return $"Sets {effect.HazardToSet} on opponent's side.";
                    case FieldConditionType.SideCondition:
                        return $"Sets {effect.SideConditionToSet}.";
                    case FieldConditionType.FieldEffect:
                        return $"Sets {effect.FieldEffectToSet}.";
                    default:
                        return baseDescription;
                }
            }

            return "Affects the field.";
        }

        #endregion

        #region PriorityModifierEffect Descriptions

        public static string GetPriorityModifierDescription(PriorityModifierEffect effect)
        {
            switch (effect.Condition)
            {
                case PriorityCondition.Always:
                    return $"Priority {(effect.PriorityChange > 0 ? "+" : "")}{effect.PriorityChange}.";
                case PriorityCondition.TerrainBased:
                    return $"+{effect.PriorityChange} priority in {effect.RequiredTerrain}.";
                case PriorityCondition.WeatherBased:
                    return $"+{effect.PriorityChange} priority in {effect.RequiredWeather}.";
                case PriorityCondition.FullHP:
                    return $"+{effect.PriorityChange} priority at full HP.";
                case PriorityCondition.LowHP:
                    return $"+{effect.PriorityChange} priority below {effect.HPThreshold * 100}% HP.";
                default:
                    return "Modifies priority.";
            }
        }

        #endregion

        #region ProtectionEffect Descriptions

        private static readonly Dictionary<ProtectionType, string> _protectionDescriptions = new Dictionary<ProtectionType, string>
        {
            { ProtectionType.Full, "Protects from all moves." },
            { ProtectionType.WideGuard, "Protects team from spread moves." },
            { ProtectionType.QuickGuard, "Protects team from priority moves." },
            { ProtectionType.CraftyShield, "Protects team from status moves." },
            { ProtectionType.MatBlock, "Protects team from damaging moves." }
        };

        public static string GetProtectionDescription(ProtectionEffect effect)
        {
            if (_protectionDescriptions.TryGetValue(effect.Type, out var description))
            {
                return description;
            }

            return "Provides protection.";
        }

        #endregion

        #region SelfDestructEffect Descriptions

        private static readonly Dictionary<SelfDestructType, string> _selfDestructDescriptions = new Dictionary<SelfDestructType, string>
        {
            { SelfDestructType.Explosion, "User faints. Deals massive damage." },
            { SelfDestructType.Memento, "User faints. Sharply lowers target's Atk and SpA." },
            { SelfDestructType.FinalGambit, "User faints. Deals damage equal to user's HP." },
            { SelfDestructType.HealingWish, "User faints. Fully heals replacement." },
            { SelfDestructType.LunarDance, "User faints. Fully heals and restores PP of replacement." }
        };

        public static string GetSelfDestructDescription(SelfDestructEffect effect)
        {
            if (_selfDestructDescriptions.TryGetValue(effect.Type, out var description))
            {
                return description;
            }

            return "User faints after using.";
        }

        #endregion
    }
}
