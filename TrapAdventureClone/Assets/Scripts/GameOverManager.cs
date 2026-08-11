using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("Game Over Modal")]
    [SerializeField] private GameObject gameOverModal;
    [SerializeField] private TextMeshProUGUI gameOverTitleText;
    [SerializeField] private TextMeshProUGUI gameOverMessageText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    [Header("Modal Settings")]
    [SerializeField] private string gameOverTitle = "GAME OVER";
    [SerializeField] private string gameOverMessage = "Time's Up!";
    [SerializeField] private bool pauseGameOnGameOver = true;
    [SerializeField] private float delayBeforeShowing = 0.5f;

    [Header("Scene Management")]
    [SerializeField] private string restartSceneName = "Level 1"; // Name of your level 1 scene
    [SerializeField] private string mainMenuSceneName = "Main Menu"; // Name of your main menu scene
    [SerializeField] private int restartSceneIndex = 1; // Alternative: use scene index instead of name

    [Header("Animation")]
    [SerializeField] private bool animateModal = true;
    [SerializeField] private float animationDuration = 0.5f;

    private PersistentCountdown timer;
    private bool isGameOver = false;
    private Vector3 originalModalScale;

    private void Start()
    {
        // Find the timer
        timer = FindObjectOfType<PersistentCountdown>();

        if (timer == null)
        {
            Debug.LogError("PersistentCountdown not found in scene!");
            return;
        }

        // Subscribe to timer completion event
        timer.OnTimerComplete += OnTimerComplete;

        // Hide modal at start
        if (gameOverModal != null)
        {
            gameOverModal.SetActive(false);
            originalModalScale = gameOverModal.transform.localScale;
        }

        // Setup buttons
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    private void OnTimerComplete()
    {
        if (isGameOver) return;

        isGameOver = true;

        // Start coroutine to show modal with delay
        StartCoroutine(ShowGameOverWithDelay());
    }

    private IEnumerator ShowGameOverWithDelay()
    {
        // Wait for delay
        yield return new WaitForSeconds(delayBeforeShowing);

        // Show the modal
        ShowGameOverModal();

        // Pause game if enabled
        if (pauseGameOnGameOver)
        {
            Time.timeScale = 0f;
        }
    }

    private void ShowGameOverModal()
    {
        if (gameOverModal == null)
        {
            Debug.LogError("GameOverModal is not assigned!");
            return;
        }

        // Set text
        if (gameOverTitleText != null)
            gameOverTitleText.text = gameOverTitle;

        if (gameOverMessageText != null)
            gameOverMessageText.text = gameOverMessage;

        // Show modal
        gameOverModal.SetActive(true);

        // Animate modal
        if (animateModal)
        {
            StartCoroutine(AnimateModalIn());
        }

        Debug.Log("Game Over Modal Shown!");
    }

    private IEnumerator AnimateModalIn()
    {
        if (gameOverModal == null) yield break;

        // Start from scale 0
        gameOverModal.transform.localScale = Vector3.zero;

        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.unscaledDeltaTime; // Use unscaled time even when paused
            float progress = elapsedTime / animationDuration;

            // Ease out bounce effect
            float scale = 1f - Mathf.Pow(1f - progress, 3f);
            scale = Mathf.Clamp01(scale);

            gameOverModal.transform.localScale = originalModalScale * scale;
            yield return null;
        }

        gameOverModal.transform.localScale = originalModalScale;
    }

    // Button Methods
    public void RestartGame()
    {
        Debug.Log("Restarting Game - Loading Level 1...");

        // Reset time scale
        Time.timeScale = 1f;

        // Reset timer
        if (timer != null)
        {
            timer.ResetTimer();
            timer.StartTimer();
        }

        // Hide modal
        if (gameOverModal != null)
            gameOverModal.SetActive(false);

        isGameOver = false;

       
        // Load Level 1 - Option 2: By scene index
        
            SceneManager.LoadScene(1);
        
    }

    public void QuitGame()
    {
        Debug.Log("Quitting to Main Menu...");

        // Reset time scale
        Time.timeScale = 1f;

        // Load Main Menu
        
        SceneManager.LoadScene(0);
       
    }

    // Alternative quit method - actually quit the application
    public void QuitApplication()
    {
        Debug.Log("Quitting Application...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Public method to manually trigger game over (for testing)
    public void TriggerGameOver()
    {
        OnTimerComplete();
    }

    // Public method to check if game is over
    public bool IsGameOver()
    {
        return isGameOver;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (timer != null)
        {
            timer.OnTimerComplete -= OnTimerComplete;
        }

        // Clean up button listeners
        if (restartButton != null)
            restartButton.onClick.RemoveListener(RestartGame);

        if (quitButton != null)
            quitButton.onClick.RemoveListener(QuitGame);
    }
}