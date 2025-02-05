using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance;

    public GameObject[] zones; // Array of zone GameObjects
    public float zoneDuration = 15f; // Time before switching zones
    private float timer;
    private int currentZoneIndex = 0; // Tracks which zone we're in

    [Header("Zone Effects")]
    public GameObject movingLines;
    public TrailRenderer cubeTrailEffect;
    public TrailRenderer sphereTrailEffect;
    public ParticleSystem coloredBackgroundParticles;
    public ParticleSystem proximityBubble;
    public ParticleSystem proximityPulse;

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
        currentZoneIndex++;
        ActivateZone(currentZoneIndex);
        timer = zoneDuration; // Reset timer
    }

    private void ActivateZone(int index)
    {
        // Deactivate all zones first
        for (int i = 0; i < zones.Length; i++)
        {
            zones[i].SetActive(i == index); // Only activate the current zone
        }

        ApplyZoneEffects(index);

        Debug.Log("Transitioned to Zone " + (index + 1));
    }

    private void ApplyZoneEffects(int zoneIndex)
    {
        // Reset all effects (to prevent unexpected behavior when switching zones)
        movingLines.SetActive(false);
        cubeTrailEffect.enabled = false;
        sphereTrailEffect.enabled = false;
        coloredBackgroundParticles.Stop();  
        proximityBubble.Stop();             
        proximityPulse.Stop();

        // Apply effects incrementally
        if (zoneIndex >= 1) movingLines.SetActive(true);  // Zone 1 effects: Moving lines
        if (zoneIndex >= 2)
        {
            cubeTrailEffect.enabled = true;
            sphereTrailEffect.enabled = true;  // Zone 2 effects: Cube & sphere trail
        }
        if (zoneIndex >= 3)
        {
            proximityBubble.gameObject.SetActive(true); // Ensure the parent is active
            proximityBubble.Play();  // Play proximity bubble
        }
        if (zoneIndex >= 4)
        {
            // Assuming proximityBubble is the parent of proximityPulse
            proximityBubble.gameObject.SetActive(true); // Ensure the parent is active
            proximityBubble.Play();  // Play proximity bubble

            proximityPulse.gameObject.SetActive(true); // Ensure proximity pulse's parent is active
            proximityPulse.Play();  // Play proximity pulse


            coloredBackgroundParticles.gameObject.SetActive(true); // Ensure proximity pulse's parent is active
            coloredBackgroundParticles.Play();  // Play proximity pulse


        }
    }


    private void HandleCheatCodes()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetZone(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetZone(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetZone(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetZone(3);
    }

    private void SetZone(int index)
    {
        if (index >= 0 && index < zones.Length)
        {
            currentZoneIndex = index;
            ActivateZone(index);
            timer = zoneDuration; // Reset timer on manual transition
        }
    }
}
