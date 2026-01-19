using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // In-game menus
    public GameObject pauseUI;
    public GameObject hudUI;
    private bool isPaused;

    public static bool IsPaused { get; private set; }

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

    void Update()
    {
        // Toggle pause with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPaused(!isPaused);
        }
    }

    void SetPaused(bool paused)
    {
        if (isPaused == paused)
            return;

        isPaused = paused;
        IsPaused = paused;

        // Pause menu
        if (pauseUI != null)
            pauseUI.SetActive(paused);

        // HUD
        if (hudUI != null)
            hudUI.SetActive(!paused); 

        // Time and cursor
        Time.timeScale = paused ? 0f : 1f;
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
    }

    // Pause
    public void OnPressPause() => SetPaused(true);

    // Resume
    public void OnPressResume() => SetPaused(false);

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
