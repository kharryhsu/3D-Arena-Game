using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseSound : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        SoundManager.Instance?.PlayLose();
    }
    
    void Update()
    {
        Debug.Log($"Player health reset to max: {PlayerStats.Instance.maxHealth}");
        PlayerStats.Instance.currentHealth = PlayerStats.Instance.maxHealth;
        Debug.Log($"Player health cur: {PlayerStats.Instance.currentHealth}");   
    }
}
