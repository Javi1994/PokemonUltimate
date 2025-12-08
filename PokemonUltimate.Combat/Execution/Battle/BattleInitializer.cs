using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Combat.Infrastructure.Factories;
using PokemonUltimate.Combat.Infrastructure.Factories.Definition;
using PokemonUltimate.Combat.Infrastructure.Providers;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Combat.Infrastructure.Validation;
using PokemonUltimate.Combat.Infrastructure.Validation.Definition;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.Execution.Battle
{
    /// <summary>
    /// Handles initialization of the battlefield and battle setup.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class BattleInitializer
    {
        private readonly IBattleFieldFactory _battleFieldFactory;
        private readonly IBattleQueueFactory _battleQueueFactory;
        private readonly IBattleStateValidator _stateValidator;

        /// <summary>
        /// Creates a new BattleInitializer with required dependencies.
        /// </summary>
        /// <param name="battleFieldFactory">Factory for creating BattleField instances. Cannot be null.</param>
        /// <param name="battleQueueFactory">Factory for creating BattleQueue instances. Cannot be null.</param>
        /// <param name="stateValidator">Battle state validator. If null, creates a default one.</param>
        /// <exception cref="ArgumentNullException">If battleFieldFactory or battleQueueFactory is null.</exception>
        public BattleInitializer(
            IBattleFieldFactory battleFieldFactory,
            IBattleQueueFactory battleQueueFactory,
            IBattleStateValidator stateValidator = null)
        {
            _battleFieldFactory = battleFieldFactory ?? throw new ArgumentNullException(nameof(battleFieldFactory));
            _battleQueueFactory = battleQueueFactory ?? throw new ArgumentNullException(nameof(battleQueueFactory));
            _stateValidator = stateValidator ?? new BattleStateValidator();
        }

        /// <summary>
        /// Initializes the battlefield with parties and action providers.
        /// </summary>
        /// <param name="rules">Battle configuration. Cannot be null.</param>
        /// <param name="playerParty">Player's Pokemon party. Cannot be null.</param>
        /// <param name="enemyParty">Enemy's Pokemon party. Cannot be null.</param>
        /// <param name="playerProvider">Provider for player actions. Cannot be null.</param>
        /// <param name="enemyProvider">Provider for enemy actions. Cannot be null.</param>
        /// <returns>A tuple containing the initialized BattleField and BattleQueue.</returns>
        /// <exception cref="ArgumentNullException">If any parameter is null.</exception>
        public (BattleField Field, BattleQueue Queue) Initialize(
            BattleRules rules,
            IReadOnlyList<PokemonInstance> playerParty,
            IReadOnlyList<PokemonInstance> enemyParty,
            IActionProvider playerProvider,
            IActionProvider enemyProvider)
        {
            if (rules == null)
                throw new ArgumentNullException(nameof(rules));
            if (playerParty == null)
                throw new ArgumentNullException(nameof(playerParty), ErrorMessages.PartyCannotBeNull);
            if (enemyParty == null)
                throw new ArgumentNullException(nameof(enemyParty), ErrorMessages.PartyCannotBeNull);
            if (playerProvider == null)
                throw new ArgumentNullException(nameof(playerProvider));
            if (enemyProvider == null)
                throw new ArgumentNullException(nameof(enemyProvider));

            // Create BattleField using factory
            var field = _battleFieldFactory.Create(rules, playerParty, enemyParty);

            // Assign action providers to slots
            foreach (var slot in field.PlayerSide.Slots)
            {
                slot.ActionProvider = playerProvider;
            }

            foreach (var slot in field.EnemySide.Slots)
            {
                slot.ActionProvider = enemyProvider;
            }

            // Create BattleQueue using factory
            var queue = _battleQueueFactory.Create();

            // Validate initial battle state
            var errors = _stateValidator.ValidateField(field);
            if (errors.Count > 0)
            {
                var errorMessage = "Battle state validation failed:\n" + string.Join("\n", errors);
                throw new InvalidOperationException(errorMessage);
            }

            return (field, queue);
        }
    }
}
