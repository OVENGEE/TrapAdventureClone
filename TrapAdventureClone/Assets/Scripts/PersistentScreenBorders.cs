using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PersistentScreenBorders : MonoBehaviour
{
    private const string BorderSpriteResourcePath = "level_block_tall_vertical";
    private const float BorderWidth = 120f;
    private const float BorderHorizontalOffset = 20f;
    private const float BorderExtraHeight = 100f;

    private static PersistentScreenBorders instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreatePersistentBorders()
    {
        if (instance != null)
        {
            return;
        }

        GameObject bordersObject = new GameObject("Persistent Screen Borders");
        instance = bordersObject.AddComponent<PersistentScreenBorders>();
        DontDestroyOnLoad(bordersObject);
        instance.BuildBorders();
    }

    private void BuildBorders()
    {
        Sprite borderSprite = Resources.Load<Sprite>(BorderSpriteResourcePath);

        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        CreateBorder(borderSprite, isLeft: true);
        CreateBorder(borderSprite, isLeft: false);
    }

    private void CreateBorder(Sprite borderSprite, bool isLeft)
    {
        GameObject borderObject = new GameObject(isLeft ? "Left Border" : "Right Border");
        borderObject.transform.SetParent(transform, false);

        RectTransform rectTransform = borderObject.AddComponent<RectTransform>();
        float anchorX = isLeft ? 0f : 1f;
        rectTransform.anchorMin = new Vector2(anchorX, 0f);
        rectTransform.anchorMax = new Vector2(anchorX, 1f);
        rectTransform.pivot = new Vector2(anchorX, 0.5f);

        float horizontalPush = isLeft ? -BorderHorizontalOffset : BorderHorizontalOffset;
        rectTransform.anchoredPosition = new Vector2(horizontalPush, 0f);
        rectTransform.sizeDelta = new Vector2(BorderWidth, BorderExtraHeight);

        GameObject imageObject = new GameObject("Border Image");
        imageObject.transform.SetParent(borderObject.transform, false);

        RectTransform imageRect = imageObject.AddComponent<RectTransform>();
        imageRect.anchorMin = Vector2.zero;
        imageRect.anchorMax = Vector2.one;
        imageRect.pivot = new Vector2(0.5f, 0.5f);
        imageRect.offsetMin = Vector2.zero;
        imageRect.offsetMax = Vector2.zero;

        if (!isLeft)
        {
            imageRect.localScale = new Vector3(-1f, 1f, 1f);
        }

        Image borderImage = imageObject.AddComponent<Image>();
        borderImage.sprite = borderSprite;
        borderImage.type = Image.Type.Simple;
        borderImage.raycastTarget = false;
    }
}