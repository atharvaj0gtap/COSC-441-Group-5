using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI streakText;  // New field for streak display

    private int currentTrial = 0;
    private bool initialPhaseComplete = false;
    private bool useBubbleCursor;
    private int currentLevel = 0;
    private bool studyCompleted = false;
    private int streakCount = 0;  // Streak count variable

    void Start()
    {
        bubbleCursor.SetActive(false);
        pointCursor.SetActive(false);
        currentLevel = 0;
        levelText.gameObject.SetActive(false);
        streakText.gameObject.SetActive(true); // Ensure streak text is active
        UpdateLevelText();
        UpdateStreakText(); // Initialize streak display
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
        levelText.gameObject.SetActive(true);
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
                bool isCorrectTarget = target.OnSelect();

                if (isCorrectTarget)
                {
                    IncrementStreak(); // Increment streak on correct selection
                }
                else
                {
                    ResetStreak(); // Reset streak on incorrect selection
                }

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
        if (studyCompleted) yield break;

        ClearScreen();
        yield return new WaitForSeconds(0.5f);

        if (studyBehavior.currentTrialIndex < studyBehavior.blockSequence.Count)
        {
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
            IncrementLevel();
        }
        else
        {
            studyCompleted = true;
            studyBehavior.EndStudy();
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
        currentLevel = Mathf.Min(currentLevel + 1, 10);
        UpdateLevelText();
    }

    private void UpdateLevelText()
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + currentLevel;
        }
    }

    private void UpdateStreakText()
    {
        if (streakText != null)
        {
            streakText.text = "Streak: " + streakCount;
        }
    }

    private void AdjustDifficultyBasedOnStreak()
    {
        int distractorMultiplier = streakCount >= 3 ? streakCount : 1;
        targetManager.SetDistractorMultiplier(distractorMultiplier);
    }

    // New method to increment streak count
    public void IncrementStreak()
    {
        streakCount++;
        UpdateStreakText();  // Update streak display
        AdjustDifficultyBasedOnStreak();  // Adjust difficulty if needed
    }

    // New method to reset streak count
    public void ResetStreak()
    {
        streakCount = 0;
        UpdateStreakText();  // Update streak display
    }
}
