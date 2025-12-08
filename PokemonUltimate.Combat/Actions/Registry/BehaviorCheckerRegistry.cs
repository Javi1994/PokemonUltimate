using PokemonUltimate.Combat.Actions.Checkers;
using PokemonUltimate.Combat.Damage.Definition;
using PokemonUltimate.Combat.Damage.Processors;
using PokemonUltimate.Combat.Effects.Registry;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Combat.Moves.MoveModifier;
using PokemonUltimate.Combat.Moves.Orchestrator;
using PokemonUltimate.Combat.Moves.Processors;
using PokemonUltimate.Combat.Utilities;
using PokemonUltimate.Combat.Utilities.Definition;

namespace PokemonUltimate.Combat.Actions.Registry
{
    /// <summary>
    /// Registry de verificadores de comportamiento (similar a MoveEffectProcessorRegistry).
    /// Centraliza el acceso a todos los checkers de comportamiento del sistema de combate.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/MEJORAS_PROPUESTAS.md`
    /// </remarks>
    public class BehaviorCheckerRegistry
    {
        private readonly OHKOPreventionChecker _ohkoPreventionChecker;
        private readonly StatChangeReversalChecker _statChangeReversalChecker;
        private readonly FocusPunchBehaviorChecker _focusPunchChecker;
        private readonly MultiTurnBehaviorChecker _multiTurnChecker;
        private readonly StatusApplicationChecker _statusApplicationChecker;
        private readonly DamageApplicationChecker _damageApplicationChecker;
        private readonly HealingApplicationChecker _healingApplicationChecker;
        private readonly FieldConditionApplicationChecker _fieldConditionChecker;
        private readonly StatChangeApplicationChecker _statChangeApplicationChecker;
        private readonly SwitchApplicationChecker _switchApplicationChecker;
        private readonly MoveExecutionChecker _moveExecutionChecker;
        private readonly ProtectionChecker _protectionChecker;
        private readonly SemiInvulnerableChecker _semiInvulnerableChecker;
        private readonly MoveStateChecker _moveStateChecker;
        private MoveEffectOrchestrator _moveEffectOrchestrator;
        private MoveAccuracyChecker _moveAccuracyChecker;

        /// <summary>
        /// Crea una nueva instancia del registry con todos los checkers inicializados.
        /// </summary>
        public BehaviorCheckerRegistry()
        {
            _ohkoPreventionChecker = new OHKOPreventionChecker();
            _statChangeReversalChecker = new StatChangeReversalChecker();
            _focusPunchChecker = new FocusPunchBehaviorChecker();
            _multiTurnChecker = new MultiTurnBehaviorChecker();
            _statusApplicationChecker = new StatusApplicationChecker();
            _damageApplicationChecker = new DamageApplicationChecker(this);
            _healingApplicationChecker = new HealingApplicationChecker();
            _fieldConditionChecker = new FieldConditionApplicationChecker();
            _statChangeApplicationChecker = new StatChangeApplicationChecker();
            _switchApplicationChecker = new SwitchApplicationChecker();
            _moveExecutionChecker = new MoveExecutionChecker();
            _protectionChecker = new ProtectionChecker();
            _semiInvulnerableChecker = new SemiInvulnerableChecker();
            _moveStateChecker = new MoveStateChecker();
        }

        /// <summary>
        /// Inicializa los procesadores que requieren dependencias externas.
        /// Debe ser llamado después de que todas las dependencias estén disponibles.
        /// </summary>
        /// <param name="damagePipeline">El pipeline de daño.</param>
        /// <param name="randomProvider">El proveedor de números aleatorios.</param>
        /// <param name="targetResolver">El resolvedor de objetivos.</param>
        /// <param name="effectProcessorRegistry">El registry de procesadores de efectos.</param>
        /// <param name="accuracyChecker">El verificador de precisión.</param>
        /// <param name="messageFormatter">El formateador de mensajes.</param>
        public void InitializeProcessors(
            IDamagePipeline damagePipeline,
            IRandomProvider randomProvider,
            ITargetResolver targetResolver,
            MoveEffectProcessorRegistry effectProcessorRegistry,
            AccuracyChecker accuracyChecker,
            Infrastructure.Messages.Definition.IBattleMessageFormatter messageFormatter = null)
        {
            // Create specialized processors
            var modificationApplier = new MoveModificationApplier(messageFormatter ?? new Infrastructure.Messages.BattleMessageFormatter());
            var damageProcessor = new MoveDamageProcessor(damagePipeline, randomProvider, targetResolver, this, messageFormatter ?? new Infrastructure.Messages.BattleMessageFormatter(), modificationApplier);
            var semiInvulnerableProcessor = new SemiInvulnerableMoveProcessor(this);

            // Create orchestrator with specialized processors
            _moveEffectOrchestrator = new MoveEffectOrchestrator(effectProcessorRegistry, damageProcessor, semiInvulnerableProcessor, modificationApplier);
            _moveAccuracyChecker = new MoveAccuracyChecker(accuracyChecker, this, messageFormatter);
        }

