using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RuntimeGamePresentationStyler : MonoBehaviour
{
    private const string PauseButtonResource = "hud_pause_button";
    private const string PausePanelResource = "pause_menu_panel";
    private const string ResumeButtonResource = "pause_resume_button";
    private const string RestartButtonResource = "restart_button";
    private const string MainMenuButtonResource = "main_menu_button";
    private const string TestFailedPanelResource = "test_failed_panel";
    private const string SpikeResource = "hazard_spike";
    private const string TimerPanelResource = "hud_timer_panel";

    private const string LevelBadgePrefix = "level_";
    private const string LevelBadgeSuffix = "_badge";
    private const string HeartContainerName = "HeartContainer";

    private static readonly Vector2 HudPanelSize =
        new Vector2(400f, 140f);

    private static readonly Vector2 LivesPanelSize =
        new Vector2(360f, 125f);

    private static readonly Vector2 LivesPanelOffset =
        new Vector2(35f, 25f);

    private static readonly Vector2 ControlsPanelSize =
        new Vector2(400f, 140f);

    // Slightly left from -95.
    private static readonly Vector2 ControlsPanelPosition =
        new Vector2(-95f, -8f);

    // Same X as before.
    // Y now matches volume button level.
    private static readonly Vector2 PauseButtonPosition =
        new Vector2(-145f, -46f);

    private static RuntimeGamePresentationStyler instance;
    private static TMP_FontAsset handjetFont;

    private Sprite pauseButtonSprite;
    private Sprite pausePanelSprite;
    private Sprite resumeButtonSprite;
    private Sprite restartButtonSprite;
    private Sprite mainMenuButtonSprite;
    private Sprite testFailedPanelSprite;
    private Sprite spikeSprite;
    private Sprite timerPanelSprite;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateRuntimeStyler()
    {
        if (instance != null)
        {
            return;
        }

        GameObject stylerObject =
            new GameObject(
                "Runtime Game Presentation Styler"
            );

        instance =
            stylerObject.AddComponent<
                RuntimeGamePresentationStyler
            >();

        DontDestroyOnLoad(stylerObject);
    }

    private void Awake()
    {
        if (
            instance != null
            &&
            instance != this
        )
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        LoadSprites();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        RequestStylePass();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        RequestStylePass();
    }

    private void RequestStylePass()
    {
        StopAllCoroutines();

        StartCoroutine(
            StyleAfterSceneSetup()
        );
    }

    private IEnumerator StyleAfterSceneSetup()
    {
        yield return null;

        StyleActiveScene();

        yield return null;

        StyleActiveScene();
    }

    private void StyleActiveScene()
    {
        string sceneName =
            SceneManager.GetActiveScene().name;

        if (!sceneName.StartsWith("Level "))
        {
            return;
        }

        Canvas gameplayCanvas =
            FindGameplayCanvas();

        if (gameplayCanvas != null)
        {
            gameplayCanvas.overrideSorting = true;
            gameplayCanvas.sortingOrder = 200;

           StylePauseButton(gameplayCanvas);
StyleLivesPanel(gameplayCanvas);
StyleTimer(gameplayCanvas);

// Runtime badge disabled because each level
// now uses its own manually placed badge.
// StyleLevelBadge(
//     gameplayCanvas,
//     sceneName
// );

StylePausePanel(gameplayCanvas);
StyleGameOverPanel(gameplayCanvas);
        }

        StylePlayerGlow();

// Spike visuals are now handled by SpikeTrap.cs.
// Do not create additional runtime spike overlays.
// StyleSpikeVisuals();
    }

    private void LoadSprites()
    {
        pauseButtonSprite =
            LoadRuntimeSprite(
                PauseButtonResource
            );

        pausePanelSprite =
            LoadRuntimeSprite(
                PausePanelResource
            );

        resumeButtonSprite =
            LoadRuntimeSprite(
                ResumeButtonResource
            );

        restartButtonSprite =
            LoadRuntimeSprite(
                RestartButtonResource
            );

        mainMenuButtonSprite =
            LoadRuntimeSprite(
                MainMenuButtonResource
            );

        testFailedPanelSprite =
            LoadRuntimeSprite(
                TestFailedPanelResource
            );

        spikeSprite =
            LoadRuntimeSprite(
                SpikeResource
            );

        timerPanelSprite =
            LoadRuntimeSprite(
                TimerPanelResource
            );
    }

    private static Canvas FindGameplayCanvas()
    {
        Canvas fallbackCanvas = null;

        foreach (
            Canvas canvas in
            FindObjectsByType<Canvas>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            )
        )
        {
            if (
                canvas.gameObject.name.Contains(
                    "Persistent Audio"
                )
            )
            {
                continue;
            }

            if (
                canvas.gameObject.name.Contains(
                    "Persistent Screen Borders"
                )
            )
            {
                continue;
            }

            fallbackCanvas ??= canvas;

            if (
                canvas.GetComponentInChildren<PauseMenu>(
                    true
                ) != null
                ||
                canvas.GetComponentInChildren<HealthUI>(
                    true
                ) != null
                ||
                canvas.GetComponentInChildren<GameOverManager>(
                    true
                ) != null
            )
            {
                return canvas;
            }
        }

        return fallbackCanvas;
    }

    private void StylePauseButton(
        Canvas canvas
    )
    {
        Button pauseButton =
            FindButton(
                canvas.gameObject,
                "pause"
            );

        if (pauseButton == null)
        {
            return;
        }

        ApplyButtonSprite(
            pauseButton,
            pauseButtonSprite
        );

        RectTransform pauseRect =
            pauseButton.GetComponent<RectTransform>();

        if (pauseRect == null)
        {
            return;
        }

        pauseRect.anchorMin =
            new Vector2(1f, 1f);

        pauseRect.anchorMax =
            new Vector2(1f, 1f);

        pauseRect.pivot =
            new Vector2(1f, 1f);

        pauseRect.anchoredPosition =
            PauseButtonPosition;

        pauseRect.sizeDelta =
            new Vector2(65f, 65f);

        HideTextChildren(
            pauseButton.gameObject
        );

        Image controlsPanel =
            EnsureChildImage(
                pauseButton.transform.parent,
                "Runtime Controls Panel",
                timerPanelSprite
            );

        controlsPanel.raycastTarget = false;

        RectTransform controlsRect =
            controlsPanel.GetComponent<RectTransform>();

        controlsRect.anchorMin =
            new Vector2(1f, 1f);

        controlsRect.anchorMax =
            new Vector2(1f, 1f);

        controlsRect.pivot =
            new Vector2(1f, 1f);

        controlsRect.anchoredPosition =
            ControlsPanelPosition;

        controlsRect.sizeDelta =
            ControlsPanelSize;

        controlsRect.SetSiblingIndex(
            pauseRect.GetSiblingIndex()
        );

        pauseRect.SetAsLastSibling();
    }

    private void StyleLivesPanel(
        Canvas canvas
    )
    {
        Transform heartContainer =
            FindChildByName(
                canvas.transform,
                HeartContainerName
            );

        if (heartContainer == null)
        {
            return;
        }

        RectTransform heartsRect =
            heartContainer.GetComponent<RectTransform>();

        if (heartsRect == null)
        {
            return;
        }

        AddHudPanelBehind(
            heartsRect,
            heartContainer.parent,
            "Runtime Lives Panel",
            LivesPanelSize,
            LivesPanelOffset
        );
    }

    private void AddHudPanelBehind(
        RectTransform contentRect,
        Transform parent,
        string panelName,
        Vector2 panelSize,
        Vector2 extraOffset
    )
    {
        Image panel =
            EnsureChildImage(
                parent,
                panelName,
                timerPanelSprite
            );

        panel.raycastTarget = false;

        RectTransform panelRect =
            panel.GetComponent<RectTransform>();

        panelRect.anchorMin =
            contentRect.anchorMin;

        panelRect.anchorMax =
            contentRect.anchorMax;

        panelRect.pivot =
            contentRect.pivot;

        panelRect.anchoredPosition =
            contentRect.anchoredPosition
            + extraOffset;

        panelRect.sizeDelta =
            panelSize;

        panelRect.SetSiblingIndex(
            contentRect.GetSiblingIndex()
        );

        contentRect.SetAsLastSibling();
    }

    private void StyleTimer(
        Canvas canvas
    )
    {
        TextMeshProUGUI timerText =
            FindTimerText(
                canvas.gameObject
            );

        if (timerText == null)
        {
            return;
        }

        timerText.color =
            new Color(
                1f,
                0.12f,
                0.08f,
                1f
            );

        timerText.fontSize =
            Mathf.Max(
                timerText.fontSize,
                54f
            );

        TMP_FontAsset font =
            GetHandjetFont();

        if (font != null)
        {
            timerText.font = font;
        }

        RectTransform textRect =
            timerText.GetComponent<RectTransform>();

        if (textRect == null)
        {
            return;
        }

        textRect.anchorMin =
            new Vector2(0.5f, 1f);

        textRect.anchorMax =
            new Vector2(0.5f, 1f);

        textRect.pivot =
            new Vector2(0.5f, 1f);

        textRect.anchoredPosition =
            new Vector2(0f, -28f);

        Image panel =
            EnsureChildImage(
                timerText.transform.parent,
                "Runtime Timer Panel",
                timerPanelSprite
            );

        panel.raycastTarget = false;

        RectTransform panelRect =
            panel.GetComponent<RectTransform>();

        panelRect.anchorMin =
            textRect.anchorMin;

        panelRect.anchorMax =
            textRect.anchorMax;

        panelRect.pivot =
            textRect.pivot;

        panelRect.anchoredPosition =
            textRect.anchoredPosition
            + new Vector2(0f, 32f);

        panelRect.sizeDelta =
            HudPanelSize;

        panelRect.SetSiblingIndex(
            textRect.GetSiblingIndex()
        );

        timerText.transform.SetAsLastSibling();
    }

    private void StyleLevelBadge(
        Canvas canvas,
        string sceneName
    )
    {
        int levelNumber =
            ParseLevelNumber(
                sceneName
            );

        if (levelNumber <= 0)
        {
            return;
        }

        HideOldLevelBadges(
            canvas.gameObject,
            levelNumber
        );

        Sprite badgeSprite =
            LoadRuntimeSprite(
                $"{LevelBadgePrefix}{levelNumber:00}{LevelBadgeSuffix}"
            );

        Image badge =
            EnsureChildImage(
                canvas.transform,
                "Runtime Level Badge",
                badgeSprite
            );

        badge.raycastTarget = false;

        RectTransform badgeRect =
            badge.GetComponent<RectTransform>();

        badgeRect.anchorMin =
            new Vector2(1f, 1f);

        badgeRect.anchorMax =
            new Vector2(1f, 1f);

        badgeRect.pivot =
            new Vector2(1f, 1f);

        badgeRect.anchoredPosition =
            new Vector2(-210f, -18f);

        badgeRect.sizeDelta =
            new Vector2(66f, 108f);

        badge.transform.SetAsLastSibling();
    }

    private void StylePausePanel(
        Canvas canvas
    )
    {
        PauseMenu pauseMenu =
            canvas.GetComponentInChildren<PauseMenu>(
                true
            );

        GameObject panel =
            GetPrivateField<GameObject>(
                pauseMenu,
                "pauseMenu"
            );

        if (panel == null)
        {
            return;
        }

        ApplyPanelSprite(
            panel,
            pausePanelSprite,
            new Vector2(370f, 500f)
        );

        HideTextChildren(panel);

        Button resume =
            FindButton(
                panel,
                "resume"
            );

        Button restart =
            FindButton(
                panel,
                "restart"
            );

        Button mainMenu =
            FindButton(panel, "main")
            ??
            FindButton(panel, "home")
            ??
            FindButton(panel, "menu");

        StylePanelButton(
            resume,
            resumeButtonSprite,
            new Vector2(0f, 80f)
        );

        StylePanelButton(
            restart,
            restartButtonSprite,
            new Vector2(0f, -10f)
        );

        StylePanelButton(
            mainMenu,
            mainMenuButtonSprite,
            new Vector2(0f, -100f)
        );
    }

    private void StyleGameOverPanel(
        Canvas canvas
    )
    {
        GameOverManager manager =
            canvas.GetComponentInChildren<GameOverManager>(
                true
            );

        GameObject panel =
            GetPrivateField<GameObject>(
                manager,
                "gameOverModal"
            );

        if (panel == null)
        {
            return;
        }

        ApplyPanelSprite(
            panel,
            testFailedPanelSprite,
            new Vector2(370f, 500f)
        );

        HideTextChildren(panel);

        Button restart =
            GetPrivateField<Button>(
                manager,
                "restartButton"
            )
            ??
            FindButton(
                panel,
                "restart"
            );

        Button mainMenu =
            GetPrivateField<Button>(
                manager,
                "quitButton"
            )
            ??
            FindButton(panel, "main")
            ??
            FindButton(panel, "home")
            ??
            FindButton(panel, "menu")
            ??
            FindButton(panel, "quit");

        StylePanelButton(
            restart,
            restartButtonSprite,
            new Vector2(0f, -35f)
        );

        StylePanelButton(
            mainMenu,
            mainMenuButtonSprite,
            new Vector2(0f, -125f)
        );
    }

    private void StylePanelButton(
        Button button,
        Sprite sprite,
        Vector2 anchoredPosition
    )
    {
        if (button == null)
        {
            return;
        }

        ApplyButtonSprite(
            button,
            sprite
        );

        HideTextChildren(
            button.gameObject
        );

        RectTransform rect =
            button.GetComponent<RectTransform>();

        if (rect == null)
        {
            return;
        }

        rect.anchorMin =
            new Vector2(0.5f, 0.5f);

        rect.anchorMax =
            new Vector2(0.5f, 0.5f);

        rect.pivot =
            new Vector2(0.5f, 0.5f);

        rect.anchoredPosition =
            anchoredPosition;

        rect.sizeDelta =
            new Vector2(230f, 72f);
    }

    private void StylePlayerGlow()
    {
        foreach (
            PlayerMovement movement in
            FindObjectsByType<PlayerMovement>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            )
        )
        {
            SpriteRenderer sourceRenderer =
                movement.GetComponent<SpriteRenderer>();

            if (
                sourceRenderer == null
                ||
                sourceRenderer.sprite == null
            )
            {
                continue;
            }

            sourceRenderer.color =
                new Color(
                    0.78f,
                    1f,
                    1f,
                    1f
                );

            Transform existingGlow =
                movement.transform.Find(
                    "Player Neon Glow"
                );

            GameObject glowObject =
                existingGlow != null
                    ? existingGlow.gameObject
                    : new GameObject(
                        "Player Neon Glow"
                    );

            glowObject.transform.SetParent(
                movement.transform,
                false
            );

            glowObject.transform.localPosition =
                Vector3.zero;

            glowObject.transform.localRotation =
                Quaternion.identity;

            glowObject.transform.localScale =
                new Vector3(
                    1.45f,
                    1.45f,
                    1f
                );

            SpriteRenderer glowRenderer =
                glowObject.GetComponent<SpriteRenderer>();

            if (glowRenderer == null)
            {
                glowRenderer =
                    glowObject.AddComponent<SpriteRenderer>();
            }

            glowRenderer.sprite =
                sourceRenderer.sprite;

            glowRenderer.color =
                new Color(
                    0.12f,
                    0.95f,
                    1f,
                    0.28f
                );

            glowRenderer.sortingLayerID =
                sourceRenderer.sortingLayerID;

            glowRenderer.sortingOrder =
                sourceRenderer.sortingOrder - 1;
        }
    }

    private void StyleSpikeVisuals()
    {
        if (spikeSprite == null)
        {
            return;
        }

        foreach (
            SpriteRenderer sourceRenderer in
            FindObjectsByType<SpriteRenderer>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            )
        )
        {
            if (!ShouldStyleAsSpike(sourceRenderer))
            {
                continue;
            }

            Transform existingOverlay =
                sourceRenderer.transform.Find(
                    "Hazard Spike Visual Overlay"
                );

            GameObject overlayObject =
                existingOverlay != null
                    ? existingOverlay.gameObject
                    : new GameObject(
                        "Hazard Spike Visual Overlay"
                    );

            overlayObject.transform.SetParent(
                sourceRenderer.transform,
                false
            );

            overlayObject.transform.localRotation =
                Quaternion.identity;

            overlayObject.transform.localPosition =
                IsWallSpike(sourceRenderer)
                    ? Vector3.zero
                    : new Vector3(
                        0f,
                        -0.18f,
                        0f
                    );

            SpriteRenderer overlayRenderer =
                overlayObject.GetComponent<SpriteRenderer>();

            if (overlayRenderer == null)
            {
                overlayRenderer =
                    overlayObject.AddComponent<SpriteRenderer>();
            }

            overlayRenderer.sprite =
                spikeSprite;

            overlayRenderer.color =
                Color.white;

            overlayRenderer.sortingLayerID =
                sourceRenderer.sortingLayerID;

            overlayRenderer.sortingOrder =
                sourceRenderer.sortingOrder + 1;

            Vector3 sourceSize =
                sourceRenderer.bounds.size;

            Vector3 parentScale =
                sourceRenderer.transform.lossyScale;

            Vector2 spriteSize =
                spikeSprite.bounds.size;

            overlayObject.transform.localScale =
                new Vector3(
                    SafeScale(
                        sourceSize.x * 1.35f,
                        spriteSize.x,
                        parentScale.x
                    ),
                    SafeScale(
                        sourceSize.y * 1.55f,
                        spriteSize.y,
                        parentScale.y
                    ),
                    1f
                );

            Color color =
                sourceRenderer.color;

            sourceRenderer.color =
                new Color(
                    color.r,
                    color.g,
                    color.b,
                    0f
                );
        }
    }

    private static bool ShouldStyleAsSpike(
        SpriteRenderer renderer
    )
    {
        if (
            renderer == null
            ||
            renderer.sprite == null
            ||
            renderer.gameObject.name.Contains(
                "Hazard Spike Visual Overlay"
            )
        )
        {
            return false;
        }

        if (
            renderer.GetComponentInParent<PlayerMovement>()
            != null
        )
        {
            return false;
        }

        string fullName =
            GetHierarchyName(
                renderer.transform
            ).ToLowerInvariant();

        if (
            !fullName.Contains("spike")
            &&
            !fullName.Contains("trap")
        )
        {
            return false;
        }

        if (
            fullName.Contains("block")
            ||
            fullName.Contains("level block")
        )
        {
            return false;
        }

        return
            renderer.bounds.size.x > 0.1f
            &&
            renderer.bounds.size.y > 0.1f;
    }

    private static bool IsWallSpike(
        SpriteRenderer renderer
    )
    {
        string fullName =
            GetHierarchyName(
                renderer.transform
            ).ToLowerInvariant();

        if (fullName.Contains("wall"))
        {
            return true;
        }

        float z =
            Mathf.Abs(
                Mathf.DeltaAngle(
                    renderer.transform.eulerAngles.z,
                    0f
                )
            );

        return
            z > 45f
            &&
            z < 135f;
    }

    private static void ApplyPanelSprite(
        GameObject panel,
        Sprite sprite,
        Vector2 size
    )
    {
        if (sprite == null)
        {
            return;
        }

        Image image =
            panel.GetComponent<Image>();

        if (image == null)
        {
            image =
                panel.AddComponent<Image>();
        }

        image.sprite = sprite;
        image.color = Color.white;
        image.preserveAspect = true;

        RectTransform rect =
            panel.GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.anchorMin =
                new Vector2(0.5f, 0.5f);

            rect.anchorMax =
                new Vector2(0.5f, 0.5f);

            rect.pivot =
                new Vector2(0.5f, 0.5f);

            rect.anchoredPosition =
                Vector2.zero;

            rect.sizeDelta = size;
        }
    }

    private static void ApplyButtonSprite(
        Button button,
        Sprite sprite
    )
    {
        if (
            button == null
            ||
            sprite == null
        )
        {
            return;
        }

        Image image =
            button.GetComponent<Image>();

        if (image == null)
        {
            image =
                button.gameObject.AddComponent<Image>();
        }

        image.sprite = sprite;
        image.color = Color.white;
        image.preserveAspect = true;

        button.targetGraphic = image;
    }

    private static Image EnsureChildImage(
        Transform parent,
        string childName,
        Sprite sprite
    )
    {
        Transform existing =
            parent.Find(childName);

        GameObject imageObject =
            existing != null
                ? existing.gameObject
                : new GameObject(childName);

        imageObject.transform.SetParent(
            parent,
            false
        );

        Image image =
            imageObject.GetComponent<Image>();

        if (image == null)
        {
            image =
                imageObject.AddComponent<Image>();
        }

        image.sprite = sprite;
        image.color = Color.white;
        image.preserveAspect = true;

        return image;
    }

    private static Transform FindChildByName(
        Transform root,
        string name
    )
    {
        if (root.name == name)
        {
            return root;
        }

        foreach (Transform child in root)
        {
            Transform result =
                FindChildByName(
                    child,
                    name
                );

            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    private static Button FindButton(
        GameObject root,
        params string[] patterns
    )
    {
        if (root == null)
        {
            return null;
        }

        foreach (
            Button button in
            root.GetComponentsInChildren<Button>(
                true
            )
        )
        {
            string searchable =
                button.gameObject.name
                    .ToLowerInvariant();

            foreach (
                TextMeshProUGUI text in
                button.GetComponentsInChildren<TextMeshProUGUI>(
                    true
                )
            )
            {
                searchable +=
                    " "
                    +
                    text.text.ToLowerInvariant();
            }

            bool matches = true;

            foreach (string pattern in patterns)
            {
                if (
                    !searchable.Contains(
                        pattern.ToLowerInvariant()
                    )
                )
                {
                    matches = false;
                    break;
                }
            }

            if (matches)
            {
                return button;
            }
        }

        return null;
    }

    private static TextMeshProUGUI FindTimerText(
        GameObject root
    )
    {
        foreach (
            TextMeshProUGUI text in
            root.GetComponentsInChildren<TextMeshProUGUI>(
                true
            )
        )
        {
            string content =
                text.text.Trim();

            string objectName =
                text.gameObject.name
                    .ToLowerInvariant();

            if (
                objectName.Contains("timer")
                ||
                objectName.Contains("time")
                ||
                content.Contains(":")
            )
            {
                return text;
            }
        }

        return null;
    }

    private static void HideOldLevelBadges(
        GameObject root,
        int levelNumber
    )
    {
        string levelText =
            levelNumber.ToString();

        foreach (
            TextMeshProUGUI text in
            root.GetComponentsInChildren<TextMeshProUGUI>(
                true
            )
        )
        {
            if (text.text.Trim() == levelText)
            {
                text.color =
                    new Color(
                        text.color.r,
                        text.color.g,
                        text.color.b,
                        0f
                    );

                Image parentImage =
                    text.GetComponentInParent<Image>();

                if (parentImage != null)
                {
                    parentImage.color =
                        new Color(
                            parentImage.color.r,
                            parentImage.color.g,
                            parentImage.color.b,
                            0f
                        );
                }
            }
        }
    }

    private static void HideTextChildren(
        GameObject root
    )
    {
        foreach (
            TextMeshProUGUI text in
            root.GetComponentsInChildren<TextMeshProUGUI>(
                true
            )
        )
        {
            text.color =
                new Color(
                    text.color.r,
                    text.color.g,
                    text.color.b,
                    0f
                );
        }
    }

    private static int ParseLevelNumber(
        string sceneName
    )
    {
        string[] pieces =
            sceneName.Split(' ');

        if (pieces.Length == 0)
        {
            return 0;
        }

        return int.TryParse(
            pieces[pieces.Length - 1],
            out int levelNumber
        )
            ? levelNumber
            : 0;
    }

    private static TMP_FontAsset GetHandjetFont()
    {
        if (handjetFont != null)
        {
            return handjetFont;
        }

        try
        {
            Font font =
                Font.CreateDynamicFontFromOSFont(
                    "Handjet",
                    64
                );

            if (font != null)
            {
                handjetFont =
                    TMP_FontAsset.CreateFontAsset(
                        font
                    );
            }
        }
        catch
        {
            handjetFont = null;
        }

        return handjetFont;
    }

    private static T GetPrivateField<T>(
        object source,
        string fieldName
    )
        where T : class
    {
        if (source == null)
        {
            return null;
        }

        FieldInfo field =
            source.GetType().GetField(
                fieldName,
                BindingFlags.Instance
                |
                BindingFlags.NonPublic
            );

        return
            field?.GetValue(source) as T;
    }

    private static Sprite LoadRuntimeSprite(
        string resourceName
    )
    {
        Sprite importedSprite =
            Resources.Load<Sprite>(
                resourceName
            );

        if (importedSprite != null)
        {
            return importedSprite;
        }

        Texture2D texture =
            Resources.Load<Texture2D>(
                resourceName
            );

        if (texture == null)
        {
            return null;
        }

        texture.wrapMode =
            TextureWrapMode.Clamp;

        texture.filterMode =
            FilterMode.Bilinear;

        return Sprite.Create(
            texture,
            new Rect(
                0f,
                0f,
                texture.width,
                texture.height
            ),
            new Vector2(
                0.5f,
                0.5f
            ),
            100f
        );
    }

    private static string GetHierarchyName(
        Transform transform
    )
    {
        string name =
            transform.name;

        Transform current =
            transform.parent;

        while (current != null)
        {
            name =
                current.name
                +
                "/"
                +
                name;

            current =
                current.parent;
        }

        return name;
    }

    private static float SafeScale(
        float targetSize,
        float spriteSize,
        float parentScale
    )
    {
        if (
            Mathf.Approximately(
                spriteSize,
                0f
            )
            ||
            Mathf.Approximately(
                parentScale,
                0f
            )
        )
        {
            return 1f;
        }

        return
            targetSize
            /
            (
                spriteSize
                *
                Mathf.Abs(parentScale)
            );
    }
}