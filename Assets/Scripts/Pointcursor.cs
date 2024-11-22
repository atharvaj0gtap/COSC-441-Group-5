using UnityEngine;

public class PointCursor : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // AudioSource component
    [SerializeField] private AudioClip correctSound; // Sound for correct goal target
    [SerializeField] private AudioClip bombSound; // Sound for non-goal target

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

            if (hit.collider != null && hit.collider.TryGetComponent(out Target target))
        {
            bool isGoalTarget = target.IsRedTarget; // Determine if the target is a goal target
            PlayClickSound(isGoalTarget);
            target.OnSelect(); // Perform target selection logic
        }
        else
        {
            PlayClickSound(false); // Play non-goal sound for invalid clicks
        
            }
        }
    }
    private void PlayClickSound(bool isGoalTarget)
    {
    AudioClip clipToPlay = isGoalTarget ? correctSound : bombSound;
    if (clipToPlay != null)
        {
        AudioSource.PlayClipAtPoint(clipToPlay, transform.position);
        }
    else
        {
        Debug.LogError("No audio clip assigned for PlayClickSound.");
        }
    }
}
