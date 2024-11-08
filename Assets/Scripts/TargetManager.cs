using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private GameObject whiteTargetPrefab;
    [SerializeField] private GameObject redTargetPrefab;
    [SerializeField] private float spacingBetweenDistractors = 1.0f;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void InitializeTrial(float amplitude, float targetSize, float EWToW_Ratio, int numberOfWhiteTargets)
    {
        Debug.Log($"Initializing trial with Amplitude: {amplitude}, Target Size: {targetSize}, EW/W Ratio: {EWToW_Ratio}, White Targets: {numberOfWhiteTargets}");

        List<Vector3> targetPositions = GenerateTargetPositions(amplitude, numberOfWhiteTargets + 1); // Include red target
        int redTargetPositionIndex = Random.Range(0, targetPositions.Count);

        for (int i = 0; i < targetPositions.Count; i++)
        {
            GameObject targetInstance;

            if (i == redTargetPositionIndex)
            {
                targetInstance = SpawnRedTarget(targetPositions[i], targetSize);
                PlaceDistractorsAroundTarget(targetInstance.transform.position, targetSize, EWToW_Ratio);
            }
            else
            {
                targetInstance = SpawnWhiteTarget(targetPositions[i], targetSize);
            }
        }
    }

    private GameObject SpawnRedTarget(Vector3 position, float targetSize)
    {
        GameObject redTarget = Instantiate(redTargetPrefab, position, Quaternion.identity, transform);
        redTarget.tag = "Target";
        redTarget.transform.localScale = Vector3.one * targetSize;

        if (redTarget.TryGetComponent(out Target targetComponent))
        {
            targetComponent.IsRedTarget = true;
            redTarget.GetComponent<SpriteRenderer>().color = Color.red;
        }

        return redTarget;
    }

    private GameObject SpawnWhiteTarget(Vector3 position, float targetSize)
    {
        GameObject whiteTarget = Instantiate(whiteTargetPrefab, position, Quaternion.identity, transform);
        whiteTarget.tag = "Target";
        whiteTarget.transform.localScale = Vector3.one * targetSize;

        return whiteTarget;
    }

    private void PlaceDistractorsAroundTarget(Vector3 redTargetPosition, float targetSize, float EWToW_Ratio)
    {
        Vector3[] positions = {
            redTargetPosition + new Vector3(-spacingBetweenDistractors * EWToW_Ratio, 0f, 0f),
            redTargetPosition + new Vector3(spacingBetweenDistractors * EWToW_Ratio, 0f, 0f),
            redTargetPosition + new Vector3(0f, spacingBetweenDistractors * EWToW_Ratio, 0f),
            redTargetPosition + new Vector3(0f, -spacingBetweenDistractors * EWToW_Ratio, 0f)
        };

        foreach (Vector3 pos in positions)
        {
            GameObject distractor = Instantiate(whiteTargetPrefab, pos, Quaternion.identity, transform);
            distractor.tag = "Target";
            distractor.transform.localScale = Vector3.one * targetSize;
        }
    }

    private List<Vector3> GenerateTargetPositions(float amplitude, int totalTargets)
    {
        List<Vector3> positions = new List<Vector3>();
        float minSpacing = 1.0f;
        int maxAttempts = 100;

        Vector3 screenBottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 10));
        Vector3 screenTopRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 10));

        float screenLeft = screenBottomLeft.x;
        float screenRight = screenTopRight.x;
        float screenTop = screenTopRight.y;
        float screenBottom = screenBottomLeft.y;

        for (int i = 0; i < totalTargets; i++)
        {
            bool validPositionFound = false;
            int attempts = 0;

            while (!validPositionFound && attempts < maxAttempts)
            {
                attempts++;

                float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                float distance = Random.Range(amplitude / 2f, amplitude);
                Vector3 offset = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * distance;

                float randomX = Random.Range(screenLeft + minSpacing, screenRight - minSpacing);
                float randomY = Random.Range(screenBottom + minSpacing, screenTop - minSpacing);
                Vector3 potentialPosition = new Vector3(randomX, randomY, 0) + offset;

                if (!IsPositionTooClose(potentialPosition, positions, minSpacing))
                {
                    positions.Add(potentialPosition);
                    validPositionFound = true;
                    Debug.Log($"Position {i} placed at {potentialPosition} after {attempts} attempts.");
                }
            }

            if (!validPositionFound)
            {
                Debug.LogWarning($"Failed to place position {i} after {maxAttempts} attempts. Consider adjusting amplitude or spacing.");
            }
        }

        return positions;
    }

    private bool IsPositionTooClose(Vector3 newPosition, List<Vector3> existingPositions, float minSpacing)
    {
        foreach (Vector3 existingPosition in existingPositions)
        {
            if (Vector3.Distance(newPosition, existingPosition) < minSpacing)
            {
                return true;
            }
        }
        return false;
    }
}

