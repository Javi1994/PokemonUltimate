using PokemonUltimate.Combat.Field;

namespace PokemonUltimate.Combat.Engine.Processors.Definition
{
    /// <summary>
    /// Processor that modifies battle state directly (no actions generated).
    /// These processors modify the battlefield state directly without generating actions.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public interface IStateModifyingPhaseProcessor : IBattlePhaseProcessor
    {
        /// <summary>
        /// Processes the phase and modifies battle state directly.
        /// </summary>
        /// <param name="field">The battlefield. Cannot be null.</param>
        void Process(BattleField field);
    }
}
