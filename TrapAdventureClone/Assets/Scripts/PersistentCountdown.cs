using UnityEngine;
using UnityEngine.Events;
using System;
using TMPro;

public class PersistentCountdown : MonoBehaviour
{
    [Header("Required Components")]
    [SerializeField] private TextMeshProUGUI displayTextMesh;

    [Header("Timer Settings")]
    [SerializeField] private float maxValue = 60f;
    [SerializeField] private bool startOnAwake = true;
    [SerializeField] private string timeFormat = @"mm\:ss";

    [Header("Events")]
    public UnityEvent onTimerComplete;
    public UnityEvent<float> onTimerUpdate;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true; // Enabled by default for debugging

    private float currentTime;
    private bool isRunning = false;
    private static PersistentCountdown instance;
    private static bool isInitialized = false;
    private static float savedTime = 0f; // Store time between scenes
    private static bool timerWasRunning = false;

    public float CurrentTime => currentTime;
    public float MaxValue => maxValue;
    public bool IsRunning => isRunning;
    public float Progress => maxValue > 0 ? currentTime / maxValue : 0f;

    public event Action OnTimerComplete;
    public event Action<float> OnTimerUpdate;

    private void Awake()
    {
        Debug.Log("=== PersistentCountdown Awake ===");
        Debug.Log($"Instance before: {instance}");
        Debug.Log($"IsInitialized: {isInitialized}");
        Debug.Log($"Saved Time: {savedTime}");

        // Check if we already have an instance
        if (instance != null && instance != this)
        {
            Debug.Log("Destroying duplicate timer instance");
            Destroy(gameObject);
            return;
        }

        // If we're the first instance or the persistent one
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log($"Setting DontDestroyOnLoad for {gameObject.name}");
        }

        // Get TextMeshPro component
        if (displayTextMesh == null)
        {
            displayTextMesh = GetComponent<TextMeshProUGUI>();
            if (displayTextMesh == null)
            {
                Debug.LogError("TextMeshProUGUI component required!");
                enabled = false;
                return;
            }
        }

        // Only initialize if not already initialized
        if (!isInitialized)
        {
            Debug.Log("First time initialization");
            InitializeTimer();
            isInitialized = true;
        }
        else
        {
            // Restore saved state
            Debug.Log($"Restoring timer state - Time: {savedTime}, Running: {timerWasRunning}");
            currentTime = savedTime;
            isRunning = timerWasRunning;
            UpdateTextDisplay();

            if (showDebugLogs)
                Debug.Log($"Timer restored with {currentTime} seconds remaining");
        }
    }

    private void InitializeTimer()
    {
        currentTime = maxValue;
        isRunning = startOnAwake;
        savedTime = currentTime;
        timerWasRunning = isRunning;
        UpdateTextDisplay();

        if (showDebugLogs)
            Debug.Log($"Timer initialized with max value: {maxValue} seconds");
    }

    private void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;
        savedTime = currentTime; // Save current time

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;
            timerWasRunning = false;
            savedTime = 0f;
            OnTimerCompleteInternal();
        }

        UpdateTextDisplay();
        OnTimerUpdateInternal(currentTime);
    }

    private void UpdateTextDisplay()
    {
        if (displayTextMesh != null)
        {
            displayTextMesh.text = GetFormattedTime(timeFormat);
        }
    }

    private void OnTimerCompleteInternal()
    {
        if (showDebugLogs)
            Debug.Log("Timer completed!");

        onTimerComplete?.Invoke();
        OnTimerComplete?.Invoke();
    }

    private void OnTimerUpdateInternal(float time)
    {
        onTimerUpdate?.Invoke(time);
        OnTimerUpdate?.Invoke(time);
    }

    public void StartTimer()
    {
        isRunning = true;
        timerWasRunning = true;
        if (showDebugLogs)
            Debug.Log("Timer started");
    }

    public void PauseTimer()
    {
        isRunning = false;
        timerWasRunning = false;
        if (showDebugLogs)
            Debug.Log("Timer paused");
    }

    public void ResumeTimer()
    {
        isRunning = true;
        timerWasRunning = true;
        if (showDebugLogs)
            Debug.Log("Timer resumed");
    }

    public void ResetTimer()
    {
        currentTime = maxValue;
        savedTime = currentTime;
        UpdateTextDisplay();
        if (showDebugLogs)
            Debug.Log($"Timer reset to {maxValue} seconds");
    }

    public void StopAndReset()
    {
        isRunning = false;
        timerWasRunning = false;
        currentTime = maxValue;
        savedTime = currentTime;
        UpdateTextDisplay();
        if (showDebugLogs)
            Debug.Log("Timer stopped and reset");
    }

    public void SetMaxValue(float newMaxValue)
    {
        if (newMaxValue < 0)
        {
            Debug.LogWarning("Max value cannot be negative. Setting to 0.");
            newMaxValue = 0f;
        }

        maxValue = newMaxValue;
        currentTime = maxValue;
        savedTime = currentTime;
        UpdateTextDisplay();

        if (showDebugLogs)
            Debug.Log($"Max value set to: {maxValue} seconds");
    }

    public void AddTime(float seconds)
    {
        currentTime += seconds;
        savedTime = currentTime;
        if (currentTime > maxValue)
            currentTime = maxValue;

        UpdateTextDisplay();

        if (showDebugLogs)
            Debug.Log($"Added {seconds} seconds. Current time: {currentTime}");
    }

    public string GetFormattedTime(string format = null)
    {
        if (string.IsNullOrEmpty(format))
            format = timeFormat;

        TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);
        return timeSpan.ToString(format);
    }

    public void SetTimeFormat(string newFormat)
    {
        timeFormat = newFormat;
        UpdateTextDisplay();
    }

    public void SetDisplayTextMesh(TextMeshProUGUI newTextMesh)
    {
        displayTextMesh = newTextMesh;
        if (displayTextMesh != null)
            UpdateTextDisplay();
    }

    private void OnDestroy()
    {
        // Save state before destruction
        if (instance == this)
        {
            Debug.Log($"Saving timer state before destruction - Time: {currentTime}, Running: {isRunning}");
            savedTime = currentTime;
            timerWasRunning = isRunning;
        }
    }

    private void OnValidate()
    {
        if (maxValue < 0)
            maxValue = 0f;

        if (Application.isPlaying && currentTime > maxValue)
            currentTime = maxValue;
    }
}