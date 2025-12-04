using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Damage.Steps;
using PokemonUltimate.Core.Blueprints;

namespace PokemonUltimate.Combat.Damage
{
    /// <summary>
    /// Calculates damage by running a series of steps in order.
    /// Each step can modify the DamageContext.
    /// 
    /// This pipeline pattern makes the damage formula:
    /// - Modular: Each step is independent and testable
    /// - Extensible: Add new steps without changing existing code
    /// - Readable: Clear order of operations
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.4: Damage Calculation Pipeline
    /// **Documentation**: See `docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md`
    /// </remarks>
    public class DamagePipeline
    {
        private readonly List<IDamageStep> _steps;

        /// <summary>
        /// Creates a new damage pipeline with the standard steps.
        /// </summary>
        public DamagePipeline()
        {
            _steps = new List<IDamageStep>
            {
                new BaseDamageStep(),        // 1. Calculate base damage from formula (includes stat modifiers)
                new CriticalHitStep(),       // 2. Check for critical hit (1.5x)
                new RandomFactorStep(),      // 3. Apply random factor (0.85-1.0)
                new StabStep(),              // 4. Apply STAB bonus (1.5x)
                new AttackerAbilityStep(),   // 5. Apply ability damage multipliers (Blaze, etc.)
                new AttackerItemStep(),      // 6. Apply item damage multipliers (Life Orb, etc.)
                new WeatherStep(),           // 7. Apply weather damage multipliers (Sun/Rain, etc.)
                new TerrainStep(),           // 8. Apply terrain damage multipliers (Electric/Grassy/Psychic/Misty)
                new ScreenStep(),            // 9. Apply screen damage reduction (Reflect/Light Screen/Aurora Veil)
                new TypeEffectivenessStep(), // 10. Apply type effectiveness
                new BurnStep(),              // 11. Apply burn penalty (0.5x for physical)
            };
        }

        /// <summary>
        /// Creates a damage pipeline with custom steps (for testing).
        /// </summary>
        /// <param name="steps">The steps to use.</param>
        public DamagePipeline(IEnumerable<IDamageStep> steps)
        {
            _steps = new List<IDamageStep>(steps);
        }

        /// <summary>
        /// Calculates damage for an attack.
        /// </summary>
        /// <param name="attacker">The attacking slot.</param>
        /// <param name="defender">The defending slot.</param>
        /// <param name="move">The move being used.</param>
        /// <param name="field">The battlefield.</param>
        /// <param name="forceCritical">Force a critical hit (for testing).</param>
        /// <param name="fixedRandomValue">Use a fixed random value 0-1 (for testing).</param>
        /// <returns>The damage context with calculated values.</returns>
        public DamageContext Calculate(
            BattleSlot attacker,
            BattleSlot defender,
            MoveData move,
            BattleField field,
            bool forceCritical = false,
            float? fixedRandomValue = null)
        {
            if (attacker == null)
                throw new ArgumentNullException(nameof(attacker));
            if (defender == null)
                throw new ArgumentNullException(nameof(defender));
            if (move == null)
                throw new ArgumentNullException(nameof(move));
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            var context = new DamageContext(
                attacker, 
                defender, 
                move, 
                field, 
                forceCritical, 
                fixedRandomValue);

            // Run each step in order
            foreach (var step in _steps)
            {
                step.Process(context);
            }

            return context;
        }

        /// <summary>
        /// Adds a step to the end of the pipeline.
        /// </summary>
        /// <param name="step">The step to add.</param>
        public void AddStep(IDamageStep step)
        {
            if (step == null)
                throw new ArgumentNullException(nameof(step));
            
            _steps.Add(step);
        }

        /// <summary>
        /// Inserts a step at a specific position.
        /// </summary>
        /// <param name="index">The position to insert at.</param>
        /// <param name="step">The step to insert.</param>
        public void InsertStep(int index, IDamageStep step)
        {
            if (step == null)
                throw new ArgumentNullException(nameof(step));
            
            _steps.Insert(index, step);
        }

        /// <summary>
        /// Gets the current step count.
        /// </summary>
        public int StepCount => _steps.Count;
    }
}

