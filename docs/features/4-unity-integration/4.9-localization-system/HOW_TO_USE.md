# How to Use Localization System

> **Guide for using the localization system in your application**

**Sub-Feature Number**: 4.9  
**Parent Feature**: Feature 4: Unity Integration

## 🚀 Quick Start: Setting Language to Spanish

### Para proyectos Windows Forms (BattleSimulator, DeveloperTools, DataViewer)

**Todos los proyectos Windows Forms ya están configurados para usar español por defecto.**

Simplemente ejecuta cualquier aplicación y verás la interfaz en español:

-   **BattleSimulator**: Títulos, tabs y mensajes en español
-   **DeveloperTools**: Todos los tabs y mensajes de error en español
-   **DataViewer**: Todos los tabs en español

**Para cambiar el idioma en código:**

```csharp
// Al inicio de tu aplicación (en Program.cs o constructor del Form)
using PokemonUltimate.Core.Localization;

// Configurar español (por defecto)
LocalizationManager.Initialize("es");

// O cambiar en tiempo de ejecución
LocalizationManager.SetSpanish();
LocalizationManager.SetEnglish();
LocalizationManager.SetFrench();
```

### Para proyectos Unity

**Paso 1**: Agrega `LocalizationManager` a tu escena de Unity:

1. Crea un GameObject vacío llamado "LocalizationManager"
2. Agrega el componente `LocalizationManager.cs` (ya creado en `PokemonUltimateUnity/Assets/Scripts/Battle/`)
3. En el Inspector, configura `Default Language` a `"es"` (español)
4. Marca `Save Language Preference` si quieres que se guarde la preferencia

**Paso 2**: Configura `BattleManager`:

1. Selecciona el GameObject con `BattleManager`
2. En el Inspector, en la sección "Localization":
    - `Language Code` = `"es"` (español)
    - `Use Localization Manager` = ✅ (marcado)

**¡Listo!** Los mensajes de batalla se mostrarán en español automáticamente.

### Opción alternativa: Solo código

Si prefieres configurarlo solo en código, el `BattleManager` ya tiene:

```csharp
[Header("Localization")]
[SerializeField] private string languageCode = "es"; // Spanish by default
```

Simplemente asegúrate de que `languageCode` esté en `"es"` en el Inspector de Unity.

### Opción 3: Cambiar idioma en tiempo de ejecución

Si tienes el `LocalizationManager` en la escena:

```csharp
// Cambiar a español
LocalizationManager.Instance.SetSpanish();

// O directamente
LocalizationManager.Instance.SetLanguage("es");
```

### Usar directamente en código C# (SDK)

```csharp
using PokemonUltimate.Content.Providers;
using PokemonUltimate.Core.Localization;

// Crear provider
var localizationProvider = new LocalizationProvider();

// Configurar español
localizationProvider.CurrentLanguage = "es";

// Usar en BattleMessageFormatter
var formatter = new BattleMessageFormatter(localizationProvider);

// Ahora todos los mensajes estarán en español
var message = formatter.FormatMoveUsed(pokemon, move);
// Resultado: "Pikachu usó Rayo!"
```

## Available Languages

-   **"en"** - English (default)
-   **"es"** - Spanish (Español)
-   **"fr"** - French (Français)

## Changing Language at Runtime

```csharp
// Change to Spanish
LocalizationManager.Instance.SetLanguage("es");

// Change to French
LocalizationManager.Instance.SetLanguage("fr");

// Change back to English
LocalizationManager.Instance.SetLanguage("en");
```

## Using Translated Content Names

### In Battle Messages

Battle messages automatically use translated names when using `BattleMessageFormatter`:

```csharp
var formatter = new BattleMessageFormatter(localizationProvider);
formatter.CurrentLanguage = "es"; // Spanish

var message = formatter.FormatMoveUsed(pokemon, move);
// Result: "Pikachu usó Rayo!" (instead of "Pikachu used Thunderbolt!")
```

### Getting Translated Names Directly

