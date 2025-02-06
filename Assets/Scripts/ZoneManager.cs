using System;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance;

    public GameObject[] zones; // Array of zone GameObjects
    public float zoneDuration = 15f; // Time before switching zones
    private float timer;
    private int currentZoneIndex = 0; // Tracks which zone we're in

    [Header("Zone Effects 2")]
    public GameObject zone2MovingLines;

    [Header("Zone Effects 3")]
    public GameObject zone3MovingLines;
    public TrailRenderer zone3CubeTrailEffect;
    public TrailRenderer zone3SphereTrailEffect;

    [Header("Zone Effects 4")]
    public GameObject zone4MovingLines;
    public TrailRenderer zone4CubeTrailEffect;
    public TrailRenderer zone4SphereTrailEffect;
    public ParticleSystem zone4ProximityBubble;
    public ParticleSystem zone4ProximityPulse;

    [Header("Zone Effects 5")]
    public GameObject zone5MovingLines;
    public TrailRenderer zone5CubeTrailEffect;
    public TrailRenderer zone5SphereTrailEffect;
    public ParticleSystem zone5ProximityBubble;
    public ParticleSystem zone5ProximityPulse;
    public ParticleSystem zone5ColoredBackgroundParticles;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        ActivateZone(0); // Start with Zone 1
        timer = zoneDuration;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        // Transition to the next zone when the timer runs out
        if (timer <= 0 && currentZoneIndex < zones.Length - 1)
        {
            TransitionToNextZone();
        }

        HandleCheatCodes(); // Allows manual zone transitions
    }

    private void TransitionToNextZone()
    {
        if (currentZoneIndex < 5)  // Adjusted since Zone 6 is removed
        {
            currentZoneIndex++;
            ActivateZone(currentZoneIndex);
            timer = zoneDuration; // Reset timer
        }
        else
        {
            Debug.Log("Already at the last zone, cannot transition further.");
        }
    }

    private void ActivateZone(int index)    
    {
        for (int i = 0; i < zones.Length; i++)
        {
            zones[i].SetActive(i == index);
        }

        ApplyZoneEffects(index);
        Debug.Log("Transitioned to Zone " + (index + 1));
    }

    private void ApplyZoneEffects(int zoneIndex)
    {
        Debug.Log("ApplyZoneEffects called for Zone: " + zoneIndex);
        ResetEffects();

        switch (zoneIndex)
        {
            case 0:
                break;
            case 1:
                zone2MovingLines.SetActive(true);
                break;
            case 2:
                zone3MovingLines.SetActive(true);
                zone3CubeTrailEffect.enabled = true;
                zone3SphereTrailEffect.enabled = true;
                break;
            case 3:
                zone4MovingLines.SetActive(true);
                zone4CubeTrailEffect.enabled = true;
                zone4SphereTrailEffect.enabled = true;
                zone4ProximityBubble.Play();
                //zone4ProximityPulse.Play();
                break;

            case 4:
                zone5MovingLines.SetActive(true);
                zone5CubeTrailEffect.enabled = true;
                zone5SphereTrailEffect.enabled = true;
                Debug.Log("Playing zone5ProximityBubble");
                zone5ProximityBubble.Play();
                //zone5ProximityPulse.Play(true)
                zone5ColoredBackgroundParticles.Play();
                break;
        }
    }

    private void ResetEffects()
    {
        zone2MovingLines.SetActive(false);

        zone3MovingLines.SetActive(false);
        zone3CubeTrailEffect.enabled = false;
        zone3SphereTrailEffect.enabled = false;

        zone4MovingLines.SetActive(false);
        zone4CubeTrailEffect.enabled = false;
        zone4SphereTrailEffect.enabled = false;
        zone4ProximityBubble.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        zone4ProximityPulse.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        zone5MovingLines.SetActive(false);
        zone5CubeTrailEffect.enabled = false;
        zone5SphereTrailEffect.enabled = false;
        zone5ColoredBackgroundParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        zone5ProximityBubble.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        zone5ProximityPulse.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void HandleCheatCodes()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetZone(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetZone(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetZone(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetZone(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SetZone(4);
    }

    private void SetZone(int index)
    {
        if (index >= 0 && index < zones.Length)
        {
            currentZoneIndex = index;
            ActivateZone(index);
            timer = zoneDuration;
        }
    }
}
