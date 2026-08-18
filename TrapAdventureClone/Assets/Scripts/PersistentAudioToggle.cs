using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PersistentAudioToggle : MonoBehaviour
{
    private const string VolumeButtonResourcePath = "audio_volume_button";
    private const string MuteButtonResourcePath = "audio_mute_button";
    private const float ButtonSize = 76f;
    private const float TopRightMargin = 26f;
    private const float DefaultFeedbackVolume = 0.55f;

    private static PersistentAudioToggle instance;
    private static EventSystem persistentEventSystem;
    private static bool isMuted;

    private Image buttonImage;
    private Sprite volumeSprite;
    private Sprite muteSprite;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreatePersistentToggle()
    {
        if (instance != null)
        {
            return;
        }

        GameObject toggleObject = new GameObject("Persistent Audio Toggle");
        instance = toggleObject.AddComponent<PersistentAudioToggle>();
        DontDestroyOnLoad(toggleObject);
        instance.BuildToggle();
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void BuildToggle()
    {
        volumeSprite = Resources.Load<Sprite>(VolumeButtonResourcePath);
        muteSprite = Resources.Load<Sprite>(MuteButtonResourcePath);

        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 5000;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        gameObject.AddComponent<GraphicRaycaster>();

        GameObject buttonObject = new GameObject("Audio Mute Button");
        buttonObject.transform.SetParent(transform, false);

        RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1f, 1f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.pivot = new Vector2(1f, 1f);
        rectTransform.anchoredPosition = new Vector2(-TopRightMargin, -TopRightMargin);
        rectTransform.sizeDelta = new Vector2(ButtonSize, ButtonSize);

        buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.preserveAspect = true;

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        button.onClick.AddListener(ToggleMuted);

        ApplyMuteState();
        EnsurePersistentEventSystem();
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsurePersistentEventSystem();
    }

    private void ToggleMuted()
    {
        isMuted = !isMuted;
        ApplyMuteState();

        if (!isMuted)
        {
            AudioFeedback.PlayButton();
        }
    }

    private void ApplyMuteState()
    {
        AudioListener.volume = isMuted ? 0f : 1f;
        BackgroundMusicPlayer.SetMuted(isMuted);
        AudioFeedback.MasterVolume = isMuted ? 0f : DefaultFeedbackVolume;

        if (buttonImage != null)
        {
            buttonImage.sprite = isMuted ? muteSprite : volumeSprite;
        }
    }

    private static void EnsurePersistentEventSystem()
    {
        EventSystem[] eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);

        if (persistentEventSystem == null)
        {
            persistentEventSystem = eventSystems.Length > 0 ? eventSystems[0] : CreateEventSystem();
            DontDestroyOnLoad(persistentEventSystem.gameObject);
        }

        foreach (EventSystem eventSystem in eventSystems)
        {
            if (eventSystem != persistentEventSystem)
            {
                Destroy(eventSystem.gameObject);
            }
        }
    }

    private static EventSystem CreateEventSystem()
    {
        GameObject eventSystemObject = new GameObject("Persistent EventSystem");
        EventSystem eventSystem = eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<InputSystemUIInputModule>();
        return eventSystem;
    }
}
