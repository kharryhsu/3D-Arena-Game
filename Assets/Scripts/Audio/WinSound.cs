using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinSound : MonoBehaviour
{
    void Awake()
    {
        SoundManager.Instance?.PlayWin();
    }
}
