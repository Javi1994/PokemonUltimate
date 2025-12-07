using UnityEngine;
using PokemonUltimate.Content.Providers;
using PokemonUltimate.Core.Localization;

/// <summary>
/// Singleton manager for localization in Unity.
/// Provides centralized access to ILocalizationProvider.
/// 
/// **Feature**: 4: Unity Integration
/// **Sub-Feature**: 4.9: Localization System
/// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/HOW_TO_USE.md`
/// </summary>
public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    [Header("Language Settings")]
    [SerializeField] private string defaultLanguage = "es"; // Spanish by default
    [SerializeField] private bool saveLanguagePreference = true;

    private ILocalizationProvider _provider;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize provider
            _provider = new LocalizationProvider();

            // Load saved language preference or use default
            string savedLanguage = saveLanguagePreference
                ? PlayerPrefs.GetString("Language", defaultLanguage)
                : defaultLanguage;

            _provider.CurrentLanguage = savedLanguage;

            Debug.Log($"[LocalizationManager] Initialized with language: {_provider.CurrentLanguage}");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Gets the localization provider instance.
    /// </summary>
    public ILocalizationProvider Provider => _provider;

    /// <summary>
    /// Gets the current language code.
    /// </summary>
    public string CurrentLanguage => _provider.CurrentLanguage;

    /// <summary>
    /// Sets the current language and saves preference if enabled.
    /// </summary>
    /// <param name="languageCode">Language code (e.g., "en", "es", "fr")</param>
    public void SetLanguage(string languageCode)
    {
        if (string.IsNullOrEmpty(languageCode))
        {
            Debug.LogWarning("[LocalizationManager] Cannot set empty language code");
            return;
        }

        _provider.CurrentLanguage = languageCode;

        if (saveLanguagePreference)
        {
            PlayerPrefs.SetString("Language", languageCode);
            PlayerPrefs.Save();
        }

        Debug.Log($"[LocalizationManager] Language changed to: {languageCode}");
    }

    /// <summary>
    /// Sets language to Spanish.
    /// </summary>
    public void SetSpanish()
    {
        SetLanguage("es");
    }

    /// <summary>
    /// Sets language to English.
    /// </summary>
    public void SetEnglish()
    {
        SetLanguage("en");
    }

    /// <summary>
    /// Sets language to French.
    /// </summary>
    public void SetFrench()
    {
        SetLanguage("fr");
    }

    /// <summary>
    /// Gets available languages.
    /// </summary>
    public System.Collections.Generic.IEnumerable<string> GetAvailableLanguages()
    {
        return _provider.GetAvailableLanguages();
    }
}