```csharp
var localizationProvider = LocalizationManager.Instance.Provider;
localizationProvider.CurrentLanguage = "es";

// Get translated move name
var move = MoveCatalog.Thunderbolt;
var moveName = move.GetDisplayName(localizationProvider); // "Rayo"

// Get translated Pokemon name
var pokemon = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
var pokemonName = pokemon.Species.GetDisplayName(localizationProvider); // "Pikachu"

// Get translated ability name
var ability = AbilityCatalog.Intimidate;
var abilityName = ability.GetDisplayName(localizationProvider); // "Intimidación"

// Get translated item name
var item = ItemCatalog.Leftovers;
var itemName = item.GetDisplayName(localizationProvider); // "Restos"
```

## Example: Language Selection Menu

```csharp
using UnityEngine;
using UnityEngine.UI;

public class LanguageSelector : MonoBehaviour
{
    public Button englishButton;
    public Button spanishButton;
    public Button frenchButton;

    void Start()
    {
        englishButton.onClick.AddListener(() => SetLanguage("en"));
        spanishButton.onClick.AddListener(() => SetLanguage("es"));
        frenchButton.onClick.AddListener(() => SetLanguage("fr"));
    }

    void SetLanguage(string languageCode)
    {
        LocalizationManager.Instance.SetLanguage(languageCode);

        // Refresh UI if needed
        RefreshUI();
    }

    void RefreshUI()
    {
        // Update all UI elements with new language
        // This depends on your UI implementation
    }
}
```

## Integration Points

### BattleMessageFormatter

Automatically uses translated names:

```csharp
var formatter = new BattleMessageFormatter(localizationProvider);
formatter.CurrentLanguage = "es";

// All messages will be in Spanish
var message1 = formatter.FormatMoveUsed(pokemon, move);
var message2 = formatter.Format(LocalizationKey.BattleMissed);
```

### UI Components

Update UI to use translated names:

```csharp
// In PokemonDisplay.cs
public void UpdatePokemon(PokemonInstance pokemon)
{
    var provider = LocalizationManager.Instance.Provider;
    pokemonNameText.text = pokemon.Species.GetDisplayName(provider);
}

// In MoveSelectionMenu.cs
public void DisplayMoves(List<MoveInstance> moves)
{
    var provider = LocalizationManager.Instance.Provider;
    foreach (var move in moves)
    {
        var moveName = move.Move.GetDisplayName(provider);
        // Display moveName in UI
    }
}
```

## Fallback Behavior

If a translation is not found:

-   **Names**: Falls back to English name
-   **Messages**: Falls back to English message
-   **No crashes**: System is fail-safe

## Testing

```csharp
// Test Spanish translation
var provider = new LocalizationProvider();
provider.CurrentLanguage = "es";

var move = MoveCatalog.Thunderbolt;
var name = move.GetDisplayName(provider);
Assert.That(name, Is.EqualTo("Rayo"));
```

## Integration Examples

### Windows Forms Application

```csharp
using PokemonUltimate.Core.Localization;

public partial class MainForm : Form
{
    public MainForm()
    {
        // Initialize localization (defaults to Spanish)
        LocalizationManager.Initialize("es");

        InitializeComponent();
    }

    private void InitializeComponent()
    {
        // Use localized strings
        this.Text = LocalizationManager.Instance.GetString(LocalizationKey.UI_BattleSimulator_Title);
        this.tabBattleMode.Text = LocalizationManager.Instance.GetString(LocalizationKey.UI_BattleSimulator_TabBattleMode);

        // Show localized error messages
        MessageBox.Show(
            LocalizationManager.Instance.GetString(LocalizationKey.UI_BattleSimulator_ConfigurePokemonForEachTeam),
            LocalizationManager.Instance.GetString(LocalizationKey.UI_BattleSimulator_ValidationError),
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
```

### Using Content Names in Windows Forms

```csharp
using PokemonUltimate.Core.Extensions;
using PokemonUltimate.Content.Catalogs.Pokemon;

// Get translated Pokemon name
var pokemon = PokemonCatalog.Pikachu;
var provider = LocalizationManager.Instance;
var pokemonName = pokemon.GetDisplayName(provider); // "Pikachu" (or translated name)

// Display in ComboBox
comboPokemon.Items.Add(pokemonName);
```

## Related Documents

-   **[Architecture.md](architecture.md)** - Technical details
-   **[CONTENT_LOCALIZATION_DESIGN.md](CONTENT_LOCALIZATION_DESIGN.md)** - Content localization design
-   **[IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)** - Implementation summary

---

**Last Updated**: 2025-01-XX
