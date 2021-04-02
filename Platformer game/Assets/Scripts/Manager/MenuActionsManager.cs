using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuActionsManager : MonoBehaviour
{
    public GameObject menuCanvas;
    public GameObject controlsCanvas;

    public string gameplayScene;
    public string mainMenuScene;

    public void QuitButton()
    {
        Application.Quit();
    }

    public void PlayButton()
    {
        if (!menuCanvas || !controlsCanvas) return;

        menuCanvas.SetActive(false);
        controlsCanvas.SetActive(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameplayScene);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}