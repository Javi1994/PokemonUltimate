# Developer Tools

Herramienta unificada de depuración para el sistema de combate Pokémon.

## Descripción

**Developer Tools** es una aplicación Windows Forms que proporciona múltiples herramientas de depuración y análisis en una interfaz con pestañas. Esta herramienta permite a los desarrolladores probar, analizar y entender diferentes aspectos del sistema de combate Pokémon.

## Características

### Herramientas Disponibles

La aplicación incluye las siguientes herramientas organizadas en pestañas:

#### 1. Battle Debugger (6.5)

-   Ejecuta múltiples combates y analiza estadísticas
-   Permite configurar equipos y ejecutar batallas en lote
-   Muestra estadísticas acumuladas de todos los combates

#### 2. Move Debugger (6.6)

-   Prueba movimientos múltiples veces y recopila estadísticas
-   Analiza la distribución de daño, precisión y efectos secundarios
-   Útil para verificar la implementación de movimientos

#### 3. Type Matchup Debugger

-   Calcula efectividad de tipos para combinaciones
-   Visualiza las relaciones de tipo entre Pokémon y movimientos
-   Ayuda a entender y verificar la tabla de efectividad de tipos

#### 4. Stat Calculator Debugger (6.1)

-   Calcula y visualiza estadísticas de Pokémon con diferentes configuraciones
-   Permite ajustar nivel, naturaleza, IVs y EVs
-   Muestra estadísticas base, efectivas y con modificadores

#### 5. Damage Calculator Debugger (6.2)

-   Visualización paso a paso del pipeline de cálculo de daño
-   Muestra cada etapa del cálculo (base damage, multiplicadores, final)
-   Útil para depurar problemas en el sistema de daño

#### 6. Status Effect Debugger (6.3)

-   Prueba efectos de estado y sus interacciones
-   Verifica aplicación, duración y efectos de estados persistentes
-   Analiza cómo los estados afectan las estadísticas y acciones

#### 7. Turn Order Debugger (6.4)

-   Visualiza la determinación del orden de turnos con velocidad y prioridad
-   Muestra cómo se ordenan las acciones en cada turno
-   Útil para entender la mecánica de velocidad y prioridad

## Estructura del Proyecto

```
PokemonUltimate.DeveloperTools/
├── Program.cs                    # Punto de entrada de la aplicación
├── MainForm.cs                   # Formulario principal con TabControl
├── Tabs/                         # Componentes de cada herramienta
│   ├── BattleDebuggerTab.cs
│   ├── MoveDebuggerTab.cs
│   ├── TypeMatchupDebuggerTab.cs
│   ├── StatCalculatorDebuggerTab.cs
│   ├── DamageCalculatorDebuggerTab.cs
│   ├── StatusEffectDebuggerTab.cs
│   └── TurnOrderDebuggerTab.cs
├── Runners/                      # Lógica de ejecución de cada herramienta
│   ├── BattleRunner.cs
│   ├── MoveRunner.cs
│   ├── DamageCalculatorRunner.cs
│   ├── StatCalculatorRunner.cs
│   ├── StatusEffectRunner.cs
│   └── TurnOrderRunner.cs
└── Localization/                 # Soporte de localización
    └── WinFormsLocalizationHelper.cs
```

## Uso

### Ejecutar la Aplicación

```bash
# Desde la raíz del proyecto
dotnet run --project PokemonUltimate.DeveloperTools

# O compilar y ejecutar
dotnet build PokemonUltimate.DeveloperTools
dotnet run --project PokemonUltimate.DeveloperTools
```

### Navegación

La aplicación utiliza un sistema de pestañas donde cada pestaña representa una herramienta diferente:

1. Selecciona la pestaña de la herramienta que deseas usar
2. Configura los parámetros necesarios en la interfaz
3. Ejecuta la herramienta y observa los resultados
4. Los resultados se muestran en tiempo real en la misma interfaz

## Propósito

### Para Desarrolladores

-   **Depuración**: Identificar y corregir bugs en el sistema de combate
-   **Verificación**: Confirmar que las mecánicas funcionan correctamente
-   **Análisis**: Entender el comportamiento interno del sistema
-   **Testing**: Probar escenarios específicos y casos límite

### Para la IA

-   **Análisis rápido**: Las herramientas proporcionan información estructurada
-   **Debugging asistido**: Los resultados ayudan a identificar problemas
-   **Validación**: Verificar que los cambios funcionan como se espera

## Integración con el Sistema

### Dependencias

-   **Feature 1: Game Data** - Usa datos de Pokémon, movimientos, habilidades, objetos
-   **Feature 2: Combat System** - Usa el motor de combate y sus componentes
-   **Feature 3: Content Expansion** - Usa catálogos de contenido

### Componentes Utilizados

-   `CombatEngine` - Motor de combate principal
-   `PokemonCatalog`, `MoveCatalog`, etc. - Catálogos de datos
-   `StatCalculator`, `DamageCalculator` - Calculadores de mecánicas
-   `StatusEffectData`, `MoveData` - Datos de efectos y movimientos

## Documentación Relacionada

-   **[Feature 6: Development Tools](../../docs/features/6-development-tools/README.md)** - Documentación principal
-   **[Sub-Features](../../docs/features/6-development-tools/)** - Documentación detallada de cada herramienta
-   **[Architecture](../../docs/features/6-development-tools/architecture.md)** - Arquitectura técnica
-   **[Use Cases](../../docs/features/6-development-tools/use_cases.md)** - Casos de uso

## Notas de Desarrollo

-   Todas las herramientas comparten la misma interfaz base (TabControl)
-   Cada herramienta es independiente y puede ejecutarse por separado
-   Los resultados se muestran en tiempo real durante la ejecución
-   La aplicación utiliza localización (por defecto en español)

---

**Última actualización**: Diciembre 2025
