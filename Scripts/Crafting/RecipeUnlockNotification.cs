using System.Collections;
using UnityEngine;

public class RecipeUnlockNotification : MonoBehaviour
{
    [Header("Referências")]
    public RectTransform notificationPanel;  
    public RectTransform exclamationIcon;  

    [Header("Configuração de Tempo")]
    public float slideDuration = 0.3f;
    public float displayDuration = 3f;
    public float exclamationScaleAmount = 1.2f; 
    public float exclamationScaleSpeed = 2f;   

    [Header("Áudio")]
    public AudioSource audioSource;
    public AudioClip notificationSound; 

    private Vector2 hiddenPosition;
    private Vector2 shownPosition;
    private bool isShowing = false;

    private void Start()
    {
        
        shownPosition = notificationPanel.anchoredPosition;
        hiddenPosition = new Vector2(shownPosition.x + 500, shownPosition.y); 

        notificationPanel.anchoredPosition = hiddenPosition;
    }

    private void OnEnable()
    {
      
    {
    Debug.Log("RecipeUnlockNotification ATIVADO");

    if (CraftingManager.Instance != null)
    {
        Debug.Log("Inscrito no evento de receita desbloqueada.");
        CraftingManager.Instance.OnRecipeUnlocked += ShowNotification;
    }
    else
    {
        Debug.LogError("CraftingManager NÃO encontrado na cena.");
    }
    }

    }

    private void OnDisable()
    {
        if (CraftingManager.Instance != null)
        {
            CraftingManager.Instance.OnRecipeUnlocked -= ShowNotification;
        }
    }

    private void ShowNotification(string recipeID)
    {
        Debug.Log("Recebido evento de desbloqueio de receita: " + recipeID);
        if (!isShowing)
        {
            StartCoroutine(DisplayNotification());
        }
        if (notificationSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(notificationSound);
        }
    }

    private IEnumerator DisplayNotification()
    {
        isShowing = true;

        yield return StartCoroutine(Slide(notificationPanel, hiddenPosition, shownPosition, slideDuration));

        float elapsed = 0f;
        while (elapsed < displayDuration)
        {
            elapsed += Time.deltaTime;

            float scale = 1 + Mathf.Abs(Mathf.Sin(elapsed * exclamationScaleSpeed)) * (exclamationScaleAmount - 1);
            exclamationIcon.localScale = new Vector3(scale, scale, 1);

            yield return null;
        }

        exclamationIcon.localScale = Vector3.one;

        yield return StartCoroutine(Slide(notificationPanel, shownPosition, hiddenPosition, slideDuration));

        isShowing = false;
    }

    private IEnumerator Slide(RectTransform target, Vector2 start, Vector2 end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            target.anchoredPosition = Vector2.Lerp(start, end, t);
            yield return null;
        }
        target.anchoredPosition = end;
    }
}
