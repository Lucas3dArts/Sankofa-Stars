using UnityEngine;

public class RocketAudio : MonoBehaviour
{
    [Header("Áudio do foguete")]
    [SerializeField] private AudioSource rocketAudioSource;
    [SerializeField] private AudioClip rocketEngineSound;

    private void Start()
    {
        if (rocketAudioSource == null)
        {
            rocketAudioSource = GetComponent<AudioSource>();
        }

        if (rocketAudioSource == null || rocketEngineSound == null)
        {
            return;
        }

        rocketAudioSource.clip = rocketEngineSound;
        rocketAudioSource.loop = true;
        rocketAudioSource.spatialBlend = 1f;
        rocketAudioSource.Play();
    }

    private void OnDestroy()
    {
        if (rocketAudioSource != null)
        {
            rocketAudioSource.Stop();
        }
    }
}