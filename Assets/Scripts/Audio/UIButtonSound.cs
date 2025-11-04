using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayButtonClickTrimmed(0.3f, 1f);
            }
        });
    }
}
