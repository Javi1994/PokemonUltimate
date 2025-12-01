using System.Threading.Tasks;

namespace PokemonUltimate.Core.Combat
{
    /// <summary>
    /// No-op implementation of IBattleView for testing.
    /// All methods complete immediately without doing anything.
    /// </summary>
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
    }
}

