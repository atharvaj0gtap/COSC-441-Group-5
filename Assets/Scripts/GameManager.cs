using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs and Managers")]
    [SerializeField] private GameObject redTargetPrefab;
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private TargetManager targetManager;
    [SerializeField] private GameObject bubbleCursor;
    [SerializeField] private GameObject pointCursor;
    [SerializeField] private StudyBehavior studyBehavior;

    [Header("UI Elements")]
    [SerializeField] private TMP_InputField participantIDInput;

    private int currentTrialIndex = 0;
    private bool isInitialPhaseCompleted = false;
    private bool useBubbleCursor;
    private bool isStudyCompleted = false;

    private void Start()
    {
        HideCursors();
    }

    public void BeginStudy()
    {
        if (!ValidateParticipantID()) return;

        uiCanvas.SetActive(false);
        useBubbleCursor = studyBehavior.StudySettings.cursorType == CursorType.BubbleCursor;

        StartInitialPhase();
    }

    private void Update()
    {
        HandleMouseClick();
    }

    private bool ValidateParticipantID()
    {
        if (participantIDInput != null && int.TryParse(participantIDInput.text, out int participantID))
        {
            studyBehavior.ParticipantID = participantID;
            return true;
        }
        else
        {
            Debug.LogError("Invalid or missing Participant ID.");
            return false;
        }
    }

    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.TryGetComponent(out Target target))
            {
                target.OnSelect();
                if (!isInitialPhaseCompleted)
                {
                    isInitialPhaseCompleted = true;
                    StartCoroutine(ProceedToNextTrial());
                }
            }
        }
    }

    private void StartInitialPhase()
    {
        Debug.Log("Starting initial phase with central red target.");
        CreateRedTarget(Vector3.zero);
    }

    private void CreateRedTarget(Vector3 position)
    {
        GameObject initialTarget = Instantiate(redTargetPrefab, position, Quaternion.identity);
        initialTarget.tag = "Target";
    }

    public IEnumerator ProceedToNextTrial()
    {
        if (isStudyCompleted) yield break;

        ClearAllTargets();
        yield return new WaitForSeconds(0.5f);

        if (studyBehavior.currentTrialIndex < studyBehavior.blockSequence.Count)
        {
            SetupNextTrial();
        }
        else
        {
            CompleteStudy();
        }
    }

    private void SetupNextTrial()
    {
        var trialData = studyBehavior.CurrentTrial;
        targetManager.InitializeTrial(trialData.amplitude, trialData.targetSize, trialData.EWToW_Ratio, trialData.numberOfWhiteTargets);

        if (useBubbleCursor)
        {
            bubbleCursor.SetActive(true);
        }
        else
        {
            pointCursor.SetActive(true);
        }
    }

    private void CompleteStudy()
    {
        isStudyCompleted = true;
        studyBehavior.EndStudy();
        EndExperiment();
    }

    private void ClearAllTargets()
    {
        GameObject[] allTargets = GameObject.FindGameObjectsWithTag("Target");
        foreach (GameObject target in allTargets)
        {
            Destroy(target);
        }
    }

    private void EndExperiment()
    {
        Debug.Log("Experiment completed. Thank you for participating.");
        HideCursors();
    }

    private void HideCursors()
    {
        bubbleCursor.SetActive(false);
        pointCursor.SetActive(false);
    }
}

