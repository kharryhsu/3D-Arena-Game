using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Score UI")]
    public TextMeshProUGUI scoreText;
    private int score = 0;

    [Header("Floating Text")]
    public GameObject floatingTextPrefab;

    [Header("Booster UI")]
    public TextMeshProUGUI boosterTimerText;

    [Header("Enemy & Door System")]
    private AreaTrigger areaTrigger;
    private DoorMechanism doorMechanism;
    private List<EnemyBase> activeEnemies = new List<EnemyBase>();

    [Header("Enemies Remaining UI")]
    public TextMeshProUGUI enemiesRemainingText;

    private Coroutine boosterCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameManager] Scene Loaded: {scene.name}");

        // Find UI by tag if not assigned
        if (!scoreText)
            scoreText = GameObject.FindWithTag("ScoreText")?.GetComponent<TextMeshProUGUI>();

        if (!boosterTimerText)
            boosterTimerText = GameObject.FindWithTag("BoosterText")?.GetComponent<TextMeshProUGUI>();

        if (!enemiesRemainingText)
            enemiesRemainingText = GameObject.FindWithTag("EnemiesText")?.GetComponent<TextMeshProUGUI>();

        // Hide booster text at the start of every scene
        if (boosterTimerText)
            boosterTimerText.gameObject.SetActive(false);

        // Find area and door in new scene
        areaTrigger = FindObjectOfType<AreaTrigger>();
        doorMechanism = FindObjectOfType<DoorMechanism>();

        // Update UI with current saved data
        UpdateScoreUI();
        UpdateEnemiesRemainingUI();
    }

    // --- REGISTER OBJECTS ---
    public void RegisterAreaTrigger(AreaTrigger trigger) => areaTrigger = trigger;
    public void RegisterDoorMechanism(DoorMechanism door) => doorMechanism = door;

    // --- ENEMY TRACKING ---
    public void RegisterEnemy(EnemyBase enemy)
    {
        if (!activeEnemies.Contains(enemy))
            activeEnemies.Add(enemy);
        UpdateEnemiesRemainingUI();
    }

    public void UnregisterEnemy(EnemyBase enemy)
    {
        if (activeEnemies.Contains(enemy))
            activeEnemies.Remove(enemy);
        UpdateEnemiesRemainingUI();

        if (activeEnemies.Count == 0)
        {
            Debug.Log("All enemies defeated! Unlocking door...");
            doorMechanism?.HideDoor();
            areaTrigger?.UnlockDoor();
        }
    }

    private void UpdateEnemiesRemainingUI()
    {
        if (enemiesRemainingText != null)
        {
            enemiesRemainingText.text = activeEnemies.Count > 0
                ? $"Enemies Remaining: {activeEnemies.Count}"
                : "<color=green><b>All enemies defeated!</b></color>";
        }
    }

    // --- SCORE SYSTEM ---
    public void AddScore(int amount, Vector3 position = default)
    {
        score += amount;
        UpdateScoreUI();

        if (floatingTextPrefab && position != default)
        {
            GameObject ft = Instantiate(floatingTextPrefab, position, Quaternion.identity);
            ft.GetComponent<FloatingText>().SetText($"+{amount}", Color.yellow);
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText)
            scoreText.text = $"Score: {score}";
    }

    public int GetScore() => score;

    // --- BOOSTER TIMER ---
    public void ShowBoosterTimer(float duration)
    {
        if (!boosterTimerText) return;
        boosterTimerText.gameObject.SetActive(true);

        if (boosterCoroutine != null)
            StopCoroutine(boosterCoroutine);

        boosterCoroutine = StartCoroutine(UpdateBoosterTimer(duration));
    }

    private IEnumerator UpdateBoosterTimer(float duration)
    {
        float remaining = duration;

        while (remaining > 0f)
        {
            remaining -= Time.deltaTime;

            boosterTimerText.color = remaining <= 3f
                ? Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time * 5f, 1f))
                : Color.cyan;

            boosterTimerText.text = $"<b>Damage Boost:</b> {remaining:F1}s";
            yield return null;
        }

        boosterTimerText.gameObject.SetActive(false);
    }

    // --- LEVEL SWITCH ---
    public void LoadNextLevel()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"Loading next level: Index {nextIndex}");
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.Log("No more levels in Build Settings!");
        }
    }
}
