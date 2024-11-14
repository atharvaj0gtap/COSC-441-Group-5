using System.Collections.Generic;
using UnityEngine;

public class MovingTarget : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 2.0f; // Base speed for moving targets
    private float speed; // Effective speed, adjusted for difficulty
    private List<Vector3> pathPoints; // Control points for the spline
    [SerializeField] private float rotationSpeed = 50f; // Optional rotation speed for added visual effect
    private float t = 0f; // Parameter for interpolation along the spline
    private int numSections;
    private int currentSection = 0;

    void Start()
    {
        // speed = baseSpeed; // Remove this line to prevent overwriting the speed set by SetSpeed()
        Debug.Log("MovingTarget initialized.");

        // Set up path points; if missing, generate fallback path
        if (pathPoints == null || pathPoints.Count == 0)
        {
            Debug.LogWarning("MovingTarget has no path points set. Using fallback path.");
            pathPoints = GenerateFallBackPath();
        }

        numSections = pathPoints.Count - 3; // Number of spline sections
        if (numSections < 1)
        {
            Debug.LogError("Not enough points to create a spline. Need at least 4 points.");
        }
    }

    void Update()
    {
        if (pathPoints != null && pathPoints.Count >= 4)
        {
            // Move along the spline
            t += (speed / 10f) * Time.deltaTime; // Adjust the divisor to control speed along the spline

            if (t > 1f)
            {
                t = 0f;
                currentSection = (currentSection + 1) % numSections;
            }

            Vector3 newPosition = CatmullRomPosition(currentSection, t);
            transform.position = newPosition;

            // Optional: Rotate the target for effect
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
    }

    public void SetPathPoints(List<Vector3> points)
    {
        pathPoints = points;

        // Ensure there are enough points for Catmull-Rom spline
        if (pathPoints.Count < 4)
        {
            // Duplicate the last point to have at least 4 points
            while (pathPoints.Count < 4)
            {
                pathPoints.Add(pathPoints[pathPoints.Count - 1]);
            }
        }

        numSections = pathPoints.Count - 3; // Update the number of sections
    }

    public void SetSpeed(float speedMultiplier)
    {
        speed = baseSpeed * speedMultiplier;
        Debug.Log($"Speed set to: {speed}");
    }

    // Catmull-Rom spline interpolation
    private Vector3 CatmullRomPosition(int section, float t)
    {
        Vector3 p0 = pathPoints[section];
        Vector3 p1 = pathPoints[section + 1];
        Vector3 p2 = pathPoints[section + 2];
        Vector3 p3 = pathPoints[section + 3];

        // Catmull-Rom spline formula
        float t2 = t * t;
        float t3 = t2 * t;

        Vector3 position = 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );

        return position;
    }

    // Generate a fallback path in case path points are missing
    private List<Vector3> GenerateFallBackPath()
    {
        List<Vector3> path = new List<Vector3>();

        // Start with the current position
        path.Add(transform.position);

        // Generate random points for path
        for (int i = 0; i < 5; i++) // Need at least 4 points for Catmull-Rom
        {
            path.Add(new Vector3(Random.Range(-8, 8), Random.Range(-4, 4), 0));
        }

        return path;
    }
}
