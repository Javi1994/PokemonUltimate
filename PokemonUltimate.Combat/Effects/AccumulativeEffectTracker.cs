using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Combat.Effects
{
    /// <summary>
    /// Tracks and processes accumulative effects (effects that escalate over time).
    /// Provides centralized logic for effects like Badly Poisoned.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.8: End-of-Turn Effects
    /// **Documentation**: See `docs/features/2-combat-system/2.8-end-of-turn-effects/architecture.md`
    /// </remarks>
    public class AccumulativeEffectTracker
    {
        private readonly Dictionary<PersistentStatus, IAccumulativeEffect> _effects;

        /// <summary>
        /// Creates a new accumulative effect tracker with default effects registered.
        /// </summary>
        public AccumulativeEffectTracker()
        {
            _effects = new Dictionary<PersistentStatus, IAccumulativeEffect>
            {
                { PersistentStatus.BadlyPoisoned, new BadlyPoisonedEffect() }
            };
        }

        /// <summary>
        /// Registers an accumulative effect for a status type.
        /// </summary>
        /// <param name="effect">The effect to register. Cannot be null.</param>
        public void RegisterEffect(IAccumulativeEffect effect)
        {
            if (effect == null)
                throw new System.ArgumentNullException(nameof(effect));

            _effects[effect.StatusType] = effect;
        }

        /// <summary>
        /// Processes an accumulative effect and returns the calculated damage.
        /// </summary>
        /// <param name="status">The status type to process.</param>
        /// <param name="maxHP">The Pokemon's maximum HP.</param>
        /// <param name="currentCounter">The current turn counter.</param>
        /// <param name="nextCounter">Output parameter for the next counter value.</param>
        /// <returns>The calculated damage, or 0 if the status is not accumulative.</returns>
        public int ProcessEffect(PersistentStatus status, int maxHP, int currentCounter, out int nextCounter)
        {
            nextCounter = currentCounter;

            if (!_effects.TryGetValue(status, out var effect))
                return 0;

            // Ensure counter is at least initial value
            if (currentCounter < effect.GetInitialCounter())
                currentCounter = effect.GetInitialCounter();

            int damage = effect.CalculateValue(maxHP, currentCounter);
            nextCounter = effect.GetNextCounter(currentCounter);

            return damage;
        }

        /// <summary>
        /// Gets the initial counter value for a status type.
        /// </summary>
        /// <param name="status">The status type.</param>
        /// <returns>The initial counter value, or 0 if not accumulative.</returns>
        public int GetInitialCounter(PersistentStatus status)
        {
            if (!_effects.TryGetValue(status, out var effect))
                return 0;

            return effect.GetInitialCounter();
        }

        /// <summary>
        /// Checks if a status type has an accumulative effect registered.
        /// </summary>
        /// <param name="status">The status type to check.</param>
        /// <returns>True if the status has an accumulative effect.</returns>
        public bool HasAccumulativeEffect(PersistentStatus status)
        {
            return _effects.ContainsKey(status);
        }
    }

    /// <summary>
    /// Implementation of Badly Poisoned accumulative effect.
    /// Damage escalates: (counter * Max HP) / 16, counter increments each turn.
    /// </summary>
    internal class BadlyPoisonedEffect : IAccumulativeEffect
    {
        private const int BaseDivisor = 16;
        private const int MinDamage = 1;

        public PersistentStatus StatusType => PersistentStatus.BadlyPoisoned;

        public int CalculateValue(int maxHP, int counter)
        {
            int damage = (counter * maxHP) / BaseDivisor;
            return System.Math.Max(MinDamage, damage);
        }

        public int GetNextCounter(int currentCounter)
        {
            return currentCounter + 1;
        }

        public int GetInitialCounter()
        {
            return 1;
        }
    }
}
