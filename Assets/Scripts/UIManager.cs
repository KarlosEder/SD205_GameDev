using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // In-game menus
    public GameObject pauseUI;

    // Lobby menus
    public CanvasGroup abilitiesGroup;
    public CanvasGroup loadoutGroup;

    public LayoutElement abilitiesLayout;
    public LayoutElement loadoutLayout;

    public GameObject abilitiesSelected;
    public GameObject abilitiesUnselected;
    public GameObject loadoutSelected;
    public GameObject loadoutUnselected;

    // Quit
    public void OnPressExit()
    {
        Application.Quit();
        Debug.Log("Quitting game");
    }

    // Start
    public void OnPressStart()
    {   
        SceneManager.LoadScene("Lobby");
    }

    // Quit to menu
    public void OnPressMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Ready up
    public void OnPressReady()
    {
        SceneManager.LoadScene("Level_01");
    }

     // Pause
    public void OnPressPause()
    {
        pauseUI.SetActive(true);
    }

    // Resume
    public void OnPressResume()
    {
        pauseUI.SetActive(false);
    }

    // Lobby 
    void Start()
    {
        ShowAbilities();
    }
    public void ShowAbilities()
    {
        SetMenu(abilitiesGroup, loadoutGroup);
        SetNavVisuals(true);

        Debug.Log("AbilityGroup");
    }

    public void ShowLoadout()
    {
        SetMenu(loadoutGroup, abilitiesGroup);
        SetNavVisuals(false);

        Debug.Log("LoadoutGroup");
    }

    void SetMenu(CanvasGroup on, CanvasGroup off)
    {
        on.alpha = 1;
        on.interactable = true;
        on.blocksRaycasts = true;

        off.alpha = 0;
        off.interactable = false;
        off.blocksRaycasts = false;
    }

    void SetNavVisuals(bool abilitiesActive)
    {
        abilitiesSelected.SetActive(abilitiesActive);
        abilitiesUnselected.SetActive(!abilitiesActive);

        loadoutSelected.SetActive(!abilitiesActive);
        loadoutUnselected.SetActive(abilitiesActive);
    }
}
