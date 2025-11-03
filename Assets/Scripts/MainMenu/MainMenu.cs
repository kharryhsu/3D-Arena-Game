using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public SceneFader sceneFader;

    public void PlayGame()
    {
        Debug.Log("Enter Level 1");

        sceneFader.FadeToScene("Level_1");
    }

    public void Back()
    {
        Debug.Log("Back to MainMenu");

        sceneFader.FadeToScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
