using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager Instance;

    public Light directionalLight;
    public AudioSource audioSource;

    private void Awake()
    {
        Instance = this;
    }

    public void SetEnvironment(EnvironmentZone zone)
    {
        StartCoroutine(SmoothAmbientTransition(RenderSettings.ambientLight, zone.ambientColor));

        RenderSettings.fogDensity = zone.fogDensity;

        if (audioSource.clip != zone.ambientSound)
        {
            audioSource.clip = zone.ambientSound;
            audioSource.Play();
        }
    }

    private IEnumerator SmoothAmbientTransition(Color fromColor, Color toColor)
    {
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            RenderSettings.ambientLight = Color.Lerp(fromColor, toColor, elapsed / duration);
            yield return null;
        }
    }
}
