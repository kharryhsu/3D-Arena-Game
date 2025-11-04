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
}
