using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Extensions;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Combat.Statistics;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Processors.Phases
{
    /// <summary>
    /// Observer that detects specific action types and calls appropriate processors.
    /// This decouples actions from processors - actions don't need to know about processors.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class ActionProcessorObserver : IBattleActionObserver
    {
        private readonly DamageTakenProcessor _damageTakenProcessor;
        private readonly ContactReceivedProcessor _contactReceivedProcessor;
        private readonly WeatherChangeProcessor _weatherChangeProcessor;
        private readonly SwitchInProcessor _switchInProcessor;
        private readonly BattleQueue _queue;

        /// <summary>
        /// Creates a new ActionProcessorObserver with all required processors.
        /// </summary>
        /// <param name="queue">The battle queue to insert processor-generated actions. Cannot be null.</param>
        /// <param name="damageTakenProcessor">Processor for damage-taken effects. If null, creates a temporary one.</param>
        /// <param name="contactReceivedProcessor">Processor for contact-received effects. If null, creates a temporary one.</param>
        /// <param name="weatherChangeProcessor">Processor for weather-change effects. If null, creates a temporary one.</param>
        /// <param name="switchInProcessor">Processor for switch-in effects. If null, creates a temporary one.</param>
        /// <exception cref="ArgumentNullException">If queue is null.</exception>
        public ActionProcessorObserver(
            BattleQueue queue,
            DamageTakenProcessor damageTakenProcessor = null,
            ContactReceivedProcessor contactReceivedProcessor = null,
            WeatherChangeProcessor weatherChangeProcessor = null,
            SwitchInProcessor switchInProcessor = null)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));

            // Create processors if not provided (temporary until full DI refactoring)
            _damageTakenProcessor = damageTakenProcessor ?? new DamageTakenProcessor();
            _contactReceivedProcessor = contactReceivedProcessor ?? new ContactReceivedProcessor();
            _weatherChangeProcessor = weatherChangeProcessor ?? new WeatherChangeProcessor();

            var damageContextFactory = new DamageContextFactory();
            _switchInProcessor = switchInProcessor ?? new SwitchInProcessor(damageContextFactory);
        }

        /// <summary>
        /// Called when an action's logic is executed.
        /// Detects specific action types and calls appropriate processors.
        /// </summary>
        public void OnActionExecuted(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions)
        {
            if (action == null || field == null)
                return;

            // Detect DamageAction and process damage-taken effects
            if (action is DamageAction damageAction)
            {
                ProcessDamageAction(damageAction, field);
            }

            // Detect SetWeatherAction and process weather-change effects
            if (action is SetWeatherAction weatherAction)
            {
                ProcessWeatherAction(weatherAction, field);
            }

            // Detect SwitchAction and process switch-in effects
            if (action is SwitchAction switchAction)
            {
                ProcessSwitchAction(switchAction, field);
            }
        }

        /// <summary>
        /// Processes damage-taken and contact-received effects after a DamageAction.
        /// </summary>
        private void ProcessDamageAction(DamageAction damageAction, BattleField field)
        {
            if (damageAction.Target == null || !damageAction.Target.IsActive())
                return;

            // Skip if Pokemon fainted (damage was already applied in ExecuteLogic)
            if (damageAction.Target.Pokemon.IsFainted)
                return;

            // Process abilities/items that activate when damage is taken
            // Only process if damage was actually dealt
            if (damageAction.Context?.FinalDamage > 0)
            {
                var damageTakenActions = _damageTakenProcessor.ProcessDamageTaken(damageAction.Target, field);
                if (damageTakenActions.Count > 0)
                {
                    _queue.InsertAtFront(damageTakenActions);
                }
            }

            // Process contact received effects if move makes contact
            if (damageAction.Context?.Move != null &&
                damageAction.Context.Move.MakesContact &&
                damageAction.Context.FinalDamage > 0 &&
                !damageAction.Target.Pokemon.IsFainted)
            {
                var contactActions = _contactReceivedProcessor.ProcessContactReceived(
                    damageAction.Target, damageAction.User, field);
                if (contactActions.Count > 0)
                {
                    _queue.InsertAtFront(contactActions);
                }
            }
        }

        /// <summary>
        /// Processes weather-change effects after a SetWeatherAction.
        /// </summary>
        private void ProcessWeatherAction(SetWeatherAction weatherAction, BattleField field)
        {
            // Only process if weather was actually set (not cleared)
            if (weatherAction.Weather == Weather.None)
                return;

            var weatherChangeActions = _weatherChangeProcessor.ProcessWeatherChange(field);
            if (weatherChangeActions.Count > 0)
            {
                _queue.InsertAtFront(weatherChangeActions);
            }
        }

        /// <summary>
        /// Processes switch-in effects after a SwitchAction.
        /// </summary>
        private void ProcessSwitchAction(SwitchAction switchAction, BattleField field)
        {
            if (switchAction.Slot == null || switchAction.NewPokemon == null)
                return;

            var switchInActions = _switchInProcessor.ProcessSwitchIn(
                switchAction.Slot, switchAction.NewPokemon, field);
            if (switchInActions.Count > 0)
            {
                _queue.InsertAtFront(switchInActions);
            }
        }

        /// <summary>
        /// Called when a battle turn starts.
        /// </summary>
        public void OnTurnStart(int turnNumber, BattleField field)
        {
            // No processing needed here
        }

        /// <summary>
        /// Called when a battle turn ends.
        /// </summary>
        public void OnTurnEnd(int turnNumber, BattleField field)
        {
            // No processing needed here
        }

        /// <summary>
        /// Called when a battle starts.
        /// </summary>
        public void OnBattleStart(BattleField field)
        {
            // No processing needed here
        }

        /// <summary>
        /// Called when a battle ends.
        /// </summary>
        public void OnBattleEnd(BattleOutcome outcome, BattleField field)
        {
            // No processing needed here
        }
    }
}
