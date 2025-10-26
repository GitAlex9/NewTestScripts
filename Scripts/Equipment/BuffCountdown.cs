using System.Collections;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class BuffCountdown : MonoBehaviour
{
    public void StartBuff(float duration, Action onBuffStart, Action onBuffEnd)
    {
        StartCoroutine(BuffDurationCoroutine(duration, onBuffStart, onBuffEnd));
    }   

    private IEnumerator BuffDurationCoroutine(float duration, Action onBuffStart, Action onBuffEnd)
    {
        float remainingTime = duration;
        onBuffStart?.Invoke();
        Debug.Log($"Buff aplicado por {duration} segundos");

        yield return new WaitForSeconds(duration);

        onBuffEnd?.Invoke();
        Debug.Log("buff expirado");
        yield break;
    }
    void Update()
    {
        
    }
}
