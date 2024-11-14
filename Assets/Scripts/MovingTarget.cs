using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTarget : MonoBehaviour
{
    [SerializeField] private float initialSpeed = 1.0f;
    [SerializeField] private Vector2[] pathPoints; // Define points for the target's path
    
    private int currentPointIndex = 0;
    private float currentSpeed;


    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = initialSpeed; 
    }

    // Update is called once per frame
    void Update()
    {
        if (pathPoints.Length > 0)
        {
            MoveAlongPath();
        }

        //IncreaseDifficulty();
    }

    private void MoveAlongPath()
    {
        // Move target toward the next point in pathPoints
            Vector3 targetPosition = pathPoints[currentPointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

        // Check if target reached the point
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
            // Cycle to next point in path
            currentPointIndex = (currentPointIndex + 1) % pathPoints.Length;
        }
    }

    // private void IncreaseDifficulty()
    // {
    //     // Increase speed gradually over time
    //     currentSpeed += difficultyIncreaseRate * Time.deltaTime;
    // }

    public void SetPathPoints(Vector2[] points)
    {
        // Set path points externally
        pathPoints = points;
    }
}
