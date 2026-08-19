using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelBlockVisualStyler
{
    private const string OverlayChildName = "Level Block Visual Overlay";
    private const float WhiteBlockThreshold = 0.92f;

    // Only smaller blocks get enlarged visually.
    private const float SmallBlockMaximumDimension = 2.4f;

    /*
     * Visual size only.
     * Real collider / transform remains untouched.
     */
    private const float SmallBlockVisualMultiplier = 1.28f;

    private static readonly Color DarkChamberBackground =
        new Color(0.13f, 0.145f, 0.155f, 1f);

    private static Sprite longHorizontalBlock;
    private static Sprite mediumHorizontalBlock;
    private static Sprite shortVerticalBlock;
    private static Sprite tallVerticalBlock;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.AfterSceneLoad
    )]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        StyleActiveScene();
    }

    private static void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        StyleActiveScene();
    }

    private static void StyleActiveScene()
    {
        EnsureCameraCanRender();
        DestroyLegacyBlockOverlays();

        Scene scene =
            SceneManager.GetActiveScene();

        if (!scene.name.StartsWith("Level "))
        {
            return;
        }

        LoadBlockSprites();
        DarkenCameraBackground();
        HideBoundaryBorderSprites();

        foreach (
            SpriteRenderer renderer in
            Object.FindObjectsByType<SpriteRenderer>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            )
        )
        {
            if (!ShouldStyleAsLevelBlock(renderer))
            {
                continue;
            }

            AddOrUpdateOverlay(renderer);

            Color originalColor =
                renderer.color;

            /*
             * Hide only the old plain block artwork.
             *
             * Collider / movement / transform are
             * completely untouched.
             */
            renderer.color =
                new Color(
                    originalColor.r,
                    originalColor.g,
                    originalColor.b,
                    0f
                );
        }
    }

    private static void EnsureCameraCanRender()
    {
        Camera[] cameras =
            Object.FindObjectsByType<Camera>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        if (cameras.Length == 0)
        {
            return;
        }

        Camera firstCamera =
            cameras[0];

        foreach (Camera camera in cameras)
        {
            camera.gameObject.SetActive(true);
            camera.enabled = true;
            camera.targetDisplay = 0;
        }

        AudioListener[] listeners =
            Object.FindObjectsByType<AudioListener>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        if (listeners.Length == 0)
        {
            firstCamera.gameObject
                .AddComponent<AudioListener>();

            return;
        }

        AudioListener listenerToKeep = null;

        foreach (
            AudioListener listener in listeners
        )
        {
            if (
                listener.GetComponent<Camera>()
                != null
            )
            {
                listenerToKeep = listener;
                break;
            }
        }

        listenerToKeep ??= listeners[0];

        foreach (
            AudioListener listener in listeners
        )
        {
            listener.enabled =
                listener == listenerToKeep;
        }
    }

    private static void DestroyLegacyBlockOverlays()
    {
        foreach (
            SpriteRenderer renderer in
            Object.FindObjectsByType<SpriteRenderer>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            )
        )
        {
            if (renderer == null)
            {
                continue;
            }

            if (
                renderer.gameObject.name.Contains(
                    "Chamber Block Overlay"
                )
            )
            {
                Object.Destroy(
                    renderer.gameObject
                );
            }
        }
    }

    private static void DarkenCameraBackground()
    {
        foreach (
            Camera camera in
            Object.FindObjectsByType<Camera>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            )
        )
        {
            camera.backgroundColor =
                DarkChamberBackground;
        }
    }

    private static void HideBoundaryBorderSprites()
    {
        foreach (
            SpriteRenderer renderer in
            Object.FindObjectsByType<SpriteRenderer>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            )
        )
        {
            if (
                renderer == null
                ||
                renderer.sprite == null
            )
            {
                continue;
            }

            Vector3 size =
                renderer.bounds.size;

            Color color =
                renderer.color;

            bool isTallBorder =
                size.y >= 5f
                &&
                size.x <= 0.8f;

            bool isMutedPinkBorder =
                color.a > 0.1f
                &&
                color.r > 0.35f
                &&
                color.g < color.r
                &&
                color.b < color.r;

            if (
                isTallBorder
                &&
                isMutedPinkBorder
            )
            {
                renderer.color =
                    new Color(
                        color.r,
                        color.g,
                        color.b,
                        0f
                    );
            }
        }
    }

    private static bool ShouldStyleAsLevelBlock(
        SpriteRenderer renderer
    )
    {
        if (
            renderer == null
            ||
            renderer.sprite == null
            ||
            renderer.gameObject.name == OverlayChildName
        )
        {
            return false;
        }

        if (
            renderer.GetComponentInParent<PlayerMovement>()
            != null
            ||
            renderer.GetComponentInParent<PlayerHealth>()
            != null
        )
        {
            return false;
        }

        string objectName =
            renderer.gameObject.name
                .ToLowerInvariant();

        /*
         * Block styler must never touch traps.
         */
        if (
            objectName.Contains("spike")
            ||
            objectName.Contains("trap")
            ||
            objectName.Contains("overlay")
        )
        {
            return false;
        }

        Color color =
            renderer.color;

        bool isWhiteBlock =
            color.a > 0.1f
            &&
            color.r >= WhiteBlockThreshold
            &&
            color.g >= WhiteBlockThreshold
            &&
            color.b >= WhiteBlockThreshold;

        return
            isWhiteBlock
            &&
            renderer.bounds.size.x > 0.2f
            &&
            renderer.bounds.size.y > 0.2f;
    }

    private static void AddOrUpdateOverlay(
        SpriteRenderer sourceRenderer
    )
    {
        Bounds targetBounds =
            sourceRenderer.bounds;

        Sprite overlaySprite =
            PickBlockSprite(
                targetBounds.size
            );

        if (overlaySprite == null)
        {
            return;
        }

        Transform existingOverlay =
            sourceRenderer.transform.Find(
                OverlayChildName
            );

        GameObject overlayObject =
            existingOverlay != null
                ? existingOverlay.gameObject
                : new GameObject(
                    OverlayChildName
                );

        overlayObject.transform.SetParent(
            sourceRenderer.transform,
            false
        );

        overlayObject.transform.localRotation =
            Quaternion.identity;

        SpriteRenderer overlayRenderer =
            overlayObject
                .GetComponent<SpriteRenderer>();

        if (overlayRenderer == null)
        {
            overlayRenderer =
                overlayObject
                    .AddComponent<SpriteRenderer>();
        }

        overlayRenderer.sprite =
            overlaySprite;

        overlayRenderer.color =
            Color.white;

        overlayRenderer.sortingLayerID =
            sourceRenderer.sortingLayerID;

        overlayRenderer.sortingOrder =
            sourceRenderer.sortingOrder + 1;

        Vector3 parentScale =
            sourceRenderer.transform.lossyScale;

        Vector2 spriteSize =
            overlaySprite.bounds.size;

        bool smallBlock =
            IsSmallBlock(
                targetBounds.size
            );

        float visualMultiplier =
            smallBlock
                ? SmallBlockVisualMultiplier
                : 1f;

        float targetWidth =
            targetBounds.size.x
            *
            visualMultiplier;

        float targetHeight =
            targetBounds.size.y
            *
            visualMultiplier;

        overlayObject.transform.localScale =
            new Vector3(
                SafeScale(
                    targetWidth,
                    spriteSize.x,
                    parentScale.x
                ),
                SafeScale(
                    targetHeight,
                    spriteSize.y,
                    parentScale.y
                ),
                1f
            );

        /*
         * IMPORTANT FIX:
         *
         * If the decorative block becomes taller,
         * shift it DOWN by half of the added height.
         *
         * This keeps the visual TOP exactly aligned
         * with the real collider's top surface.
         *
         * Player no longer appears to sink.
         */
        if (smallBlock)
        {
            float addedWorldHeight =
                targetHeight
                -
                targetBounds.size.y;

            float worldOffsetY =
                -(addedWorldHeight * 0.5f);

            float localOffsetY =
                Mathf.Approximately(
                    parentScale.y,
                    0f
                )
                    ? 0f
                    : worldOffsetY
                      /
                      parentScale.y;

            overlayObject.transform.localPosition =
                new Vector3(
                    0f,
                    localOffsetY,
                    0f
                );
        }
        else
        {
            overlayObject.transform.localPosition =
                Vector3.zero;
        }
    }

    private static bool IsSmallBlock(
        Vector3 size
    )
    {
        float largestDimension =
            Mathf.Max(
                size.x,
                size.y
            );

        return
            largestDimension
            <=
            SmallBlockMaximumDimension;
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

    private static Sprite PickBlockSprite(
        Vector3 worldSize
    )
    {
        float aspectRatio =
            worldSize.x
            /
            Mathf.Max(
                worldSize.y,
                0.01f
            );

        if (aspectRatio >= 3f)
        {
            return longHorizontalBlock;
        }

        if (aspectRatio >= 1.2f)
        {
            return mediumHorizontalBlock;
        }

        if (
            worldSize.y >= 2.2f
            ||
            aspectRatio <= 0.75f
        )
        {
            return tallVerticalBlock;
        }

        return shortVerticalBlock;
    }

    private static void LoadBlockSprites()
    {
        longHorizontalBlock ??=
            CreateSpriteFromResource(
                "level_block_long_horizontal"
            );

        mediumHorizontalBlock ??=
            CreateSpriteFromResource(
                "level_block_medium_horizontal"
            );

        shortVerticalBlock ??=
            CreateSpriteFromResource(
                "level_block_short_vertical"
            );

        tallVerticalBlock ??=
            CreateSpriteFromResource(
                "level_block_tall_vertical"
            );
    }

    private static Sprite CreateSpriteFromResource(
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
            Debug.LogWarning(
                $"LevelBlockVisualStyler could not load UI block texture: {resourceName}"
            );

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
}