using UnityEngine;
using UnityEngine.InputSystem;

public abstract class BaseCharacter : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float forwardSpeed = 10f;
    public float forwardSpeedVariation = 1f;
    public float smoothTime = 0.1f;

    [Header("Distance Settings")]
    public float maxDistance = 4f;
    public float minDistance = 2f;
    public float distanceSpeedMultiplier = 0.1f;
    public float proximityRange = 3f;

    [Header("References")]
    public Transform otherPlayer;
    public TrailRenderer trailRenderer;
    public ParticleSystem proximityBubble;
    public ParticleSystem energyPulse;

    [Header("Background Effects")]
    public ParticleSystem blackBackgroundParticles;
    public ParticleSystem coloredBackgroundParticles;
    public float maxBackgroundEmission = 50f;
    public float minBackgroundEmission = 5f;

    protected PlayerControls controls;
    protected Vector2 moveInput;
    protected Vector3 velocity;
    protected float currentForwardSpeed;
    protected bool isMoving;

    protected virtual void Awake()
    {
        controls = new PlayerControls();
        SetupControls();
    }

    protected abstract void SetupControls();

    protected virtual void Start()
    {
        InitializePosition();
        currentForwardSpeed = forwardSpeed + Random.Range(-forwardSpeedVariation, forwardSpeedVariation);

        if (blackBackgroundParticles != null)
            blackBackgroundParticles.Play();
    }

    protected virtual void InitializePosition()
    {
        if (transform.position == Vector3.zero)
            transform.position = GetDefaultPosition();
    }

    protected abstract Vector3 GetDefaultPosition();

    protected virtual void Update()
    {
        AdjustForwardSpeed();
        HandleMovement();
        AdjustTrailEffect();
        HandleProximityBubble();
        UpdateBackgroundParticles();
    }

    protected virtual void HandleMovement()
    {
        Vector3 inputMovement = new Vector3(moveInput.x * moveSpeed, 0f, 0f);
        Vector3 targetPosition = transform.position + inputMovement;

        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        if (!isMoving)
            smoothedPosition += CalculateSeparationDirection() * moveSpeed * Time.deltaTime;

        transform.position = new Vector3(smoothedPosition.x, transform.position.y, transform.position.z + (currentForwardSpeed * Time.deltaTime));
    }

    protected virtual Vector3 CalculateSeparationDirection()
    {
        return (transform.position.x < otherPlayer.position.x) ? Vector3.left : Vector3.right;
    }

    protected virtual void AdjustForwardSpeed()
    {
        float distance = Vector3.Distance(transform.position, otherPlayer.position);

        if (distance > maxDistance)
            currentForwardSpeed += distanceSpeedMultiplier;
        else if (distance < minDistance)
            currentForwardSpeed -= distanceSpeedMultiplier;

        currentForwardSpeed = Mathf.Clamp(currentForwardSpeed, forwardSpeed - forwardSpeedVariation, forwardSpeed + forwardSpeedVariation);
    }

    protected abstract void AdjustTrailEffect();

    public void HandleProximityBubble(bool forceActive = false)
    {
        if (otherPlayer == null) return;

        float distance = Vector3.Distance(transform.position, otherPlayer.position);
        bool isWithinProximity = distance <= proximityRange;

        if (isWithinProximity || forceActive)  // Ensure activation if forced
        {
            if (!proximityBubble.isPlaying)
            {
                proximityBubble.Play();  // Corrected here
                TriggerEnergyPulse();
            }

            proximityBubble.transform.position = (transform.position + otherPlayer.position) / 2f;

            float bubbleSize = Mathf.Lerp(1f, 5f, Mathf.PingPong(Time.time, 1));
            var mainModule = proximityBubble.main;
            mainModule.startSize = bubbleSize;

            var emission = proximityBubble.emission;
            emission.rateOverTime = Mathf.Lerp(10, 0, Mathf.InverseLerp(minDistance, proximityRange, distance));
        }
        else if (proximityBubble.isPlaying)
        {
            proximityBubble.Stop();  // Corrected here
        }
    }

    void TriggerEnergyPulse()
    {
        if (energyPulse == null) return;

        float distance = Vector3.Distance(transform.position, otherPlayer.position);
        float normalizedDistance = Mathf.InverseLerp(minDistance, proximityRange, distance);

        float pulseSize = Mathf.Lerp(0.5f, 2f, 1 - normalizedDistance);
        float pulseOpacity = Mathf.Lerp(0.1f, 1f, 1 - normalizedDistance);

        var mainModule = energyPulse.main;
        mainModule.startSize = pulseSize;
        mainModule.startColor = new Color(1f, 0.5f, 0f, pulseOpacity);

        if (!energyPulse.isPlaying && normalizedDistance > 0.2f)
            energyPulse.Play();  // Corrected here
        else if (energyPulse.isPlaying && normalizedDistance <= 0.2f)
            energyPulse.Stop();  // Corrected here
    }

    void UpdateBackgroundParticles()
    {
        if (blackBackgroundParticles != null)
            MoveBackgroundParticles(blackBackgroundParticles);

        if (coloredBackgroundParticles != null)
            MoveBackgroundParticles(coloredBackgroundParticles);
    }

    void MoveBackgroundParticles(ParticleSystem particles)
    {
        if (particles == null || otherPlayer == null) return;

        Vector3 newPosition = particles.transform.position;
        newPosition.z += currentForwardSpeed * Time.deltaTime;
        particles.transform.position = newPosition;

        float distance = Vector3.Distance(transform.position, otherPlayer.position);
        float normalizedDistance = Mathf.InverseLerp(minDistance, proximityRange, distance);

        var emission = particles.emission;
        emission.rateOverTime = Mathf.Max(10, Mathf.Lerp(maxBackgroundEmission, minBackgroundEmission, normalizedDistance));  // Ensure emission doesn't drop too low

        var mainModule = particles.main;
        mainModule.startColor = Color.Lerp(Color.magenta, Color.black, normalizedDistance);
    }

    protected virtual void OnEnable()
    {
        controls?.Enable();
    }

    protected virtual void OnDisable()
    {
        controls?.Disable();
    }

    void OnDrawGizmos()
    {
        if (otherPlayer != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, proximityRange);
        }
    }
}
