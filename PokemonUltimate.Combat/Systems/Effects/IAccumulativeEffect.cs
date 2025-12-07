using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Systems.Effects
{
    /// <summary>
    /// Interface for effects that accumulate over time (e.g., Badly Poisoned counter).
    /// Encapsulates the logic for calculating damage/effects based on turn counter.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.8: End-of-Turn Effects
    /// **Documentation**: See `docs/features/2-combat-system/2.8-end-of-turn-effects/architecture.md`
    /// </remarks>
    public interface IAccumulativeEffect
    {
        /// <summary>
        /// Gets the status type this effect applies to.
        /// </summary>
        PersistentStatus StatusType { get; }

        /// <summary>
        /// Calculates the damage or effect value based on the current counter.
        /// </summary>
        /// <param name="maxHP">The Pokemon's maximum HP.</param>
        /// <param name="counter">The current turn counter.</param>
        /// <returns>The calculated damage or effect value.</returns>
        int CalculateValue(int maxHP, int counter);

        /// <summary>
        /// Gets the next counter value after processing this turn.
        /// </summary>
        /// <param name="currentCounter">The current counter value.</param>
        /// <returns>The counter value for the next turn.</returns>
        int GetNextCounter(int currentCounter);

        /// <summary>
        /// Gets the initial counter value when the status is first applied.
        /// </summary>
        /// <returns>The initial counter value.</returns>
        int GetInitialCounter();
    }
}
