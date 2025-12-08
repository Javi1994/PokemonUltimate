using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;

namespace PokemonUltimate.Combat.Infrastructure.Providers.Definition
{
    /// <summary>
    /// Provides actions for a Pokemon slot during battle.
    /// Abstracts the source of decisions (player input, AI, etc.).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.7: Integration
    /// **Documentation**: See `docs/features/2-combat-system/2.7-integration/architecture.md`
    /// </remarks>
    public interface IActionProvider
    {
        /// <summary>
        /// Gets the action the Pokemon wants to perform this turn.
        /// </summary>
        /// <param name="field">The current battlefield state.</param>
        /// <param name="mySlot">The slot requesting an action.</param>
        /// <returns>The action to perform this turn.</returns>
        Task<BattleAction> GetAction(BattleField field, BattleSlot mySlot);
    }
}

