using System.Collections.Generic;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;

namespace PokemonUltimate.Combat.Engine.Processors.Definition
{
    /// <summary>
    /// Processor that generates actions during a phase.
    /// These processors analyze the battlefield state and generate actions to execute.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public interface IActionGeneratingPhaseProcessor : IBattlePhaseProcessor
    {
        /// <summary>
        /// Processes the phase and generates actions to execute.
        /// </summary>
        /// <param name="field">The battlefield. Cannot be null.</param>
        /// <returns>List of actions to execute for this phase.</returns>
        Task<List<BattleAction>> ProcessAsync(BattleField field);
    }
}
