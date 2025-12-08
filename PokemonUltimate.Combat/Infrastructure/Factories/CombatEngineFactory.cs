using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Damage.Definition;
using PokemonUltimate.Combat.Effects.Registry;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Combat.Engine.Validation;
using PokemonUltimate.Combat.Infrastructure.Logging;
using PokemonUltimate.Combat.Infrastructure.Providers;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Combat.Utilities;

namespace PokemonUltimate.Combat.Infrastructure.Factories
{
    /// <summary>
    /// Factory for creating CombatEngine instances with all required dependencies configured.
    /// Provides a convenient way to create a fully configured CombatEngine for production use.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public static class CombatEngineFactory
    {
        /// <summary>
        /// Creates a new CombatEngine with all default dependencies configured.
        /// Uses production-ready implementations for all components.
        /// </summary>
        /// <param name="randomSeed">Optional seed for random number generation. If null, uses system time.</param>
        /// <returns>A fully configured CombatEngine instance.</returns>
        public static CombatEngine Create(int? randomSeed = null)
        {
            // Create random provider
            var randomProvider = randomSeed.HasValue
                ? new RandomProvider(randomSeed.Value)
                : new RandomProvider();

            // Create factories
            var battleFieldFactory = new BattleFieldFactory();
            var battleQueueFactory = new BattleQueueFactory();
            var damageContextFactory = new DamageContextFactory();

            // Create helpers
            var accuracyChecker = new AccuracyChecker(randomProvider);
            var damagePipeline = new DamagePipeline(randomProvider);

            // Create effect processor registry
            var effectProcessorRegistry = new MoveEffectProcessorRegistry(randomProvider, damageContextFactory);

            // Create validators and loggers
            var stateValidator = new BattleStateValidator();
            var logger = new BattleLogger("CombatEngine");

            // Create and return CombatEngine with all dependencies
            return new CombatEngine(
                battleFieldFactory,
                battleQueueFactory,
                randomProvider,
                accuracyChecker,
                damagePipeline,
                effectProcessorRegistry,
                stateValidator,
                logger);
        }

        /// <summary>
        /// Creates helper instances for use in tests or standalone scenarios.
        /// </summary>
        /// <param name="randomSeed">Optional seed for random number generation.</param>
        /// <returns>A container with all helper instances.</returns>
        public static CombatHelpers CreateHelpers(int? randomSeed = null)
        {
            var randomProvider = randomSeed.HasValue
                ? new RandomProvider(randomSeed.Value)
                : new RandomProvider();

            return new CombatHelpers
            {
                RandomProvider = randomProvider,
                TurnOrderResolver = new TurnOrderResolver(randomProvider),
                TargetResolver = new TargetResolver(),
                AccuracyChecker = new AccuracyChecker(randomProvider),
                DamagePipeline = new DamagePipeline(randomProvider)
            };
        }
    }

    /// <summary>
    /// Container for combat helper instances.
    /// </summary>
    public class CombatHelpers
    {
        public IRandomProvider RandomProvider { get; set; }
        public TurnOrderResolver TurnOrderResolver { get; set; }
        public TargetResolver TargetResolver { get; set; }
        public AccuracyChecker AccuracyChecker { get; set; }
        public IDamagePipeline DamagePipeline { get; set; }
    }
}
