using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    // Audio clips
    public AudioSource audioSrc;
    public AudioClip[] pressedClips;
    public AudioClip[] highlightClips;

    // When button pressed
    public void Pressed()
    {
        int index = Random.Range(0, pressedClips.Length);
        audioSrc.PlayOneShot(pressedClips[index]);
    }

    // When button highlighted
    public void Highlighted()
    {
        if (highlightClips.Length == 0 || audioSrc == null) return;
        audioSrc.PlayOneShot(highlightClips[Random.Range(0, highlightClips.Length)]);
    }
}
