using UnityEngine;

public class BackgroundMusicPlayer : MonoBehaviour
{
    private const string BackgroundMusicResourcePath = "background_music_on_patrol";
    private const float DefaultVolume = 0.22f;

    private static BackgroundMusicPlayer instance;

    private AudioSource musicSource;

    public static bool IsMuted { get; private set; }
    public static float Volume { get; private set; } = DefaultVolume;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void StartBackgroundMusic()
    {
        if (instance != null)
        {
            return;
        }

        AudioClip backgroundTrack = Resources.Load<AudioClip>(BackgroundMusicResourcePath);
        if (backgroundTrack == null)
        {
            Debug.LogWarning($"Background music clip not found at Resources/{BackgroundMusicResourcePath}.");
            return;
        }

        GameObject musicObject = new GameObject("Background Music Player");
        instance = musicObject.AddComponent<BackgroundMusicPlayer>();
        instance.Initialise(backgroundTrack);
        DontDestroyOnLoad(musicObject);
    }

    public static void SetMuted(bool muted)
    {
        IsMuted = muted;
        if (instance != null && instance.musicSource != null)
        {
            instance.musicSource.mute = IsMuted;
        }
    }

    public static void SetVolume(float volume)
    {
        Volume = Mathf.Clamp01(volume);
        if (instance != null && instance.musicSource != null)
        {
            instance.musicSource.volume = Volume;
        }
    }

    private void Initialise(AudioClip backgroundTrack)
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = backgroundTrack;
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f;
        musicSource.volume = Volume;
        musicSource.mute = IsMuted;
        musicSource.Play();
    }
}
