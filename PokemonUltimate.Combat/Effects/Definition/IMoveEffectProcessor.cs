using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Effects.Definition;

namespace PokemonUltimate.Combat.Effects.Definition
{
    /// <summary>
    /// Interface for processing move effects.
    /// Each implementation handles a specific type of move effect.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public interface IMoveEffectProcessor
    {
        /// <summary>
        /// Processes a move effect and adds corresponding actions to the list.
        /// </summary>
        /// <param name="effect">The effect to process. Cannot be null.</param>
        /// <param name="user">The slot using the move. Cannot be null.</param>
        /// <param name="target">The target slot. Cannot be null.</param>
        /// <param name="move">The move data. Cannot be null.</param>
        /// <param name="field">The battlefield. Cannot be null.</param>
        /// <param name="damageDealt">The damage dealt by the move (for effects that depend on damage).</param>
        /// <param name="actions">The list of actions to add to. Cannot be null.</param>
        void Process(
            IMoveEffect effect,
            BattleSlot user,
            BattleSlot target,
            MoveData move,
            BattleField field,
            int damageDealt,
            List<BattleAction> actions);
    }
}
