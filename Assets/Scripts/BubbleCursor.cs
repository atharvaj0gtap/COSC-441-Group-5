using System.Collections.Generic;
using UnityEngine;

public class BubbleCursor : MonoBehaviour
{
    [SerializeField] private float maxRadius = 3.0f;
    [SerializeField] private float minRadius = 1.0f;
    [SerializeField] private ContactFilter2D contactFilter;
    [SerializeField] private SpriteRenderer bubbleSprite;
    [SerializeField] private SpriteRenderer innerRingSprite; // Inner ring for dual-layer feedback
    [SerializeField] private Color hoverColor = Color.yellow; // Color when hovering over the goal target
    [SerializeField] private Color defaultColor = Color.white; // Default color of the bubble cursor
    [SerializeField] private Color nonGoalHoverColor = Color.red; // Color when hovering over a non-goal target
    [SerializeField] private Color targetInRangeColor = Color.green; // Color when a target is fully within range
    [SerializeField] private AudioSource audioSource; // AudioSource component
    [SerializeField] private AudioClip correctSound; // Sound for correct goal target
    [SerializeField] private AudioClip bombSound; // Sound for non-goal target
    [SerializeField] private Color correctClickColor = Color.green;  // Color for correct click feedback
    [SerializeField] private Color wrongClickColor = Color.red;  // Color for incorrect click feedback


    private Camera mainCam;
    private List<Collider2D> results = new List<Collider2D>();
    private Collider2D previousDetectedCollider = null;
    private Collider2D closestCollider = null;
    private Material innerRingMaterial;
    private GameManager gameManager;

    void Awake()
    {
        mainCam = Camera.main;
        gameManager = FindObjectOfType<GameManager>(); // Reference to GameManager to update streak

        if (bubbleSprite == null)
        {
            Debug.LogError("SpriteRenderer component missing for the BubbleCursor!");
        }

        if (innerRingSprite == null)
        {
            Debug.LogError("SpriteRenderer component missing for the Inner Ring!");
        }
        else
        {
            innerRingMaterial = innerRingSprite.material;
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component missing for the BubbleCursor!");
        }
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f;
        transform.position = mainCam.ScreenToWorldPoint(mousePosition);

        Physics2D.OverlapCircle(transform.position, maxRadius, contactFilter, results);

        if (results.Count > 0)
        {
            closestCollider = FindClosestTarget();
            AdjustRadius();
            UnHoverPreviousTarget(closestCollider);
            HoverTarget(closestCollider);
        }
        else
        {
            UnHoverPreviousTarget();
            bubbleSprite.color = defaultColor; // Reset to default color when no target is detected
            innerRingMaterial.SetFloat("_FillAmount", 0); // Reset inner ring fill amount
            innerRingMaterial.color = defaultColor; // Reset inner ring color
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }

        previousDetectedCollider = closestCollider;
    }

    private void HandleClick()
    {
        // Check if a target is within the minimum radius on click
        
        if (closestCollider != null && Vector3.Distance(transform.position, closestCollider.transform.position) <= minRadius)
        {
            Debug.Log($"Attempting to select target: {closestCollider.gameObject.name} with tag: {closestCollider.gameObject.tag}");
            SelectTarget(closestCollider);
        }
    }

    private void AdjustRadius()
    {
        if (closestCollider != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, closestCollider.transform.position);
            float newRadius = Mathf.Clamp(distanceToTarget, minRadius, maxRadius);
            bubbleSprite.transform.localScale = new Vector3(newRadius, newRadius, 1f);

            // Adjust inner ring fill amount based on distance to target
            float fillAmount = 1 - (distanceToTarget / maxRadius);
            innerRingMaterial.SetFloat("_FillAmount", Mathf.Clamp01(fillAmount));
        }
    }

    private Collider2D FindClosestTarget()
    {
        Collider2D closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (var collider in results)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = collider;
            }
        }

        return closest;
    }

    private void HoverTarget(Collider2D collider)
    {
        if (collider.TryGetComponent(out Target target))
        {
            if (target.IsRedTarget)
            {
                bubbleSprite.color = hoverColor; // Change color when hovering over the goal target
                innerRingMaterial.color = hoverColor; // Change inner ring color when hovering over the goal target
            }
            else
            {
                bubbleSprite.color = nonGoalHoverColor; // Change color when hovering over a non-goal target
                innerRingMaterial.color = nonGoalHoverColor; // Change inner ring color when hovering over a non-goal target
            }

            // Change color to green if the target is fully within range
            if (Vector3.Distance(transform.position, collider.transform.position) <= minRadius)
            {
                bubbleSprite.color = targetInRangeColor;
                innerRingMaterial.color = targetInRangeColor;
            }

            target.OnHoverEnter();
        }
    }

    private void UnHoverPreviousTarget()
    {
        if (previousDetectedCollider != null && previousDetectedCollider.TryGetComponent(out Target t))
        {
            t.OnHoverExit();
        }
    }

    private void UnHoverPreviousTarget(Collider2D collider)
    {
        if (previousDetectedCollider != null && collider != previousDetectedCollider && previousDetectedCollider.TryGetComponent(out Target t))
        {
            t.OnHoverExit();
        }
    }

    private void SelectTarget(Collider2D collider)
    {
    if (collider != null && collider.TryGetComponent(out Target target))
        {
            // Check if the selected target is the correct red target
            bool isCorrectTarget = target.IsRedTarget;

            if (isCorrectTarget)
            {
                Debug.Log("IncrementStreak called from BubbleCursor.");
                gameManager.OnCorrectTargetSelected();  // signal gamemanager
                PlayClickSound(true); // Play correct sound for red target
                innerRingSprite.color = correctClickColor; // Set inner circle to correct color

            }
            else
            {
                gameManager.ResetStreak(); // Reset streak if incorrect target is selected
                PlayClickSound(false); // Play bomb sound for incorrect selection
                innerRingSprite.color = wrongClickColor; // Set inner circle to incorrect color

            }

            target.OnSelect(); // Mark the target as selected
        }
    }
    

    

    private void PlayClickSound(bool isGoalTarget)
    {
        if (audioSource != null)
        {
            audioSource.clip = isGoalTarget ? correctSound : bombSound;
            audioSource.Play();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maxRadius);
    }
}

