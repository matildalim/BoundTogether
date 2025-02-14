using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip bgmTrack;
    private AudioSource audioSource;
    private bool isPlaying = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayBGM()
    {
        if (!isPlaying)
        {
            audioSource.clip = bgmTrack;
            audioSource.loop = true;
            audioSource.Play();
            isPlaying = true;
        }
    }

    public void StopBGM()
    {
        if (isPlaying)
        {
            audioSource.Stop();
            isPlaying = false;
        }
    }
}
