using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;   // Drag your Main Menu GameObject here
    public GameObject settings;   // Drag your Settings GameObject here

    public void PlayGame()
    {
        Debug.Log("ENTERED FUNC");
        // Load your actual gameplay scene here
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        Debug.Log("scene loaded");
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        settings.SetActive(false);
    }

    public void ShowSettings()
    {
        mainMenu.SetActive(false);
        settings.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
