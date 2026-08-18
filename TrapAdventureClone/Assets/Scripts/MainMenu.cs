using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Rehope Gemes (2023) Make Your MAIN MENU Quickly! | Unity UI Tutorial For Beginners. [Online] Avalable at : https://youtu.be/DX7HyN7oJjE?si=v6Hp4J08YA2dhcAB (Accessed: 16 September 2025)
public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        AudioFeedback.PlayButton();
        SceneManager.LoadSceneAsync(1);
    }

    public void QuitGame()
    {
        AudioFeedback.PlayButton();
        Application.Quit();
    }
}
