using UnityEngine;

public class PointCursor : MonoBehaviour
{
    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        // Get mouse position in world coordinates
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z += 9f; // Ensure the cursor isn't occluded by the camera
        transform.position = mainCam.ScreenToWorldPoint(mousePosition);

        // Handle clicks
        HandleMouseClick();
    }

    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.TryGetComponent(out Target target))
                {
                    target.OnSelect(); // Select the target when clicked
                }
            }
        }
    }
}
