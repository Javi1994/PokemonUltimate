# Battle Events System

Sistema de eventos para batallas que permite fácil integración, estadísticas y debug.

## Características

-   ✅ **Fácil integración**: Suscríbete a todos los eventos de una vez
-   ✅ **Eventos de producción**: Siempre disponibles (BattleStart, BattleEnd, TurnStart, TurnEnd, ActionExecuted)
-   ✅ **Eventos de debug**: Solo se lanzan si la batalla está en modo debug (StepExecuted, StepStarted, StepFinished, BattleStateChanged)
-   ✅ **Modular y desacoplado**: No afecta la lógica de batalla

## Uso Básico

### Suscribirse a todos los eventos de producción

```csharp
using PokemonUltimate.Combat.Infrastructure.Events;

// Suscribirse a todos los eventos de producción de una vez
BattleEventSubscriber.SubscribeToProductionEvents(
    onBattleStart: (s, e) => Console.WriteLine($"Battle started!"),
    onBattleEnd: (s, e) => Console.WriteLine($"Battle ended: {e.Outcome}"),
    onTurnStart: (s, e) => Console.WriteLine($"Turn {e.TurnNumber} started"),
    onTurnEnd: (s, e) => Console.WriteLine($"Turn {e.TurnNumber} ended"),
    onActionExecuted: (s, e) => Console.WriteLine($"Action executed: {e.Action.GetType().Name}")
);
```

### Suscribirse a eventos de debug

```csharp
// Solo funcionan si la batalla está en modo debug
BattleEventSubscriber.SubscribeToDebugEvents(
    onStepExecuted: (s, e) => Console.WriteLine($"Step {e.StepName} took {e.Duration.TotalMilliseconds}ms"),
    onStepStarted: (s, e) => Console.WriteLine($"Step {e.StepName} started"),
    onStepFinished: (s, e) => Console.WriteLine($"Step {e.StepName} finished (continue: {e.ShouldContinue})")
);
```

### Suscribirse a todos los eventos (producción + debug)

```csharp
// Suscríbete a todos de una vez
BattleEventSubscriber.SubscribeToAllEvents(
    onBattleStart: (s, e) => { /* ... */ },
    onBattleEnd: (s, e) => { /* ... */ },
    onTurnStart: (s, e) => { /* ... */ },
    onTurnEnd: (s, e) => { /* ... */ },
    onActionExecuted: (s, e) => { /* ... */ },
    onStepExecuted: (s, e) => { /* ... */ }, // Solo en debug mode
    onStepStarted: (s, e) => { /* ... */ },  // Solo en debug mode
    onStepFinished: (s, e) => { /* ... */ }  // Solo en debug mode
);
```

### Usar con BattleBuilder

```csharp
// Habilitar modo debug
var engine = BattleBuilder.Create()
    .Singles()
    .Random(50)
    .WithDebugMode()  // Habilita eventos de debug
    .WithRandomAI()
    .Build();
```

### Usar con Statistics Collector

```csharp
using PokemonUltimate.Combat.Infrastructure.Statistics;

// Crear y suscribir collector
var collector = new BattleStatisticsCollector();
collector.Subscribe(); // Se suscribe automáticamente a todos los eventos necesarios

// Ejecutar batalla
var engine = BattleBuilder.Create()
    .Singles()
    .Random(50)
    .WithRandomAI()
    .Build();

await engine.RunBattle();

// Obtener estadísticas
var stats = collector.GetStatistics();
Console.WriteLine($"Total turns: {stats.TotalTurns}");
Console.WriteLine($"Total actions: {stats.TotalActions}");
Console.WriteLine($"Player damage: {stats.PlayerDamageDealt}");
```

## Eventos Disponibles

### Eventos de Producción (Siempre Disponibles)

-   `BattleStart` - Cuando inicia la batalla
-   `BattleEnd` - Cuando termina la batalla
-   `TurnStart` - Cuando inicia un turno
-   `TurnEnd` - Cuando termina un turno
-   `ActionExecuted` - Cuando se ejecuta una acción

### Eventos de Debug (Solo en Modo Debug)

-   `StepExecuted` - Cuando se ejecuta un step (con duración)
-   `StepStarted` - Cuando inicia un step
-   `StepFinished` - Cuando termina un step (con duración y si debe continuar)
-   `BattleStateChanged` - Cuando cambia el estado de la batalla

## Integración Automática

Los eventos se disparan automáticamente desde:

-   `BattleFlowExecutor` - Para steps de flujo de batalla
-   `TurnStepExecutor` - Para steps de turno
-   `BattleQueueService` - Para acciones ejecutadas
-   `BattleStartFlowStep` - Para inicio de batalla
-   `BattleEndFlowStep` - Para fin de batalla
-   `ExecuteBattleLoopStep` - Para inicio/fin de turnos

No necesitas hacer nada adicional, solo suscribirte a los eventos que necesites.
