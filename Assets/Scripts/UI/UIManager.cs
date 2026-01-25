using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // In-game menus
    public GameObject pauseUI;
    public GameObject hudUI;

    // Lobby menus
    public CanvasGroup abilitiesGroup;
    public CanvasGroup loadoutGroup;

    public LayoutElement abilitiesLayout;
    public LayoutElement loadoutLayout;

    public GameObject abilitiesSelected;
    public GameObject abilitiesUnselected;
    public GameObject loadoutSelected;
    public GameObject loadoutUnselected;

    private bool isPaused = false;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // Menu cursor default
        if (!SceneManager.GetActiveScene().name.StartsWith("Level"))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // Show Abilities menu by default
        ShowAbilities();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPaused(!isPaused);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset pause state
        SetPaused(false);

        // Hide cursor by default for gameplay scenes
        if (scene.name.StartsWith("Level"))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else // menu or lobby
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // Make Abilities default when returning to lobby
            ShowAbilities();
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void SetPaused(bool paused)
    {
        isPaused = paused;

        if (pauseUI != null) pauseUI.SetActive(paused);
        if (hudUI != null) hudUI.SetActive(!paused);

        Time.timeScale = paused ? 0f : 1f;
        Cursor.visible = paused;
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public bool GetIsPaused()
    {
        return isPaused;
    }

    // Go to Main Menu
    public void OnPressMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Go to Lobby
    public void OnPressLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    // Quit
    public void OnPressQuit()
    {
        Application.Quit();
        Debug.Log("Quitting game"); 
    }

    // Go to Level_City 
    public void OnPressReady()
    {
        if (Application.CanStreamedLevelBeLoaded("Level_City"))
            SceneManager.LoadScene("Level_City");
    }

    public void ShowAbilities()
    {
        // Make Abilities visible, Loadout hidden
        SetMenu(abilitiesGroup, loadoutGroup);

        // Update button selection states
        if (abilitiesSelected != null) abilitiesSelected.SetActive(true);
        if (abilitiesUnselected != null) abilitiesUnselected.SetActive(false);
        if (loadoutSelected != null) loadoutSelected.SetActive(false);
        if (loadoutUnselected != null) loadoutUnselected.SetActive(true);
    }

    public void ShowLoadout()
    {
        // Make Loadout visible, Abilities hidden
        SetMenu(loadoutGroup, abilitiesGroup);

        // Update button selection states
        if (loadoutSelected != null) loadoutSelected.SetActive(true);
        if (loadoutUnselected != null) loadoutUnselected.SetActive(false);
        if (abilitiesSelected != null) abilitiesSelected.SetActive(false);
        if (abilitiesUnselected != null) abilitiesUnselected.SetActive(true);
    }

    void SetMenu(CanvasGroup on, CanvasGroup off)
    {
        if (on != null)
        {
            on.alpha = 1;
            on.interactable = true;
            on.blocksRaycasts = true;
        }

        if (off != null)
        {
            off.alpha = 0;
            off.interactable = false;
            off.blocksRaycasts = false;
        }
    }
}
