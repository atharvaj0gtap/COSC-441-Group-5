using System.Collections.Generic;
using UnityEngine;

public class StudyBehavior : MonoBehaviour
{
    public TrialConditions CurrentTrial => blockSequence[currentTrialIndex];
    public StudySettings StudySettings => studySettings;
    
    public int ParticipantID
    {
        get => participantID;
        set => participantID = value;
    }

    public List<TrialConditions> blockSequence = new(); // Make blockSequence public for access in GameManager

    private int participantID;
    [SerializeField] private StudySettings studySettings;
    [SerializeField] private int totalTrials; // Total number of trials to be created

    
    private float timer = 0f;
    private int misClick = 0;
    public int currentTrialIndex = 0;
    private float totalTime = 0f;          // Total time across all trials
    private int totalMissedClicks = 0;     // Total missed clicks across all trials

    private string[] header = { "PID", "CT", "A", "W", "EWW", "MT", "MissedClicks", "MovingTargets" };

    private void Start()
    {
        // Set file path for the CSV file in the Assets/Data folder
        CSVManager.SetFilePath(Application.dataPath, studySettings.cursorType.ToString());

        LogHeader(); // Log CSV header for data format
        CreateBlock(); // Create trial block sequence based on settings
        StartTrial(); // Begin the first trial
    }

    private void Update()
    {
        timer += Time.deltaTime; // Update timer to record movement time (MT)
    }

    private void StartTrial()
{
    timer = 0f;      // Reset timer for the trial
    misClick = 0;    // Reset missed click counter for the trial
}


    public void NextTrial()
{
    LogData();  // Log data for the completed trial

    // Accumulate totals before resetting for the next trial
    totalTime += timer;
    totalMissedClicks += misClick;

    currentTrialIndex++;
    
    // Check if the trial index has reached the total number of trials
    if (currentTrialIndex >= blockSequence.Count)
    {
        EndStudy();  // End the study if all trials are completed
    }
    else
    {
        StartTrial();  // Start the next trial
        FindObjectOfType<GameManager>().StartCoroutine("ProceedToNextTrial");
    }
}



   public void CreateBlock()
{
    blockSequence.Clear(); // Start with an empty list of trials

    List<TrialConditions> stationaryTrials = new List<TrialConditions>();
    List<TrialConditions> movingTrials = new List<TrialConditions>();

    int numberOfStationaryTrials = 5;
    int stationaryTrialsAdded = 0;

    // Generate all possible combinations of conditions
    foreach (float EW in studySettings.EWToW_Ratio)
    {
        foreach (float size in studySettings.targetSizes)
        {
            foreach (float amp in studySettings.targetAmplitudes)
            {
                var trialCondition = new TrialConditions()
                {
                    amplitude = amp,
                    targetSize = size,
                    EWToW_Ratio = EW,
                    numberOfWhiteTargets = studySettings.numberOfWhiteTargets,
                    includeMovingTargets = false
                };
                if (stationaryTrialsAdded < numberOfStationaryTrials)
                {
                    stationaryTrials.Add(trialCondition);
                    stationaryTrialsAdded++;
                }
                else
                {
                trialCondition.includeMovingTargets = true;
                movingTrials.Add(trialCondition);
                }
            }
        }
    }

    // Shuffle the list of combinations for random ordering
    stationaryTrials = YatesShuffle(stationaryTrials);
    movingTrials = YatesShuffle(movingTrials);

    // Combine stationary and moving trials
    List<TrialConditions> allTrials = new List<TrialConditions>();
    allTrials.AddRange(stationaryTrials);
    allTrials.AddRange(movingTrials);

    // Add trials to blockSequence until we reach totalTrials
    int trialsAdded = 0;
    foreach (var trial in allTrials)
    {
        if (trialsAdded >= totalTrials)
            break;
            blockSequence.Add(trial);
            trialsAdded++;
        }
    Debug.Log("Total number of trials created: " + blockSequence.Count); // Log the actual number of trials created
}



    public void LogHeader()
    {
        CSVManager.AppendToCSV(header); // Log CSV header for data columns
    }

    private void LogData()
    {
        var trial = blockSequence[currentTrialIndex]; // Define trial

        // Data row for each trial
        string[] data =
        {
            participantID.ToString(),
            studySettings.cursorType.ToString(),
            blockSequence[currentTrialIndex].amplitude.ToString(),
            blockSequence[currentTrialIndex].targetSize.ToString(),
            blockSequence[currentTrialIndex].EWToW_Ratio.ToString(),
            timer.ToString(),  // Movement time (MT)
            misClick.ToString(),  // Missed clicks
            trial.includeMovingTargets ? "1" : "0" // Indicate if moving targets were included

        };
        CSVManager.AppendToCSV(data); // Log trial data to CSV
    }

    public void HandleMissedClick()
    {
        misClick++; // Increment missed click counter
    }

    public void EndStudy()
    {
    Debug.Log("Study completed. Ending session.");
    // Here you can add code to display a message, load an end scene, or log final data
    FindObjectOfType<GameManager>().EndExperiment();
    }

    public void SetParticipantID(int ID)
    {
        participantID = ID;
    }

    private static List<T> YatesShuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
        return list;
    }

    public float GetTotalTime()
    {        
        return totalTime;
    }

    public int GetTotalMissedClicks()
    {
        return totalMissedClicks;
    }
}

[System.Serializable]
public class StudySettings
{
    public List<float> targetSizes; // List of target sizes
    public List<float> targetAmplitudes; // List of target amplitudes
    public List<float> EWToW_Ratio; // Effective Width to Width ratio
    public CursorType cursorType; // Cursor type (PointCursor or BubbleCursor)
    public int numberOfWhiteTargets; // Number of additional white targets
}

public enum CursorType
{
    PointCursor = 0,
    BubbleCursor = 1
}

[System.Serializable]
public class TrialConditions
{
    public float amplitude; // Target amplitude
    public float targetSize; // Target size
    public float EWToW_Ratio; // EW/W ratio for distractors
    public int numberOfWhiteTargets; // Number of additional white targets
    public bool includeMovingTargets; // NEW: Whether moving targets are included
}
