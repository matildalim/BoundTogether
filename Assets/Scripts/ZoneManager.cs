using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public GameObject zone1Objects;  // Objects for Zone 1 (e.g., particles)
    public GameObject zone2Objects;  // Objects for Zone 2 (e.g., geometric shapes)
    public GameObject zone3Objects;  // Objects for Zone 3 (e.g., particles + geometric shapes)

    public float zoneDuration = 60f;  // Duration of each zone (1 minute)
    private float timer;

    private bool isInZone1Active = true;
    private bool isInZone2Active = false;
    private bool isInZone3Active = false;

    private void Start()
    {
        // Debug log for initial setup
        Debug.Log("Start method called");

        zone1Objects.SetActive(true);  // Enable Zone 1
        zone2Objects.SetActive(false); // Disable Zone 2
        zone3Objects.SetActive(false); // Disable Zone 3

        Debug.Log("Zone 1 Active: " + zone1Objects.activeSelf);  // Check Zone 1 status
        Debug.Log("Zone 2 Active: " + zone2Objects.activeSelf);  // Check Zone 2 status
        Debug.Log("Zone 3 Active: " + zone3Objects.activeSelf);  // Check Zone 3 status

        isInZone1Active = true;  // Ensure Zone 1 is active
        timer = zoneDuration;     // Initialize the timer for Zone 1 duration
    }

    private void Update()
    {
        if (isInZone1Active)
        {
            timer -= Time.deltaTime;  // Decrease the timer

            // Check if the timer has finished and transition to Zone 2
            if (timer <= 0)
            {
                TransitionToZone2();
            }
        }

        HandleCheatCodes();  // Cheat code to switch zones manually (if needed)
    }
    private void TransitionToZone2()
    {
        // Deactivate Zone 1 and activate Zone 2
        isInZone1Active = false;
        isInZone2Active = true;
        zone1Objects.SetActive(false); // Disable Zone 1
        zone2Objects.SetActive(true);  // Enable Zone 2

        // Reset the timer for Zone 2
        timer = zoneDuration;
    }

    private void TransitionToZone3()
    {
        // Deactivate Zone 2 and activate Zone 3
        isInZone2Active = false;
        isInZone3Active = true;
        zone2Objects.SetActive(false); // Disable Zone 2
        zone3Objects.SetActive(true);  // Enable Zone 3

        // Reset the timer for Zone 3
        timer = zoneDuration;
    }

    private void HandleCheatCodes()
    {
        // Example of cheat codes using input keys to transition between zones
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Press '1' to go to Zone 1
        {
            GoToZone1();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) // Press '2' to go to Zone 2
        {
            GoToZone2();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) // Press '3' to go to Zone 3
        {
            GoToZone3();
        }
    }

    private void GoToZone1()
    {
        // Manually switch to Zone 1
        isInZone1Active = true;
        isInZone2Active = false;
        isInZone3Active = false;
        zone1Objects.SetActive(true);
        zone2Objects.SetActive(false);
        zone3Objects.SetActive(false);

        // Reset the timer for Zone 1
        timer = zoneDuration;
    }

    private void GoToZone2()
    {
        // Manually switch to Zone 2
        isInZone1Active = false;
        isInZone2Active = true;
        isInZone3Active = false;
        zone1Objects.SetActive(false);
        zone2Objects.SetActive(true);
        zone3Objects.SetActive(false);

        // Reset the timer for Zone 2
        timer = zoneDuration;
    }

    private void GoToZone3()
    {
        // Manually switch to Zone 3
        isInZone1Active = false;
        isInZone2Active = false;
        isInZone3Active = true;
        zone1Objects.SetActive(false);
        zone2Objects.SetActive(false);
        zone3Objects.SetActive(true);

        // Reset the timer for Zone 3
        timer = zoneDuration;
    }
}
