using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private GameObject redTargetPrefab;
    [SerializeField] private GameObject movingTargetPrefab; // Add reference to moving target prefab

    private int distractorMultiplier = 1; // Base multiplier
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void SetDistractorMultiplier(int multiplier)
    {
        distractorMultiplier = multiplier;
    }

    public void SetupTrial(float amplitude, float targetSize, float EWToW_Ratio, int numberOfWhiteTargets, bool includeMovingTargets, int currentLevel)
    {
        int adjustedDistractors = numberOfWhiteTargets * distractorMultiplier;
        Debug.Log($"Setting up trial with Amplitude: {amplitude}, Target Size: {targetSize}, EW/W: {EWToW_Ratio}, Extra White Targets: {adjustedDistractors}");
        
        List<Vector3> points = GenerateRandomPoints(amplitude, adjustedDistractors + 1); // +1 for red target
        int redTargetIndex = Random.Range(0, points.Count);

        for (int i = 0; i < points.Count; i++)
        {
            GameObject targetObject;
            
            if (i == redTargetIndex)
            {
                // spawn red main target
                targetObject = Instantiate(redTargetPrefab, points[i], Quaternion.identity, transform);
                targetObject.tag = "Target";
                targetObject.transform.localScale = Vector3.one * targetSize;
                
                var targetScript = targetObject.GetComponent<Target>();
                if (targetScript != null)
                {
                    targetScript.IsRedTarget = true;
                    Debug.Log($"Red target instantiated. IsRedTarget: {targetScript.IsRedTarget}");
                }
            }
            else if (includeMovingTargets) 
            {
            // For levels above 5, spawn moving target distractors
            targetObject = Instantiate(movingTargetPrefab, points[i], Quaternion.identity, transform);
            targetObject.tag = "MovingTarget";
            targetObject.transform.localScale = Vector3.one * targetSize;
            
            // Set IsRedTarget to false
            var targetScript = targetObject.GetComponent<Target>();
            if (targetScript != null)
            {
                targetScript.IsRedTarget = false;
            }
            var movingTargetScript = targetObject.GetComponent<MovingTarget>();
            if (movingTargetScript != null)
            {
                List<Vector3> path = GenerateRandomPath(amplitude);
                movingTargetScript.SetPathPoints(path); 
                movingTargetScript.SetSpeed(1 * Mathf.Pow(1.1f, currentLevel));
            }
                }
                else
                {
                // spawn regular white distractor targets for levels 5 and below
                targetObject = Instantiate(targetPrefab, points[i], Quaternion.identity, transform);
                targetObject.tag = "Target";
                targetObject.transform.localScale = Vector3.one * targetSize;

                var targetScript = targetObject.GetComponent<Target>();
                if (targetScript != null)
                {
                    targetScript.IsRedTarget = false;
                    Debug.Log($"White target instantiated. IsRedTarget: {targetScript.IsRedTarget}");
                }
            }
        }
    }

    private List<Vector3> GenerateRandomPoints(float amplitude, int numberOfPoints)
    {
        List<Vector3> points = new List<Vector3>();
        float minimumSpacing = 1.0f;
        int maxAttemptsPerPoint = 100;
        Vector3 screenBottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 10));
        Vector3 screenTopRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 10));
        float screenLeft = screenBottomLeft.x;
        float screenRight = screenTopRight.x;
        float screenTop = screenTopRight.y;
        float screenBottom = screenBottomLeft.y;

        for (int i = 0; i < numberOfPoints; i++)
        {
            bool positionFound = false;
            int attempts = 0;
            while (!positionFound && attempts < maxAttemptsPerPoint)
            {
                attempts++;
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                float randomDistance = Random.Range(amplitude / 2f, amplitude);
                Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * randomDistance;
                float randomX = Random.Range(screenLeft + minimumSpacing, screenRight - minimumSpacing);
                float randomY = Random.Range(screenBottom + minimumSpacing, screenTop - minimumSpacing);
                Vector3 position = new Vector3(randomX, randomY, 0) + offset;

                bool tooClose = false;
                foreach (Vector3 existingPoint in points)
                {
                    if (Vector3.Distance(position, existingPoint) < minimumSpacing)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (!tooClose)
                {
                    points.Add(position);
                    positionFound = true;
                    Debug.Log($"Point {i} placed at {position} after {attempts} attempts.");
                }
            }

            if (!positionFound)
            {
                Debug.LogWarning($"Could not place point {i} after {maxAttemptsPerPoint} attempts.");
            }
        }

        return points;
    }
    
    // Helper function to generate a random path for moving targets
    private List<Vector3> GenerateRandomPath(float amplitude, int points = 5)
{
    List<Vector3> path = new List<Vector3>();
    Vector3 screenBottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
    Vector3 screenTopRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.nearClipPlane));
    float padding = 0f;
    for (int i = 0; i < points; i++)
    {
        Vector3 randomPoint = new Vector3(
            Random.Range(screenBottomLeft.x + padding, screenTopRight.x - padding),
            Random.Range(screenBottomLeft.y + padding, screenTopRight.y - padding),
            0);
        path.Add(randomPoint);
    }
    return path;
    }
}
