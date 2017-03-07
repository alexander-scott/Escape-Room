using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiagnosticsController : MonoBehaviour
{
    public float rotationSpeed;

    [Space(10)]

    public GameObject submarine;
    public Text engineResponseLabel;
    public Text errorLabel;
    public Text taskLabel;

	void Start()
    {
        StartCoroutine(RotateSub());

        StartCoroutine(FlashErrorText(0.2f, 0.2f));
    }

    public void ToggleUI(bool success)
    {
        if (success)
        {
            errorLabel.gameObject.SetActive(false);
            taskLabel.gameObject.SetActive(false);

            engineResponseLabel.text = "ON";
        }
        else
        {
            errorLabel.gameObject.SetActive(false);
            taskLabel.gameObject.SetActive(false);

            engineResponseLabel.text = "OFF";
        }
    }

    private IEnumerator RotateSub()
    {
        while (true)
        {
            submarine.transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));

            yield return null;
        }
    }

    private IEnumerator FlashErrorText(float timeFadedIn, float timeFadedOut)
    {
        while (true)
        {
            // Fade in
            StartCoroutine(FadeInSingleText(errorLabel, Color.red, timeFadedIn));

            // Time to wait faded in
            yield return new WaitForSeconds(timeFadedIn);

            // Fade out
            StartCoroutine(FadeOutSingleText(errorLabel, timeFadedOut, false));

            // Time to wait faded out
            yield return new WaitForSeconds(timeFadedOut);
        }
    }

    private static IEnumerator FadeInSingleText(Text text, Color32 targetColor, float duration)
    {
        float smoothness = 0.02f;
        float progress = 0; // This float will serve as the 3rd parameter of the lerp function.
        float increment = smoothness / duration; // The amount of change to apply.

        // If its been deactivated by the FadeOutSingleIcon coroutine it'll be inactive in the hierarchy so reactivate it now
        if (!text.gameObject.activeInHierarchy)
        {
            text.gameObject.SetActive(true);
            text.color = Color.clear;
        }

        Color32 startColour = text.color;

        while (progress < 1)
        {
            text.color = Color32.Lerp(startColour, targetColor, progress);

            progress += increment;
            yield return new WaitForSeconds(smoothness);
        }
    }

    private static IEnumerator FadeOutSingleText(Text text, float duration, bool deactivate)
    {
        float smoothness = 0.02f;
        float progress = 0; // This float will serve as the 3rd parameter of the lerp function.
        float increment = smoothness / duration; // The amount of change to apply.

        Color32 startColour = text.color;

        while (progress < 1)
        {
            text.color = Color32.Lerp(startColour, Color.clear, progress);

            progress += increment;
            yield return new WaitForSeconds(smoothness);
        }

        if (deactivate)
        {
            text.gameObject.SetActive(false);
        }
    }
}
