using UnityEngine;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour
{
    public static ScreenFlash instance;

    public Image flashImage;
    public float flashDuration = 0.15f;

    void Awake()
    {
        instance = this;
        flashImage.color = new Color(1, 1, 1, 0);
    }

    public void Flash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    System.Collections.IEnumerator FlashRoutine()
    {
        float t = 0;

        while (t < flashDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0, 1, t / flashDuration);
            flashImage.color = new Color(1, 1, 1, a);
            yield return null;
        }

        t = 0;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1, 0, t / flashDuration);
            flashImage.color = new Color(1, 1, 1, a);
            yield return null;
        }

        flashImage.color = new Color(1, 1, 1, 0);
    }
}
