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