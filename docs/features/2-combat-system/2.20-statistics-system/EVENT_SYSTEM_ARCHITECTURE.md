# Sistema de Eventos de Batalla - Arquitectura

## Objetivo

Reemplazar logs hardcodeados con un sistema de eventos estructurado que permita:

-   **Estadísticas centralizadas**: Toda la información se acumula automáticamente
-   **Logging flexible**: El logger escucha eventos, no tiene código hardcodeado
-   **Análisis de batallas**: Fácil extraer información para análisis
-   **Extensibilidad**: Nuevos componentes solo necesitan suscribirse a eventos

## Arquitectura Implementada

### 1. Sistema de Eventos (`BattleEvent`, `BattleEventBus`)

**Archivos creados:**

-   `PokemonUltimate.Combat/Events/BattleEvent.cs` - Eventos tipados con datos estructurados
-   `PokemonUltimate.Combat/Events/IBattleEventPublisher.cs` - Interfaces para publicar/suscribir
-   `PokemonUltimate.Combat/Events/BattleEventBus.cs` - Bus centralizado de eventos

**Tipos de eventos:**

-   `PokemonFainted` - Cuando un Pokemon se desmaya
-   `PokemonSwitched` - Cuando hay un cambio de Pokemon
-   `AIDecisionMade` - Cuando la IA toma una decisión
-   `MoveUsed` - Cuando se usa un movimiento
-   `DamageDealt` - Cuando se hace daño
-   `TurnStarted/Ended` - Eventos de turno
-   `BattleStarted/Ended` - Eventos de batalla

### 2. Estadísticas Extendidas

**Archivo modificado:**

-   `PokemonUltimate.Combat/Statistics/BattleStatistics.cs` - Agregadas estadísticas de equipo:
    -   `FaintedPokemon` - Lista de Pokemon desmayados por equipo
    -   `SwitchCount` - Conteo de cambios por equipo
    -   `AIDecisions` - Decisiones de IA por tipo
    -   `TeamStatusHistory` - Historial de estado del equipo

**Tracker nuevo:**

-   `PokemonUltimate.Combat/Statistics/Trackers/TeamBattleTracker.cs` - Rastrea eventos de equipo

### 3. Componentes Actualizados

**CombatEngine:**

-   ✅ Tiene `EventBus` público
-   ✅ Publica eventos de turno y batalla
-   ⏳ Falta: Publicar eventos desde acciones (FaintAction, SwitchAction, etc.)

**TeamBattleAI:**

-   ✅ Puede recibir `IBattleEventPublisher`
-   ✅ Publica eventos de decisiones de IA
-   ✅ Método `SetEventPublisher()` para configuración post-inicialización

**BattleSimulator:**

-   ✅ Suscribe `EventBasedBattleLogger` al event bus
-   ✅ Configura AIs con event publisher

### 4. Logger Basado en Eventos

**Archivo creado:**

-   `PokemonUltimate.BattleSimulator/Logging/EventBasedBattleLogger.cs` - Convierte eventos en logs

## Flujo de Datos

```
┌─────────────────┐
│  CombatEngine   │───publica───┐
│  TeamBattleAI   │───publica───┤
│  FaintAction    │───publica───┤
│  SwitchAction   │───publica───┤
└─────────────────┘              │
                                 ▼
                        ┌─────────────────┐
                        │  BattleEventBus │
                        └─────────────────┘
                                 │
                ┌────────────────┼────────────────┐
                ▼                ▼                ▼
        ┌──────────────┐ ┌──────────────┐ ┌──────────────┐
        │ EventLogger   │ │ Statistics    │ │ UI Component │
        │ (convierte    │ │ Collector     │ │ (futuro)     │
        │  eventos a    │ │ (acumula      │ │              │
        │  logs)        │ │  estadísticas)│ │              │
        └──────────────┘ └──────────────┘ └──────────────┘
```

## Ventajas del Sistema

1. **Sin logs hardcodeados**: Todo pasa por eventos estructurados
2. **Estadísticas automáticas**: Se acumulan sin código adicional
3. **Extensible**: Nuevos componentes solo necesitan suscribirse
4. **Testeable**: Fácil mockear eventos para tests
5. **Separación de responsabilidades**: Lógica vs logging vs estadísticas

## Próximos Pasos

1. ⏳ Modificar `DetailedBattleLoggerObserver` para publicar eventos cuando detecta acciones
2. ⏳ Modificar `FaintAction` para publicar eventos (necesita acceso a event bus)
3. ⏳ Modificar `SwitchAction` para publicar eventos
4. ⏳ Modificar `DamageAction` para publicar eventos
5. ⏳ Migrar todos los logs hardcodeados a eventos
6. ⏳ Crear UI que muestre estadísticas en tiempo real

## Ejemplo de Uso

```csharp
// En el simulador
var eventBus = engine.EventBus;
var eventLogger = new EventBasedBattleLogger(logger);
eventBus.Subscribe(eventLogger);

// La IA publica eventos automáticamente
// El logger los convierte en mensajes de log
// Las estadísticas se acumulan automáticamente
```
