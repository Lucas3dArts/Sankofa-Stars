using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource effectsSource;
    [SerializeField] private AudioSource powerSource;

    [Header("Música")]
    [SerializeField] private AudioClip backgroundMusic;

    [Header("Sons")]
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip powerUpSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }

    // ============================
    // MÚSICA
    // ============================

    public void PlayBackgroundMusic()
    {
        if (musicSource == null)
        {
            Debug.LogWarning(
                "Music Source não foi configurado no AudioManager."
            );
            return;
        }

        if (backgroundMusic == null)
        {
            Debug.LogWarning(
                "Background Music não foi configurada no AudioManager."
            );
            return;
        }

        if (musicSource.isPlaying &&
            musicSource.clip == backgroundMusic)
        {
            return;
        }

        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PauseMusic()
    {
        if (musicSource != null)
        {
            musicSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (musicSource != null)
        {
            musicSource.UnPause();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = Mathf.Clamp01(volume);
        }
    }

    // ============================
    // EFEITOS SONOROS
    // ============================

    public void PlaySound(AudioClip audioClip)
    {
        if (effectsSource == null)
        {
            Debug.LogWarning(
                "Effects Source não foi configurado no AudioManager."
            );
            return;
        }

        if (audioClip == null)
        {
            Debug.LogWarning(
                "O AudioClip enviado ao AudioManager está vazio."
            );
            return;
        }

        effectsSource.PlayOneShot(audioClip);
    }

    public void PlayShootSound()
    {
        PlaySound(shootSound);
    }

    public void PlayExplosionSound()
    {
        PlaySound(explosionSound);
    }

    public void PlayPowerUpSound()
    {
        PlaySound(powerUpSound);
    }



    public void SetEffectsVolume(float volume)
    {
        if (effectsSource != null)
        {
            effectsSource.volume = Mathf.Clamp01(volume);
        }
    }

    public void StopEffects()
    {
        if (effectsSource != null)
        {
            effectsSource.Stop();
        }
    }
}