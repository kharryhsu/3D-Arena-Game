using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider slider;
    public TMPro.TextMeshProUGUI healthText;

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
        if (healthText) healthText.text = $"HP: {health}/{health}";
    }

    public void SetHealth(float health)
    {
        slider.value = health;
        if (healthText) healthText.text = $"HP: {health}/{slider.maxValue}";
    }
}
