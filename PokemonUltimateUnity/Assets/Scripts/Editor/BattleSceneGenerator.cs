using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// Editor script to automatically generate the battle scene with all UI components.
/// 
/// **Feature**: 4: Unity Integration
/// **Sub-Feature**: 4.2: UI Foundation
/// **Documentation**: See `docs/features/4-unity-integration/4.2-ui-foundation/SCENE_SETUP_GUIDE.md`
/// </summary>
public class BattleSceneGenerator : EditorWindow
{
    [MenuItem("PokemonUltimate/Generate Battle Scene")]
    public static void GenerateBattleScene()
    {
        // Create new scene
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        newScene.name = "BattleScene";

        // Create Canvas
        GameObject canvas = CreateCanvas();
        
        // Create Player Side UI
        GameObject playerSide = CreatePlayerSide(canvas);
        
        // Create Enemy Side UI
        GameObject enemySide = CreateEnemySide(canvas);
        
        // Create Dialog Box
        GameObject dialogBox = CreateDialogBox(canvas);
        
        // Create Battle UI Setup
        GameObject battleSetup = CreateBattleUISetup(canvas, playerSide, enemySide, dialogBox);
        
        // Save scene
        string scenePath = "Assets/Scenes/BattleScene.unity";
        EditorSceneManager.SaveScene(newScene, scenePath);
        
        Debug.Log($"✅ Battle Scene generated successfully at {scenePath}");
        EditorUtility.DisplayDialog("Success", "Battle Scene generated successfully!", "OK");
    }

    private static GameObject CreateCanvas()
    {
        GameObject canvasObj = new GameObject("BattleCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        return canvasObj;
    }

    private static GameObject CreatePlayerSide(GameObject parent)
    {
        // Create Player Side Panel
        GameObject playerSide = CreateUIElement("PlayerSide", parent);
        RectTransform playerRect = playerSide.GetComponent<RectTransform>();
        playerRect.anchorMin = new Vector2(0, 0);
        playerRect.anchorMax = new Vector2(0, 0);
        playerRect.pivot = new Vector2(0, 0);
        playerRect.anchoredPosition = new Vector2(20, 20);
        playerRect.sizeDelta = new Vector2(450, 160);
        
        // Create Player Pokemon Display
        GameObject playerDisplay = CreatePlayerPokemonDisplay(playerSide);
        
        // Create Player HP Bar
        GameObject playerHPBar = CreatePlayerHPBar(playerSide);
        
        return playerSide;
    }

    private static GameObject CreatePlayerPokemonDisplay(GameObject parent)
    {
        GameObject display = CreateUIElement("PlayerPokemonDisplay", parent);
        RectTransform rect = display.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0.5f);
        rect.anchorMax = new Vector2(0, 0.5f);
        rect.pivot = new Vector2(0, 0.5f);
        rect.anchoredPosition = new Vector2(10, 0);
        rect.sizeDelta = new Vector2(220, 70);
        
        // Add PokemonDisplay component
        PokemonDisplay pokemonDisplay = display.AddComponent<PokemonDisplay>();
        
        // Create Sprite Image
        GameObject spriteImage = CreateUIElement("SpriteImage", display);
        Image image = spriteImage.AddComponent<Image>();
        image.color = new Color(0.8f, 0.8f, 0.8f, 1f); // Light gray placeholder
        RectTransform spriteRect = spriteImage.GetComponent<RectTransform>();
        spriteRect.anchorMin = new Vector2(0, 0);
        spriteRect.anchorMax = new Vector2(0, 0);
        spriteRect.pivot = new Vector2(0.5f, 0.5f);
        spriteRect.anchoredPosition = new Vector2(35, 0);
        spriteRect.sizeDelta = new Vector2(50, 50);
        
        // Create Name Text
        GameObject nameText = CreateTextElement("NameText", display, "Pikachu", 24);
        RectTransform nameRect = nameText.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.5f);
        nameRect.anchorMax = new Vector2(1, 1);
        nameRect.pivot = new Vector2(0, 0.5f);
        nameRect.sizeDelta = Vector2.zero;
        nameRect.anchoredPosition = new Vector2(80, 5);
        nameText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;
        nameText.GetComponent<TextMeshProUGUI>().enableAutoSizing = false;
        nameText.GetComponent<TextMeshProUGUI>().overflowMode = TextOverflowModes.Overflow;
        
