# Battle Simulator

Herramienta interactiva de simulación de combates Pokémon para desarrollo y depuración.

## Descripción

El **Battle Simulator** es una aplicación WinForms que permite simular combates Pokémon de forma interactiva. Esta herramienta está diseñada para facilitar el desarrollo, testing y depuración del sistema de combate del juego.

## Características Principales

### Modos de Combate

-   **Singles (1v1)**: Combate uno contra uno
-   **Doubles (2v2)**: Combate dos contra dos
-   **Triples (3v3)**: Combate tres contra tres
-   **Horde (1v3 / 1v5)**: Un Pokémon contra múltiples enemigos
-   **Custom**: Configuración personalizada de slots

### Configuración de Equipos

-   Selección manual de Pokémon con nivel, naturaleza e IVs
-   Generación aleatoria de equipos
-   Configuración de hasta 6 Pokémon por equipo (party completo)
-   Soporte para múltiples slots activos simultáneamente

### Simulación en Lote

-   Ejecución de múltiples combates consecutivos
-   Estadísticas acumuladas de todos los combates
-   Exportación opcional de logs consolidados

### Visualización en Tiempo Real

-   Logs de combate en tiempo real con filtrado
-   Estadísticas detalladas por equipo
-   Historial de kills y eventos del combate
-   Resultados separados por equipo (Jugador/Enemigo)

## Sistema de Logs Automático

### Guardado Automático

El simulador **guarda automáticamente todos los logs** de cada combate en la carpeta `Logs` ubicada en el directorio del proyecto (`PokemonUltimate.BattleSimulator/Logs/`).

#### Formato de Archivos

-   **Combates individuales**: `battle_logs_YYYYMMDD_HHMMSS.txt`
-   **Combates en lote**: `battle_logs_YYYYMMDD_HHMMSS_battleXofY.txt`
-   **Export consolidado** (opcional): `battle_logs_batch_YYYYMMDD_HHMMSS_Xbattles.txt`

Cada archivo contiene:

-   Timestamp del combate
-   Resultado del combate (Victoria/Derrota/Empate)
-   Todos los eventos y mensajes del combate en orden cronológico
-   Información detallada de cada acción, movimiento, daño, efectos de estado, etc.
-   Registro completo de cuando los efectos de estado impiden moverse (parálisis, sueño, etc.)

### Propósito de los Logs

Los logs guardados automáticamente están diseñados para facilitar el **debugging rápido por parte de la IA**. Cuando se detecta un comportamiento inesperado o un bug en el sistema de combate, los logs proporcionan:

1. **Trazabilidad completa**: Cada acción del combate está registrada con timestamp preciso
2. **Contexto completo**: Información de estado de todos los Pokémon en cada momento
3. **Análisis rápido**: La IA puede revisar los logs para identificar rápidamente:

    - Errores en cálculos de daño
    - Problemas con efectos de estado
    - Bugs en la lógica de turnos
    - Comportamientos anómalos de la IA
    - Problemas con cambios de estadísticas

4. **Reproducción de bugs**: Los logs permiten entender exactamente qué ocurrió en un combate problemático

### Estructura de los Logs

Los logs incluyen información detallada sobre:

-   **Inicio y fin del combate**: Configuración inicial, resultado final
-   **Selección de movimientos**: Decisiones de IA y movimientos usados
-   **Cálculos de precisión y daño**: Base damage, multiplicadores, daño final, efectividad de tipos
-   **Aplicación de efectos de estado**: Cuando se aplican estados (Parálisis, Sueño, Quemadura, etc.)
-   **Efectos de estado impidiendo movimiento**: Mensajes cuando la parálisis o el sueño impiden moverse
-   **Cambios de estadísticas**: Aumentos y reducciones de stat stages
-   **Cambios de Pokémon**: Cambios automáticos cuando un Pokémon se debilita
-   **Eventos especiales**: Golpes críticos, efectos secundarios, inmunidades
-   **Estado de los Pokémon**: HP y estado al inicio/fin de cada turno
-   **Resultado final del combate**: Victoria, derrota o empate con estadísticas completas

## Uso

1. **Configurar el modo de combate** en la pestaña "Battle Mode"
2. **Configurar los equipos** en la pestaña "Pokemon" (o usar generación aleatoria)
3. **Iniciar el combate** con el botón "Start Battle"
4. **Observar los logs** en tiempo real en la pestaña "Logs"
5. **Revisar las estadísticas** en las pestañas "Equipo Jugador" y "Equipo Enemigo"

Los logs se guardan automáticamente en `Logs/` después de cada combate.

## Ubicación de los Logs

Los logs se guardan automáticamente en:

```
PokemonUltimate.BattleSimulator\Logs\
```

Esta carpeta se crea automáticamente si no existe. Todos los combates generan un archivo de log único con timestamp para facilitar la identificación y el análisis posterior.

## Notas para Desarrollo

-   Los logs son esenciales para el debugging asistido por IA
-   Cada combate genera un log completo independientemente del resultado
-   Los logs incluyen información suficiente para reproducir y analizar cualquier situación
-   Los archivos de log pueden ser compartidos con la IA para análisis rápido de problemas

## Integración con el Sistema de Combate

Este simulador utiliza el mismo sistema de combate (`CombatEngine`) que el juego principal, lo que garantiza que los resultados y comportamientos sean consistentes con el juego real.
