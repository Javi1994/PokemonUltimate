# Refactor de Procesadores: Sistema de Handlers para Habilidades e Items

## Problema Actual

Los procesadores (`TurnEndProcessor`, `BeforeMoveProcessor`, `AfterMoveProcessor`, `SwitchInProcessor`, `DamageTakenProcessor`, etc.) tienen métodos `ProcessAbility` y `ProcessItem` que contienen grandes `switch` statements para verificar qué habilidad o item está presente y qué efecto tiene.

Además, existe un sistema similar en las **Actions** con `BehaviorCheckerRegistry` que contiene checkers que verifican comportamientos específicos (Contrary, Focus Sash, etc.) y procesan aplicaciones (daño, status, etc.).

Esto causa varios problemas:

1. **Procesadores muy grandes**: Cada procesador tiene que conocer todos los efectos posibles
2. **Difícil de mantener**: Agregar una nueva habilidad/item requiere modificar múltiples procesadores
3. **Violación del principio abierto/cerrado**: Los procesadores deben modificarse cada vez que se agrega contenido nuevo
4. **Duplicación de código**: La misma lógica de verificación se repite en múltiples lugares
5. **Sistemas duplicados**: Los checkers en Actions y los handlers en Processors hacen cosas similares pero están separados

## Solución Propuesta

**Unificar** los sistemas de checkers y handlers en un **sistema único de Ability/Item Effect Handlers** que puede:

1. **Verificar comportamientos** (como los checkers actuales): Verificar si un Pokemon tiene un comportamiento específico
2. **Procesar efectos y generar acciones** (como los handlers propuestos): Generar acciones cuando se activan triggers

### Arquitectura Unificada

```
AbilityItemEffectHandlerRegistry (unificado)
├── IAbilityEffectHandler (por trigger + efecto)
│   ├── Verificación de comportamiento (HasBehavior)
│   ├── Procesamiento de efecto (Process)
│   └── Modificación de valores (CalculateModifiedValue, etc.)
│
├── Handlers de Verificación (usados en Actions)
│   ├── OHKOPreventionHandler → Verifica Focus Sash/Sturdy, previene OHKO
│   ├── StatChangeReversalHandler → Verifica Contrary, invierte cambios
│   └── ...
│
└── Handlers de Procesamiento (usados en Processors)
    ├── OnTurnEnd + RaiseOwnStat → SpeedBoostHandler
    ├── OnSwitchIn + LowerOpponentStat → IntimidateHandler
    ├── OnTurnEnd + HealAmount > 0 → LeftoversHandler
    └── ...
```

### Ventajas de la Unificación

1. **Un solo sistema**: Un único registry para todas las interacciones con abilities/items
2. **Reutilización**: Los handlers pueden usarse tanto en Actions como en Processors
3. **Consistencia**: Misma forma de acceder a abilities/items en todo el código
4. **Extensibilidad**: Agregar nuevas abilities/items solo requiere crear un handler
5. **Mantenibilidad**: Todo el código relacionado con abilities/items está en un lugar

### Componentes

#### 1. Interfaces Unificadas

**IAbilityEffectHandler** (unificado - puede verificar Y procesar)

```csharp
public interface IAbilityEffectHandler
{
    /// <summary>
    /// El trigger que activa este handler (None si es solo verificación).
    /// </summary>
    AbilityTrigger Trigger { get; }

    /// <summary>
    /// El efecto de habilidad que maneja este handler (None si es solo verificación).
    /// </summary>
    AbilityEffect Effect { get; }

    /// <summary>
    /// Verifica si el Pokemon tiene el comportamiento que maneja este handler.
    /// Usado por Actions para verificar condiciones.
    /// </summary>
    bool HasBehavior(PokemonInstance pokemon);

    /// <summary>
    /// Procesa el efecto de la habilidad y genera acciones.
    /// Usado por Processors cuando se activa el trigger.
    /// </summary>
    List<BattleAction> Process(AbilityData ability, BattleSlot slot, BattleField field);

    /// <summary>
    /// Modifica un valor basado en el comportamiento (opcional).
    /// Usado por Actions para modificar daño, stats, etc.
    /// </summary>
    int? ModifyValue(PokemonInstance pokemon, int originalValue, string valueType);
}
```

**IItemEffectHandler** (unificado - puede verificar Y procesar)

```csharp
public interface IItemEffectHandler
{
    /// <summary>
    /// El trigger que activa este handler (None si es solo verificación).
    /// </summary>
    ItemTrigger Trigger { get; }

    /// <summary>
    /// Verifica si puede manejar este item.
    /// </summary>
    bool CanHandle(ItemData item);

    /// <summary>
    /// Verifica si el Pokemon tiene el comportamiento que maneja este handler.
    /// Usado por Actions para verificar condiciones.
    /// </summary>
    bool HasBehavior(PokemonInstance pokemon);

    /// <summary>
    /// Procesa el efecto del item y genera acciones.
    /// Usado por Processors cuando se activa el trigger.
    /// </summary>
    List<BattleAction> Process(ItemData item, BattleSlot slot, BattleField field);

    /// <summary>
    /// Modifica un valor basado en el comportamiento (opcional).
    /// Usado por Actions para modificar daño, stats, etc.
    /// </summary>
    int? ModifyValue(PokemonInstance pokemon, int originalValue, string valueType);
}
```

