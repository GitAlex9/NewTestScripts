using System;
using System.Collections;
using UnityEngine;

public class DamageBox : MonoBehaviour
{
    public Damage damage;
    public String targetTag;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != damage.damageOwner && other.CompareTag(targetTag))
        {
            Entity entityHitted = other.GetComponent<Entity>();
            if (entityHitted != null)
            {
                entityHitted.TakeDamage(damage);
            }
        }
    }

    private void Start()
    {
        StartCoroutine(DestroyAfterFrames(15));
    }

    private IEnumerator DestroyAfterFrames(int frames)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        DestroyBox();
    }

    private void DestroyBox()
    {
        Destroy(gameObject);
    }
}