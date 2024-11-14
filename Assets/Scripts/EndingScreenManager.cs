using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;

public class EndingScreenManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI thankYouText;
    [SerializeField] private TextMeshProUGUI performanceSummaryText;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button restartButton;

    private void onDisable()
    {
    if (exitButton != null) 
    {
        exitButton.onClick.AddListener(ExitApplication);
    }
    else {
        Debug.LogError("Exit or Restart button not set in the inspector.");
    }
    if (restartButton != null)
    {
        restartButton.onClick.AddListener(RestartExperiment);
    }
    else
    {
        Debug.LogError("Restart button not set in the inspector.");
    }
}

    public void DisplayEndingScreen(int totalTrials, float totalTime, int totalMissedClicks, int highestStreak)
    {
        thankYouText.text = "Thank you for participating!";
        performanceSummaryText.text = $"Total Trials: {totalTrials}\n" +
                                      $"Total Time: {totalTime:F2} seconds\n" +
                                      $"Total Missed Clicks: {totalMissedClicks}\n" +
                                      $"Highest Streak: {highestStreak}";
                                        gameObject.SetActive(true);
    Debug.Log("Ending screen displayed.");
    }

    public void ExitApplication()
    {
        Debug.Log("ExitApplication called.");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void RestartExperiment()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

