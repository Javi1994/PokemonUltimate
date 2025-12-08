using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine.TurnFlow.Definition;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Effects;
using PokemonUltimate.Combat.Utilities.Extensions;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Engine.TurnFlow.Steps
{
    /// <summary>
    /// Step que procesa efectos de fin de turno (daño de status, clima, terreno, etc.).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.8: End-of-Turn Effects
    /// **Documentation**: See `RESPONSIBILITY_REVIEW.md`
    /// </remarks>
    public class EndOfTurnEffectsStep : ITurnStep
    {
        private readonly StatusDamageHandler _statusDamageHandler;
        private readonly WeatherDamageHandler _weatherDamageHandler;
        private readonly TerrainHealingHandler _terrainHealingHandler;

        public string StepName => "End of Turn Effects";
        public bool ExecuteEvenIfFainted => false;

        public EndOfTurnEffectsStep(
            StatusDamageHandler statusDamageHandler,
            WeatherDamageHandler weatherDamageHandler,
            TerrainHealingHandler terrainHealingHandler)
        {
            _statusDamageHandler = statusDamageHandler ?? throw new ArgumentNullException(nameof(statusDamageHandler));
            _weatherDamageHandler = weatherDamageHandler ?? throw new ArgumentNullException(nameof(weatherDamageHandler));
            _terrainHealingHandler = terrainHealingHandler ?? throw new ArgumentNullException(nameof(terrainHealingHandler));
        }

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            var actions = new List<BattleAction>();

            // Process status damage for all active slots
            foreach (var slot in context.Field.GetAllActiveSlots())
            {
                if (!slot.IsActive())
                    continue;

                var statusActions = _statusDamageHandler.ProcessStatusDamage(slot, context.Field);
                actions.AddRange(statusActions);
            }

            // Process weather damage for all active slots
            var weatherActions = _weatherDamageHandler.ProcessWeatherDamage(context.Field);
            actions.AddRange(weatherActions);

            // Process terrain healing for all active slots
            var terrainHealingActions = _terrainHealingHandler.ProcessTerrainHealing(context.Field);
            actions.AddRange(terrainHealingActions);

            // Remove Protect status at end of turn (but keep consecutive uses counter)
            // Reset damage tracking for Counter/Mirror Coat
            // Prepare semi-invulnerable moves for attack turn (if charging)
            ResetVolatileStatus(context.Field);

            if (actions.Count > 0)
            {
                context.GeneratedActions.AddRange(actions);
            }

            return await Task.FromResult(true);
        }

        /// <summary>
        /// Resets volatile status and damage tracking at end of turn.
        /// </summary>
        private void ResetVolatileStatus(BattleField field)
        {
            foreach (var slot in field.GetAllActiveSlots())
            {
                if (slot.HasVolatileStatus(VolatileStatus.Protected))
                {
                    slot.RemoveVolatileStatus(VolatileStatus.Protected);
                }

                // If semi-invulnerable and charging, mark as ready for attack turn
                if (slot.HasVolatileStatus(VolatileStatus.SemiInvulnerable) && slot.IsSemiInvulnerableCharging)
                {
                    slot.SetSemiInvulnerableReady();
                }

                // Reset damage tracking for next turn (but keep stat stages and other volatile status)
                slot.ResetDamageTracking();
            }
        }

    }
}
