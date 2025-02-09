using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance;

    public GameObject[] zones;
    public float zoneDuration = 15f;
    private float timer;
    private int currentZoneIndex = 0;
    private bool isZone6 = false;

    [System.Serializable]
    public class ZoneEffects
    {
        public GameObject movingLines;
        public TrailRenderer cubeTrailEffect;
        public TrailRenderer sphereTrailEffect;
        public ParticleSystem proximityBubble;
        public ParticleSystem energyPulse;
        public ParticleSystem blackBackgroundParticles;
        public ParticleSystem coloredBackgroundParticles;
    }

    [Header("Zone Effects")]
    public ZoneEffects[] zoneEffects;

    [Header("Proximity Settings")]
    public float proximityRange = 5f;
    public float minDistance = 1f;
    public Transform player1;
    public Transform player2;
    private Transform otherPlayer => (player1 != null && player2 != null) ? player2 : null;

    [Header("Background Particle Settings")]
    public float currentForwardSpeed = 1f;
    public float minBackgroundEmission = 10f;
    public float maxBackgroundEmission = 50f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        ActivateZone(0);
        timer = zoneDuration;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0 && currentZoneIndex < zones.Length - 1)
        {
            TransitionToNextZone();
        }

        HandleProximityBubble();
        UpdateBackgroundParticles();
        HandleCheatCodes(); // Allows manual zone transitions
    }

    private void TransitionToNextZone()
    {
        if (currentZoneIndex < zones.Length - 1)
        {
            currentZoneIndex++;
            ActivateZone(currentZoneIndex);
        }

        timer = zoneDuration;
    }


    private void ActivateZone(int index)
    {
        isZone6 = (index == 5);

        for (int i = 0; i < zones.Length; i++)
        {
            zones[i].SetActive(i == index);
        }

        ApplyZoneEffects(index);
    }

    private void ApplyZoneEffects(int zoneIndex)
    {
        ResetEffects();

        if (zoneIndex >= 0 && zoneIndex < zoneEffects.Length)
        {
            var effects = zoneEffects[zoneIndex];

            if (effects.movingLines != null) effects.movingLines.SetActive(true);
            if (effects.cubeTrailEffect != null) effects.cubeTrailEffect.enabled = true;
            if (effects.sphereTrailEffect != null) effects.sphereTrailEffect.enabled = true;
            if (effects.proximityBubble != null) effects.proximityBubble.Play();
            if (effects.energyPulse != null) effects.energyPulse.Play();
            if (effects.blackBackgroundParticles != null) effects.blackBackgroundParticles.Play();
            if (effects.coloredBackgroundParticles != null) effects.coloredBackgroundParticles.Play();
        }
    }

    private void ResetEffects()
    {
        foreach (var effects in zoneEffects)
        {
            if (effects.movingLines != null) effects.movingLines.SetActive(false);
            if (effects.cubeTrailEffect != null) effects.cubeTrailEffect.enabled = false;
            if (effects.sphereTrailEffect != null) effects.sphereTrailEffect.enabled = false;
            if (effects.proximityBubble != null) effects.proximityBubble.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (effects.energyPulse != null) effects.energyPulse.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (effects.blackBackgroundParticles != null) effects.blackBackgroundParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (effects.coloredBackgroundParticles != null) effects.coloredBackgroundParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
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

    public void AdjustTrailEffect(TrailRenderer trailRenderer, Transform otherPlayer, float minDistance, float maxDistance, bool isCube, bool isSphere)
    {
        if (trailRenderer == null || otherPlayer == null)
        {
            Debug.LogWarning("TrailRenderer or OtherPlayer is missing.");
            return;
        }

        float distance = Vector3.Distance(trailRenderer.transform.position, otherPlayer.position);
        float normalizedDistance = Mathf.InverseLerp(minDistance, maxDistance, distance);

        float newWidth = Mathf.Lerp(0.1f, 0.5f, 1 - normalizedDistance);
        float opacity = Mathf.Lerp(1f, 0.2f, 1 - normalizedDistance);

        // Adjust width and trail time based on whether it's a cube or sphere
        if (isCube)
        {
            newWidth = Mathf.Lerp(0.1f, 0.4f, 1 - Mathf.InverseLerp(minDistance, maxDistance, distance)); // Larger width for the cube
            trailRenderer.time = 0.8f; // Longer trail for the cube
            trailRenderer.endWidth = newWidth * 0.3f;
        }
        else if (isSphere)
        {
            newWidth = Mathf.Lerp(0.1f, 0.2f, 1 - Mathf.InverseLerp(minDistance, maxDistance, distance)); // Larger width for the cube
            trailRenderer.time = 0.1f;
            trailRenderer.endWidth = newWidth * 0.5f;
        }
        else
        {
            newWidth = Mathf.Lerp(0.05f, 0.2f, 1 - Mathf.InverseLerp(minDistance, maxDistance, distance)); // Smaller width for the sphere
            trailRenderer.time = 0.3f; // Shorter trail for the sphere
        }

        Color baseColor = Color.yellow;
        Color fadedColor = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(baseColor, 0.0f), new GradientColorKey(fadedColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(opacity, 0.0f), new GradientAlphaKey(0f, 1.0f) }
        );

        trailRenderer.startWidth = newWidth;
        //trailRenderer.endWidth = newWidth * 0.5f; // Keep the end width smaller for a rigid effect
        trailRenderer.colorGradient = gradient;
    }


    public void HandleProximityBubble(bool forceActive = false)
    {
        if (player1 == null || otherPlayer == null) return;
        {
            BaseCharacter playerCharacter = player1.GetComponent<BaseCharacter>();
            if (playerCharacter == null) return;
            
            bool isWithinProximity = playerCharacter.IsInProximity();

            var effects = zoneEffects[currentZoneIndex];
            if (effects.proximityBubble == null) return;

            if (isWithinProximity || forceActive)
            {
                if (!effects.proximityBubble.isPlaying)
                {
                    effects.proximityBubble.Play();
                    TriggerEnergyPulse();
                }

                //transform.position functions of particle system
                Camera mainCamera = Camera.main;
                Vector3 bubblePosition = (player1.position + otherPlayer.position) / 2f;
                bubblePosition.z = mainCamera.transform.position.z + 12f;
                effects.proximityBubble.transform.position = bubblePosition;
                //effects.proximityBubble.transform.position = (player1.position + otherPlayer.position) / 2f;

                float bubbleSize = Mathf.Lerp(1f, 5f, Mathf.PingPong(Time.time, 1));
                var mainModule = effects.proximityBubble.main;
                mainModule.startSize = bubbleSize;

                var emission = effects.proximityBubble.emission;
                emission.rateOverTime = Mathf.Lerp(10, 0, Mathf.InverseLerp(playerCharacter.minDistance, playerCharacter.proximityRange, Vector3.Distance(player1.position, otherPlayer.position)));
            }
            else if (effects.proximityBubble.isPlaying)
            {
                effects.proximityBubble.Stop();
            }
        }
    }

    private void TriggerEnergyPulse()
    {
        if (zoneEffects[currentZoneIndex].energyPulse == null) return;

        float distance = Vector3.Distance(player1.position, otherPlayer.position);
        float normalizedDistance = Mathf.InverseLerp(minDistance, proximityRange, distance);

        var pulse = zoneEffects[currentZoneIndex].energyPulse;
        var mainModule = pulse.main;
        mainModule.startSize = Mathf.Lerp(0.5f, 2f, 1 - normalizedDistance);
        mainModule.startColor = new Color(0f, 1f, 0f, Mathf.Lerp(0.1f, 1f, 1 - normalizedDistance));

        if (!pulse.isPlaying && normalizedDistance > 0.2f)
            pulse.Play();
        else if (pulse.isPlaying && normalizedDistance <= 0.2f)
            pulse.Stop();
    }

    private void UpdateBackgroundParticles()
    {
        var effects = zoneEffects[currentZoneIndex];

        if (effects.blackBackgroundParticles != null)
            MoveBackgroundParticles(effects.blackBackgroundParticles);

        if (effects.coloredBackgroundParticles != null)
            MoveColoredBackgroundParticles(effects.coloredBackgroundParticles);
    }

    private void MoveBackgroundParticles(ParticleSystem particles)
    {
        if (particles == null || otherPlayer == null) return;

        // Keep particles at a fixed distance in front of the camera
        Camera mainCamera = Camera.main;
        Vector3 newPosition = (player1.position + otherPlayer.position) / 2f;
        newPosition.z = mainCamera.transform.position.z + 30f; //offset from camera

        particles.transform.position = newPosition;

        float distance = Vector3.Distance(player1.position, otherPlayer.position);
        float normalizedDistance = Mathf.InverseLerp(minDistance, proximityRange, distance);

        var emission = particles.emission;
        emission.rateOverTime = Mathf.Max(10, Mathf.Lerp(maxBackgroundEmission, minBackgroundEmission, normalizedDistance));

        var mainModule = particles.main;
        mainModule.startColor = Color.Lerp(Color.magenta, Color.black, normalizedDistance);
    }

    private void MoveColoredBackgroundParticles(ParticleSystem particles)
    {
        if (particles == null || otherPlayer == null) return;

        // Keep particles at a fixed distance in front of the camera
        Camera mainCamera = Camera.main;
        Vector3 newPosition = (player1.position + otherPlayer.position) / 2f;
        newPosition.z = mainCamera.transform.position.z + 30f; // Offset from camera

        particles.transform.position = newPosition;

        float distance = Vector3.Distance(player1.position, otherPlayer.position);
        float normalizedDistance = Mathf.InverseLerp(minDistance, proximityRange, distance);

        // Adjust emission rate based on distance
        var emission = particles.emission;
        emission.rateOverTime = Mathf.Max(5, Mathf.Lerp(maxBackgroundEmission, minBackgroundEmission, Mathf.Pow(normalizedDistance, 2f)));

        // Adjust color
        var mainModule = particles.main;
        mainModule.startColor = Color.Lerp(Color.yellow, Color.blue, normalizedDistance);

        // Adjust lifetime to keep them visible longer
        mainModule.startLifetime = Mathf.Lerp(10f, 20f, normalizedDistance); // Longer lasting particles

        // Make sure particles don't move with velocity
        var velocity = particles.velocityOverLifetime;
        velocity.enabled = false;

        // Spread particles across the world
        var shape = particles.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box; // Or use Sphere for a more radial effect
        shape.scale = new Vector3(50f, 50f, 50f); // Adjust to match world size

        // Add a slow pulsing movement
        var noise = particles.noise;
        noise.enabled = true;
        noise.strength = 2f; // Controls how much they move
        noise.frequency = 0.1f; // Slow movement
        noise.scrollSpeed = 0.02f; // Controls pulsing effect
    }

}
