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

  public void OnSelect()
{
    if (isSelected || sprite == null || studyBehavior == null) return;

    isSelected = true;

    if (IsRedTarget)
    {
        sprite.color = Color.green; // Change color to indicate correct selection
        StartCoroutine(DestroyAndProceed(0.5f));
    }
    else
    {
        sprite.color = Color.red; // Change color to indicate incorrect (missed) selection
        studyBehavior.HandleMissedClick(); // Only count as missed click for non-red targets
        StartCoroutine(DestroyAfterDelay(0.5f));
    }
}


    public void OnHoverEnter()
    {
        if (!isSelected && !IsRedTarget && sprite != null)
        {
            sprite.color = Color.yellow; // Highlight the target
        }
    }

    public void OnHoverExit()
    {
        if (!isSelected && !IsRedTarget && sprite != null)
        {
            sprite.color = Color.white; // Reset the color of the non-red target
        }
    }

    private IEnumerator DestroyAndProceed(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
        studyBehavior.NextTrial();
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
