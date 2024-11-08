using UnityEngine;
using UnityEngine.UI; // Add this if using UnityEngine.UI.Text
using TMPro; // Add this if using TextMeshPro
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject redTargetPrefab;
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private TargetManager targetManager;
    [SerializeField] private GameObject bubbleCursor;
    [SerializeField] private GameObject pointCursor;
    [SerializeField] private StudyBehavior studyBehavior;
    [SerializeField] private TMP_InputField participantIDInput;
    [SerializeField] private TextMeshProUGUI levelText; // Reference to the TextMeshProUGUI component
    // [SerializeField] private Text levelText; // Use this if using UnityEngine.UI.Text

    private int currentTrial = 0;
    private bool initialPhaseComplete = false;
    private bool useBubbleCursor;
    private int currentLevel = 0; // Initialize current level
    private bool studyCompleted = false;

    void Start()
    {
        bubbleCursor.SetActive(false);
        pointCursor.SetActive(false);
        currentLevel = 0; // Reset level when the game starts
        levelText.gameObject.SetActive(false); // Set the level text to inactive initially
        UpdateLevelText(); // Update the level text at the start
    }

    public void StartGame()
    {
        if (participantIDInput != null && int.TryParse(participantIDInput.text, out int participantID))
        {
            studyBehavior.ParticipantID = participantID;
        }
        else
        {
            Debug.LogError("Participant ID Input is missing or invalid.");
            return;
        }

        uiCanvas.SetActive(false);
        levelText.gameObject.SetActive(true); // Ensure the level text is active when the game starts
        useBubbleCursor = studyBehavior.StudySettings.cursorType == CursorType.BubbleCursor;
        StartInitialPhase();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.TryGetComponent(out Target target))
            {
                target.OnSelect();
                if (!initialPhaseComplete)
                {
                    initialPhaseComplete = true;
                    StartCoroutine(ProceedToNextTrial());
                }
            }
        }
    }

    void StartInitialPhase()
    {
        Debug.Log("Starting initial phase with central red target.");
        var initialTarget = Instantiate(redTargetPrefab, Vector3.zero, Quaternion.identity);
        initialTarget.tag = "Target";
    }

    public IEnumerator ProceedToNextTrial()
    {
        if (studyCompleted) yield break;  // Stop the coroutine if the study is completed

        ClearScreen();
        yield return new WaitForSeconds(0.5f);

        if (studyBehavior.currentTrialIndex < studyBehavior.blockSequence.Count)
        {
            // Setup the next trial only if the study is still ongoing
            var trialData = studyBehavior.CurrentTrial;
            targetManager.SetupTrial(trialData.amplitude, trialData.targetSize, trialData.EWToW_Ratio, currentLevel);

            if (useBubbleCursor)
            {
                bubbleCursor.SetActive(true);
            }
            else
            {
                pointCursor.SetActive(true);
            }

            IncrementLevel(); // Increment level after each trial
        }
        else
        {
            studyCompleted = true; // Mark study as completed
            studyBehavior.EndStudy(); // End the study if all trials are completed
        }
    }

    void ClearScreen()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
        foreach (GameObject target in targets)
        {
            Destroy(target);
        }
    }

    void EndExperiment()
    {
        Debug.Log("Experiment completed. Thank you for participating.");
        bubbleCursor.SetActive(false);
        pointCursor.SetActive(false);
    }

    public void IncrementLevel()
    {
        currentLevel = Mathf.Min(currentLevel + 1, 10); // Increment level up to a maximum of 10
        UpdateLevelText(); // Update the level text when the level changes
    }

    private void UpdateLevelText()
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + currentLevel;
        }
    }
}