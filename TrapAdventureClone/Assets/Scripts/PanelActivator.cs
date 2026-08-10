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
                panelToEnable.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    public void GoToLevel1()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level 1");
    }
}