using System.Collections;
using System.Xml.Serialization;
using TMPro;
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
    private bool gameStarted = false;


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
        public GameObject floatingBubbleCube;
        public GameObject floatingBubbleSphere;
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

    private Material bubbleMaterialCube, bubbleMaterialSphere;
    private float bubbleAlpha = 0f;
    private bool bubblesVisible = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
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

        if (isZone6) HandleFloatingBubbles();

    }

    public void StartGame()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            ActivateZone(0);
            timer = zoneDuration;
            AudioManager.Instance.PlayBGM();
        }
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

            if (isZone6)
            {
                if (effects.floatingBubbleCube != null)
                {
                    effects.floatingBubbleCube.SetActive(true);
                    bubbleMaterialCube = effects.floatingBubbleCube.GetComponent<Renderer>().material;
                }

                if (effects.floatingBubbleSphere != null)
                {
                    effects.floatingBubbleSphere.SetActive(true);
                    bubbleMaterialSphere = effects.floatingBubbleSphere.GetComponent<Renderer>().material;
                }

                bubbleAlpha = 0f;  // Start invisible
                bubblesVisible = false;
            }
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
            if (effects.floatingBubbleCube != null) effects.floatingBubbleCube.SetActive(false);
            if (effects.floatingBubbleSphere != null) effects.floatingBubbleSphere.SetActive(false);
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

        float newWidth = Mathf.Lerp(0.05f, 0.2f, 1 - normalizedDistance);
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
            newWidth = Mathf.Lerp(0.03f, 0.1f, 1 - Mathf.InverseLerp(minDistance, maxDistance, distance)); // Larger width for the cube
            trailRenderer.time = 0.6f;
            trailRenderer.endWidth = newWidth * 0.8f;
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
            //new GradientColorKey[] { new GradientColorKey(baseColor, 0.0f), new GradientColorKey(fadedColor, 1.0f) },
            //new GradientAlphaKey[] { new GradientAlphaKey(opacity, 0.0f), new GradientAlphaKey(0f, 1.0f) }
            new GradientColorKey[]
        {
            new GradientColorKey(baseColor, 0.0f),
            new GradientColorKey(baseColor * 0.8f, 0.5f),  // Midpoint color to smooth out fading
            new GradientColorKey(fadedColor, 1.0f)
        },
        new GradientAlphaKey[]
        {
            new GradientAlphaKey(opacity, 0.0f),
            new GradientAlphaKey(opacity * 0.5f, 0.5f),
            new GradientAlphaKey(0f, 1.0f)
        }
        );

        trailRenderer.startWidth = newWidth;
        trailRenderer.endWidth = newWidth * 0.5f; // Keep the end width smaller for a rigid effect
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

                ////transform.position functions of particle system
                Camera mainCamera = Camera.main;
                Vector3 bubblePosition = (player1.position + otherPlayer.position) / 2f;
                bubblePosition.z = mainCamera.transform.position.z + 12f;
                effects.proximityBubble.transform.position = bubblePosition;

                float floatOffset = Mathf.Sin(Time.time * 2f) * 0.5f; // More dynamic up-down movement
                bubblePosition.y += floatOffset;

                //Smoothly Move Bubble Towards Target Position
                effects.proximityBubble.transform.position = Vector3.Lerp(
                    effects.proximityBubble.transform.position,
                    bubblePosition,
                    Time.deltaTime * 3f // Smooth movement speed
                );

                //Adjust Bubble Size Based on Distance Between Cube & Sphere
                float distance = Vector3.Distance(player1.position, otherPlayer.position);
                float sizeFactor = Mathf.Lerp(1f, 3f, 1 - Mathf.InverseLerp(minDistance, proximityRange, distance));

                effects.proximityBubble.transform.localScale = new Vector3(sizeFactor, sizeFactor, sizeFactor);

                //Dynamic Bubble Opacity (Fades in When Close)
                float targetAlpha = Mathf.Lerp(0.3f, 1f, 1 - Mathf.InverseLerp(minDistance, proximityRange, distance));
                StartCoroutine(FadeBubble(effects.proximityBubble, targetAlpha, false));

                // **Adjust Particle Emission Rate Based on Distance
                var emission = effects.proximityBubble.emission;
                emission.rateOverTime = Mathf.Lerp(5, 25, 1 - Mathf.InverseLerp(minDistance, proximityRange, distance));

                //trigger fadeout when distance exceeds proximity range
                if (distance >= proximityRange * 1.1f)
                {
                    StartCoroutine(FadeBubble(effects.proximityBubble, 0f, true));
                }
                else if (!effects.proximityBubble.isPlaying)
                {
                    effects.proximityBubble.Play();
                }
            }
            else if (effects.proximityBubble.isPlaying)
            {
                effects.proximityBubble.Stop();
            }
        }
    }

    //private IEnumerator FadeBubble(ParticleSystem bubble, float targetAlpha)
    //{
    //    var mainModule = bubble.main;
    //    float startAlpha = mainModule.startColor.color.a;
    //    float elapsedTime = 0f;
    //    float fadeSpeed = 2.0f;

    //    while (elapsedTime < fadeSpeed)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        Color color = mainModule.startColor.color;
    //        color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeSpeed);
    //        mainModule.startColor = color;
    //        yield return null;
    //    }
    //}

    private IEnumerator FadeBubble(ParticleSystem bubble, float targetAlpha, bool fadeOut)
    {
        float startAlpha = bubble.main.startColor.color.a;
        float elapsedTime = 0f;
        float fadeSpeed = 2.0f;

        var mainModule = bubble.main;
        Color bubbleColor = mainModule.startColor.color;

        while (elapsedTime < fadeSpeed)
        {
            elapsedTime += Time.deltaTime;
            bubbleColor.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeSpeed);
            mainModule.startColor = new ParticleSystem.MinMaxGradient(bubbleColor);

            // Shrink bubble if it's fading out
            if (fadeOut)
            {
                float scaleFactor = Mathf.Lerp(bubble.transform.localScale.x, 0f, elapsedTime / fadeSpeed);
                bubble.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            }

            yield return null;
        }

        // Fully hide the bubble if faded out
        if (fadeOut)
        {
            bubble.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
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
        newPosition.z = mainCamera.transform.position.z + 30f; // Offset from camera

        particles.transform.position = newPosition;

        float distance = Vector3.Distance(player1.position, otherPlayer.position);
        float normalizedDistance = Mathf.InverseLerp(minDistance, proximityRange, distance);

        var emission = particles.emission;
        float targetEmissionRate = Mathf.Lerp(maxBackgroundEmission, minBackgroundEmission, normalizedDistance);

        // **Gradually Increase Emission Rate**
        emission.rateOverTime = Mathf.Lerp(emission.rateOverTime.constant, targetEmissionRate, Time.deltaTime * 2f);

        var mainModule = particles.main;
        mainModule.startColor = Color.Lerp(Color.magenta, Color.black, normalizedDistance);

        // **Gradually Fade In Particles by Adjusting Alpha**
        Color startColor = mainModule.startColor.color;
        startColor.a = Mathf.Lerp(startColor.a, 1f, Time.deltaTime * 1.5f);
        mainModule.startColor = startColor;
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

        var emission = particles.emission;
        float targetEmissionRate = Mathf.Lerp(maxBackgroundEmission, minBackgroundEmission, Mathf.Pow(normalizedDistance, 2f));

        // **Smoothly Increase Emission Rate**
        emission.rateOverTime = Mathf.Lerp(emission.rateOverTime.constant, targetEmissionRate, Time.deltaTime * 2f);

        var mainModule = particles.main;
        mainModule.startColor = Color.Lerp(Color.yellow, Color.blue, normalizedDistance);

        // **Gradually Increase Opacity for a Smooth Effect**
        Color startColor = mainModule.startColor.color;
        startColor.a = Mathf.Lerp(startColor.a, 1f, Time.deltaTime * 1.5f);
        mainModule.startColor = startColor;

        // Adjust lifetime to keep them visible longer
        mainModule.startLifetime = Mathf.Lerp(10f, 20f, normalizedDistance);

        // Spread particles across the world
        var shape = particles.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(50f, 50f, 50f);

        // Add a slow pulsing movement
        var noise = particles.noise;
        noise.enabled = true;
        noise.strength = 2f;
        noise.frequency = 0.1f;
        noise.scrollSpeed = 0.02f;
    }

    private void HandleFloatingBubbles()
    {
        if (zoneEffects[currentZoneIndex].floatingBubbleCube == null || zoneEffects[currentZoneIndex].floatingBubbleSphere == null) return;

        GameObject bubbleCube = zoneEffects[currentZoneIndex].floatingBubbleCube;
        GameObject bubbleSphere = zoneEffects[currentZoneIndex].floatingBubbleSphere;

        if (!bubbleCube.activeSelf || !bubbleSphere.activeSelf) return;

        // Smoothly move bubbles to follow the players
        Vector3 cubeTarget = player1.position;
        Vector3 sphereTarget = player2.position;

        float floatOffset = Mathf.Sin(Time.time * 0.5f) * 0.5f; // Smooth vertical movement

        cubeTarget.y += floatOffset;
        sphereTarget.y += floatOffset;

        bubbleCube.transform.position = Vector3.Lerp(bubbleCube.transform.position, cubeTarget, Time.deltaTime * 3f);
        bubbleSphere.transform.position = Vector3.Lerp(bubbleSphere.transform.position, sphereTarget, Time.deltaTime * 3f);

        // Adjust bubble sizes dynamically
        float distance = Vector3.Distance(player1.position, player2.position);
        float sizeFactor = Mathf.Lerp(0.5f, 1.5f, Mathf.InverseLerp(minDistance, proximityRange, distance));

        bubbleCube.transform.localScale = new Vector3(sizeFactor, sizeFactor, sizeFactor);
        bubbleSphere.transform.localScale = new Vector3(sizeFactor, sizeFactor, sizeFactor);

        // Smooth Fade-in Effect
        if (!bubblesVisible)
        {
            bubblesVisible = true;
            StartCoroutine(FadeBubble(bubbleMaterialCube, 1f));
            StartCoroutine(FadeBubble(bubbleMaterialSphere, 1f));
        }
    }

    private IEnumerator FadeBubble(Material bubbleMat, float targetAlpha)
    {
        float startAlpha = bubbleMat.color.a;
        float elapsedTime = 0f;
        float fadeSpeed = 2.0f;

        while (elapsedTime < fadeSpeed)
        {
            elapsedTime += Time.deltaTime;
            Color color = bubbleMat.color;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeSpeed);
            bubbleMat.color = color;
            yield return null;
        }
    }


}  
