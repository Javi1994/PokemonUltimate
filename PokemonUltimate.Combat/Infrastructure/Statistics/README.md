# Battle Statistics System

Sistema completo de recolección de estadísticas de batalla. Recolecta automáticamente información detallada sobre movimientos, acciones, daño, y más.

## Características

-   ✅ **Recolección automática**: Se suscribe a eventos automáticamente
-   ✅ **Estadísticas detalladas**: Movimientos, acciones, daño, switches, critical hits
-   ✅ **Fácil integración**: Método simple en el builder
-   ✅ **Modular**: Puede extenderse fácilmente

## Estadísticas Recolectadas

### Información General

-   Total de turnos
-   Total de acciones ejecutadas
-   Acciones por tipo
-   Acciones por turno
-   Resultado de la batalla

### Movimientos

-   Movimientos usados por el jugador
-   Movimientos usados por el enemigo
-   Movimientos usados por cada Pokemon (qué Pokemon usó qué movimiento)
-   Daño total por movimiento
-   Critical hits

### Daño y Curación

-   Daño total del jugador
-   Daño total del enemigo
-   Curación total del jugador
-   Curación total del enemigo

### Pokemon

-   Pokemon que se desmayaron (por lado)
-   Cambios de Pokemon realizados

### Efectos

-   Efectos de estado aplicados
-   Cambios de estadísticas por Pokemon
-   Cambios de clima
-   Cambios de terreno

### Debug (si está habilitado)

-   Tiempos de ejecución de steps

## Uso Básico

### Con el Builder (Recomendado)

```csharp
using PokemonUltimate.Combat.Infrastructure.Builders;
using PokemonUltimate.Combat.Infrastructure.Statistics;

// Crear batalla con estadísticas automáticas
var collector = BattleBuilder.Create()
    .Singles()
    .Random(50)
    .WithStatistics()  // Habilita recolección automática
    .WithRandomAI()
    .Build();

await engine.RunBattle();

// Acceder a las estadísticas
var stats = collector.StatisticsCollector.GetStatistics();
Console.WriteLine($"Total turns: {stats.TotalTurns}");
Console.WriteLine($"Total actions: {stats.TotalActions}");
Console.WriteLine($"Player damage: {stats.PlayerDamageDealt}");
Console.WriteLine($"Critical hits: {stats.CriticalHits}");

// Ver movimientos usados
foreach (var move in stats.PlayerMoveUsage)
{
    Console.WriteLine($"{move.Key}: {move.Value} veces");
}

// Ver movimientos por Pokemon
foreach (var pokemon in stats.MoveUsageByPokemon)
{
    Console.WriteLine($"{pokemon.Key}:");
    foreach (var move in pokemon.Value)
    {
        Console.WriteLine($"  - {move.Key}: {move.Value} veces");
    }
}
```

### Manualmente

```csharp
using PokemonUltimate.Combat.Infrastructure.Statistics;

// Crear collector
var collector = new BattleStatisticsCollector();
collector.Subscribe(); // Suscribirse a eventos

// Crear y ejecutar batalla
var engine = BattleBuilder.Create()
    .Singles()
    .Random(50)
    .WithRandomAI()
    .Build();

await engine.RunBattle();

// Obtener estadísticas
var stats = collector.GetStatistics();

// Usar estadísticas
Console.WriteLine($"Total actions: {stats.TotalActions}");
Console.WriteLine($"Actions by type: {string.Join(", ", stats.ActionsByType)}");
```

## Estadísticas Disponibles

### `BattleStatistics` Properties

```csharp
// Información general
int TotalTurns
int TotalActions
Dictionary<string, int> ActionsByType
Dictionary<int, int> ActionsPerTurn
BattleOutcome Outcome
BattleField FinalField

// Movimientos
Dictionary<string, int> PlayerMoveUsage        // Movimiento -> veces usado
Dictionary<string, int> EnemyMoveUsage         // Movimiento -> veces usado
Dictionary<string, Dictionary<string, int>> MoveUsageByPokemon  // Pokemon -> Movimiento -> veces
Dictionary<string, int> DamageByMove           // Movimiento -> daño total

// Daño y curación
int PlayerDamageDealt
int EnemyDamageDealt
int PlayerHealing
int EnemyHealing

// Pokemon
List<string> PlayerFainted
List<string> EnemyFainted
Dictionary<string, int> PokemonSwitches         // Pokemon -> veces cambiado

// Efectos
Dictionary<string, int> StatusEffectsApplied   // Estado -> veces aplicado
Dictionary<string, Dictionary<string, int>> StatChanges  // Pokemon -> Estadística -> cambio total
List<string> WeatherChanges
List<string> TerrainChanges

// Especiales
int CriticalHits
int MissedMoves

// Debug (solo si debug mode está habilitado)
Dictionary<string, List<TimeSpan>> StepExecutionTimes
```

## Ejemplo Completo

```csharp
var collector = BattleBuilder.Create()
    .Singles()
    .FullTeam()
    .Random(50)
    .WithDebugMode()  // Para tiempos de ejecución de steps
    .WithStatistics() // Habilita estadísticas
    .WithRandomAI()
    .Build();

await engine.RunBattle();

var stats = collector.StatisticsCollector.GetStatistics();

// Análisis de la batalla
Console.WriteLine("=== Battle Statistics ===");
Console.WriteLine($"Turns: {stats.TotalTurns}");
Console.WriteLine($"Total Actions: {stats.TotalActions}");
Console.WriteLine($"Outcome: {stats.Outcome}");
Console.WriteLine();

Console.WriteLine("=== Damage ===");
Console.WriteLine($"Player: {stats.PlayerDamageDealt}");
Console.WriteLine($"Enemy: {stats.EnemyDamageDealt}");
Console.WriteLine($"Critical Hits: {stats.CriticalHits}");
Console.WriteLine();

Console.WriteLine("=== Moves Used ===");
foreach (var move in stats.PlayerMoveUsage.OrderByDescending(m => m.Value))
{
    var damage = stats.DamageByMove.GetValueOrDefault(move.Key, 0);
    Console.WriteLine($"{move.Key}: {move.Value} veces, {damage} daño total");
}
Console.WriteLine();

Console.WriteLine("=== Moves by Pokemon ===");
foreach (var pokemon in stats.MoveUsageByPokemon)
{
    Console.WriteLine($"{pokemon.Key}:");
    foreach (var move in pokemon.Value.OrderByDescending(m => m.Value))
    {
        Console.WriteLine($"  - {move.Key}: {move.Value} veces");
    }
}
Console.WriteLine();

Console.WriteLine("=== Actions per Turn ===");
foreach (var turn in stats.ActionsPerTurn.OrderBy(t => t.Key))
{
    Console.WriteLine($"Turn {turn.Key}: {turn.Value} acciones");
}
```

## Resetear Estadísticas

```csharp
collector.Reset(); // Limpia todas las estadísticas
```

## Desuscribirse

```csharp
collector.Unsubscribe(); // Deja de escuchar eventos
```
