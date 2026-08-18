using System.Collections.Generic;
using UnityEngine;

public static class AudioFeedback
{
    private static AudioSource source;
    private static readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
    private static readonly Dictionary<string, AudioClip> resourceClips = new Dictionary<string, AudioClip>();

    public static float MasterVolume { get; set; } = 0.55f;

    public static void PlayJump() => PlayResourceClip("player_jump", 0.7f, "jump", 520f, 720f, 0.1f);
    public static void PlayTrapReveal() => PlayResourceClip("spike_trap_popup", 0.65f, "trapReveal", 180f, 90f, 0.16f);
    public static void PlayTrapHit() => PlayTone("trapHit", 130f, 80f, 0.13f, 0.5f);
    public static void PlayDamage() => PlayResourceClip("player_life_lost", 0.65f, "damage", 120f, 45f, 0.22f);
    public static void PlayButton() => PlayTone("button", 420f, 520f, 0.08f, 0.35f);
    public static void PlayLevelComplete() => PlayResourceClip("level_transition_success", 0.65f, "levelComplete", 640f, 880f, 0.26f);
    public static void PlayOverallWin() => PlayResourceClip("overall_win_fretless", 0.75f, "overallWin", 760f, 1040f, 0.45f);
    public static void PlayGameOver() => PlayResourceClip("game_over_lose", 0.7f, "gameOver", 240f, 70f, 0.45f);
    public static void PlayTimerWarning() => PlayTone("timerWarning", 880f, 660f, 0.08f, 0.35f);

    private static void PlayTone(string key, float startFrequency, float endFrequency, float duration, float volume)
    {
        EnsureSource();

        if (!clips.TryGetValue(key, out AudioClip clip))
        {
            clip = CreateToneClip(key, startFrequency, endFrequency, duration);
            clips[key] = clip;
        }

        source.PlayOneShot(clip, volume * MasterVolume);
    }

    private static void PlayResourceClip(
        string resourcePath,
        float volume,
        string fallbackToneKey,
        float fallbackStartFrequency,
        float fallbackEndFrequency,
        float fallbackDuration)
    {
        EnsureSource();

        if (!resourceClips.TryGetValue(resourcePath, out AudioClip clip))
        {
            clip = Resources.Load<AudioClip>(resourcePath);
            if (clip != null)
            {
                resourceClips[resourcePath] = clip;
            }
        }

        if (clip == null)
        {
            PlayTone(fallbackToneKey, fallbackStartFrequency, fallbackEndFrequency, fallbackDuration, volume);
            return;
        }

        source.PlayOneShot(clip, volume * MasterVolume);
    }

    private static void EnsureSource()
    {
        if (source != null)
        {
            return;
        }

        GameObject audioObject = new GameObject("Audio Feedback");
        Object.DontDestroyOnLoad(audioObject);
        source = audioObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
    }

    private static AudioClip CreateToneClip(string name, float startFrequency, float endFrequency, float duration)
    {
        const int sampleRate = 44100;
        int sampleCount = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float progress = i / (float)sampleCount;
            float frequency = Mathf.Lerp(startFrequency, endFrequency, progress);
            float envelope = Mathf.Sin(progress * Mathf.PI);
            data[i] = Mathf.Sin(2f * Mathf.PI * frequency * i / sampleRate) * envelope;
        }

        AudioClip clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
