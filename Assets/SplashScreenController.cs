using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashScreenController : MonoBehaviour
{
    // Public variables to be set in the Inspector
    public string nextSceneName = "BoundTogether"; // Name of the next scene to load
    public float displayTime = 3.0f; // Time in seconds to show the splash screen

    void Start()
    {
        // Start the coroutine to show the splash screen
        StartCoroutine(ShowSplashScreen());
    }

    IEnumerator ShowSplashScreen()
    {
        // Wait for the specified display time
        yield return new WaitForSeconds(displayTime);

        // Load the next scene
        SceneManager.LoadScene(nextSceneName);
    }
}