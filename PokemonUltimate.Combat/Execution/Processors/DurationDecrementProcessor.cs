using System;
using PokemonUltimate.Combat.Execution.Battle;
using PokemonUltimate.Combat.Execution.Processors.Definition;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Core.Data.Constants;

namespace PokemonUltimate.Combat.Execution.Processors
{
    /// <summary>
    /// Processes duration decrement for weather, terrain, and side conditions.
    /// Modifies battle state directly without generating actions.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class DurationDecrementProcessor : IStateModifyingPhaseProcessor
    {
        /// <summary>
        /// Gets the phase this processor handles.
        /// </summary>
        public BattlePhase Phase => BattlePhase.DurationDecrement;

        /// <summary>
        /// Processes duration decrement for weather, terrain, and side conditions.
        /// </summary>
        /// <param name="field">The battlefield. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If field is null.</exception>
        public void Process(BattleField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            // Decrement weather duration
            field.DecrementWeatherDuration();

            // Decrement terrain duration
            field.DecrementTerrainDuration();

            // Decrement side condition durations
            field.PlayerSide.DecrementAllSideConditionDurations();
            field.EnemySide.DecrementAllSideConditionDurations();
        }
    }
}
