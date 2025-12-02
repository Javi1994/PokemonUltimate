using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;

namespace PokemonUltimate.Combat
{
    /// <summary>
    /// Provides actions for a Pokemon slot during battle.
    /// Abstracts the source of decisions (player input, AI, etc.).
    /// </summary>
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

