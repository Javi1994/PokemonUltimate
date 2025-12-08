# Pokemon Battle Test - Console Application

Proyecto de consola para probar el nuevo sistema de combate con Pokemon aleatorios.

## Uso

### Ejecutar una batalla

```bash
dotnet run --project PokemonUltimate.BattleTest
```

### Ejecutar múltiples batallas

```bash
dotnet run --project PokemonUltimate.BattleTest -- 10
```

### Ejecutar batallas con nivel específico

```bash
dotnet run --project PokemonUltimate.BattleTest -- 5 100
```

## Parámetros

-   **Primer argumento**: Número de batallas (default: 1)
-   **Segundo argumento**: Nivel de los Pokemon (default: 50)

## Ejemplos

```bash
# Una batalla con Pokemon nivel 50
dotnet run --project PokemonUltimate.BattleTest

# 10 batallas con Pokemon nivel 50
dotnet run --project PokemonUltimate.BattleTest -- 10

# 5 batallas con Pokemon nivel 100
dotnet run --project PokemonUltimate.BattleTest -- 5 100
```

## Características

-   ✅ Selecciona Pokemon aleatorios del catálogo
-   ✅ Usa RandomAI para ambos lados (AI vs AI)
-   ✅ Muestra resultados detallados de cada batalla
-   ✅ Soporta múltiples batallas en secuencia
-   ✅ Configurable por línea de comandos

## Salida

El programa muestra:

-   Pokemon participantes (Player y Enemy)
-   Resultado de la batalla (Victory/Defeat/Draw)
-   Número de turnos
-   Duración de la batalla
-   HP final y estado de cada Pokemon
