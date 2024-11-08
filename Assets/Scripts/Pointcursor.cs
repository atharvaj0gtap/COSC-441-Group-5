using UnityEngine;

public class PointCursor : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        UpdateCursorPosition();
        ProcessMouseClick();
    }

    private void UpdateCursorPosition()
    {
        // Update the cursor's position based on the mouse's position in world coordinates
        Vector3 cursorPosition = Input.mousePosition;
        cursorPosition.z = 9f;  // Offset to ensure it appears in front of the camera
        transform.position = mainCamera.ScreenToWorldPoint(cursorPosition);
    }

    private void ProcessMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DetectAndSelectTarget();
        }
    }

    private void DetectAndSelectTarget()
    {
        Vector3 clickPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hitInfo = Physics2D.Raycast(clickPosition, Vector2.zero);

        if (hitInfo.collider != null && hitInfo.collider.TryGetComponent(out Target target))
        {
            target.OnSelect();  // Invoke the target's select method on click
        }
    }
}


