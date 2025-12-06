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
            Debug.LogWarning("PokemonDisplay: Cannot display null Pokemon");
            return;
        }

        if (nameText != null)
        {
            nameText.text = pokemon.DisplayName;
        }

        if (levelText != null)
        {
            levelText.text = $"Lv. {pokemon.Level}";
        }

        // TODO: Load sprite from Resources or Addressables
        // For now, sprite loading will be implemented in a later phase
        if (spriteImage != null)
        {
            // spriteImage.sprite = LoadPokemonSprite(pokemon.Species.Name);
        }
    }
}

