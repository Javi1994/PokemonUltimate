using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Combat.Events;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Combat.Providers;

namespace PokemonUltimate.Tests.Systems.Combat.Engine
{
    /// <summary>
    /// Helper class for creating CombatEngine instances in tests.
    /// </summary>
    public static class CombatEngineTestHelper
    {
        /// <summary>
        /// Creates a CombatEngine with all required dependencies for testing.
        /// </summary>
        /// <param name="seed">Optional seed for RandomProvider. Default is 42 for reproducible tests.</param>
        /// <returns>A new CombatEngine instance ready for testing.</returns>
        public static CombatEngine CreateCombatEngine(int seed = 42)
        {
            var randomProvider = new RandomProvider(seed);
            var battleFieldFactory = new BattleFieldFactory();
            var battleQueueFactory = new BattleQueueFactory();
            var damageContextFactory = new DamageContextFactory();
            var endOfTurnProcessor = new EndOfTurnProcessor(damageContextFactory);
            var battleTriggerProcessor = new BattleTriggerProcessor();

            return new CombatEngine(
                battleFieldFactory,
                battleQueueFactory,
                randomProvider,
                endOfTurnProcessor,
                battleTriggerProcessor);
        }
    }
}
