using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void OnPressExit()
    {
        Application.Quit();
        Debug.Log("Quitting game");
    }

    public void OnPressStart()
    {   
        SceneManager.LoadScene("Lobby");
    }
}
