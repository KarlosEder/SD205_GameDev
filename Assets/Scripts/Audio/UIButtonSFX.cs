using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIButtonSFX : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    public SoundEffectsManager sfxManager;

    float lastTime;
    const float cooldown = 0.05f;

    void Awake()
    {
        // Initial assignment
        AssignManager();

        // Also re-assign after scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignManager();
    }

    void AssignManager()
    {
        sfxManager = Object.FindFirstObjectByType<SoundEffectsManager>();
        if (sfxManager == null)
            Debug.LogWarning($"No SoundEffectsManager found in scene for {gameObject.name}!");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayHighlight();
    }

    public void OnSelect(BaseEventData eventData)
    {
        PlayHighlight();
    }

    public void OnClick() 
    {
        if (sfxManager != null)
            sfxManager.Pressed();
    }

    void PlayHighlight()
    {
        if (sfxManager == null) return;
        if (Time.unscaledTime - lastTime < cooldown) return;

        lastTime = Time.unscaledTime;
        sfxManager.Highlighted();
    }
}