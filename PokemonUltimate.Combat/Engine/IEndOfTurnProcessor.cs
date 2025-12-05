using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;

namespace PokemonUltimate.Combat.Engine
{
    /// <summary>
    /// Interface for processing end-of-turn effects.
    /// Processes status damage, weather damage, terrain effects, and item effects.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.8: End-of-Turn Effects
    /// **Documentation**: See `docs/features/2-combat-system/2.8-end-of-turn-effects/architecture.md`
    /// </remarks>
    public interface IEndOfTurnProcessor
    {
        /// <summary>
        /// Processes all end-of-turn effects for the battlefield.
        /// Returns actions to be executed (status damage, healing, etc.).
        /// </summary>
        /// <param name="field">The battlefield. Cannot be null.</param>
        /// <returns>List of actions to execute for end-of-turn effects.</returns>
        List<BattleAction> ProcessEffects(BattleField field);
    }
}
