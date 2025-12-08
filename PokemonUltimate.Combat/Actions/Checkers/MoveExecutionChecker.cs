using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Constants;
using PokemonUltimate.Combat.Infrastructure.Messages;
using PokemonUltimate.Combat.Infrastructure.Messages.Definition;
using PokemonUltimate.Combat.Infrastructure.Providers;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Domain.Instances.Move;
using PokemonUltimate.Core.Domain.Instances.Pokemon;
using PokemonUltimate.Localization.Constants;
using PokemonUltimate.Localization.Services;

namespace PokemonUltimate.Combat.Actions.Checkers
{
    /// <summary>
    /// Verificador de ejecución de movimientos.
    /// Valida PP, Flinch, y condiciones de estado antes de ejecutar un movimiento.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/MEJORAS_PROPUESTAS.md`
    /// </remarks>
    public class MoveExecutionChecker
    {
        private readonly IRandomProvider _randomProvider;
        private readonly IBattleMessageFormatter _messageFormatter;

        /// <summary>
        /// Crea una nueva instancia del verificador de ejecución de movimientos.
        /// </summary>
        /// <param name="randomProvider">El proveedor de números aleatorios. Si es null, crea uno temporal.</param>
        /// <param name="messageFormatter">El formateador de mensajes. Si es null, crea uno por defecto.</param>
        public MoveExecutionChecker(IRandomProvider randomProvider = null, IBattleMessageFormatter messageFormatter = null)
        {
            _randomProvider = randomProvider ?? new RandomProvider();
            _messageFormatter = messageFormatter ?? new BattleMessageFormatter();
        }

        /// <summary>
        /// Resultado de la validación de ejecución de movimiento.
        /// </summary>
        public class MoveExecutionValidationResult
        {
            /// <summary>
            /// Indica si el movimiento puede ejecutarse.
            /// </summary>
            public bool CanExecute { get; set; }

            /// <summary>
            /// Mensaje de error si el movimiento no puede ejecutarse. Null si puede ejecutarse.
            /// </summary>
            public string FailureMessage { get; set; }

            /// <summary>
            /// El slot del usuario (para mensajes).
            /// </summary>
            public BattleSlot UserSlot { get; set; }
        }

        /// <summary>
        /// Valida si un movimiento puede ejecutarse (PP, Flinch, Status).
        /// </summary>
        /// <param name="moveInstance">La instancia del movimiento. No puede ser null.</param>
        /// <param name="userPokemon">El Pokemon que usa el movimiento. No puede ser null.</param>
        /// <param name="userSlot">El slot del usuario. Puede ser null.</param>
        /// <returns>El resultado de la validación.</returns>
        public MoveExecutionValidationResult ValidateExecution(MoveInstance moveInstance, PokemonInstance userPokemon, BattleSlot userSlot)
        {
            var result = new MoveExecutionValidationResult { CanExecute = true, UserSlot = userSlot };

            // Check PP
            if (!moveInstance.HasPP)
            {
                result.CanExecute = false;
                result.FailureMessage = _messageFormatter.Format(LocalizationKey.BattleNoPP, userPokemon.DisplayName);
                return result;
            }

            // Check Flinch (volatile status)
            if (userSlot != null && userSlot.HasVolatileStatus(VolatileStatus.Flinch))
            {
                result.CanExecute = false;
                result.FailureMessage = _messageFormatter.Format(LocalizationKey.BattleFlinched, userPokemon.DisplayName);
                // Consume flinch
                userSlot.RemoveVolatileStatus(VolatileStatus.Flinch);
                return result;
            }

            // Check persistent status conditions
            var statusResult = CheckStatusConditions(userPokemon);
            if (statusResult != null)
            {
                result.CanExecute = false;
                result.FailureMessage = statusResult;
                return result;
            }

            return result;
        }

        /// <summary>
        /// Verifica condiciones de estado persistentes (Sleep, Freeze, Paralysis).
        /// </summary>
        /// <param name="pokemon">El Pokemon a verificar. No puede ser null.</param>
        /// <returns>El mensaje de error si el Pokemon no puede moverse, null en caso contrario.</returns>
        private string CheckStatusConditions(PokemonInstance pokemon)
        {
            switch (pokemon.Status)
            {
                case PersistentStatus.Sleep:
                    return _messageFormatter.Format(LocalizationKey.BattleAsleep, pokemon.DisplayName);

                case PersistentStatus.Freeze:
                    var provider = LocalizationService.Instance;
                    return provider.GetString(LocalizationKey.BattleFrozen, pokemon.DisplayName);

                case PersistentStatus.Paralysis:
                    // 25% chance to be fully paralyzed
                    if (_randomProvider.Next(100) < StatusConstants.ParalysisFullParalysisChance)
                    {
                        return _messageFormatter.Format(LocalizationKey.BattleParalyzed, pokemon.DisplayName);
                    }
                    break;
            }

            return null;
        }
    }
}
