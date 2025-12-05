using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Combat.Engine
{
    /// <summary>
    /// Interface for processing entry hazards when a Pokemon switches in.
    /// Generates actions for damage, status application, and stat changes.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.14: Hazards System
    /// **Documentation**: See `docs/features/2-combat-system/2.14-hazards-system/README.md`
    /// </remarks>
    public interface IEntryHazardProcessor
    {
        /// <summary>
        /// Processes all entry hazards on the opposing side when a Pokemon switches in.
        /// </summary>
        /// <param name="slot">The slot the Pokemon is switching into.</param>
        /// <param name="pokemon">The Pokemon switching in.</param>
        /// <param name="field">The battlefield.</param>
        /// <param name="getHazardData">Function to get HazardData by type. Cannot be null.</param>
        /// <returns>List of actions to execute for entry hazards.</returns>
        List<BattleAction> ProcessHazards(BattleSlot slot, PokemonInstance pokemon, BattleField field, Func<HazardType, HazardData> getHazardData);
    }
}