        /// <summary>
        /// Obtiene el verificador de prevención de OHKO (Focus Sash, Sturdy).
        /// </summary>
        /// <returns>El verificador de prevención de OHKO.</returns>
        public OHKOPreventionChecker GetOHKOPreventionChecker() => _ohkoPreventionChecker;

        /// <summary>
        /// Obtiene el verificador de inversión de cambios de estadísticas (Contrary).
        /// </summary>
        /// <returns>El verificador de inversión de cambios de stats.</returns>
        public StatChangeReversalChecker GetStatChangeReversalChecker() => _statChangeReversalChecker;

        /// <summary>
        /// Obtiene el verificador de comportamiento Focus Punch.
        /// </summary>
        /// <returns>El verificador de comportamiento Focus Punch.</returns>
        public FocusPunchBehaviorChecker GetFocusPunchChecker() => _focusPunchChecker;

        /// <summary>
        /// Obtiene el verificador de comportamiento Multi-Turn.
        /// </summary>
        /// <returns>El verificador de comportamiento Multi-Turn.</returns>
        public MultiTurnBehaviorChecker GetMultiTurnChecker() => _multiTurnChecker;

        /// <summary>
        /// Obtiene el verificador de aplicación de status.
        /// </summary>
        /// <returns>El verificador de aplicación de status.</returns>
        public StatusApplicationChecker GetStatusApplicationChecker() => _statusApplicationChecker;

        /// <summary>
        /// Obtiene el verificador de aplicación de daño.
        /// </summary>
        /// <returns>El verificador de aplicación de daño.</returns>
        public DamageApplicationChecker GetDamageApplicationChecker() => _damageApplicationChecker;

        /// <summary>
        /// Obtiene el verificador de aplicación de curación.
        /// </summary>
        /// <returns>El verificador de aplicación de curación.</returns>
        public HealingApplicationChecker GetHealingApplicationChecker() => _healingApplicationChecker;

        /// <summary>
        /// Obtiene el verificador de aplicación de condiciones de campo.
        /// </summary>
        /// <returns>El verificador de aplicación de condiciones de campo.</returns>
        public FieldConditionApplicationChecker GetFieldConditionChecker() => _fieldConditionChecker;

        /// <summary>
        /// Obtiene el verificador de aplicación de cambios de stats.
        /// </summary>
        /// <returns>El verificador de aplicación de cambios de stats.</returns>
        public StatChangeApplicationChecker GetStatChangeApplicationChecker() => _statChangeApplicationChecker;

        /// <summary>
        /// Obtiene el verificador de aplicación de cambios de Pokemon.
        /// </summary>
        /// <returns>El verificador de aplicación de cambios de Pokemon.</returns>
        public SwitchApplicationChecker GetSwitchApplicationChecker() => _switchApplicationChecker;

        /// <summary>
        /// Obtiene el verificador de ejecución de movimientos.
        /// </summary>
        /// <returns>El verificador de ejecución de movimientos.</returns>
        public MoveExecutionChecker GetMoveExecutionChecker() => _moveExecutionChecker;

        /// <summary>
        /// Obtiene el verificador de protección.
        /// </summary>
        /// <returns>El verificador de protección.</returns>
        public ProtectionChecker GetProtectionChecker() => _protectionChecker;

        /// <summary>
        /// Obtiene el verificador de semi-invulnerabilidad.
        /// </summary>
        /// <returns>El verificador de semi-invulnerabilidad.</returns>
        public SemiInvulnerableChecker GetSemiInvulnerableChecker() => _semiInvulnerableChecker;

        /// <summary>
        /// Obtiene el verificador de estados de movimientos.
        /// </summary>
        /// <returns>El verificador de estados de movimientos.</returns>
        public MoveStateChecker GetMoveStateChecker() => _moveStateChecker;

        /// <summary>
        /// Obtiene el orquestador de efectos de movimientos.
        /// </summary>
        /// <returns>El orquestador de efectos de movimientos.</returns>
        /// <exception cref="System.InvalidOperationException">Si los procesadores no han sido inicializados.</exception>
        public MoveEffectOrchestrator GetMoveEffectOrchestrator()
        {
            if (_moveEffectOrchestrator == null)
                throw new System.InvalidOperationException("Processors must be initialized before use. Call InitializeProcessors first.");
            return _moveEffectOrchestrator;
        }

        /// <summary>
        /// Obtiene el verificador de precisión de movimientos.
        /// </summary>
        /// <returns>El verificador de precisión de movimientos.</returns>
        /// <exception cref="System.InvalidOperationException">Si los procesadores no han sido inicializados.</exception>
        public MoveAccuracyChecker GetMoveAccuracyChecker()
        {
            if (_moveAccuracyChecker == null)
                throw new System.InvalidOperationException("Processors must be initialized before use. Call InitializeProcessors first.");
            return _moveAccuracyChecker;
        }
    }
}
