# Localization Standardization Guide

> **Guía para estandarizar el uso del sistema de localización en todo el proyecto**

**Sub-Feature Number**: 4.9  
**Parent Feature**: Feature 4: Unity Integration

## 🎯 Objetivo

Estandarizar el uso del sistema de localización para que **TODO** el texto que se muestra al usuario pase automáticamente por el sistema de traducciones, evitando tener que cambiar código uno por uno.

## 📋 Principios

1. **Nunca hardcodear strings** - Todo texto visible debe usar `LocalizationManager`
2. **Usar extension methods** - Para enums y objetos que tienen nombres (Pokemon, Moves, Types, Status)
3. **Usar helpers** - Para operaciones comunes (MessageBox, ComboBox, DataGridView)
4. **Consistencia** - Mismo patrón en todos los proyectos

## 🔧 Extension Methods Disponibles

### Para Enums

```csharp
// Tipos de Pokemon
var type = PokemonType.Fire;
var translatedName = type.GetDisplayName(LocalizationManager.Instance); // "Fuego"

// Estados persistentes
var status = PersistentStatus.Burn;
var translatedName = status.GetDisplayName(LocalizationManager.Instance); // "Quemadura"

// Estados volátiles
var volatileStatus = VolatileStatus.Confusion;
var translatedName = volatileStatus.GetDisplayName(LocalizationManager.Instance); // "Confusión"
```

### Para Contenido

```csharp
// Movimientos
var move = MoveCatalog.Thunderbolt;
var name = move.GetDisplayName(LocalizationManager.Instance); // "Rayo"
var description = move.GetDescription(LocalizationManager.Instance); // Descripción traducida

// Pokemon
var pokemon = PokemonCatalog.Pikachu;
var name = pokemon.GetDisplayName(LocalizationManager.Instance); // "Pikachu"

// Habilidades
var ability = AbilityCatalog.Intimidate;
var name = ability.GetDisplayName(LocalizationManager.Instance); // "Intimidación"

// Items
var item = ItemCatalog.Leftovers;
var name = item.GetDisplayName(LocalizationManager.Instance); // "Restos"
```

## 🛠️ LocalizationHelper - Helpers Estandarizados

### Para Proyectos Windows Forms

En proyectos Windows Forms (`DeveloperTools`, `BattleSimulator`, `DataViewer`), usa `WinFormsLocalizationHelper`:

```csharp
using PokemonUltimate.DeveloperTools.Localization; // O el namespace correspondiente

// ComboBox con Enums Traducidos
WinFormsLocalizationHelper.PopulateEnumComboBox<PokemonType>(
    comboBox,
    includeNone: false,
    filter: type => type != PokemonType.None
);

// Obtener valor seleccionado
var selectedType = WinFormsLocalizationHelper.GetEnumFromComboBox<PokemonType>(comboBox);

// Seleccionar valor
WinFormsLocalizationHelper.SetSelectedEnum(comboBox, PokemonType.Fire);

// MessageBox Localizado
WinFormsLocalizationHelper.ShowLocalizedMessage(LocalizationKey.UI_BattleSimulator_Error);
WinFormsLocalizationHelper.ShowLocalizedError(LocalizationKey.UI_DeveloperTools_PokemonNotFound);
WinFormsLocalizationHelper.ShowLocalizedWarning(LocalizationKey.UI_BattleSimulator_ValidationError);
```

### Para Proyectos Cross-Platform (Core, Unity)

En proyectos que no dependen de Windows Forms, usa `LocalizationHelper`:

```csharp
using PokemonUltimate.Core.Localization;

// Obtener nombre traducido de enum
var typeName = LocalizationHelper.GetEnumDisplayName(PokemonType.Fire);

// Obtener título de aplicación
var title = LocalizationHelper.GetApplicationTitle();
```

## 📝 Patrones de Uso

### Patrón 1: Labels y Textos Estáticos

**❌ Incorrecto:**

```csharp
label.Text = "Configuration";
button.Text = "Run Battles";
```

**✅ Correcto:**

```csharp
label.Text = LocalizationManager.Instance.GetString(LocalizationKey.UI_DeveloperTools_Configuration);
button.Text = LocalizationManager.Instance.GetString(LocalizationKey.UI_DeveloperTools_RunBattles);
```

### Patrón 2: ComboBox con Enums

**❌ Incorrecto:**

```csharp
foreach (PokemonType type in Enum.GetValues<PokemonType>())
{
    comboBox.Items.Add(type); // Muestra "Fire" en lugar de "Fuego"
}
```

**✅ Correcto:**

```csharp
// En proyectos Windows Forms
WinFormsLocalizationHelper.PopulateEnumComboBox<PokemonType>(comboBox);

// O manualmente
var provider = LocalizationManager.Instance;
foreach (PokemonType type in Enum.GetValues<PokemonType>())
{
    comboBox.Items.Add(new TypeDisplayItem(type, provider));
}
```

### Patrón 3: DataGridView con Datos Traducidos

**❌ Incorrecto:**

```csharp
row.CreateCells(dgv, pokemon.Name, pokemon.PrimaryType.ToString());
```

**✅ Correcto:**

```csharp
var provider = LocalizationManager.Instance;
row.CreateCells(
    dgv,
    pokemon.GetDisplayName(provider),
    pokemon.PrimaryType.GetDisplayName(provider)
);
```

### Patrón 4: MessageBox

