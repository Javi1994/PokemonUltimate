using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;

namespace PokemonUltimate.DeveloperTools.Runners
{
    /// <summary>
    /// AI provider that never provides an action (always returns null).
    /// Used for testing scenarios where we want only one side to act.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.6: Move Debugger
    /// **Documentation**: See `docs/features/6-development-tools/6.6-move-debugger/README.md`
    /// </remarks>
    public class NoActionAI : ActionProviderBase
    {
        /// <summary>
        /// Always returns null, meaning this Pokemon will not act.
        /// </summary>
        public override Task<BattleAction> GetAction(BattleField field, BattleSlot mySlot)
        {
            // Always return null - this Pokemon will skip its turn
            return Task.FromResult<BattleAction>(null);
        }
    }
}
