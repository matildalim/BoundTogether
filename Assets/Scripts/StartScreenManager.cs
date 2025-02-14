using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class StartScreenManager : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    public TMP_Text startText; // UI Text: "Press ENTER to Begin"
    public TMP_Text controlsHint; // UI element for movement controls
    public TMP_Text introText; // "Bound Together" text

    public GameObject startScreenPanel; // Start screen UI
    public GameObject introPanel; // Black screen with "Bound Together" & Controls

    public float fadeSpeed = 1.5f; // Speed of text fade-in and fade-out
    public float proximityThreshold = 1f; // Distance required for them to "connect"
    public float magneticPullStrength = 2f;

    private bool canStart = false;
    private bool gameStarted = false;
    private CanvasGroup startTextCanvasGroup;
    private CanvasGroup controlsCanvasGroup;
    private CanvasGroup introCanvasGroup;
    private CanvasGroup introTextCanvasGroup;

    void Start()
    {
        // Assign CanvasGroups for UI elements, or add them if missing
        introCanvasGroup = AssignCanvasGroup(introPanel);
        introTextCanvasGroup = AssignCanvasGroup(introText);
        startTextCanvasGroup = AssignCanvasGroup(startText);
        controlsCanvasGroup = AssignCanvasGroup(controlsHint);

        // Initial Visibility States
        introCanvasGroup.alpha = 1f; // Fully visible intro screen
        introTextCanvasGroup.alpha = 0f; // "Bound Together" starts hidden
        controlsCanvasGroup.alpha = 0f; // Controls hint starts hidden
        startTextCanvasGroup.alpha = 0f; // Start text hidden at first

        startScreenPanel.SetActive(false); // Ensure start screen is hidden at the beginning
        StartCoroutine(IntroSequence()); // Start intro animation
    }

    private IEnumerator IntroSequence()
    {
        yield return new WaitForSeconds(0.5f); // Small delay before showing text

        // Fade in "Bound Together" and controls hint
        StartCoroutine(FadeTextIn(introTextCanvasGroup));
        yield return StartCoroutine(FadeTextIn(controlsCanvasGroup));

        yield return new WaitForSeconds(2f); // Hold for 2 seconds

        // Fade out intro text and controls hint
        yield return StartCoroutine(FadeTextOut(introTextCanvasGroup));
        yield return StartCoroutine(FadeTextOut(controlsCanvasGroup));

        // Then fade out the entire black screen
        yield return StartCoroutine(FadeTextOut(introCanvasGroup));

        // Disable intro panel
        introPanel.SetActive(false);

        // Activate start screen UI
        startScreenPanel.SetActive(true);
        startText.gameObject.SetActive(true);

        // Fade in the start text
        StartCoroutine(FadeTextIn(startTextCanvasGroup));
    }

    void Update()
    {
        if (gameStarted) return;

        float distance = Vector3.Distance(player1.position, player2.position);

        // Apply "magnetic pull" when they get close
        if (distance < proximityThreshold * 3)
        {
            player1.position = Vector3.Lerp(player1.position, player2.position, Time.deltaTime * magneticPullStrength);
            player2.position = Vector3.Lerp(player2.position, player1.position, Time.deltaTime * magneticPullStrength);
        }

        // When players touch, show "Press ENTER"
        if (distance < proximityThreshold && !canStart)
        {
            canStart = true;
            StartCoroutine(FadeTextIn(startTextCanvasGroup)); // Smooth fade-in effect
        }

        if (canStart && Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(StartGameSequence());
        }
    }

    private IEnumerator StartGameSequence()
    {
        gameStarted = true;

        // Fade out text
        yield return StartCoroutine(FadeTextOut(startTextCanvasGroup));

        // Hide start screen panel
        startScreenPanel.SetActive(false);

        yield return new WaitForSeconds(0.5f); // Optional delay

        // Start Zone 1 & BGM
        ZoneManager.Instance.StartGame();
    }

    private IEnumerator FadeTextIn(CanvasGroup canvasGroup)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeSpeed)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 0.5f, elapsedTime / fadeSpeed);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeTextOut(CanvasGroup canvasGroup)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeSpeed)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0.5f, 0f, elapsedTime / fadeSpeed);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    private CanvasGroup AssignCanvasGroup(GameObject obj)
    {
        if (obj == null) return null;
        return obj.GetComponent<CanvasGroup>() ?? obj.AddComponent<CanvasGroup>();
    }

    private CanvasGroup AssignCanvasGroup(TMP_Text text)
    {
        if (text == null) return null;
        return text.GetComponent<CanvasGroup>() ?? text.gameObject.AddComponent<CanvasGroup>();
    }
}
