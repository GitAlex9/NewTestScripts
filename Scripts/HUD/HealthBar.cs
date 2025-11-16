using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    void OnEnable()
    {
        HealthEvents.OnMaxHealthChanged += SetMaxHealth;
        HealthEvents.OnCurrentHealthChanged += SetHealth;
    }

    void OnDisable()
    {
        HealthEvents.OnMaxHealthChanged -= SetMaxHealth;
        HealthEvents.OnCurrentHealthChanged -= SetHealth;
    }

    private void SetMaxHealth(float maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    private void SetHealth(float health)
    {
        slider.value = health;
    }
}