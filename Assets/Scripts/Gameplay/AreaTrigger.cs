using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class AreaTrigger : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI levelClearText;   // Assign in Canvas (TMP text)
    public TextMeshProUGUI doorUnlockedText; // Assign in Canvas (TMP text for "door unlocked")
    public SceneFader sceneFader;

    [Header("Transition Settings")]
    public float delayBeforeLoad = 2f;       // Time before loading next level

    private bool isUnlocked = false;

    void Start()
    {
        // Register this area trigger in GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.RegisterAreaTrigger(this);

        if (levelClearText)
            levelClearText.gameObject.SetActive(false);

        if (doorUnlockedText)
            doorUnlockedText.gameObject.SetActive(false);
    }

    // Called by GameManager when all enemies are dead
    public void UnlockDoor()
    {
        if (isUnlocked) return;
        isUnlocked = true;

        Debug.Log("Door is unlocked! You can go to the next area.");

        // Show "door unlocked" message
        if (doorUnlockedText)
        {
            StartCoroutine(ShowTemporaryText(
                doorUnlockedText,
                "<size=32><color=#FFD700><b>The door is unlocked! Please head up.</b></color></size>",
                5f
            ));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isUnlocked) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered area trigger → showing Level Cleared!");

            if (levelClearText)
            {
                SoundManager.Instance?.PlayLevelClear();
                levelClearText.gameObject.SetActive(true);
                levelClearText.text = "<size=60><color=#00FF00><b>LEVEL CLEARED!</b></color></size>";
            }

            StartCoroutine(LoadNextSceneAfterDelay());
        }
    }

    private IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoad);

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"Loading next level: Index {nextIndex}");
            string nextSceneName = SceneUtility.GetScenePathByBuildIndex(nextIndex);
            nextSceneName = System.IO.Path.GetFileNameWithoutExtension(nextSceneName);
            sceneFader.FadeToScene(nextSceneName);
        }
        else
        {
            Debug.Log("No more levels left — game complete!");
            if (levelClearText)
                levelClearText.text = "<size=60><color=#FFD700><b>CONGRATULATIONS! YOU CLEARED ALL LEVELS!</b></color></size>";
        }
    }

    private IEnumerator ShowTemporaryText(TextMeshProUGUI textUI, string message, float duration)
    {
        textUI.gameObject.SetActive(true);
        textUI.text = message;
        yield return new WaitForSeconds(duration);
        textUI.gameObject.SetActive(false);
    }
}
