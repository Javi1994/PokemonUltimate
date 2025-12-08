using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Damage.Definition;
using PokemonUltimate.Combat.Damage.Steps;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Providers;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Infrastructure.Factories;
using PokemonUltimate.Core.Domain.Instances;
using PokemonInstance = PokemonUltimate.Core.Domain.Instances.Pokemon.PokemonInstance;

namespace PokemonUltimate.DeveloperTools.Runners
{
    /// <summary>
    /// Calculates damage and visualizes the damage pipeline step-by-step for the Damage Calculator Debugger.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.2: Damage Calculator Debugger
    /// **Documentation**: See `docs/features/6-development-tools/6.2-damage-calculator-debugger/README.md`
    /// </remarks>
    public class DamageCalculatorRunner
    {
        /// <summary>
        /// Configuration for damage calculation.
        /// </summary>
        public class DamageCalculationConfig
        {
            public PokemonSpeciesData AttackerSpecies { get; set; } = null!;
            public PokemonSpeciesData DefenderSpecies { get; set; } = null!;
            public MoveData Move { get; set; } = null!;
            public int Level { get; set; } = 50;
            public bool ForceCritical { get; set; } = false;
            public float? FixedRandomValue { get; set; } = null;
        }

        /// <summary>
        /// Represents a single step in the damage pipeline.
        /// </summary>
        public class PipelineStep
        {
            public string StepName { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public float MultiplierBefore { get; set; }
            public float MultiplierAfter { get; set; }
            public float MultiplierChange { get; set; }
            public float DamageBefore { get; set; }
            public float DamageAfter { get; set; }
            public bool IsCritical { get; set; }
            public float RandomFactor { get; set; }
            public float TypeEffectiveness { get; set; }
            public bool IsStab { get; set; }
            public string Details { get; set; } = string.Empty;
        }

        /// <summary>
        /// Complete damage calculation result with pipeline steps.
        /// </summary>
        public class DamageCalculationResult
        {
            public float BaseDamage { get; set; }
            public float FinalMultiplier { get; set; }
            public int FinalDamage { get; set; }
            public float FinalDamageFloat { get; set; }
            public bool IsCritical { get; set; }
            public float RandomFactor { get; set; }
            public float TypeEffectiveness { get; set; }
            public bool IsStab { get; set; }
            public int DefenderMaxHP { get; set; }
            public float DamagePercentage { get; set; }
            public List<PipelineStep> PipelineSteps { get; set; } = new List<PipelineStep>();
            public int MinDamage { get; set; }
            public int MaxDamage { get; set; }
        }

        /// <summary>
        /// Calculates damage and returns step-by-step pipeline visualization.
        /// </summary>
        public DamageCalculationResult CalculateDamage(DamageCalculationConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (config.AttackerSpecies == null)
                throw new ArgumentException("AttackerSpecies cannot be null", nameof(config));
            if (config.DefenderSpecies == null)
                throw new ArgumentException("DefenderSpecies cannot be null", nameof(config));
            if (config.Move == null)
                throw new ArgumentException("Move cannot be null", nameof(config));

            // Create Pokemon instances
            var attacker = PokemonFactory.Create(config.AttackerSpecies, config.Level);
            var defender = PokemonFactory.Create(config.DefenderSpecies, config.Level);

            // Create battle field and slots
            var field = new BattleField();
            var playerParty = new List<PokemonInstance> { attacker };
            var enemyParty = new List<PokemonInstance> { defender };
            field.Initialize(BattleRules.Singles, playerParty, enemyParty);

            var attackerSlot = field.PlayerSide.Slots[0];
            var defenderSlot = field.EnemySide.Slots[0];

            // Create pipeline with step tracking for main calculation
            var pipeline = new TrackingDamagePipeline(config.ForceCritical, config.FixedRandomValue);

            // Calculate damage
            var context = pipeline.Calculate(attackerSlot, defenderSlot, config.Move, field);

            // Build result
            var result = new DamageCalculationResult
            {
                BaseDamage = context.BaseDamage,
                FinalMultiplier = context.Multiplier,
                FinalDamage = context.FinalDamage,
                FinalDamageFloat = context.BaseDamage * context.Multiplier,
                IsCritical = context.IsCritical,
                RandomFactor = context.RandomFactor,
                TypeEffectiveness = context.TypeEffectiveness,
                IsStab = context.IsStab,
                DefenderMaxHP = defender.MaxHP,
                DamagePercentage = defender.MaxHP > 0 ? (context.FinalDamage / (float)defender.MaxHP) * 100f : 0f,
                PipelineSteps = pipeline.GetSteps()
            };

            // Calculate damage range (min/max with random factor) - use standard pipeline without tracking
            var standardPipeline = new DamagePipeline();
            var minContext = standardPipeline.Calculate(attackerSlot, defenderSlot, config.Move, field, config.ForceCritical, 0.0f);
            var maxContext = standardPipeline.Calculate(attackerSlot, defenderSlot, config.Move, field, config.ForceCritical, 1.0f);
            result.MinDamage = minContext.FinalDamage;
            result.MaxDamage = maxContext.FinalDamage;

            return result;
        }

