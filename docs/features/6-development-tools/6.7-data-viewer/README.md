# Sub-Feature 6.7: Data Viewer

> Simple console application for quickly viewing game data (Pokemon, Moves, Items, Abilities).

**Feature Number**: 6  
**Sub-Feature Number**: 6.7  
**Feature Name**: Development Tools  
**Sub-Feature Name**: Data Viewer  
**See**: [`../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

The Data Viewer is a simple console application that allows developers to quickly browse and view game data from the catalogs. It provides a fast way to inspect Pokemon, Moves, Items, and Abilities without needing to open the full Developer Tools application.

**Status**: 🎯 In Progress

## Purpose

- **Quick Data Access**: View game data quickly from command line
- **Simple Interface**: No complex UI, just console output
- **Fast Navigation**: Browse catalogs efficiently
- **Development Tool**: Help developers verify data during development

## Features

- View all Pokemon in catalog
- View all Moves in catalog
- View all Items in catalog
- View all Abilities in catalog
- View all Status Effects in catalog
- View all Weather conditions in catalog
- View all Terrain conditions in catalog
- View all Hazards in catalog
- View all Side Conditions in catalog
- View all Field Effects in catalog
- Interactive selection - click rows to see detailed information
- Search by name (future enhancement)
- Filter by type (future enhancement)

## Project Structure

```
PokemonUltimate.DataViewer/
├── Program.cs              # Application entry point
├── MainForm.cs             # Main form with TabControl
└── Tabs/
    ├── PokemonDataTab.cs      # Pokemon data display tab
    ├── MoveDataTab.cs         # Move data display tab
    ├── ItemDataTab.cs         # Item data display tab
    ├── AbilityDataTab.cs      # Ability data display tab
    ├── StatusDataTab.cs       # Status Effect data display tab
    ├── WeatherDataTab.cs      # Weather data display tab
    ├── TerrainDataTab.cs      # Terrain data display tab
    ├── HazardDataTab.cs       # Hazard data display tab
    ├── SideConditionDataTab.cs # Side Condition data display tab
    └── FieldEffectDataTab.cs  # Field Effect data display tab
```

## Features

- **Visual Interface**: Windows Forms application with tabbed interface
- **Data Grids**: DataGridView controls for displaying data in tables
- **Details Panel**: RichTextBox showing detailed information for selected items
- **Ten Tabs**: Pokemon, Moves, Items, Abilities, Status Effects, Weather, Terrain, Hazards, Side Conditions, and Field Effects
- **Interactive**: Click on any row to see detailed information
- **Consistent Layout**: All tabs follow the same pattern with title, count, and data grid

## Usage

```bash
# Run the data viewer
dotnet run --project PokemonUltimate.DataViewer

# Or build and run
dotnet build PokemonUltimate.DataViewer
dotnet run --project PokemonUltimate.DataViewer
```

The application will open a Windows Forms window with tabs for each data type.

## Integration Points

### With Game Data (Feature 1)

- Uses `PokemonSpeciesData` from `PokemonUltimate.Core.Blueprints`
- Uses `MoveData` from `PokemonUltimate.Core.Blueprints`
- Uses `ItemData` from `PokemonUltimate.Core.Blueprints`
- Uses `AbilityData` from `PokemonUltimate.Core.Blueprints`

### With Content

- Uses `PokemonCatalog`, `MoveCatalog`, `ItemCatalog`, `AbilityCatalog` from `PokemonUltimate.Content.Catalogs`

## Related Documents

- **[Feature 6: Development Tools](../README.md)** - Parent feature
- **[Feature 6 Architecture](../architecture.md)** - Technical specification
- **[Feature 1: Game Data](../../1-game-data/README.md)** - Game data structures

---

**Last Updated**: January 2025

