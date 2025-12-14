using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("Clips")]
    public AudioClip waterSfx;
    public AudioClip fertilizeSfx;
    public AudioClip matureSfx;
    public AudioClip objectiveCompleteSfx;

    public AudioClip backgroundMusic;
    public float musicVolume = 0.35f;


    void Start()
    {
        PlayMusic(backgroundMusic, musicVolume);
    }


    public void PlaySfx(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip musicClip, float volume = 0.4f)
    {
        if (musicSource == null || musicClip == null) return;
        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.volume = volume;
        musicSource.Play();
    }
}

