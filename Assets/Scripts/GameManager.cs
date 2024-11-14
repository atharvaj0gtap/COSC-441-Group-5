using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject redTargetPrefab;
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private GameObject endingScreenCanvas;
    [SerializeField] private TargetManager targetManager;
    [SerializeField] private GameObject bubbleCursor;
    [SerializeField] private GameObject pointCursor;
    [SerializeField] private StudyBehavior studyBehavior;
    [SerializeField] private TMP_InputField participantIDInput;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI streakText;  // New field for streak display
    [SerializeField] private Button startButton;


    private bool initialPhaseComplete = false;
    private bool useBubbleCursor;
    private int currentLevel = 0;
    private bool studyCompleted = false;
    private int streakCount = 0;  // Streak count variable
    private int highestStreak = 0;
    private Vector2[] pathPoints = new Vector2[] {
        new Vector2(-8, 4), new Vector2(8, 4), new Vector2(8, -4), new Vector2(-8, -4)
    };


    void Start()
    {
        uiCanvas.SetActive(true);
        bubbleCursor.SetActive(false);
        pointCursor.SetActive(false);
        currentLevel = 0;
        levelText.gameObject.SetActive(false);
        streakText.gameObject.SetActive(true); 
        endingScreenCanvas.SetActive(false);

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

        uiCanvas.SetActive(true);
        //levelCanvas.SetActive(true);
        levelText.gameObject.SetActive(true);
        streakText.gameObject.SetActive(true);  
        participantIDInput.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        useBubbleCursor = studyBehavior.StudySettings.cursorType == CursorType.BubbleCursor;
        StartInitialPhase();
    }

    // New method for handling correct target selection
    public void OnCorrectTargetSelected()
    {
        IncrementStreak(); // Increment the streak here
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.TryGetComponent(out Target target))
            {
                Debug.Log($"Clicked on target: {target.gameObject.name}, IsRedTarget: {target.IsRedTarget}");
                bool isCorrectTarget = target.OnSelect();

                if (!isCorrectTarget)
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
        // Reset the flag when the mouse button is released
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
            
            targetManager.SetupTrial(
                trialData.amplitude, 
                trialData.targetSize, 
                trialData.EWToW_Ratio, 
                trialData.numberOfWhiteTargets, 
                trialData.includeMovingTargets,
                currentLevel
                );
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
        GameObject[] movingTargets = GameObject.FindGameObjectsWithTag("MovingTarget");
        foreach (GameObject movingTarget in movingTargets)
        {
            Destroy(movingTarget);
        }
    }

    public void EndExperiment()
    {
        Debug.Log("Experiment completed. Thank you for participating.");
        bubbleCursor.SetActive(false);
        pointCursor.SetActive(false);
        levelText.gameObject.SetActive(false);
        streakText.gameObject.SetActive(false);

        ClearScreen();

        if (endingScreenCanvas != null)
        {
            endingScreenCanvas.SetActive(true);
        }

        // Display the ending screen
        var endingScreenManager = endingScreenCanvas.GetComponent<EndingScreenManager>();
        if (endingScreenManager != null)
            {
            int totalTrials = studyBehavior.blockSequence.Count;
            float totalTime = studyBehavior.GetTotalTime();
            int totalMissedClicks = studyBehavior.GetTotalMissedClicks();
            int highestStreak = GetHighestStreak();
            endingScreenManager.DisplayEndingScreen(totalTrials, totalTime, totalMissedClicks, highestStreak);
            }
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
        if (streakCount > highestStreak)
        {
            highestStreak = streakCount;
        }
        Debug.Log("IncrementStreak called from GameManager. Current streak: " + streakCount);
        UpdateStreakText();  // Update streak display
        AdjustDifficultyBasedOnStreak();  // Adjust difficulty if needed
    }

    // New method to reset streak count
    public void ResetStreak()
    {
        streakCount = 0;
        UpdateStreakText();  // Update streak display
    }

    public int GetHighestStreak()
    {
        return highestStreak;
    }
}
