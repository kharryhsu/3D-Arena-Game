using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinSound : MonoBehaviour
{
    void Awake()
    {
        SoundManager.Instance?.PlayWin();
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.currentHealth = PlayerStats.Instance.maxHealth;
    }

    void Update()
    {
        GameManager.Instance?.ResetGame();
    }
}
