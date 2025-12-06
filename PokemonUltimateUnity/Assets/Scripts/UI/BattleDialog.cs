using System.Threading.Tasks;
using UnityEngine;
using TMPro;

/// <summary>
/// Battle Dialog component for displaying battle messages.
/// Supports typewriter effect and input waiting.
/// 
/// **Feature**: 4: Unity Integration
/// **Sub-Feature**: 4.2: UI Foundation
/// **Documentation**: See `docs/features/4-unity-integration/4.2-ui-foundation/README.md`
/// </summary>
public class BattleDialog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private GameObject dialogBox;
    
    [SerializeField] private float typewriterDelay = 0.03f; // Delay per character in seconds
    private bool _waitingForInput = false;

    /// <summary>
    /// Shows a message in the dialog box with optional typewriter effect.
    /// </summary>
    /// <param name="text">The message to display</param>
    /// <param name="waitForInput">Whether to wait for player input before completing</param>
    /// <returns>Task that completes when message is shown (and input received if waitForInput is true)</returns>
    public async Task ShowMessage(string text, bool waitForInput = true)
    {
        if (dialogBox != null)
        {
            dialogBox.SetActive(true);
        }

        if (dialogText != null)
        {
            // Typewriter effect
            dialogText.text = "";
            foreach (char c in text)
            {
                dialogText.text += c;
                await Task.Delay((int)(typewriterDelay * 1000)); // Convert to milliseconds
            }
        }

        if (waitForInput)
        {
            _waitingForInput = true;
            await WaitForInput();
            _waitingForInput = false;
        }
    }

    /// <summary>
    /// Waits for player input (Space or Enter key).
    /// </summary>
    private async Task WaitForInput()
    {
        while (!Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyDown(KeyCode.Return))
        {
            await Task.Yield();
        }
    }

    /// <summary>
    /// Hides the dialog box.
    /// </summary>
    public void Hide()
    {
        if (dialogBox != null)
        {
            dialogBox.SetActive(false);
        }
    }
}