        // Create Level Text
        GameObject levelText = CreateTextElement("LevelText", display, "Lv. 50", 18);
        RectTransform levelRect = levelText.GetComponent<RectTransform>();
        levelRect.anchorMin = new Vector2(0, 0);
        levelRect.anchorMax = new Vector2(1, 0.5f);
        levelRect.pivot = new Vector2(0, 0.5f);
        levelRect.sizeDelta = Vector2.zero;
        levelRect.anchoredPosition = new Vector2(80, -5);
        levelText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;
        
        // Assign references using reflection
        SetPrivateField(pokemonDisplay, "spriteImage", image);
        SetPrivateField(pokemonDisplay, "nameText", nameText.GetComponent<TextMeshProUGUI>());
        SetPrivateField(pokemonDisplay, "levelText", levelText.GetComponent<TextMeshProUGUI>());
        
        return display;
    }

    private static GameObject CreatePlayerHPBar(GameObject parent)
    {
        GameObject hpBarObj = CreateUIElement("PlayerHPBar", parent);
        RectTransform hpBarRect = hpBarObj.GetComponent<RectTransform>();
        hpBarRect.anchorMin = new Vector2(0, 0);
        hpBarRect.anchorMax = new Vector2(1, 0);
        hpBarRect.pivot = new Vector2(0, 0);
        hpBarRect.sizeDelta = new Vector2(-20, 30);
        hpBarRect.anchoredPosition = new Vector2(10, 10);
        
        // Add Slider as base
        Slider slider = hpBarObj.AddComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.value = 100;
        slider.interactable = false;
        
        // Create Background
        GameObject background = CreateUIElement("Background", hpBarObj);
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        slider.targetGraphic = bgImage;
        
        // Create Fill Area
        GameObject fillArea = CreateUIElement("Fill Area", hpBarObj);
        RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.sizeDelta = Vector2.zero;
        fillAreaRect.anchoredPosition = Vector2.zero;
        
        // Create Fill
        GameObject fill = CreateUIElement("Fill", fillArea);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.2f, 0.8f, 0.2f, 1f); // Green
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(1, 1);
        fillRect.sizeDelta = Vector2.zero;
        slider.fillRect = fillRect;
        
        // Create HP Text
        GameObject hpText = CreateTextElement("HPText", hpBarObj, "100/100", 18);
        RectTransform hpTextRect = hpText.GetComponent<RectTransform>();
        hpTextRect.anchorMin = new Vector2(0.5f, 0.5f);
        hpTextRect.anchorMax = new Vector2(0.5f, 0.5f);
        hpTextRect.sizeDelta = new Vector2(120, 30);
        hpTextRect.anchoredPosition = Vector2.zero;
        hpText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        
        // Add HPBar component
        HPBar hpBar = hpBarObj.AddComponent<HPBar>();
        SetPrivateField(hpBar, "fillImage", fillImage);
        SetPrivateField(hpBar, "hpText", hpText.GetComponent<TextMeshProUGUI>());
        
        return hpBarObj;
    }

    private static GameObject CreateEnemySide(GameObject parent)
    {
        // Create Enemy Side Panel
        GameObject enemySide = CreateUIElement("EnemySide", parent);
        RectTransform enemyRect = enemySide.GetComponent<RectTransform>();
        enemyRect.anchorMin = new Vector2(1, 1);
        enemyRect.anchorMax = new Vector2(1, 1);
        enemyRect.pivot = new Vector2(1, 1);
        enemyRect.anchoredPosition = new Vector2(-20, -20);
        enemyRect.sizeDelta = new Vector2(450, 160);
        
        // Create Enemy Pokemon Display
        GameObject enemyDisplay = CreateEnemyPokemonDisplay(enemySide);
        
        // Create Enemy HP Bar
        GameObject enemyHPBar = CreateEnemyHPBar(enemySide);
        
        return enemySide;
    }

    private static GameObject CreateEnemyPokemonDisplay(GameObject parent)
    {
        GameObject display = CreateUIElement("EnemyPokemonDisplay", parent);
        RectTransform rect = display.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 0.5f);
        rect.anchorMax = new Vector2(1, 0.5f);
        rect.pivot = new Vector2(1, 0.5f);
        rect.anchoredPosition = new Vector2(-10, 0);
        rect.sizeDelta = new Vector2(220, 70);
        
        PokemonDisplay pokemonDisplay = display.AddComponent<PokemonDisplay>();
        
        // Create Sprite Image
        GameObject spriteImage = CreateUIElement("SpriteImage", display);
        Image image = spriteImage.AddComponent<Image>();
        image.color = new Color(0.8f, 0.8f, 0.8f, 1f);
        RectTransform spriteRect = spriteImage.GetComponent<RectTransform>();
        spriteRect.anchorMin = new Vector2(1, 0);
        spriteRect.anchorMax = new Vector2(1, 0);
        spriteRect.pivot = new Vector2(0.5f, 0.5f);
        spriteRect.anchoredPosition = new Vector2(-35, 0);
        spriteRect.sizeDelta = new Vector2(50, 50);
        
        // Create Name Text
        GameObject nameText = CreateTextElement("NameText", display, "Charmander", 24);
        RectTransform nameRect = nameText.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.5f);
        nameRect.anchorMax = new Vector2(1, 1);
        nameRect.pivot = new Vector2(1, 0.5f);
        nameRect.sizeDelta = Vector2.zero;
        nameRect.anchoredPosition = new Vector2(-80, 5);
        nameText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;
        nameText.GetComponent<TextMeshProUGUI>().enableAutoSizing = false;
        nameText.GetComponent<TextMeshProUGUI>().overflowMode = TextOverflowModes.Overflow;
        
        // Create Level Text
        GameObject levelText = CreateTextElement("LevelText", display, "Lv. 50", 18);
        RectTransform levelRect = levelText.GetComponent<RectTransform>();
        levelRect.anchorMin = new Vector2(0, 0);
        levelRect.anchorMax = new Vector2(1, 0.5f);
        levelRect.pivot = new Vector2(1, 0.5f);
        levelRect.sizeDelta = Vector2.zero;
        levelRect.anchoredPosition = new Vector2(-80, -5);
        levelText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;
        
        SetPrivateField(pokemonDisplay, "spriteImage", image);
        SetPrivateField(pokemonDisplay, "nameText", nameText.GetComponent<TextMeshProUGUI>());
        SetPrivateField(pokemonDisplay, "levelText", levelText.GetComponent<TextMeshProUGUI>());
        
        return display;
    }

    private static GameObject CreateEnemyHPBar(GameObject parent)
    {
        GameObject hpBarObj = CreateUIElement("EnemyHPBar", parent);
        RectTransform hpBarRect = hpBarObj.GetComponent<RectTransform>();
        hpBarRect.anchorMin = new Vector2(0, 1);
        hpBarRect.anchorMax = new Vector2(1, 1);
        hpBarRect.pivot = new Vector2(0.5f, 1);
        hpBarRect.sizeDelta = new Vector2(-20, 30);
        hpBarRect.anchoredPosition = new Vector2(0, -10);
        
        Slider slider = hpBarObj.AddComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.value = 100;
        slider.interactable = false;
        
        // Create Background
        GameObject background = CreateUIElement("Background", hpBarObj);
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        slider.targetGraphic = bgImage;
        
        // Create Fill Area
        GameObject fillArea = CreateUIElement("Fill Area", hpBarObj);
        RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.sizeDelta = Vector2.zero;
        
        // Create Fill
        GameObject fill = CreateUIElement("Fill", fillArea);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.8f, 0.2f, 0.2f, 1f); // Red for enemy
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(1, 1);
        fillRect.sizeDelta = Vector2.zero;
        slider.fillRect = fillRect;
        
        // Create HP Text
        GameObject hpText = CreateTextElement("HPText", hpBarObj, "100/100", 16);
        RectTransform hpTextRect = hpText.GetComponent<RectTransform>();
        hpTextRect.anchorMin = new Vector2(0.5f, 0.5f);
        hpTextRect.anchorMax = new Vector2(0.5f, 0.5f);
        hpTextRect.sizeDelta = new Vector2(100, 30);
        hpTextRect.anchoredPosition = Vector2.zero;
        hpText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        
        HPBar hpBar = hpBarObj.AddComponent<HPBar>();
        SetPrivateField(hpBar, "fillImage", fillImage);
        SetPrivateField(hpBar, "hpText", hpText.GetComponent<TextMeshProUGUI>());
        
        return hpBarObj;
    }

    private static GameObject CreateDialogBox(GameObject parent)
    {
        GameObject dialogBox = CreateUIElement("DialogBox", parent);
        RectTransform dialogRect = dialogBox.GetComponent<RectTransform>();
        dialogRect.anchorMin = new Vector2(0.5f, 0);
        dialogRect.anchorMax = new Vector2(0.5f, 0);
        dialogRect.pivot = new Vector2(0.5f, 0);
        dialogRect.anchoredPosition = new Vector2(0, 20);
        dialogRect.sizeDelta = new Vector2(800, 100);
        
        // Add background image
        Image bgImage = dialogBox.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        
        // Initially disabled
        dialogBox.SetActive(false);
        
        // Create Dialog Text
        GameObject dialogText = CreateTextElement("DialogText", dialogBox, "", 24);
        RectTransform textRect = dialogText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = new Vector2(-40, -40);
        textRect.anchoredPosition = Vector2.zero;
        
        TextMeshProUGUI textComponent = dialogText.GetComponent<TextMeshProUGUI>();
        textComponent.alignment = TextAlignmentOptions.Left;
        textComponent.enableWordWrapping = true;
        
        // Add BattleDialog component
        BattleDialog battleDialog = dialogBox.AddComponent<BattleDialog>();
        SetPrivateField(battleDialog, "dialogText", textComponent);
        SetPrivateField(battleDialog, "dialogBox", dialogBox);
        
        return dialogBox;
    }

    private static GameObject CreateBattleUISetup(GameObject parent, GameObject playerSide, GameObject enemySide, GameObject dialogBox)
    {
        // Create UnityBattleView
        GameObject battleViewObj = new GameObject("UnityBattleView");
        battleViewObj.transform.SetParent(parent.transform);
        UnityBattleView battleView = battleViewObj.AddComponent<UnityBattleView>();
        
        // Find components
        HPBar playerHPBar = playerSide.GetComponentInChildren<HPBar>();
        HPBar enemyHPBar = enemySide.GetComponentInChildren<HPBar>();
        PokemonDisplay playerDisplay = playerSide.GetComponentInChildren<PokemonDisplay>();
        PokemonDisplay enemyDisplay = enemySide.GetComponentInChildren<PokemonDisplay>();
        BattleDialog dialog = dialogBox.GetComponent<BattleDialog>();
        
        // Assign references to UnityBattleView
        SetPrivateField(battleView, "dialog", dialog);
        SetPrivateField(battleView, "playerHPBar", playerHPBar);
        SetPrivateField(battleView, "enemyHPBar", enemyHPBar);
        SetPrivateField(battleView, "playerDisplay", playerDisplay);
        SetPrivateField(battleView, "enemyDisplay", enemyDisplay);
        
        // Create BattleManager
        GameObject battleManagerObj = new GameObject("BattleManager");
        battleManagerObj.transform.SetParent(parent.transform);
        BattleManager battleManager = battleManagerObj.AddComponent<BattleManager>();
        
        // Assign UnityBattleView to BattleManager
        SetPrivateField(battleManager, "battleView", battleView);
        
        return battleManagerObj;
    }

    private static GameObject CreateUIElement(string name, GameObject parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent.transform);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        return obj;
    }

    private static GameObject CreateTextElement(string name, GameObject parent, string text, int fontSize)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform);
        
        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.color = Color.white;
        
        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.localScale = Vector3.one;
        
        return textObj;
    }

    private static void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName, 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(obj, value);
    }
}

