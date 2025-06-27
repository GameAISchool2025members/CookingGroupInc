using System.Collections;
using UnityEngine;

public class MenuHighliter : MonoBehaviour
{
    // The scale factor to apply when the mouse hovers over the button.
    [Tooltip("The scale factor to apply when the mouse hovers over the button.")]
    public float hoverScaleFactor = 1.1f;

    // The duration of the scaling animation.
    [Tooltip("The duration of the scaling animation.")]
    public float animationDuration = 0.1f;


    private Vector3 originalScale; // Stores the initial scale of the button.
    private Coroutine currentScaleCoroutine;


    void Awake()
    {
        // Store the original scale of the button when the script starts.
        originalScale = transform.localScale;
    }
    public void OnEnter()
    {
        if (currentScaleCoroutine != null)
        {
            StopCoroutine(currentScaleCoroutine);
        }
        // Start the coroutine to scale up the button.
        currentScaleCoroutine = StartCoroutine(ScaleButton(originalScale * hoverScaleFactor));
    }

    public void OnExit() 
    {
        if (currentScaleCoroutine != null)
        {
            StopCoroutine(currentScaleCoroutine);
        }
        // Start the coroutine to scale down the button.
        currentScaleCoroutine = StartCoroutine(ScaleButton(originalScale));
    }

    public void QuitGame()
    {
        Application.Quit();
    }



    private IEnumerator ScaleButton(Vector3 targetScale)
    {
        Vector3 startScale = transform.localScale; // The scale at the start of the animation.
        float time = 0; // Timer for the animation duration.

        // Loop while the animation duration has not been met.
        while (time < animationDuration)
        {
            // Increment time based on the time since the last frame.
            time += Time.deltaTime;
            // Calculate the interpolation factor (0 to 1).
            float lerpFactor = time / animationDuration;
            // Smoothly interpolate between the start and target scales using Lerp.
            transform.localScale = Vector3.Lerp(startScale, targetScale, lerpFactor);
            // Wait for the next frame.
            yield return null;
        }
        // Ensure the button reaches the exact target scale at the end.
        transform.localScale = targetScale;
    }
}
