using UnityEngine;
using UnityEngine.SceneManagement;

// Rehope Games (2023) How to Create a PAUSE MENU in Unity ! | 6 June.[Online] Avalable at : https://youtu.be/MNUYe0PWNNs?si=2UOGneRyqiDg9p7K (Accessed: 17 September 2025)
public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    public void Pause()
    {
        AudioFeedback.PlayButton();
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Home()
    {
        AudioFeedback.PlayButton();
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void Resume()
    {
        AudioFeedback.PlayButton();
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void Restart()
    {
        AudioFeedback.PlayButton();
        PlayerHealth.ResetSavedHealth();
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }
}
