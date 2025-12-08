using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Factories.Definition;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.Infrastructure.Factories
{
    /// <summary>
    /// Factory for creating BattleField instances.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class BattleFieldFactory : IBattleFieldFactory
    {
        /// <summary>
        /// Creates a new BattleField and initializes it with the provided rules and parties.
        /// </summary>
        /// <param name="rules">Battle configuration. Cannot be null.</param>
        /// <param name="playerParty">Player's Pokemon party. Cannot be null.</param>
        /// <param name="enemyParty">Enemy's Pokemon party. Cannot be null.</param>
        /// <returns>A new initialized BattleField.</returns>
        /// <exception cref="ArgumentNullException">If any parameter is null.</exception>
        public BattleField Create(
            BattleRules rules,
            IReadOnlyList<PokemonInstance> playerParty,
            IReadOnlyList<PokemonInstance> enemyParty)
        {
            if (rules == null)
                throw new ArgumentNullException(nameof(rules));
            if (playerParty == null)
                throw new ArgumentNullException(nameof(playerParty), ErrorMessages.PartyCannotBeNull);
            if (enemyParty == null)
                throw new ArgumentNullException(nameof(enemyParty), ErrorMessages.PartyCannotBeNull);

            var field = new BattleField();
            field.Initialize(rules, playerParty, enemyParty);
            return field;
        }
    }
}
