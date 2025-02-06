using NUnit.Framework;
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

    [Header("Zone Effects 6")]
    public GameObject zone6MovingLines;
    public TrailRenderer zone6CubeTrailEffect;
    public TrailRenderer zone6SphereTrailEffect;
    public OrbitingCircle orbitingCircleScript; // Reference to the OrbitingCircle script
    public ParticleSystem zone6OrbitingCircle;
    public ParticleSystem zone6BackgroundParticles;

    public float moveSpeed = 5f;

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

        // Ensure the Orbiting Circle effect is running in Zone 6
        if (currentZoneIndex == 5 && orbitingCircleScript != null)
        {
            orbitingCircleScript.gameObject.SetActive(true);
        }
        moveZoneEffectsForward(moveSpeed);
    }

    void moveZoneEffectsForward(float moveSpeed)
    {

        // Ensure the orbiting circle continues to play (it may not need positional adjustment)
        if (zone6OrbitingCircle != null && !zone6OrbitingCircle.isPlaying)
        {
            zone6OrbitingCircle.Play(true);
        }

        // Move the background particles forward (if they have a transform component)
        if (zone6BackgroundParticles != null)
        {
            var particleTransform = zone6BackgroundParticles.transform;
            particleTransform.position += Vector3.forward * moveSpeed * Time.deltaTime;
            if (!zone6BackgroundParticles.isPlaying)
            {
                zone6BackgroundParticles.Play();
            }
        }
    }



    //private void TransitionToNextZone()
    //{
    //    if (currentZoneIndex < 6)  // Adjusted since Zone 6 is removed
    //    {
    //        currentZoneIndex++;
    //        ActivateZone(currentZoneIndex);
    //        timer = zoneDuration; // Reset timer
    //    }
    //    else if (currentZoneIndex == 4)
    //    {
    //        currentZoneIndex = 5; // Manually set to Zone 6
    //        ActivateZone(currentZoneIndex); // Activate Zone 6
    //        timer = zoneDuration; // Reset timer
    //    }
    //    else
    //    {
    //        Debug.Log("Already at the last zone, cannot transition further.");
    //    }
    //}

    private void TransitionToNextZone()
    {
        if (currentZoneIndex == 4) // Zone 5 should continue after activating Zone 6
        {
            currentZoneIndex = 5; // Transition to Zone 6 but keep Zone 5 effects active
            ActivateZone(currentZoneIndex); // Activate Zone 6 effects
            timer = zoneDuration; // Reset timer
        }
        else if (currentZoneIndex < 5) // Other transitions
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
            case 5:
                zone6MovingLines.SetActive(true);
                zone6CubeTrailEffect.enabled = true;
                zone6SphereTrailEffect.enabled = true;
                zone6OrbitingCircle.Play(true);
                //Debug.Log("Playing zone5ProximityBubble");
                //zone5ProximityBubble.Play();
                //zone5ProximityPulse.Play(true)
                zone6BackgroundParticles.Play();
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

        zone6MovingLines.SetActive(false);
        zone6CubeTrailEffect.enabled = false;
        zone6SphereTrailEffect.enabled = false;
        zone6BackgroundParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        zone6OrbitingCircle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        //zone6ProximityBubble.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        //zone6ProximityPulse.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void HandleCheatCodes()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetZone(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetZone(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetZone(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetZone(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SetZone(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SetZone(5);
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
