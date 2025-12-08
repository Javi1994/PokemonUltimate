using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Constants;
using PokemonUltimate.Combat.View.Definition;
using PokemonUltimate.Core.Domain.Instances.Move;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.View
{
    /// <summary>
    /// No-op implementation of IBattleView for testing.
    /// All methods complete immediately without doing anything.
    /// Input methods return default values (first option or null).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.7: Integration
    /// **Documentation**: See `docs/features/2-combat-system/2.7-integration/architecture.md`
    /// </remarks>
    public class NullBattleView : IBattleView
    {
        /// <summary>
        /// Singleton instance for convenience.
        /// </summary>
        public static readonly NullBattleView Instance = new NullBattleView();

        public Task ShowMessage(string message) => Task.CompletedTask;

        public Task PlayDamageAnimation(BattleSlot slot) => Task.CompletedTask;

        public Task UpdateHPBar(BattleSlot slot) => Task.CompletedTask;

        public Task PlayMoveAnimation(BattleSlot user, BattleSlot target, string moveId) => Task.CompletedTask;

        public Task PlayFaintAnimation(BattleSlot slot) => Task.CompletedTask;

        public Task PlayStatusAnimation(BattleSlot slot, string statusName) => Task.CompletedTask;

        public Task ShowStatChange(BattleSlot slot, string statName, int stages) => Task.CompletedTask;

        public Task PlaySwitchOutAnimation(BattleSlot slot) => Task.CompletedTask;

        public Task PlaySwitchInAnimation(BattleSlot slot) => Task.CompletedTask;

        // Input methods - return defaults for testing
        public Task<BattleActionType> SelectActionType(BattleSlot slot) =>
            Task.FromResult(BattleActionType.Fight);

        public Task<MoveInstance> SelectMove(IReadOnlyList<MoveInstance> moves) =>
            Task.FromResult(moves?.FirstOrDefault());

        public Task<BattleSlot> SelectTarget(IReadOnlyList<BattleSlot> validTargets) =>
            Task.FromResult(validTargets?.FirstOrDefault());

        public Task<PokemonInstance> SelectSwitch(IReadOnlyList<PokemonInstance> availablePokemon) =>
            Task.FromResult(availablePokemon?.FirstOrDefault());
    }
}

