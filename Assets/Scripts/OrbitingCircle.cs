using UnityEngine;

public class OrbitingCircle : MonoBehaviour
{
    public ParticleSystem orbitingParticleSystem;

    public float orbitSpeed = 30f;
    public float orbitRadius = 5f;
    public float moveSpeed = 0f; // Forward movement speed

    private ParticleSystem.Particle[] particles;
    private Vector3 initialPosition; // Store the original position

    void Start()
    {
        if (orbitingParticleSystem == null)
            orbitingParticleSystem = GetComponent<ParticleSystem>();

        particles = new ParticleSystem.Particle[orbitingParticleSystem.main.maxParticles];

        // Save initial position
        initialPosition = transform.position;
    }

    void Update()
    {
        // Move the orbiting circle forward at the same speed
        transform.position += new Vector3(0, 0, moveSpeed * Time.deltaTime);

        // Get the current particles
        int particleCount = orbitingParticleSystem.GetParticles(particles);

        // Update each particle's position to follow a circular orbit
        for (int i = 0; i < particleCount; i++)
        {
            float angle = Time.time * orbitSpeed + i * Mathf.PI * 2f / particleCount;
            Vector3 newPosition = new Vector3(Mathf.Cos(angle) * orbitRadius, 0, Mathf.Sin(angle) * orbitRadius);
            particles[i].position = newPosition;
        }

        // Apply the updated particles
        orbitingParticleSystem.SetParticles(particles, particleCount);
    }

    // Set the forward movement speed from outside this script (e.g., from PlayerController)
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
}
