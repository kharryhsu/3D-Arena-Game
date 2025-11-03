using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;
    public string sceneToLoad;

    void Start()
    {
        // Start with fade-in effect
        StartCoroutine(FadeIn());
    }

    public void FadeToScene(string sceneName)
    {
        sceneToLoad = sceneName;
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeIn()
    {
        float t = fadeDuration;
        while (t > 0)
        {
            t -= Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.blocksRaycasts = false;
    }

    IEnumerator FadeOut()
    {
        fadeCanvasGroup.blocksRaycasts = true;
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}