**❌ Incorrecto:**

```csharp
MessageBox.Show("Error occurred", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
```

**✅ Correcto:**

```csharp
// En proyectos Windows Forms
WinFormsLocalizationHelper.ShowLocalizedError(LocalizationKey.UI_DeveloperTools_ErrorOccurred);
```

### Patrón 5: Strings con Formato

**❌ Incorrecto:**

```csharp
var message = $"Battle ended: {result.Outcome}";
```

**✅ Correcto:**

```csharp
var message = LocalizationManager.Instance.GetString(
    LocalizationKey.UI_BattleSimulator_BattleEnded,
    result.Outcome.ToString()
);
```

## 🔍 Checklist para Nuevo Código

Al agregar nuevo código que muestra texto al usuario:

-   [ ] ¿Hay strings hardcodeados? → Usar `LocalizationManager.Instance.GetString()`
-   [ ] ¿Se muestran enums? → Usar extension methods `GetDisplayName()`
-   [ ] ¿Hay ComboBox con enums? → Usar `WinFormsLocalizationHelper.PopulateEnumComboBox()` (WinForms) o `LocalizationHelper.GetEnumDisplayName()` (cross-platform)
-   [ ] ¿Hay MessageBox? → Usar `WinFormsLocalizationHelper.ShowLocalizedMessage()` (WinForms)
-   [ ] ¿Hay DataGridView? → Traducir valores antes de agregar a la tabla
-   [ ] ¿Hay nombres de contenido? → Usar extension methods (`GetDisplayName()`)

## 📚 Extension Methods Disponibles

| Tipo                 | Extension Method                       | Ubicación                         |
| -------------------- | -------------------------------------- | --------------------------------- |
| `PokemonType`        | `GetDisplayName()`                     | `PokemonTypeExtensions.cs`        |
| `PersistentStatus`   | `GetDisplayName()`                     | `PersistentStatusExtensions.cs`   |
| `VolatileStatus`     | `GetDisplayName()`                     | `VolatileStatusExtensions.cs`     |
| `MoveData`           | `GetDisplayName()`, `GetDescription()` | `MoveDataExtensions.cs`           |
| `PokemonSpeciesData` | `GetDisplayName()`                     | `PokemonSpeciesDataExtensions.cs` |
| `AbilityData`        | `GetDisplayName()`, `GetDescription()` | `AbilityDataExtensions.cs`        |
| `ItemData`           | `GetDisplayName()`, `GetDescription()` | `ItemDataExtensions.cs`           |

## 🎨 Ejemplo Completo: Tab con Localización

```csharp
public partial class MyTab : UserControl
{
    private ComboBox comboType;
    private Button btnRun;
    private Label lblStatus;

    public MyTab()
    {
        InitializeComponent();
        LoadLocalizedData();
    }

    private void InitializeComponent()
    {
        // Labels traducidos
        var lblTitle = new Label
        {
            Text = LocalizationManager.Instance.GetString(LocalizationKey.UI_MyTab_Title)
        };

        // ComboBox con tipos traducidos
        comboType = new ComboBox();
        LocalizationHelper.PopulateEnumComboBox<PokemonType>(comboType);

        // Botón traducido
        btnRun = new Button
        {
            Text = LocalizationManager.Instance.GetString(LocalizationKey.UI_MyTab_RunButton)
        };
        btnRun.Click += BtnRun_Click;
    }

    private void BtnRun_Click(object sender, EventArgs e)
    {
        try
        {
            var selectedType = WinFormsLocalizationHelper.GetEnumFromComboBox<PokemonType>(comboType);
            // ... lógica ...

            lblStatus.Text = LocalizationManager.Instance.GetString(
                LocalizationKey.UI_MyTab_Success
            );
        }
        catch (Exception ex)
        {
            WinFormsLocalizationHelper.ShowLocalizedError(
                LocalizationKey.UI_MyTab_Error,
                ex.Message
            );
        }
    }
}
```

## 🚀 Migración de Código Existente

### Paso 1: Identificar Strings Hardcodeados

```bash
# Buscar strings hardcodeados comunes
grep -r '\.Text = "' --include="*.cs"
grep -r 'MessageBox\.Show' --include="*.cs"
grep -r '\.ToString()' --include="*.cs" | grep -v "//"
```

### Paso 2: Crear Claves de Localización

Agregar claves en `LocalizationKey.cs`:

```csharp
public const string UI_MyFeature_MyText = "ui_my_feature_my_text";
```

### Paso 3: Agregar Traducciones

En `LocalizationDataProvider.cs`:

```csharp
Register(LocalizationKey.UI_MyFeature_MyText, new Dictionary<string, string>
{
    { "en", "My Text" },
    { "es", "Mi Texto" },
    { "fr", "Mon Texte" }
});
```

### Paso 4: Reemplazar en Código

```csharp
// Antes
label.Text = "My Text";

// Después
label.Text = LocalizationManager.Instance.GetString(LocalizationKey.UI_MyFeature_MyText);
```

## 📖 Referencias

-   **[HOW_TO_USE.md](HOW_TO_USE.md)** - Guía de uso básico
-   **[Architecture.md](architecture.md)** - Arquitectura del sistema
-   **[LocalizationHelper.cs](../../../../PokemonUltimate.Core/Localization/LocalizationHelper.cs)** - Helpers disponibles

---

**Last Updated**: 2025-01-XX
