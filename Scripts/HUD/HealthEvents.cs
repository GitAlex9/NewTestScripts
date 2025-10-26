// HealthEvents.cs
using UnityEngine;
using System;

public static class HealthEvents
{
    public static event Action<float> OnMaxHealthChanged;
    
    public static event Action<float> OnCurrentHealthChanged;
    
    public static void TriggerMaxHealthChanged(float maxHealth)
    {
        OnMaxHealthChanged?.Invoke(maxHealth);
    }
    
    public static void TriggerCurrentHealthChanged(float currentHealth)
    {
        OnCurrentHealthChanged?.Invoke(currentHealth);
    }
}