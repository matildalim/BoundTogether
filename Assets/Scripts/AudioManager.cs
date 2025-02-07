using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip bgmTrack; // Single BGM track for the entire game
    private AudioSource audioSource; // The AudioSource to play the BGM

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
    }

    void Start()
    {
        PlayBGM(); // Play the BGM when the game starts
    }

    public void PlayBGM()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = bgmTrack;
            audioSource.loop = true; // Loop the BGM for the whole game
            audioSource.Play();
            Debug.Log("Playing background music.");
        }
    }

    public void StopBGM()
    {
        audioSource.Stop();
        Debug.Log("BGM Stopped.");
    }
}
