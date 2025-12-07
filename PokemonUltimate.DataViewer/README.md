# Data Viewer

Visualizador de datos del juego con interfaz gráfica para explorar todos los datos de contenido.

## Descripción

**Data Viewer** es una aplicación Windows Forms que permite visualizar y explorar todos los datos de contenido del juego de forma interactiva. Proporciona una interfaz gráfica intuitiva para navegar por Pokémon, movimientos, objetos, habilidades y otros datos del juego.

## Características Principales

### Visualización de Datos

La aplicación incluye 10 pestañas para diferentes tipos de datos:

1. **Pokemon** - Especies de Pokémon con estadísticas, tipos, habilidades, etc.
2. **Moves** - Movimientos con tipo, categoría, poder, precisión, efectos
3. **Items** - Objetos con descripción y efectos
4. **Abilities** - Habilidades con descripción y efectos
5. **Status Effects** - Efectos de estado (Quemadura, Parálisis, Sueño, etc.)
6. **Weather** - Condiciones climáticas (Lluvia, Sol, Granizo, etc.)
7. **Terrain** - Terrenos (Eléctrico, Psíquico, Hierba, etc.)
8. **Hazards** - Peligros de campo (Stealth Rock, Spikes, etc.)
9. **Side Conditions** - Condiciones de lado (Reflect, Light Screen, etc.)
10. **Field Effects** - Efectos de campo (Trick Room, Gravity, etc.)

### Interfaz Interactiva

-   **DataGridView**: Tabla con todos los elementos del catálogo
-   **Panel de Detalles**: Muestra información detallada del elemento seleccionado
-   **Navegación**: Click en cualquier fila para ver detalles completos
-   **Búsqueda Visual**: Navegación rápida por todos los datos

## Estructura del Proyecto

```
PokemonUltimate.DataViewer/
├── Program.cs                    # Punto de entrada de la aplicación
├── MainForm.cs                   # Formulario principal con TabControl
├── Tabs/                         # Componentes de cada pestaña de datos
│   ├── PokemonDataTab.cs
│   ├── MoveDataTab.cs
│   ├── ItemDataTab.cs
│   ├── AbilityDataTab.cs
│   ├── StatusDataTab.cs
│   ├── WeatherDataTab.cs
│   ├── TerrainDataTab.cs
│   ├── HazardDataTab.cs
│   ├── SideConditionDataTab.cs
│   └── FieldEffectDataTab.cs
├── Viewers/                      # Visualizadores especializados
│   └── ItemViewer.cs
└── Localization/                 # Soporte de localización
    └── WinFormsLocalizationHelper.cs
```

## Uso

### Ejecutar la Aplicación

```bash
# Desde la raíz del proyecto
dotnet run --project PokemonUltimate.DataViewer

# O compilar y ejecutar
dotnet build PokemonUltimate.DataViewer
dotnet run --project PokemonUltimate.DataViewer
```

### Navegación

1. **Selecciona una pestaña** del tipo de dato que deseas explorar
2. **Navega por la tabla** usando la barra de desplazamiento
3. **Haz click en una fila** para ver información detallada en el panel inferior
4. **Cambia de pestaña** para explorar otros tipos de datos

## Propósito

### Para Desarrolladores

-   **Referencia rápida**: Acceso rápido a todos los datos del juego
-   **Verificación de datos**: Confirmar que los datos están correctamente cargados
-   **Exploración**: Descubrir relaciones entre diferentes datos
-   **Debugging**: Verificar valores específicos durante el desarrollo

### Para la IA

-   **Contexto completo**: Acceso a todos los datos del juego en un formato estructurado
-   **Análisis rápido**: Información organizada para análisis y debugging
-   **Validación**: Verificar que los datos son correctos y consistentes

## Datos Mostrados

### Pokemon Tab

-   Nombre, tipos, estadísticas base
-   Habilidades disponibles (primaria, secundaria, oculta)
-   Movimientos aprendibles
-   Información de evolución

### Moves Tab

-   Nombre, tipo, categoría (Físico/Especial/Estado)
-   Poder, precisión, PP
-   Efectos y efectos secundarios
-   Probabilidades de efectos

### Items Tab

-   Nombre y descripción
-   Efectos y propiedades
-   Categoría del objeto

### Abilities Tab

-   Nombre y descripción
-   Efectos y triggers
-   Condiciones de activación

### Status Effects Tab

-   Nombre y descripción
-   Efectos (reducción de velocidad, daño por turno, etc.)
-   Inmunidades por tipo
-   Duración y probabilidades

### Weather/Terrain/Hazards/Side Conditions/Field Effects Tabs

-   Propiedades y efectos
-   Duración y condiciones
-   Interacciones con otros sistemas

## Integración con el Sistema

### Dependencias

-   **Feature 1: Game Data** - Estructuras de datos base
-   **Feature 3: Content Expansion** - Catálogos de contenido

### Catálogos Utilizados

-   `PokemonCatalog` - Todas las especies de Pokémon
-   `MoveCatalog` - Todos los movimientos
-   `ItemCatalog` - Todos los objetos
-   `AbilityCatalog` - Todas las habilidades
-   `StatusCatalog` - Todos los efectos de estado
-   `WeatherCatalog` - Todas las condiciones climáticas
-   `TerrainCatalog` - Todos los terrenos
-   `HazardCatalog` - Todos los peligros
-   `SideConditionCatalog` - Todas las condiciones de lado
-   `FieldEffectCatalog` - Todos los efectos de campo

## Documentación Relacionada

-   **[Feature 6: Development Tools](../../docs/features/6-development-tools/README.md)** - Documentación principal
-   **[Sub-Feature 6.7: Data Viewer](../../docs/features/6-development-tools/6.7-data-viewer/README.md)** - Documentación detallada
-   **[Feature 1: Game Data](../../docs/features/1-game-data/README.md)** - Estructuras de datos
-   **[Feature 3: Content Expansion](../../docs/features/3-content-expansion/README.md)** - Catálogos de contenido

## Notas de Desarrollo

-   La interfaz es consistente en todas las pestañas
-   Los datos se cargan desde los catálogos en tiempo de ejecución
-   La aplicación utiliza localización (por defecto en español)
-   Cada pestaña muestra un contador del número de elementos en el catálogo

---

**Última actualización**: Diciembre 2025
