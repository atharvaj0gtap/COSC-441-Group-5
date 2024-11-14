using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    private SpriteRenderer sprite;
    private bool isSelected = false;
    private StudyBehavior studyBehavior;

    public bool IsRedTarget { get; set; }

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        studyBehavior = FindObjectOfType<StudyBehavior>();

        if (sprite == null)
        {
            Debug.LogError("SpriteRenderer component missing on Target!");
        }

        if (studyBehavior == null)
        {
            Debug.LogError("StudyBehavior not found in the scene.");
        }
    }

    public bool OnSelect()
    {
        if (isSelected || sprite == null || studyBehavior == null) return false;

        isSelected = true;
            Debug.Log($"Target selected. IsRedTarget: {IsRedTarget}"); // Add this debug line

        if (IsRedTarget)
        {
            sprite.color = Color.green;
            StartCoroutine(DestroyAndProceed(0.5f));
            return true;
        }
        else
        {
            sprite.color = Color.red;
            studyBehavior.HandleMissedClick();
            StartCoroutine(DestroyAfterDelay(0.5f));
            return false;
        }
    }

    public void OnHoverEnter()
    {
        if (!isSelected && !IsRedTarget && sprite != null)
        {
            sprite.color = Color.yellow;
        }
    }

    public void OnHoverExit()
    {
        if (!isSelected && !IsRedTarget && sprite != null)
        {
            sprite.color = Color.white;
        }
    }

    private IEnumerator DestroyAndProceed(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
        Debug.Log("Proceeding to next trial.");
        studyBehavior.NextTrial();
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