#### 2. Registry Unificado

**AbilityItemEffectHandlerRegistry** (reemplaza BehaviorCheckerRegistry)

-   Registra handlers por trigger y efecto
-   Proporciona métodos para obtener handlers apropiados
-   Inicializa todos los handlers conocidos
-   **Métodos de compatibilidad**: Mantiene métodos `GetXXXChecker()` para compatibilidad con código existente
-   **Búsqueda flexible**: Puede buscar handlers por trigger, efecto, o comportamiento específico

**Métodos principales:**

```csharp
public class AbilityItemEffectHandlerRegistry
{
    // Para Processors: Procesar efectos cuando se activa un trigger
    List<BattleAction> ProcessAbility(AbilityData ability, BattleSlot slot, BattleField field, AbilityTrigger trigger);
    List<BattleAction> ProcessItem(ItemData item, BattleSlot slot, BattleField field, ItemTrigger trigger);

    // Para Actions: Verificar comportamientos
    IAbilityEffectHandler GetAbilityHandler(string abilityId); // Por ID específico
    IItemEffectHandler GetItemHandler(string itemId); // Por ID específico

    // Compatibilidad con código existente
    OHKOPreventionHandler GetOHKOPreventionChecker(); // → Devuelve handler específico
    StatChangeReversalHandler GetStatChangeReversalChecker(); // → Devuelve handler específico
    // ... etc
}
```

#### 3. Handlers Específicos Unificados

**Handlers de Verificación (usados en Actions):**

-   `OHKOPreventionHandler` - Verifica Focus Sash/Sturdy, previene OHKO, genera mensajes
-   `StatChangeReversalHandler` - Verifica Contrary, invierte cambios de stats
-   `FocusPunchHandler` - Verifica estado de Focus Punch, bloquea movimiento si es necesario
-   `MultiTurnHandler` - Verifica movimientos multi-turno, maneja estados

**Handlers de Procesamiento (usados en Processors):**

-   `SpeedBoostHandler` - OnTurnEnd + RaiseOwnStat → Genera StatChangeAction
-   `IntimidateHandler` - OnSwitchIn + LowerOpponentStat → Genera StatChangeAction
-   `MoxieHandler` - OnAfterMove + RaiseStatOnKO → Genera StatChangeAction
-   `TruantHandler` - OnBeforeMove + SkipTurn → Genera acciones para bloquear movimiento

**Handlers de Items:**

-   `LeftoversHandler` - OnTurnEnd + HealAmount > 0 → Genera HealAction
-   `LifeOrbHandler` - OnDamageDealt + RecoilPercent > 0 → Genera DamageAction (recoil)
-   `FocusSashHandler` - OnWouldFaint → Previene OHKO, genera mensajes (también usado en Actions)
-   `ChoiceBandHandler` - Passive → Modifica stats (usado en damage pipeline, no genera acciones)

#### 4. Refactor de Procesadores

Los procesadores se simplifican a:

```csharp
public class TurnEndProcessor : IActionGeneratingPhaseProcessor
{
    private readonly AbilityItemEffectHandlerRegistry _handlerRegistry;

    public async Task<List<BattleAction>> ProcessAsync(BattleField field)
    {
        var actions = new List<BattleAction>();

        foreach (var slot in field.GetAllActiveSlots())
        {
            if (!slot.IsActive()) continue;

            // Procesar habilidad
            if (slot.Pokemon.Ability != null)
            {
                var abilityActions = _handlerRegistry.ProcessAbility(
                    slot.Pokemon.Ability,
                    slot,
                    field,
                    AbilityTrigger.OnTurnEnd);
                actions.AddRange(abilityActions);
            }

            // Procesar item
            if (slot.Pokemon.HeldItem != null)
            {
                var itemActions = _handlerRegistry.ProcessItem(
                    slot.Pokemon.HeldItem,
                    slot,
                    field,
                    ItemTrigger.OnTurnEnd);
                actions.AddRange(itemActions);
            }
        }

        return await Task.FromResult(actions);
    }
}
```

## Beneficios

1. **Procesadores más pequeños**: Solo orquestan, no implementan lógica específica
2. **Extensibilidad**: Agregar nuevas habilidades/items solo requiere crear nuevos handlers
3. **Mantenibilidad**: Cada handler es independiente y testeable
4. **Reutilización**: Los handlers pueden ser reutilizados en diferentes contextos
5. **Testabilidad**: Cada handler puede ser testeado de forma aislada

