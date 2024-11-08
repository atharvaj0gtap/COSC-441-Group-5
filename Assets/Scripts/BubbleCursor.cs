using System.Collections.Generic;
using UnityEngine;

public class BubbleCursor : MonoBehaviour
{
    [Header("Bubble Cursor Settings")]
    [SerializeField] private float maximumRadius = 2.8f;
    [SerializeField] private float minimumRadius = 1.0f;
    [SerializeField] private ContactFilter2D targetFilter;
    [SerializeField] private SpriteRenderer cursorSpriteRenderer;

    private Camera mainCamera;
    private List<Collider2D> detectedTargets = new List<Collider2D>();
    private Collider2D lastHoveredTarget = null;
    private Collider2D nearestTarget = null;

    private void Start()
    {
        mainCamera = Camera.main;
        if (cursorSpriteRenderer == null)
        {
            Debug.LogError("BubbleCursor: Missing SpriteRenderer reference!");
        }
    }

    private void Update()
    {
        FollowMousePosition();
        DetectTargetsInRange();
        HandleTargetInteraction();
    }

    private void FollowMousePosition()
    {
        Vector3 cursorPosition = Input.mousePosition;
        cursorPosition.z = 10f;  // Ensure the cursor is placed correctly in the world
        transform.position = mainCamera.ScreenToWorldPoint(cursorPosition);
    }

    private void DetectTargetsInRange()
    {
        Physics2D.OverlapCircle(transform.position, maximumRadius, targetFilter, detectedTargets);

        if (detectedTargets.Count > 0)
        {
            nearestTarget = GetClosestTarget();
            AdjustCursorRadius();
            UpdateHoveredTarget(nearestTarget);
        }
        else
        {
            ResetHoveredTarget();
        }
    }

    private void HandleTargetInteraction()
    {
        if (Input.GetMouseButtonDown(0) && nearestTarget != null)
        {
            SelectTarget(nearestTarget);
        }

        lastHoveredTarget = nearestTarget;  // Save the current hovered target for comparison in the next frame
    }

    private Collider2D GetClosestTarget()
    {
        Collider2D closest = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider2D target in detectedTargets)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (distanceToTarget < shortestDistance)
            {
                shortestDistance = distanceToTarget;
                closest = target;
            }
        }

        return closest;
    }

    private void AdjustCursorRadius()
    {
        if (nearestTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, nearestTarget.transform.position);
            float adjustedRadius = Mathf.Clamp(distanceToTarget, minimumRadius, maximumRadius);
            cursorSpriteRenderer.transform.localScale = Vector3.one * adjustedRadius;
        }
    }

    private void UpdateHoveredTarget(Collider2D newTarget)
    {
        if (lastHoveredTarget != null && newTarget != lastHoveredTarget)
        {
            ResetHoveredTarget();  // Reset the last hovered target if it's no longer the closest
        }

        if (newTarget != null && newTarget.TryGetComponent(out Target target))
        {
            if (!target.IsRedTarget)
            {
                target.OnHoverEnter();
            }
        }
    }

    private void ResetHoveredTarget()
    {
        if (lastHoveredTarget != null && lastHoveredTarget.TryGetComponent(out Target target))
        {
            if (!target.IsRedTarget)
            {
                target.OnHoverExit();
            }
        }
    }

    private void SelectTarget(Collider2D targetCollider)
    {
        if (targetCollider != null && targetCollider.TryGetComponent(out Target target))
        {
            target.OnSelect();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maximumRadius);  // Visualize the detection range of the bubble cursor
    }
}