        /// <summary>
        /// Custom pipeline that tracks step-by-step changes.
        /// </summary>
        private class TrackingDamagePipeline : IDamagePipeline
        {
            private readonly List<PipelineStep> _steps = new List<PipelineStep>();
            private readonly bool _forceCritical;
            private readonly float? _fixedRandomValue;

            public TrackingDamagePipeline(bool forceCritical, float? fixedRandomValue)
            {
                _forceCritical = forceCritical;
                _fixedRandomValue = fixedRandomValue;
            }

            public DamageContext Calculate(
                BattleSlot attacker,
                BattleSlot defender,
                MoveData move,
                BattleField field,
                bool forceCritical = false,
                float? fixedRandomValue = null)
            {
                // Use instance values if not overridden
                bool useCritical = forceCritical || _forceCritical;
                float? useRandom = fixedRandomValue ?? _fixedRandomValue;

                var context = new DamageContext(attacker, defender, move, field, useCritical, useRandom);
                var randomProvider = new RandomProvider();

                // Track each step
                float multiplierBefore = 1.0f;
                float damageBefore = 0f;

                // Step 1: Base Damage
                var baseStep = new BaseDamageStep();
                baseStep.Process(context);
                AddStep("Base Damage",
                    $"Calculates base damage from formula: ((2 * Level / 5 + 2) * Power * Attack / Defense) / 50 + 2",
                    multiplierBefore, context.Multiplier, context.BaseDamage, context.BaseDamage,
                    context.IsCritical, context.RandomFactor, context.TypeEffectiveness, context.IsStab,
                    $"Base: {context.BaseDamage:F2}");
                multiplierBefore = context.Multiplier;
                damageBefore = context.BaseDamage;

                // Step 2: Critical Hit
                var critStep = new CriticalHitStep(randomProvider);
                critStep.Process(context);
                AddStep("Critical Hit",
                    context.IsCritical ? "Critical hit! 1.5x multiplier applied" : "No critical hit",
                    multiplierBefore, context.Multiplier, context.BaseDamage, context.BaseDamage * context.Multiplier,
                    context.IsCritical, context.RandomFactor, context.TypeEffectiveness, context.IsStab,
                    context.IsCritical ? "Critical: Yes (×1.5)" : "Critical: No");
                multiplierBefore = context.Multiplier;
                damageBefore = context.BaseDamage * context.Multiplier;

                // Step 3: Random Factor
                var randomStep = new RandomFactorStep(randomProvider);
                randomStep.Process(context);
                AddStep("Random Factor",
                    $"Random damage variation: {context.RandomFactor:F3} (range: 0.85-1.0)",
                    multiplierBefore, context.Multiplier, damageBefore, context.BaseDamage * context.Multiplier,
                    context.IsCritical, context.RandomFactor, context.TypeEffectiveness, context.IsStab,
                    $"Random: {context.RandomFactor:F3}");
                multiplierBefore = context.Multiplier;
                damageBefore = context.BaseDamage * context.Multiplier;

                // Step 4: STAB
                var stabStep = new StabStep();
                stabStep.Process(context);
                AddStep("STAB",
                    context.IsStab ? "Same Type Attack Bonus: 1.5x multiplier" : "No STAB bonus",
                    multiplierBefore, context.Multiplier, damageBefore, context.BaseDamage * context.Multiplier,
                    context.IsCritical, context.RandomFactor, context.TypeEffectiveness, context.IsStab,
                    context.IsStab ? "STAB: Yes (×1.5)" : "STAB: No");
                multiplierBefore = context.Multiplier;
                damageBefore = context.BaseDamage * context.Multiplier;

                // Step 5: Attacker Ability
                var abilityStep = new AttackerAbilityStep();
                abilityStep.Process(context);
                float abilityMultiplier = multiplierBefore > 0 ? context.Multiplier / multiplierBefore : 1.0f;
                AddStep("Attacker Ability",
                    abilityMultiplier != 1.0f ? $"Ability modifier: {abilityMultiplier:F2}x" : "No ability modifier",
                    multiplierBefore, context.Multiplier, damageBefore, context.BaseDamage * context.Multiplier,
                    context.IsCritical, context.RandomFactor, context.TypeEffectiveness, context.IsStab,
                    abilityMultiplier != 1.0f ? $"Ability: {abilityMultiplier:F2}x" : "Ability: None");
                multiplierBefore = context.Multiplier;
                damageBefore = context.BaseDamage * context.Multiplier;

                // Step 6: Attacker Item
                var itemStep = new AttackerItemStep();
                itemStep.Process(context);
                float itemMultiplier = multiplierBefore > 0 ? context.Multiplier / multiplierBefore : 1.0f;
                AddStep("Attacker Item",
                    itemMultiplier != 1.0f ? $"Item modifier: {itemMultiplier:F2}x" : "No item modifier",
                    multiplierBefore, context.Multiplier, damageBefore, context.BaseDamage * context.Multiplier,
                    context.IsCritical, context.RandomFactor, context.TypeEffectiveness, context.IsStab,
                    itemMultiplier != 1.0f ? $"Item: {itemMultiplier:F2}x" : "Item: None");
                multiplierBefore = context.Multiplier;
                damageBefore = context.BaseDamage * context.Multiplier;

                // Step 7: Weather
                var weatherStep = new WeatherStep();
                weatherStep.Process(context);
                float weatherMultiplier = multiplierBefore > 0 ? context.Multiplier / multiplierBefore : 1.0f;
                AddStep("Weather",
                    weatherMultiplier != 1.0f ? $"Weather modifier: {weatherMultiplier:F2}x" : "No weather modifier",
                    multiplierBefore, context.Multiplier, damageBefore, context.BaseDamage * context.Multiplier,
                    context.IsCritical, context.RandomFactor, context.TypeEffectiveness, context.IsStab,
                    weatherMultiplier != 1.0f ? $"Weather: {weatherMultiplier:F2}x" : "Weather: None");
                multiplierBefore = context.Multiplier;
                damageBefore = context.BaseDamage * context.Multiplier;

                // Step 8: Terrain
                var terrainStep = new TerrainStep();
                terrainStep.Process(context);
                float terrainMultiplier = multiplierBefore > 0 ? context.Multiplier / multiplierBefore : 1.0f;
                AddStep("Terrain",
                    terrainMultiplier != 1.0f ? $"Terrain modifier: {terrainMultiplier:F2}x" : "No terrain modifier",
                    multiplierBefore, context.Multiplier, damageBefore, context.BaseDamage * context.Multiplier,
                    context.IsCritical, context.RandomFactor, context.TypeEffectiveness, context.IsStab,
                    terrainMultiplier != 1.0f ? $"Terrain: {terrainMultiplier:F2}x" : "Terrain: None");
                multiplierBefore = context.Multiplier;
                damageBefore = context.BaseDamage * context.Multiplier;

                // Step 9: Screen
                var screenStep = new ScreenStep();
                screenStep.Process(context);
                float screenMultiplier = multiplierBefore > 0 ? context.Multiplier / multiplierBefore : 1.0f;
                AddStep("Screen",
                    screenMultiplier != 1.0f ? $"Screen modifier: {screenMultiplier:F2}x" : "No screen modifier",
                    multiplierBefore, context.Multiplier, damageBefore, context.BaseDamage * context.Multiplier,
                    context.IsCritical, context.RandomFactor, context.TypeEffectiveness, context.IsStab,
                    screenMultiplier != 1.0f ? $"Screen: {screenMultiplier:F2}x" : "Screen: None");
                multiplierBefore = context.Multiplier;
                damageBefore = context.BaseDamage * context.Multiplier;

                // Step 10: Type Effectiveness
                var typeStep = new TypeEffectivenessStep();
                typeStep.Process(context);
                AddStep("Type Effectiveness",
                    GetTypeEffectivenessDescription(context.TypeEffectiveness),
                    multiplierBefore, context.Multiplier, damageBefore, context.BaseDamage * context.Multiplier,
                    context.IsCritical, context.RandomFactor, context.TypeEffectiveness, context.IsStab,
                    $"Type: {context.TypeEffectiveness:F2}x ({GetTypeEffectivenessName(context.TypeEffectiveness)})");
                multiplierBefore = context.Multiplier;
                damageBefore = context.BaseDamage * context.Multiplier;

                // Step 11: Burn Penalty
                var burnStep = new BurnStep();
                burnStep.Process(context);
                float burnMultiplier = multiplierBefore > 0 ? context.Multiplier / multiplierBefore : 1.0f;
                AddStep("Burn Penalty",
                    burnMultiplier != 1.0f ? "Burn reduces physical damage by 50%" : "No burn penalty",
                    multiplierBefore, context.Multiplier, damageBefore, context.BaseDamage * context.Multiplier,
                    context.IsCritical, context.RandomFactor, context.TypeEffectiveness, context.IsStab,
                    burnMultiplier != 1.0f ? "Burn: Yes (×0.5)" : "Burn: No");

                return context;
            }

