using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Combat.Engine
{
    /// <summary>
    /// Processes entry hazards when a Pokemon switches in.
    /// Generates actions for damage, status application, and stat changes.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.14: Hazards System
    /// **Documentation**: See `docs/features/2-combat-system/2.14-hazards-system/README.md`
    /// </remarks>
    public static class EntryHazardProcessor
    {
        /// <summary>
        /// Processes all entry hazards on the opposing side when a Pokemon switches in.
        /// </summary>
        /// <param name="slot">The slot the Pokemon is switching into.</param>
        /// <param name="pokemon">The Pokemon switching in.</param>
        /// <param name="field">The battlefield.</param>
        /// <param name="getHazardData">Function to get HazardData by type. Cannot be null.</param>
        /// <returns>List of actions to execute for entry hazards.</returns>
        public static List<BattleAction> ProcessHazards(BattleSlot slot, PokemonInstance pokemon, BattleField field, Func<HazardType, HazardData> getHazardData)
        {
            if (slot == null)
                throw new ArgumentNullException(nameof(slot));
            if (pokemon == null)
                throw new ArgumentNullException(nameof(pokemon));
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);
            if (getHazardData == null)
                throw new ArgumentNullException(nameof(getHazardData));

            var actions = new List<BattleAction>();
            var opposingSide = field.GetOppositeSide(slot.Side);

            // Process each hazard type
            ProcessSpikes(opposingSide, slot, pokemon, field, getHazardData, actions);
            ProcessStealthRock(opposingSide, slot, pokemon, field, getHazardData, actions);
            ProcessToxicSpikes(opposingSide, slot, pokemon, field, getHazardData, actions);
            ProcessStickyWeb(opposingSide, slot, pokemon, field, getHazardData, actions);

            return actions;
        }

        /// <summary>
        /// Processes Spikes hazard.
        /// </summary>
        private static void ProcessSpikes(BattleSide side, BattleSlot slot, PokemonInstance pokemon, BattleField field, Func<HazardType, HazardData> getHazardData, List<BattleAction> actions)
        {
            if (!side.HasHazard(HazardType.Spikes))
                return;

            var hazardData = getHazardData(HazardType.Spikes);
            if (hazardData == null)
                return;

            // Check if Pokemon is affected (not Flying, not Levitate)
            if (!hazardData.AffectsPokemon(pokemon.Species.PrimaryType, pokemon.Species.SecondaryType, pokemon.Ability?.Name))
                return;

            int layers = side.GetHazardLayers(HazardType.Spikes);
            float damageFraction = hazardData.GetDamage(layers);
            int damage = CalculateHazardDamage(pokemon.MaxHP, damageFraction);

            if (damage > 0)
            {
                actions.Add(new MessageAction(string.Format(GameMessages.HazardSpikesDamage, pokemon.DisplayName)));
                actions.Add(CreateHazardDamageAction(slot, field, damage));
            }
        }

        /// <summary>
        /// Processes Stealth Rock hazard.
        /// </summary>
        private static void ProcessStealthRock(BattleSide side, BattleSlot slot, PokemonInstance pokemon, BattleField field, Func<HazardType, HazardData> getHazardData, List<BattleAction> actions)
        {
            if (!side.HasHazard(HazardType.StealthRock))
                return;

            var hazardData = getHazardData(HazardType.StealthRock);
            if (hazardData == null)
                return;

            // Stealth Rock affects all Pokemon (including Flying)
            // Calculate type effectiveness vs Rock
            float effectiveness = CalculateTypeEffectiveness(
                hazardData.DamageType.Value,
                pokemon.Species.PrimaryType,
                pokemon.Species.SecondaryType);

            float baseDamageFraction = hazardData.GetDamage(1); // Always 1 layer
            float damageFraction = baseDamageFraction * effectiveness;
            int damage = CalculateHazardDamage(pokemon.MaxHP, damageFraction);

            if (damage > 0)
            {
                actions.Add(new MessageAction(string.Format(GameMessages.HazardStealthRockDamage, pokemon.DisplayName)));
                actions.Add(CreateHazardDamageAction(slot, field, damage));
            }
        }

        /// <summary>
        /// Processes Toxic Spikes hazard.
        /// </summary>
        private static void ProcessToxicSpikes(BattleSide side, BattleSlot slot, PokemonInstance pokemon, BattleField field, Func<HazardType, HazardData> getHazardData, List<BattleAction> actions)
        {
            if (!side.HasHazard(HazardType.ToxicSpikes))
                return;

            var hazardData = getHazardData(HazardType.ToxicSpikes);
            if (hazardData == null)
                return;

            // Check if Poison type absorbs (removes spikes)
            if (hazardData.IsPoisonAbsorber(pokemon.Species.PrimaryType, pokemon.Species.SecondaryType))
            {
                side.RemoveHazard(HazardType.ToxicSpikes);
                actions.Add(new MessageAction(string.Format(GameMessages.HazardToxicSpikesAbsorbed, pokemon.DisplayName)));
                return;
            }

            // Check if Pokemon is affected (not Flying, not Levitate)
            if (!hazardData.AffectsPokemon(pokemon.Species.PrimaryType, pokemon.Species.SecondaryType, pokemon.Ability?.Name))
                return;

            int layers = side.GetHazardLayers(HazardType.ToxicSpikes);
            PersistentStatus status = hazardData.GetStatus(layers);

            if (status != PersistentStatus.None)
            {
                actions.Add(new MessageAction(string.Format(GameMessages.HazardToxicSpikesStatus, pokemon.DisplayName)));
                actions.Add(new ApplyStatusAction(null, slot, status));
            }
        }

        /// <summary>
        /// Processes Sticky Web hazard.
        /// </summary>
        private static void ProcessStickyWeb(BattleSide side, BattleSlot slot, PokemonInstance pokemon, BattleField field, Func<HazardType, HazardData> getHazardData, List<BattleAction> actions)
        {
            if (!side.HasHazard(HazardType.StickyWeb))
                return;

            var hazardData = getHazardData(HazardType.StickyWeb);
            if (hazardData == null)
                return;

            // Check if Pokemon is affected (not Flying, not Levitate)
            if (!hazardData.AffectsPokemon(pokemon.Species.PrimaryType, pokemon.Species.SecondaryType, pokemon.Ability?.Name))
                return;

            if (hazardData.LowersStat)
            {
                int stages = hazardData.StatStages;
                
                // Check for Contrary ability (reverses stat changes)
                if (pokemon.Ability != null && pokemon.Ability.Name.Equals("Contrary", StringComparison.OrdinalIgnoreCase))
                {
                    stages = -stages; // Reverse the stat change
                }

                actions.Add(new MessageAction(string.Format(GameMessages.HazardStickyWebSpeed, pokemon.DisplayName)));
                actions.Add(new StatChangeAction(null, slot, hazardData.StatToLower.Value, stages));
            }
        }

        /// <summary>
        /// Calculates type effectiveness for a defending Pokemon.
        /// </summary>
        private static float CalculateTypeEffectiveness(PokemonType attackingType, PokemonType primaryType, PokemonType? secondaryType)
        {
            float effectiveness1 = TypeEffectiveness.GetEffectiveness(attackingType, primaryType);
            float effectiveness2 = secondaryType.HasValue 
                ? TypeEffectiveness.GetEffectiveness(attackingType, secondaryType.Value) 
                : 1.0f;

            return effectiveness1 * effectiveness2;
        }

        /// <summary>
        /// Calculates hazard damage based on max HP and damage fraction.
        /// </summary>
        private static int CalculateHazardDamage(int maxHP, float damageFraction)
        {
            int damage = (int)(maxHP * damageFraction);
            return Math.Max(1, damage); // Minimum 1 damage
        }

        /// <summary>
        /// Creates a DamageAction for hazard damage.
        /// </summary>
        private static DamageAction CreateHazardDamageAction(BattleSlot slot, BattleField field, int damage)
        {
            // Create a dummy move for hazard damage context
            var dummyMove = new MoveData
            {
                Name = "Entry Hazard",
                Power = 1, // Non-zero power so it's treated as a damaging move
                Accuracy = 100,
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                MaxPP = 0,
                Priority = 0,
                TargetScope = TargetScope.Self
            };

            var context = new DamageContext(slot, slot, dummyMove, field);
            context.BaseDamage = damage; // Set base damage to the calculated hazard damage
            context.Multiplier = 1.0f; // No additional multipliers
            context.TypeEffectiveness = 1.0f; // Hazard damage is always effective

            return new DamageAction(null, slot, context); // null user = system action
        }
    }
}

