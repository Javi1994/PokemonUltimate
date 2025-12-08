using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Engine.Service;
using PokemonUltimate.Combat.Engine.Validation.Definition;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Logging.Definition;
using PokemonUltimate.Combat.Utilities;
using PokemonUltimate.Combat.View.Definition;

namespace PokemonUltimate.Combat.Engine.TurnFlow
{
    /// <summary>
    /// Contexto mutable que contiene el estado del turno durante la ejecución.
    /// Pasa a través de todos los steps del pipeline.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `TURN_STEPS_PROPOSAL.md`
    /// </remarks>
    public class TurnContext
    {
        /// <summary>
        /// El campo de batalla. No puede ser null.
        /// </summary>
        public BattleField Field { get; set; }

        /// <summary>
        /// La cola de acciones. No puede ser null.
        /// </summary>
        public BattleQueueService QueueService { get; set; }

        /// <summary>
        /// La vista de batalla para feedback visual. No puede ser null.
        /// </summary>
        public IBattleView View { get; set; }

        /// <summary>
        /// El número del turno actual.
        /// </summary>
        public int TurnNumber { get; set; }

        /// <summary>
        /// Acciones recolectadas durante el turno.
        /// </summary>
        public List<BattleAction> CollectedActions { get; set; } = new List<BattleAction>();

        /// <summary>
        /// Acciones ordenadas por prioridad y velocidad.
        /// </summary>
        public List<BattleAction> SortedActions { get; set; } = new List<BattleAction>();

        /// <summary>
        /// Indica si hay Pokemon debilitados que necesitan ser manejados.
        /// </summary>
        public bool HasFaintedPokemon { get; set; }

        /// <summary>
        /// El validador de estado de batalla.
        /// </summary>
        public IBattleStateValidator StateValidator { get; set; }

        /// <summary>
        /// El logger de batalla.
        /// </summary>
        public IBattleLogger Logger { get; set; }

        /// <summary>
        /// El resolvedor de orden de turno.
        /// </summary>
        public TurnOrderResolver TurnOrderResolver { get; set; }

        /// <summary>
        /// El resolvedor de targets.
        /// </summary>
        public Utilities.TargetResolver TargetResolver { get; set; }

        #region Desacoplamiento de Steps

        /// <summary>
        /// Validaciones de movimientos realizadas por MoveValidationStep.
        /// </summary>
        public Dictionary<UseMoveAction, bool> MoveValidations { get; set; }
            = new Dictionary<UseMoveAction, bool>();

        /// <summary>
        /// Cálculos de daño realizados por MoveDamageCalculationStep.
        /// </summary>
        public Dictionary<UseMoveAction, DamageContext> DamageCalculations { get; set; }
            = new Dictionary<UseMoveAction, DamageContext>();

        /// <summary>
        /// Acciones generadas por steps que necesitan ser procesadas.
        /// </summary>
        public List<BattleAction> GeneratedActions { get; set; }
            = new List<BattleAction>();

        /// <summary>
        /// Acciones ya procesadas (para steps reactivos).
        /// </summary>
        public List<BattleAction> ProcessedActions { get; set; }
            = new List<BattleAction>();

        /// <summary>
        /// Resultados de checks de precisión.
        /// </summary>
        public Dictionary<UseMoveAction, bool> AccuracyChecks { get; set; }
            = new Dictionary<UseMoveAction, bool>();

        /// <summary>
        /// Resultados de checks de protección.
        /// </summary>
        public Dictionary<UseMoveAction, bool> ProtectionChecks { get; set; }
            = new Dictionary<UseMoveAction, bool>();

        #endregion
    }
}
