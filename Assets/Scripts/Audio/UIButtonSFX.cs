using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSFX : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    public SoundEffectsManager sfxManager;

    float lastTime;
    const float cooldown = 0.05f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Play();
    }

    public void OnSelect(BaseEventData eventData)
    {
        Play();
    }

    void Play()
    {
        if (sfxManager == null) return;
        if (Time.unscaledTime - lastTime < cooldown) return;

        lastTime = Time.unscaledTime;
        sfxManager.Highlighted();
    }
}