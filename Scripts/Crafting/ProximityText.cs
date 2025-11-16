using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class ProximityText : MonoBehaviour
{
    [Header("Textos que vão aparecer/desaparecer")]
    public TMP_Text[] proximityTexts;

    [Header("Configuração do Fade")]
    [Range(0.1f, 10f)]
    public float fadeSpeed = 4f;

    private bool playerIsNearby = false;

    private Color[] visibleColors;
    private Color[] hiddenColors;

    private void Start()
    {
        if (proximityTexts == null || proximityTexts.Length == 0)
        {
            Debug.LogError("ProximityText: Nenhum TextMeshPro foi atribuído.");
            enabled = false;
            return;
        }

        visibleColors = new Color[proximityTexts.Length];
        hiddenColors = new Color[proximityTexts.Length];

        for (int i = 0; i < proximityTexts.Length; i++)
        {
            TMP_Text text = proximityTexts[i];

            visibleColors[i] = new Color(text.color.r, text.color.g, text.color.b, 1f);
            hiddenColors[i] = new Color(text.color.r, text.color.g, text.color.b, 0f);

            text.color = hiddenColors[i];
        }

        GetComponent<Collider>().isTrigger = true;
    }

    private void Update()
    {
        for (int i = 0; i < proximityTexts.Length; i++)
        {
            TMP_Text text = proximityTexts[i];

            Color targetColor = playerIsNearby ? visibleColors[i] : hiddenColors[i];

            text.color = Color.Lerp(
                text.color,
                targetColor,
                Time.deltaTime * fadeSpeed
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerIsNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerIsNearby = false;
    }
}
