using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelActivator : MonoBehaviour
{
    [SerializeField] private GameObject panelToEnable;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (panelToEnable != null)
            {
                AudioFeedback.PlayOverallWin();
                panelToEnable.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }

    public void GoToMainMenu()
    {
        AudioFeedback.PlayButton();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    public void GoToLevel1()
    {
        AudioFeedback.PlayButton();
        PlayerHealth.ResetSavedHealth();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level 1");
    }
}
