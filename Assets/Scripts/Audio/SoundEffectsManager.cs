using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    public static SoundEffectsManager Instance;

    // Audio source
    public AudioSource audioSrc;

    // Audio clips
    public AudioClip[] pressedClips;
    public AudioClip[] highlightClips;

    void Awake()
    {
        // Ensure singleton AudioManager exists
        if (SoundEffectsManager.Instance == null)
            SoundEffectsManager.Instance = this;

        // Get the SFX AudioSource from the persistent AudioManager
        if (audioSrc == null)
        {
            GameObject audioManager = GameObject.Find("AudioManager");
            if (audioManager != null)
            {
                audioSrc = audioManager.transform.Find("SFX").GetComponent<AudioSource>();
            }
            else
            {
                Debug.LogWarning("AudioManager not found in scene!");
            }
        }
    }

    void Start()
    {
        if (audioSrc == null)
            audioSrc = GetComponent<AudioSource>();
    }

    // When button pressed
    public void Pressed()
    {
        if (audioSrc == null || pressedClips.Length == 0) return;
        int index = Random.Range(0, pressedClips.Length);
        audioSrc.PlayOneShot(pressedClips[index]);
    }

    // When button highlighted
    public void Highlighted()
    {
        if (audioSrc == null || highlightClips.Length == 0) return;
        int index = Random.Range(0, highlightClips.Length);
        audioSrc.PlayOneShot(highlightClips[index]);
    }
}
