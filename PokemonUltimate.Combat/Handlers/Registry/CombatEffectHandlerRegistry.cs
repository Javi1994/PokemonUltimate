using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Abilities;
using PokemonUltimate.Combat.Handlers.Checkers;
using PokemonUltimate.Combat.Handlers.Definition;
using PokemonUltimate.Combat.Handlers.Effects;
using PokemonUltimate.Combat.Handlers.Items;
using PokemonUltimate.Combat.Infrastructure.Factories;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Data.Effects;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Handlers.Registry
{
    /// <summary>
    /// Registry unificado para handlers de efectos de habilidades e items.
    /// Reemplaza y unifica el sistema de BehaviorCheckers y handlers de procesadores.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public class CombatEffectHandlerRegistry
    {
        private bool _isInitialized = false;

        /// <summary>
        /// Indica si el registry ha sido inicializado.
        /// </summary>
        public bool IsInitialized => _isInitialized;
        // Registros por trigger y efecto para abilities
        private readonly Dictionary<(AbilityTrigger trigger, AbilityEffect effect), IAbilityEffectHandler> _abilityHandlersByTriggerEffect
            = new Dictionary<(AbilityTrigger, AbilityEffect), IAbilityEffectHandler>();

        // Registro por ID de ability para búsqueda rápida
        private readonly Dictionary<string, IAbilityEffectHandler> _abilityHandlersById
            = new Dictionary<string, IAbilityEffectHandler>();

        // Registros por trigger para items
        private readonly Dictionary<ItemTrigger, List<IItemEffectHandler>> _itemHandlersByTrigger
            = new Dictionary<ItemTrigger, List<IItemEffectHandler>>();

        // Registro por ID de item para búsqueda rápida
        private readonly Dictionary<string, IItemEffectHandler> _itemHandlersById
            = new Dictionary<string, IItemEffectHandler>();

        // Registro de handlers de efectos de movimientos por tipo
        private readonly Dictionary<System.Type, Definition.IMoveEffectHandler> _moveEffectHandlers
            = new Dictionary<System.Type, Definition.IMoveEffectHandler>();

        /// <summary>
        /// Registra un handler de habilidad.
        /// </summary>
        /// <param name="handler">El handler a registrar. No puede ser null.</param>
        /// <exception cref="ArgumentNullException">Si handler es null.</exception>
        public void RegisterAbilityHandler(IAbilityEffectHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler), ErrorMessages.HandlerCannotBeNull);

            // Registrar por trigger y efecto
            if (handler.Trigger != AbilityTrigger.None && handler.Effect != AbilityEffect.None)
            {
                var key = (handler.Trigger, handler.Effect);
                _abilityHandlersByTriggerEffect[key] = handler;
            }
        }

        /// <summary>
        /// Registra un handler de habilidad por ID específico.
        /// </summary>
        /// <param name="abilityId">El ID de la habilidad.</param>
        /// <param name="handler">El handler a registrar. No puede ser null.</param>
        /// <exception cref="ArgumentNullException">Si abilityId o handler son null.</exception>
        public void RegisterAbilityHandlerById(string abilityId, IAbilityEffectHandler handler)
        {
            if (string.IsNullOrEmpty(abilityId))
                throw new ArgumentNullException(nameof(abilityId), ErrorMessages.AbilityIdCannotBeNull);
            if (handler == null)
                throw new ArgumentNullException(nameof(handler), ErrorMessages.HandlerCannotBeNull);

            _abilityHandlersById[abilityId] = handler;
        }

        /// <summary>
        /// Registra un handler de item.
        /// </summary>
        /// <param name="handler">El handler a registrar. No puede ser null.</param>
        /// <exception cref="ArgumentNullException">Si handler es null.</exception>
        public void RegisterItemHandler(IItemEffectHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler), ErrorMessages.HandlerCannotBeNull);

            // Registrar por trigger
            if (handler.Trigger != ItemTrigger.None)
            {
                if (!_itemHandlersByTrigger.ContainsKey(handler.Trigger))
                {
                    _itemHandlersByTrigger[handler.Trigger] = new List<IItemEffectHandler>();
                }
                _itemHandlersByTrigger[handler.Trigger].Add(handler);
            }
        }

        /// <summary>
        /// Registra un handler de item por ID específico.
        /// </summary>
        /// <param name="itemId">El ID del item.</param>
        /// <param name="handler">El handler a registrar. No puede ser null.</param>
        /// <exception cref="ArgumentNullException">Si itemId o handler son null.</exception>
        public void RegisterItemHandlerById(string itemId, IItemEffectHandler handler)
        {
            if (string.IsNullOrEmpty(itemId))
                throw new ArgumentNullException(nameof(itemId), ErrorMessages.ItemIdCannotBeNull);
            if (handler == null)
                throw new ArgumentNullException(nameof(handler), ErrorMessages.HandlerCannotBeNull);

            _itemHandlersById[itemId] = handler;
        }

        /// <summary>
        /// Procesa una habilidad cuando se activa un trigger específico.
        /// Usado por Steps para generar acciones.
        /// </summary>
        /// <param name="ability">La habilidad a procesar. No puede ser null.</param>
        /// <param name="slot">El slot del Pokemon con esta habilidad. No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <param name="trigger">El trigger que se activó.</param>
        /// <returns>Lista de acciones generadas. Nunca null, puede estar vacía.</returns>
        public List<BattleAction> ProcessAbility(AbilityData ability, BattleSlot slot, BattleField field, AbilityTrigger trigger)
        {
            return ProcessAbility(ability, slot, field, trigger, null);
        }

        /// <summary>
        /// Procesa una habilidad cuando se activa un trigger específico, con contexto adicional.
        /// Usado por Steps para generar acciones cuando se necesita información adicional (como el attacker en contact effects).
        /// </summary>
        /// <param name="ability">La habilidad a procesar. No puede ser null.</param>
        /// <param name="slot">El slot del Pokemon con esta habilidad. No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <param name="trigger">El trigger que se activó.</param>
        /// <param name="attacker">El slot del atacante (para efectos de contacto). Puede ser null.</param>
        /// <returns>Lista de acciones generadas. Nunca null, puede estar vacía.</returns>
        public List<BattleAction> ProcessAbility(AbilityData ability, BattleSlot slot, BattleField field, AbilityTrigger trigger, BattleSlot attacker)
        {
            if (ability == null)
                throw new ArgumentNullException(nameof(ability), ErrorMessages.AbilityCannotBeNull);
            if (slot == null)
                throw new ArgumentNullException(nameof(slot), ErrorMessages.SlotCannotBeNull);
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            var actions = new List<BattleAction>();

            // Verificar si la habilidad escucha este trigger
            if (!ability.ListensTo(trigger))
                return actions;

            // Intentar buscar por ID primero (más específico)
            if (_abilityHandlersById.TryGetValue(ability.Id, out var handlerById))
            {
                if (handlerById.Trigger == trigger || handlerById.Trigger == AbilityTrigger.None)
                {
                    // Si el handler implementa IContactAbilityHandler, pasar el attacker
                    if (handlerById is IContactAbilityHandler contactHandler && attacker != null)
                    {
                        actions.AddRange(contactHandler.ProcessWithAttacker(ability, slot, field, attacker));
                    }
                    else
                    {
                        actions.AddRange(handlerById.Process(ability, slot, field));
                    }
                }
            }

            // Buscar por trigger y efecto
            var key = (trigger, ability.Effect);
            if (_abilityHandlersByTriggerEffect.TryGetValue(key, out var handler))
            {
                // Si el handler implementa IContactAbilityHandler, pasar el attacker
                if (handler is IContactAbilityHandler contactHandler && attacker != null)
                {
                    actions.AddRange(contactHandler.ProcessWithAttacker(ability, slot, field, attacker));
                }
                else
                {
                    actions.AddRange(handler.Process(ability, slot, field));
                }
            }

            return actions;
        }

        /// <summary>
        /// Procesa un item cuando se activa un trigger específico.
        /// Usado por Steps para generar acciones.
        /// </summary>
        /// <param name="item">El item a procesar. No puede ser null.</param>
        /// <param name="slot">El slot del Pokemon con este item. No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <param name="trigger">El trigger que se activó.</param>
        /// <returns>Lista de acciones generadas. Nunca null, puede estar vacía.</returns>
        public List<BattleAction> ProcessItem(ItemData item, BattleSlot slot, BattleField field, ItemTrigger trigger)
        {
            return ProcessItem(item, slot, field, trigger, null);
        }

        /// <summary>
        /// Procesa un item cuando se activa un trigger específico, con contexto adicional.
        /// Usado por Steps para generar acciones cuando se necesita información adicional (como el attacker en contact effects).
        /// </summary>
        /// <param name="item">El item a procesar. No puede ser null.</param>
        /// <param name="slot">El slot del Pokemon con este item. No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <param name="trigger">El trigger que se activó.</param>
        /// <param name="attacker">El slot del atacante (para efectos de contacto). Puede ser null.</param>
        /// <returns>Lista de acciones generadas. Nunca null, puede estar vacía.</returns>
        public List<BattleAction> ProcessItem(ItemData item, BattleSlot slot, BattleField field, ItemTrigger trigger, BattleSlot attacker)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), ErrorMessages.ItemCannotBeNull);
            if (slot == null)
                throw new ArgumentNullException(nameof(slot), ErrorMessages.SlotCannotBeNull);
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            var actions = new List<BattleAction>();

            // Verificar si el item escucha este trigger
            if (!item.ListensTo(trigger))
                return actions;

            // Intentar buscar por ID primero (más específico)
            if (_itemHandlersById.TryGetValue(item.Id, out var handlerById))
            {
                if (handlerById.Trigger == trigger || handlerById.Trigger == ItemTrigger.None)
                {
                    // Si el handler implementa IContactItemHandler, pasar el attacker
                    if (handlerById is IContactItemHandler contactHandler && attacker != null)
                    {
                        actions.AddRange(contactHandler.ProcessWithAttacker(item, slot, field, attacker));
                    }
                    else
                    {
                        actions.AddRange(handlerById.Process(item, slot, field));
                    }
                }
            }

            // Buscar handlers por trigger que puedan manejar este item
            if (_itemHandlersByTrigger.TryGetValue(trigger, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    if (handler.CanHandle(item))
                    {
                        // Si el handler implementa IContactItemHandler, pasar el attacker
                        if (handler is IContactItemHandler contactHandler && attacker != null)
                        {
                            actions.AddRange(contactHandler.ProcessWithAttacker(item, slot, field, attacker));
                        }
                        else
                        {
                            actions.AddRange(handler.Process(item, slot, field));
                        }
                    }
                }
            }

            return actions;
        }

        /// <summary>
        /// Obtiene un handler de habilidad por ID.
        /// Usado para verificación de comportamientos en Actions.
        /// </summary>
        /// <param name="abilityId">El ID de la habilidad.</param>
        /// <returns>El handler si existe, null en caso contrario.</returns>
        public IAbilityEffectHandler GetAbilityHandler(string abilityId)
        {
            if (string.IsNullOrEmpty(abilityId))
                return null;

            _abilityHandlersById.TryGetValue(abilityId, out var handler);
            return handler;
        }

        /// <summary>
        /// Obtiene un handler de item por ID.
        /// Usado para verificación de comportamientos en Actions.
        /// </summary>
        /// <param name="itemId">El ID del item.</param>
        /// <returns>El handler si existe, null en caso contrario.</returns>
        public IItemEffectHandler GetItemHandler(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
                return null;

            _itemHandlersById.TryGetValue(itemId, out var handler);
            return handler;
        }

        /// <summary>
        /// Obtiene el handler de prevención de OHKO (Focus Sash, Sturdy).
        /// </summary>
        /// <returns>El handler de prevención de OHKO.</returns>
        public OHKOPreventionHandler GetOHKOPreventionHandler()
        {
            // Buscar por ID de item primero (Focus Sash)
            if (_itemHandlersById.TryGetValue("focus-sash", out var itemHandler) && itemHandler is OHKOPreventionHandler ohkoItem)
                return ohkoItem;

            // Buscar por ID de ability (Sturdy)
            if (_abilityHandlersById.TryGetValue("sturdy", out var abilityHandler) && abilityHandler is OHKOPreventionHandler ohkoAbility)
                return ohkoAbility;

            // Si no está registrado, crear uno nuevo (fallback)
            return new OHKOPreventionHandler();
        }

        /// <summary>
        /// Obtiene el handler de inversión de cambios de estadísticas (Contrary).
        /// </summary>
        /// <returns>El handler de inversión de cambios de stats.</returns>
        public StatChangeReversalHandler GetStatChangeReversalHandler()
        {
            // Buscar en handlers de abilities
            foreach (var handler in _abilityHandlersById.Values)
            {
                if (handler is StatChangeReversalHandler reversalHandler)
                    return reversalHandler;
            }

            // Si no está registrado, crear uno nuevo (fallback)
            return new StatChangeReversalHandler();
        }

        /// <summary>
        /// Obtiene el handler de aplicación de daño.
        /// </summary>
        /// <returns>El handler de aplicación de daño.</returns>
        public Checkers.DamageApplicationHandler GetDamageApplicationHandler()
        {
            return new Checkers.DamageApplicationHandler(this);
        }

        /// <summary>
        /// Obtiene el handler de aplicación de status.
        /// </summary>
        /// <returns>El handler de aplicación de status.</returns>
        public Checkers.StatusApplicationHandler GetStatusApplicationHandler()
        {
            return new Checkers.StatusApplicationHandler();
        }

        /// <summary>
        /// Obtiene el handler de aplicación de cambios de stats.
        /// </summary>
        /// <returns>El handler de aplicación de cambios de stats.</returns>
        public Checkers.StatChangeApplicationHandler GetStatChangeApplicationHandler()
        {
            return new Checkers.StatChangeApplicationHandler();
        }

        /// <summary>
        /// Obtiene el handler de aplicación de curación.
        /// </summary>
        /// <returns>El handler de aplicación de curación.</returns>
        public Checkers.HealingApplicationHandler GetHealingApplicationHandler()
        {
            return new Checkers.HealingApplicationHandler();
        }

        /// <summary>
        /// Obtiene el handler de aplicación de cambios de Pokemon.
        /// </summary>
        /// <returns>El handler de aplicación de cambios de Pokemon.</returns>
        public Checkers.SwitchApplicationHandler GetSwitchApplicationHandler()
        {
            return new Checkers.SwitchApplicationHandler();
        }

        /// <summary>
        /// Obtiene el handler de aplicación de condiciones de campo.
        /// </summary>
        /// <returns>El handler de aplicación de condiciones de campo.</returns>
        public Checkers.FieldConditionApplicationHandler GetFieldConditionHandler()
        {
            return new Checkers.FieldConditionApplicationHandler();
        }

        /// <summary>
        /// Obtiene el handler de estados de movimientos.
        /// </summary>
        /// <returns>El handler de estados de movimientos.</returns>
        public Checkers.MoveStateHandler GetMoveStateHandler()
        {
            return new Checkers.MoveStateHandler();
        }

        /// <summary>
        /// Obtiene el handler de ejecución de movimientos.
        /// </summary>
        /// <param name="randomProvider">El proveedor de números aleatorios. Si es null, crea uno temporal.</param>
        /// <param name="messageFormatter">El formateador de mensajes. Si es null, crea uno por defecto.</param>
        /// <returns>El handler de ejecución de movimientos.</returns>
        public Checkers.MoveExecutionHandler GetMoveExecutionHandler(
            Infrastructure.Providers.Definition.IRandomProvider randomProvider = null,
            Infrastructure.Messages.Definition.IBattleMessageFormatter messageFormatter = null)
        {
            return new Checkers.MoveExecutionHandler(randomProvider, messageFormatter);
        }

        /// <summary>
        /// Obtiene el handler de comportamiento Focus Punch.
        /// </summary>
        /// <returns>El handler de comportamiento Focus Punch.</returns>
        public Checkers.FocusPunchHandler GetFocusPunchHandler()
        {
            return new Checkers.FocusPunchHandler();
        }

        /// <summary>
        /// Obtiene el handler de comportamiento Multi-Turn.
        /// </summary>
        /// <returns>El handler de comportamiento Multi-Turn.</returns>
        public Checkers.MultiTurnHandler GetMultiTurnHandler()
        {
            return new Checkers.MultiTurnHandler();
        }

        /// <summary>
        /// Obtiene el handler de protección.
        /// </summary>
        /// <param name="messageFormatter">El formateador de mensajes. Si es null, crea uno por defecto.</param>
        /// <returns>El handler de protección.</returns>
        public Checkers.ProtectionHandler GetProtectionHandler(
            Infrastructure.Messages.Definition.IBattleMessageFormatter messageFormatter = null)
        {
            return new Checkers.ProtectionHandler(messageFormatter);
        }

        /// <summary>
        /// Obtiene el handler de semi-invulnerabilidad.
        /// </summary>
        /// <param name="messageFormatter">El formateador de mensajes. Si es null, crea uno por defecto.</param>
        /// <returns>El handler de semi-invulnerabilidad.</returns>
        public Checkers.SemiInvulnerableHandler GetSemiInvulnerableHandler(
            Infrastructure.Messages.Definition.IBattleMessageFormatter messageFormatter = null)
        {
            return new Checkers.SemiInvulnerableHandler(messageFormatter);
        }

        /// <summary>
        /// Obtiene el handler de precisión de movimientos.
        /// </summary>
        /// <param name="accuracyChecker">El verificador de precisión. No puede ser null.</param>
        /// <param name="randomProvider">El proveedor de números aleatorios. Si es null, crea uno temporal.</param>
        /// <param name="messageFormatter">El formateador de mensajes. Si es null, crea uno por defecto.</param>
        /// <returns>El handler de precisión de movimientos.</returns>
        public Checkers.MoveAccuracyHandler GetMoveAccuracyHandler(
            Utilities.AccuracyChecker accuracyChecker,
            Infrastructure.Providers.Definition.IRandomProvider randomProvider = null,
            Infrastructure.Messages.Definition.IBattleMessageFormatter messageFormatter = null)
        {
            if (accuracyChecker == null)
                throw new System.ArgumentNullException(nameof(accuracyChecker));

            return new Checkers.MoveAccuracyHandler(accuracyChecker, this, messageFormatter);
        }

        /// <summary>
        /// Registra un handler de efecto de movimiento.
        /// </summary>
        /// <param name="handler">El handler a registrar. No puede ser null.</param>
        /// <exception cref="ArgumentNullException">Si handler es null.</exception>
        public void RegisterMoveEffectHandler(Definition.IMoveEffectHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler), ErrorMessages.HandlerCannotBeNull);

            _moveEffectHandlers[handler.EffectType] = handler;
        }

        /// <summary>
        /// Obtiene un handler de efecto de movimiento por tipo.
        /// </summary>
        /// <param name="effectType">El tipo del efecto.</param>
        /// <returns>El handler si existe, null en caso contrario.</returns>
        public Definition.IMoveEffectHandler GetMoveEffectHandler(System.Type effectType)
        {
            if (effectType == null)
                return null;

            _moveEffectHandlers.TryGetValue(effectType, out var handler);
            return handler;
        }

        /// <summary>
        /// Procesa un efecto de movimiento usando el handler apropiado.
        /// </summary>
        /// <param name="effect">El efecto a procesar. No puede ser null.</param>
        /// <param name="user">El slot del usuario. No puede ser null.</param>
        /// <param name="target">El slot objetivo. No puede ser null.</param>
        /// <param name="move">Los datos del movimiento. No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <param name="damageDealt">El daño infligido por el movimiento.</param>
        /// <returns>Lista de acciones generadas. Nunca null, puede estar vacía.</returns>
        public List<BattleAction> ProcessMoveEffect(
            Core.Data.Effects.Definition.IMoveEffect effect,
            BattleSlot user,
            BattleSlot target,
            MoveData move,
            BattleField field,
            int damageDealt)
        {
            if (effect == null)
                throw new ArgumentNullException(nameof(effect), "Effect cannot be null");
            if (user == null)
                throw new ArgumentNullException(nameof(user), ErrorMessages.SlotCannotBeNull);
            if (target == null)
                throw new ArgumentNullException(nameof(target), ErrorMessages.SlotCannotBeNull);
            if (move == null)
                throw new ArgumentNullException(nameof(move), "Move cannot be null");
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            var effectType = effect.GetType();
            if (_moveEffectHandlers.TryGetValue(effectType, out var handler))
            {
                return handler.Process(effect, user, target, move, field, damageDealt);
            }

            // Si no hay handler registrado, retornar lista vacía (efecto desconocido)
            return new List<BattleAction>();
        }

        /// <summary>
        /// Crea una nueva instancia del registry inicializada con todos los handlers conocidos.
        /// Método de conveniencia para evitar duplicación de código.
        /// </summary>
        /// <param name="randomProvider">El proveedor de números aleatorios para efectos que lo necesiten. Si es null, crea uno temporal.</param>
        /// <param name="damageContextFactory">La factory para crear contextos de daño. Si es null, crea una por defecto.</param>
        /// <returns>Una nueva instancia del registry inicializada.</returns>
        public static CombatEffectHandlerRegistry CreateDefault(
            IRandomProvider randomProvider = null,
            DamageContextFactory damageContextFactory = null)
        {
            var registry = new CombatEffectHandlerRegistry();
            registry.Initialize(randomProvider, damageContextFactory);
            return registry;
        }

        /// <summary>
        /// Inicializa el registry con todos los handlers conocidos.
        /// Debe ser llamado una vez antes de usar el registry.
        /// </summary>
        /// <param name="randomProvider">El proveedor de números aleatorios para efectos que lo necesiten. Si es null, crea uno temporal.</param>
        /// <param name="damageContextFactory">La factory para crear contextos de daño. Si es null, crea una por defecto.</param>
        public void Initialize(
            IRandomProvider randomProvider = null,
            DamageContextFactory damageContextFactory = null)
        {
            if (_isInitialized)
                return;

            // Crear dependencias si no se proporcionan
            var randomProviderInstance = randomProvider ?? new Infrastructure.Providers.RandomProvider();
            var damageContextFactoryInstance = damageContextFactory ?? new DamageContextFactory();

            // Registrar handlers de habilidades
            RegisterAbilityHandler(new SpeedBoostHandler());
            RegisterAbilityHandler(new IntimidateHandler());
            RegisterAbilityHandler(new MoxieHandler());

            // Registrar handlers de contacto (Static, Rough Skin, Iron Barbs, etc.)
            var contactAbilityHandler = new ContactAbilityHandler(randomProviderInstance);
            // Registrar por trigger y efecto manualmente para ambos efectos
            _abilityHandlersByTriggerEffect[(AbilityTrigger.OnContactReceived, AbilityEffect.ChanceToStatusOnContact)] = contactAbilityHandler;
            _abilityHandlersByTriggerEffect[(AbilityTrigger.OnContactReceived, AbilityEffect.DamageOnContact)] = contactAbilityHandler;

            // Registrar handlers de checkers (comportamientos)
            var ohkoHandler = new OHKOPreventionHandler();
            RegisterAbilityHandlerById("sturdy", ohkoHandler);
            RegisterItemHandlerById("focus-sash", ohkoHandler);
            RegisterAbilityHandler(new StatChangeReversalHandler());

            // TODO: Agregar más handlers de habilidades cuando se creen

            // Registrar handlers de items
            RegisterItemHandler(new LeftoversHandler());
            RegisterItemHandler(new LifeOrbHandler());

            // Registrar handler de Rocky Helmet
            var rockyHelmetHandler = new RockyHelmetHandler();
            RegisterItemHandlerById("rocky-helmet", rockyHelmetHandler);
            RegisterItemHandler(rockyHelmetHandler);

            // TODO: Agregar más handlers de items cuando se creen

            // Registrar handlers de efectos de movimientos
            // Nota: Pasamos 'this' como referencia al registry para evitar referencia circular
            RegisterMoveEffectHandler(new StatusEffectHandler(randomProviderInstance, this));
            RegisterMoveEffectHandler(new StatChangeEffectHandler(randomProviderInstance, this));
            RegisterMoveEffectHandler(new RecoilEffectHandler(damageContextFactoryInstance, this));
            RegisterMoveEffectHandler(new DrainEffectHandler(this));
            RegisterMoveEffectHandler(new HealEffectHandler(this));
            RegisterMoveEffectHandler(new FlinchEffectHandler(randomProviderInstance));
            RegisterMoveEffectHandler(new ProtectEffectHandler(randomProviderInstance));
            RegisterMoveEffectHandler(new CounterEffectHandler(damageContextFactoryInstance));
            // TODO: Agregar más handlers de efectos cuando se creen

            _isInitialized = true;
        }
    }
}
