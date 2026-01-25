using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip levelMusic;

    private void Awake()
    {
        // Check if any other MusicManager exists
        MusicManager[] managers = Object.FindObjectsByType<MusicManager>(FindObjectsSortMode.None);
        if (managers.Length > 1)
        {
            Destroy(gameObject); // destroy this duplicate
            return;
        }

        // Set singleton
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (!musicSource.isPlaying)
            PlayMenuMusic();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.StartsWith("Level"))
        {
            PlayLevelMusic();
        }
        else
        {
            PlayMenuMusic();
        }
    }

    // Music control
    public void PlayMenuMusic()
    {
        if (musicSource.clip == menuMusic) return;

        musicSource.clip = menuMusic;
        musicSource.loop = true;
        musicSource.volume = 0.25f;
        musicSource.Play();
    }

    public void PlayLevelMusic()
    {
        if (musicSource.clip == levelMusic) return;

        musicSource.clip = levelMusic;
        musicSource.loop = true;
        musicSource.volume = 0.25f;
        musicSource.Play();
    }

    public void FadeOut(float duration = 1f)
    {
        StartCoroutine(FadeOutRoutine(duration));
    }

    IEnumerator FadeOutRoutine(float duration)
    {
        float startVolume = musicSource.volume;

        while (musicSource.volume > 0f)
        {
            musicSource.volume -= startVolume * Time.unscaledDeltaTime / duration;
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume;
    }

    // Transition between music
    public void TransitionToLevelMusic(float fadeDuration)
    {
        StopAllCoroutines();
        StartCoroutine(TransitionRoutine(levelMusic, fadeDuration));
    }

    public void TransitionToMenuMusic(float fadeDuration = 1f)
    {
        StopAllCoroutines();
        StartCoroutine(TransitionRoutine(menuMusic, fadeDuration));
    }

    // Stop music from restarting
    IEnumerator TransitionRoutine(AudioClip nextClip, float duration)
    {
        if (musicSource.clip == nextClip)
            yield break;

        float startVolume = musicSource.volume;

        while (musicSource.volume > 0f)
        {
            musicSource.volume -= startVolume * Time.unscaledDeltaTime / duration;
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = nextClip;
        musicSource.loop = true;
        musicSource.volume = startVolume;
        musicSource.Play();
    }
}