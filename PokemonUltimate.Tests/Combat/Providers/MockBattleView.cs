using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Combat.Providers
{
    /// <summary>
    /// Mock implementation of IBattleView for testing PlayerInputProvider.
    /// Allows setting return values for input methods.
    /// </summary>
    public class MockBattleView : IBattleView
    {
        // Visualization methods (no-op)
        public Task ShowMessage(string message) => Task.CompletedTask;
        public Task PlayDamageAnimation(BattleSlot slot) => Task.CompletedTask;
        public Task UpdateHPBar(BattleSlot slot) => Task.CompletedTask;
        public Task PlayMoveAnimation(BattleSlot user, BattleSlot target, string moveId) => Task.CompletedTask;
        public Task PlayFaintAnimation(BattleSlot slot) => Task.CompletedTask;
        public Task PlayStatusAnimation(BattleSlot slot, string statusName) => Task.CompletedTask;
        public Task ShowStatChange(BattleSlot slot, string statName, int stages) => Task.CompletedTask;
        public Task PlaySwitchOutAnimation(BattleSlot slot) => Task.CompletedTask;
        public Task PlaySwitchInAnimation(BattleSlot slot) => Task.CompletedTask;

        // Input methods - configurable return values
        public BattleActionType? ActionTypeToReturn { get; set; }
        public MoveInstance MoveToReturn { get; set; } = null; // Default to null to test cancellation
        public BattleSlot TargetToReturn { get; set; } = null; // Default to null to test cancellation
        public PokemonInstance PokemonToReturn { get; set; } = null; // Default to null to test cancellation

        public Task<BattleActionType> SelectActionType(BattleSlot slot)
        {
            return Task.FromResult(ActionTypeToReturn ?? BattleActionType.Fight);
        }

        public Task<MoveInstance> SelectMove(IReadOnlyList<MoveInstance> moves)
        {
            // Return MoveToReturn (can be null to simulate cancellation)
            // If not explicitly set, return first move (default behavior for other tests)
            return Task.FromResult(MoveToReturn ?? moves?.FirstOrDefault());
        }

        public Task<BattleSlot> SelectTarget(IReadOnlyList<BattleSlot> validTargets)
        {
            return Task.FromResult(TargetToReturn ?? validTargets?.FirstOrDefault());
        }

        public Task<PokemonInstance> SelectSwitch(IReadOnlyList<PokemonInstance> availablePokemon)
        {
            return Task.FromResult(PokemonToReturn ?? availablePokemon?.FirstOrDefault());
        }
    }
}

