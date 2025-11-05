using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinSound : MonoBehaviour
{
    void Awake()
    {
        SoundManager.Instance?.PlayWin();
    }
    
    void Update()
    {
        Debug.Log($"Player health reset to max: {PlayerStats.Instance.maxHealth}");
        PlayerStats.Instance.currentHealth = PlayerStats.Instance.maxHealth;
        Debug.Log($"Player health cur: {PlayerStats.Instance.currentHealth}");   
    }
}