## Plan de Implementación

### Fase 1: Crear Sistema Unificado

1. ✅ Crear interfaces unificadas `IAbilityEffectHandler` e `IItemEffectHandler`
2. ✅ Crear `AbilityItemEffectHandlerRegistry` (reemplaza `BehaviorCheckerRegistry`)
3. ✅ Migrar checkers existentes a handlers unificados:
    - `OHKOPreventionChecker` → `OHKOPreventionHandler`
    - `StatChangeReversalChecker` → `StatChangeReversalHandler`
    - `FocusPunchBehaviorChecker` → `FocusPunchHandler`
    - `MultiTurnBehaviorChecker` → `MultiTurnHandler`
    - `StatusApplicationChecker` → `StatusApplicationHandler`
    - `DamageApplicationChecker` → `DamageApplicationHandler`
    - `HealingApplicationChecker` → `HealingApplicationHandler`
    - `FieldConditionApplicationChecker` → `FieldConditionHandler`
    - `StatChangeApplicationChecker` → `StatChangeApplicationHandler`
    - `SwitchApplicationChecker` → `SwitchApplicationHandler`
    - `MoveExecutionChecker` → `MoveExecutionHandler`
    - `ProtectionChecker` → `ProtectionHandler`
    - `SemiInvulnerableChecker` → `SemiInvulnerableHandler`
    - `MoveStateChecker` → `MoveStateHandler`
    - `MoveAccuracyChecker` → `MoveAccuracyHandler`

### Fase 2: Crear Handlers de Procesamiento

4. ✅ Crear handlers de procesamiento para habilidades comunes:
    - `SpeedBoostHandler` - OnTurnEnd + RaiseOwnStat
    - `IntimidateHandler` - OnSwitchIn + LowerOpponentStat
    - `MoxieHandler` - OnAfterMove + RaiseStatOnKO
    - `TruantHandler` - OnBeforeMove + SkipTurn
5. ✅ Crear handlers de procesamiento para items comunes:
    - `LeftoversHandler` - OnTurnEnd + HealAmount > 0
    - `LifeOrbHandler` - OnDamageDealt + RecoilPercent > 0
    - `FocusSashHandler` - OnWouldFaint (ya existe como checker, extenderlo)

### Fase 3: Refactorizar Processors

6. ✅ Refactorizar `TurnEndProcessor` para usar el registry unificado
7. ✅ Refactorizar `BeforeMoveProcessor` para usar el registry unificado
8. ✅ Refactorizar `AfterMoveProcessor` para usar el registry unificado
9. ✅ Refactorizar `SwitchInProcessor` para usar el registry unificado
10. ✅ Refactorizar `DamageTakenProcessor` para usar el registry unificado

### Fase 4: Actualizar Actions

11. ✅ Actualizar todas las Actions para usar el registry unificado en lugar de `BehaviorCheckerRegistry`
12. ✅ Actualizar `BehaviorCheckerRegistry` para que sea un wrapper del nuevo registry (compatibilidad temporal)

### Fase 5: Limpieza

13. ✅ Eliminar `BehaviorCheckerRegistry` antiguo (después de migración completa)
14. ✅ Actualizar tests existentes
15. ✅ Actualizar documentación

## Ubicación del Código

-   **Interfaces**: `PokemonUltimate.Combat/Engine/Processors/Handlers/Definition/`
-   **Registry**: `PokemonUltimate.Combat/Engine/Processors/Handlers/Registry/`
-   **Handlers de Verificación**: `PokemonUltimate.Combat/Engine/Processors/Handlers/Checkers/` (migrados desde Actions/Checkers)
-   **Handlers de Procesamiento**: `PokemonUltimate.Combat/Engine/Processors/Handlers/Abilities/` y `Items/`
-   **Procesadores refactorizados**: `PokemonUltimate.Combat/Engine/Processors/`
-   **Actions actualizadas**: `PokemonUltimate.Combat/Actions/` (usan el nuevo registry)

## Notas de Diseño

-   Los handlers deben ser **stateless** cuando sea posible
-   Si un handler necesita estado, debe almacenarse en el `BattleSlot` o `BattleField`
-   Los handlers deben validar sus propias condiciones (HP thresholds, stat stages, etc.)
-   Los handlers deben generar mensajes de localización apropiados
-   El registry debe inicializarse una vez y reutilizarse durante toda la batalla
-   **Compatibilidad**: Mantener métodos de compatibilidad en el registry para no romper código existente durante la migración
-   **Dual Purpose**: Los handlers pueden implementar tanto verificación (HasBehavior) como procesamiento (Process)
-   **Búsqueda flexible**: El registry debe poder buscar handlers por múltiples criterios (trigger, efecto, ID de ability/item)
-   **Migración gradual**: Migrar primero los checkers existentes, luego crear nuevos handlers, finalmente refactorizar processors
