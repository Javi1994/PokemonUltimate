using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PokemonUltimate.Core.Instances;

/// <summary>
/// Pokemon Display component for showing Pokemon sprite, name, and level.
/// 
/// **Feature**: 4: Unity Integration
/// **Sub-Feature**: 4.2: UI Foundation
/// **Documentation**: See `docs/features/4-unity-integration/4.2-ui-foundation/README.md`
/// </summary>
public class PokemonDisplay : MonoBehaviour
{
    [SerializeField] private Image spriteImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;

    /// <summary>
    /// Displays a Pokemon instance with its sprite, name, and level.
    /// </summary>
    /// <param name="pokemon">The Pokemon instance to display</param>
    public void Display(PokemonInstance pokemon)
    {
        if (pokemon == null)
        {
            Debug.LogWarning("[UI] PokemonDisplay.Display() - Cannot display null Pokemon");
            return;
        }

        Debug.Log($"[UI] PokemonDisplay.Display() called - Updating UI for: {pokemon.DisplayName} Lv.{pokemon.Level}");

        if (nameText != null)
        {
            nameText.text = pokemon.DisplayName;
            Debug.Log($"[UI] PokemonDisplay - Name text updated: \"{nameText.text}\"");
        }
        else
        {
            Debug.LogWarning("[UI] PokemonDisplay - nameText is null, cannot update name!");
        }

        if (levelText != null)
        {
            levelText.text = $"Lv. {pokemon.Level}";
            Debug.Log($"[UI] PokemonDisplay - Level text updated: \"{levelText.text}\"");
        }
        else
        {
            Debug.LogWarning("[UI] PokemonDisplay - levelText is null, cannot update level!");
        }

        // TODO: Load sprite from Resources or Addressables
        // For now, sprite loading will be implemented in a later phase
        if (spriteImage != null)
        {
            // spriteImage.sprite = LoadPokemonSprite(pokemon.Species.Name);
            Debug.Log($"[UI] PokemonDisplay - Sprite image component found (sprite loading not yet implemented)");
        }
        else
        {
            Debug.LogWarning("[UI] PokemonDisplay - spriteImage is null!");
        }
        
        Debug.Log($"[UI] PokemonDisplay.Display() completed - UI updated successfully");
    }
}

