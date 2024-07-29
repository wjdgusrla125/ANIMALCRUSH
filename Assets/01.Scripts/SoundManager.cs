using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource bgmSource;
    public AudioSource[] sfxSources;
    public AudioClip[] bgmClips;
    public AudioClip[] sfxClips;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize AudioSources
        bgmSource = gameObject.AddComponent<AudioSource>();
        sfxSources = new AudioSource[10]; // Adjust the size as needed
        for (int i = 0; i < sfxSources.Length; i++)
        {
            sfxSources[i] = gameObject.AddComponent<AudioSource>();
        }
        
        PlayBGM(0);
    }

    public void PlayBGM(int clipIndex)
    {
        if (clipIndex < 0 || clipIndex >= bgmClips.Length)
        {
            Debug.LogError("BGM clip index out of range");
            return;
        }
        bgmSource.clip = bgmClips[clipIndex];
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PauseBGM()
    {
        bgmSource.Pause();
    }

    public void PlaySFX(int clipIndex)
    {
        if (clipIndex < 0 || clipIndex >= sfxClips.Length)
        {
            Debug.LogError("SFX clip index out of range");
            return;
        }

        foreach (AudioSource source in sfxSources)
        {
            if (!source.isPlaying)
            {
                source.clip = sfxClips[clipIndex];
                source.Play();
                return;
            }
        }

        Debug.LogWarning("All SFX sources are currently playing");
    }

    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        foreach (AudioSource source in sfxSources)
        {
            source.volume = volume;
        }
    }
}
