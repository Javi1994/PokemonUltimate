using PokemonUltimate.Combat.Handlers.Effects;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Combat.Infrastructure.Factories;

namespace PokemonUltimate.Combat.Engine.Service
{
    /// <summary>
    /// Factory para crear servicios y handlers relacionados con la batalla.
    /// Centraliza la creación de objetos para reducir responsabilidades en CombatEngine.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `ENGINE_RESPONSIBILITY_REVIEW.md`
    /// </remarks>
    public class BattleServicesFactory
    {
        private readonly DamageContextFactory _damageContextFactory;
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        /// <summary>
        /// Creates a new BattleServicesFactory.
        /// </summary>
        /// <param name="damageContextFactory">Factory for creating damage contexts. Cannot be null.</param>
        /// <param name="handlerRegistry">Handler registry. Cannot be null.</param>
        public BattleServicesFactory(DamageContextFactory damageContextFactory, CombatEffectHandlerRegistry handlerRegistry)
        {
            _damageContextFactory = damageContextFactory ?? throw new System.ArgumentNullException(nameof(damageContextFactory));
            _handlerRegistry = handlerRegistry ?? throw new System.ArgumentNullException(nameof(handlerRegistry));
        }

        /// <summary>
        /// Creates all end-of-turn effect handlers.
        /// </summary>
        /// <returns>A tuple containing all end-of-turn handlers.</returns>
        public (StatusDamageHandler statusDamage, WeatherDamageHandler weatherDamage, TerrainHealingHandler terrainHealing, EntryHazardHandler entryHazard) CreateEndOfTurnHandlers()
        {
            return (
                new StatusDamageHandler(_damageContextFactory),
                new WeatherDamageHandler(_damageContextFactory),
                new TerrainHealingHandler(),
                new EntryHazardHandler(_damageContextFactory, _handlerRegistry)
            );
        }
    }
}
