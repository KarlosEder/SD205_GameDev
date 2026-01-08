using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject pauseUI;

    public void OnPressExit()
    {
        Application.Quit();
        Debug.Log("Quitting game");
    }

    public void OnPressStart()
    {   
        SceneManager.LoadScene("Lobby");
    }

    public void OnPressMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnPressPause()
    {
        pauseUI.SetActive(true);
    }

    public void OnPressResume()
    {
        pauseUI.SetActive(false);
    }
}