            private void AddStep(string stepName, string description, float multiplierBefore, float multiplierAfter,
                float damageBefore, float damageAfter, bool isCritical, float randomFactor,
                float typeEffectiveness, bool isStab, string details)
            {
                _steps.Add(new PipelineStep
                {
                    StepName = stepName,
                    Description = description,
                    MultiplierBefore = multiplierBefore,
                    MultiplierAfter = multiplierAfter,
                    MultiplierChange = multiplierAfter - multiplierBefore,
                    DamageBefore = damageBefore,
                    DamageAfter = damageAfter,
                    IsCritical = isCritical,
                    RandomFactor = randomFactor,
                    TypeEffectiveness = typeEffectiveness,
                    IsStab = isStab,
                    Details = details
                });
            }

            public List<PipelineStep> GetSteps()
            {
                return new List<PipelineStep>(_steps);
            }

            private string GetTypeEffectivenessDescription(float effectiveness)
            {
                if (effectiveness == 0f)
                    return "No effect (immune)";
                if (effectiveness == 0.25f)
                    return "Not very effective (0.25x)";
                if (effectiveness == 0.5f)
                    return "Not very effective (0.5x)";
                if (effectiveness == 1f)
                    return "Normal effectiveness (1x)";
                if (effectiveness == 2f)
                    return "Super effective (2x)";
                if (effectiveness == 4f)
                    return "Super effective (4x)";
                return $"Effectiveness: {effectiveness:F2}x";
            }

            private string GetTypeEffectivenessName(float effectiveness)
            {
                if (effectiveness == 0f)
                    return "Immune";
                if (effectiveness == 0.25f)
                    return "0.25x";
                if (effectiveness == 0.5f)
                    return "0.5x";
                if (effectiveness == 1f)
                    return "1x";
                if (effectiveness == 2f)
                    return "2x";
                if (effectiveness == 4f)
                    return "4x";
                return $"{effectiveness:F2}x";
            }
        }
    }
}

