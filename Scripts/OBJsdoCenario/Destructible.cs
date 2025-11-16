using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class Destructible : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 5;
    public int currentHealth;

    [Header("Item Drop")]
    public GameObject dropPrefab;
    public ItemData dropItem;
    public int minDrops = 1;
    public int maxDrops = 3;
    public float dropForce = 2f;

    [Header("Feedback Visual")]
    public Material defaultMaterial;
    public Material whiteFlashMaterial;
    public float flashDuration = 0.1f;
    private Renderer[] renderers;
    private Material instanceMaterial;

    [Header("Efeito de Brilho Local")]
    public bool enableGlow = true;
    public float glowInterval = 15f;
    public float glowDuration = 2f;
    public float sweepSpeed = 3f;
    public Color glowColor = new Color(1f, 0.95f, 0.6f);
    private float glowTimer;
    private bool glowing = false;

    [Header("Áudio")]
    public List<AudioClip> hitSFX = new List<AudioClip>();
    public AudioClip breakSFX;
    public AudioSource audioSource;
    private bool isDestroyed = false;

    private void Awake()
    {
        if (currentHealth <= 0)
            currentHealth = maxHealth;

        renderers = GetComponentsInChildren<Renderer>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (renderers.Length > 0)
        {
            instanceMaterial = new Material(defaultMaterial);
            foreach (Renderer r in renderers)
                r.material = instanceMaterial;

            instanceMaterial.EnableKeyword("_EMISSION");
            instanceMaterial.SetColor("_EmissionColor", Color.black);
        }
    }

    private void Update()
    {
        if (isDestroyed) return;

        if (enableGlow && !glowing)
        {
            glowTimer += Time.deltaTime;
            if (glowTimer >= glowInterval)
            {
                StartCoroutine(GlowSweep());
                glowTimer = 0f;
            }
        }
    }

    private IEnumerator GlowSweep()
    {
        glowing = true;
        float elapsed = 0f;

        while (elapsed < glowDuration)
        {
            float t = Mathf.PingPong(Time.time * sweepSpeed, 1f);
            float brightness = Mathf.SmoothStep(0f, 1f, Mathf.Sin(t * Mathf.PI));

            Color emission = glowColor * Mathf.LinearToGammaSpace(brightness * 2f);
            instanceMaterial.SetColor("_EmissionColor", emission);

            elapsed += Time.deltaTime;
            yield return null;
        }

        instanceMaterial.SetColor("_EmissionColor", Color.black);
        glowing = false;
    }

    public void TakeDamage(int amount)
    {
        if (isDestroyed) return;

        currentHealth -= amount;
        FlashWhite();
        PlayRandomHitSound();

        if (currentHealth <= 0)
            Break();
    }

    private void FlashWhite()
    {
        if (whiteFlashMaterial == null || renderers.Length == 0)
            return;

        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        foreach (Renderer r in renderers)
        {
            if (r != null)
                r.material = whiteFlashMaterial;
        }

        yield return new WaitForSeconds(flashDuration);

        foreach (Renderer r in renderers)
        {
            if (r != null)
                r.material = instanceMaterial;
        }
    }

    private void PlayRandomHitSound()
    {
        if (hitSFX.Count == 0 || audioSource == null) return;

        int randomIndex = Random.Range(0, hitSFX.Count);
        AudioClip clip = hitSFX[randomIndex];
        audioSource.PlayOneShot(clip);
    }

    private void Break()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        if (breakSFX != null && audioSource != null)
            audioSource.PlayOneShot(breakSFX);

        if (dropPrefab != null && dropItem != null)
        {
            int dropCount = Random.Range(minDrops, maxDrops + 1);

            for (int i = 0; i < dropCount; i++)
            {
                Vector3 spawnPos = transform.position + Vector3.up * 0.5f + Random.insideUnitSphere * 0.3f;
                GameObject drop = Instantiate(dropPrefab, spawnPos, Quaternion.identity);

                Rigidbody rb = drop.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 force = (Vector3.up + Random.insideUnitSphere).normalized * dropForce;
                    rb.AddForce(force, ForceMode.Impulse);
                }

                ItemPickup pickup = drop.GetComponent<ItemPickup>();
                if (pickup != null)
                {
                    pickup.item = dropItem;
                }

                CoisaQueDropa floating = drop.GetComponent<CoisaQueDropa>();
                if (floating != null)
                {
                    floating.StartFloating();
                }
            }
        }

        Destroy(gameObject);
    }
}
