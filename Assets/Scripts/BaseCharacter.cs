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

    protected bool isCube;
    protected bool isSphere;

    protected virtual void Awake()
    {
        controls = new PlayerControls();
        SetupControls();
    }

    protected abstract void SetupControls();

    protected virtual void Start()
    {
        isSphere = this is SphereController;
        isCube = this is CubeController;
        InitializePosition();
        currentForwardSpeed = forwardSpeed + Random.Range(-forwardSpeedVariation, forwardSpeedVariation);
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

        //if (IsInProximity())
        //{
        //    Debug.Log(gameObject.name + " is close to the other player.");
        //}
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

    public virtual bool IsInProximity()
    {
        if (otherPlayer == null) return false;

        float distance = Vector3.Distance(transform.position, otherPlayer.position);
        return distance <= proximityRange;
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
