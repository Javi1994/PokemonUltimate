using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// HP Bar component for displaying Pokemon HP.
/// Updates fill amount and text based on current/max HP.
/// 
/// **Feature**: 4: Unity Integration
/// **Sub-Feature**: 4.2: UI Foundation
/// **Documentation**: See `docs/features/4-unity-integration/4.2-ui-foundation/README.md`
/// </summary>
public class HPBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI hpText;

    /// <summary>
    /// Updates the HP bar display with current and max HP values.
    /// </summary>
    /// <param name="current">Current HP value</param>
    /// <param name="max">Maximum HP value</param>
    public void UpdateHP(int current, int max)
    {
        Debug.Log($"[UI] HPBar.UpdateHP() called - Updating UI: {current}/{max} ({(max > 0 ? (current * 100f / max) : 0):F1}%)");
        
        if (fillImage != null)
        {
            float fillAmount = max > 0 ? (float)current / max : 0f;
            fillImage.fillAmount = Mathf.Clamp01(fillAmount);
            Debug.Log($"[UI] HPBar - Fill image updated: fillAmount = {fillImage.fillAmount:F3}");
        }
        else
        {
            Debug.LogWarning("[UI] HPBar - fillImage is null, cannot update fill amount!");
        }

        if (hpText != null)
        {
            hpText.text = $"{current}/{max}";
            Debug.Log($"[UI] HPBar - Text updated: \"{hpText.text}\"");
        }
        else
        {
            Debug.LogWarning("[UI] HPBar - hpText is null, cannot update text!");
        }
        
        Debug.Log($"[UI] HPBar.UpdateHP() completed - UI updated successfully");
    }
}

