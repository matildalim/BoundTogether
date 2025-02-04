using System.Xml.Schema;
using Unity.Hierarchy;
using Unity.VisualScripting;
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
    public ParticleSystem backgroundParticles;
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
    
        if (backgroundParticles != null)
        {
            backgroundParticles.Play();
        }
    
    }

    protected virtual void InitializePosition()
    {
        if (transform.position == Vector3.zero)
        {
            transform.position = GetDefaultPosition();
        }
    }

    protected abstract Vector3 GetDefaultPosition();

    protected virtual void Update()
    {
        //Debug.Log("BaseCharacter Update running on " + gameObject.name);

        AdjustForwardSpeed();
        HandleMovement();
        AdjustTrailEffect();
        HandleProximityBubble();
        UpdateBackgroundParticles();
    }

    protected virtual void HandleMovement()
    {
        // Calculate base movement
        Vector3 inputMovement = new Vector3(moveInput.x * moveSpeed, 0f, 0f);
        Vector3 targetPosition = transform.position + inputMovement;

        // Apply smoothing
        Vector3 smoothedPosition = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );

        // Handle automatic separation when not moving
        if (!isMoving)
        {
            Vector3 directionToMove = CalculateSeparationDirection();
            smoothedPosition += directionToMove * moveSpeed * Time.deltaTime;
        }

        // Apply final position
        transform.position = new Vector3(
            smoothedPosition.x,
            transform.position.y,
            transform.position.z + (currentForwardSpeed * Time.deltaTime)
        );
    }

    protected virtual Vector3 CalculateSeparationDirection()
    {
        return (transform.position.x < otherPlayer.position.x) ? Vector3.left : Vector3.right;
    }

    protected virtual void AdjustForwardSpeed()
    {
        float distance = Vector3.Distance(transform.position, otherPlayer.position);

        if (distance > maxDistance)
        {
            currentForwardSpeed += distanceSpeedMultiplier;
        }
        else if (distance < minDistance)
        {
            currentForwardSpeed -= distanceSpeedMultiplier;
        }

        currentForwardSpeed = Mathf.Clamp(
            currentForwardSpeed,
            forwardSpeed - forwardSpeedVariation,
            forwardSpeed + forwardSpeedVariation
        );
    }

    protected abstract void AdjustTrailEffect(); //making it abstract allows the subclass to define it

    public void HandleProximityBubble()
    {
        if (otherPlayer != null)
        {
            float distance = Vector3.Distance(transform.position, otherPlayer.position);
            bool isWithinProximity = distance <= proximityRange;

            // Debug logs
            Debug.Log("Distance: " + distance);
            Debug.Log("Proximity range: " + proximityRange);
            Debug.Log("Is within proximity: " + isWithinProximity);

            if (isWithinProximity) //play or stop bubble within proximity
            {
                if (!proximityBubble.isPlaying)
                {
                    proximityBubble.Play(); // Show bubble
                    TriggerEnergyPulse();

                }

                Vector3 midpoint = (transform.position + otherPlayer.position) / 2f; // update bubble position to be between the two objects
                proximityBubble.transform.position = midpoint;

                float normalizedDistance = Mathf.InverseLerp(minDistance, proximityRange, distance);

                float bubbleSize = Mathf.Lerp(1f, 5f, Mathf.PingPong(Time.time, 1));
                //proximityBubble.transform.localScale = new Vector3(bubbleSize, bubbleSize, bubbleSize);

                var mainModule = proximityBubble.main;
                mainModule.startSize = bubbleSize;


                var emission = proximityBubble.emission; //emission rate based on proximity
                emission.rateOverTime = Mathf.Lerp(10, 0, normalizedDistance);
            }
            else if (!isWithinProximity && proximityBubble.isPlaying)
            {
                proximityBubble.Stop(); // Hide bubble
                Debug.Log("Stopping proximity bubble.");
            }

        }
    }

    void TriggerEnergyPulse()
    {
        if (energyPulse != null)
        {
            float distance = Vector3.Distance(transform.position, otherPlayer.position);
            float normalizedDistance = Mathf.InverseLerp(minDistance, proximityRange, distance);

            float pulseSize = Mathf.Lerp(0.5f, 2f, 1 - normalizedDistance);
            float pulseOpacity = Mathf.Lerp(0.1f, 1f, 1 - normalizedDistance);

            var mainModule = energyPulse.main;
            mainModule.startSize = pulseSize;
            mainModule.startColor = new Color(1f, 0.5f, 0f, pulseOpacity);

            if (!energyPulse.isPlaying && normalizedDistance > 0.2f)
            {
                energyPulse.Play();
            }
            else if (energyPulse.isPlaying && normalizedDistance <= 0.2f)
            {
                energyPulse.Stop();
            }
        }
    }


    void UpdateBackgroundParticles()
    {
        if (backgroundParticles != null && otherPlayer != null)
        {
            // Move the background particles forward
            Vector3 newPosition = backgroundParticles.transform.position;
            newPosition.z += currentForwardSpeed * Time.deltaTime;
            backgroundParticles.transform.position = newPosition;

            // Calculate distance between players
            float distance = Vector3.Distance(transform.position, otherPlayer.position);

            // Normalize distance (0 when close, 1 when far)
            float normalizedDistance = Mathf.InverseLerp(minDistance, proximityRange, distance);

            // Adjust background particle emission dynamically
            var emission = backgroundParticles.emission;
            float targetEmissionRate = Mathf.Lerp(maxBackgroundEmission, minBackgroundEmission, normalizedDistance);

            // Smooth transition for better visual effect
            emission.rateOverTime = Mathf.Lerp(emission.rateOverTime.constant, targetEmissionRate, Time.deltaTime * 5f);

            var mainModule = backgroundParticles.main;
            mainModule.startColor = Color.Lerp(Color.magenta, Color.black, normalizedDistance);

        }
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
            //Gizmos.color = Color.yellow;
            //Gizmos.DrawWireSphere(transform.position, maxDistance);
            //Gizmos.color = Color.red;
            //Gizmos.DrawWireSphere(transform.position, minDistance);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, proximityRange); // Debug proximity bubble
        }
    }
}