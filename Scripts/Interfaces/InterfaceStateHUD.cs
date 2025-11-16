using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI; 


public class InterfaceStateHUD : InterfaceState // <-- MUDANÇA
{
    [Header("Interface Elements")]
    [Tooltip("Elementos da interface que terão animação de fade (em ordem de aparição)")]
    public List<CanvasGroup> interfaceElements = new List<CanvasGroup>();
    
    [Header("Fade In Settings")]
    [Tooltip("Duração do fade in de cada elemento")]
    public float fadeInDuration = 0.3f;
    
    [Tooltip("Delay entre cada elemento na animação de entrada")]
    public float delayBetweenElements = 0.1f;
    
    [Tooltip("Tipo de easing para o fade in")]
    public Ease fadeInEase = Ease.OutQuad;
    
    [Header("Fade Out Settings")]
    [Tooltip("Duração do fade out de cada elemento")]
    public float fadeOutDuration = 0.2f;
    
    [Tooltip("Delay entre cada elemento na animação de saída")]
    public float delayBetweenElementsOut = 0.05f;
    
    [Tooltip("Tipo de easing para o fade out")]
    public Ease fadeOutEase = Ease.InQuad;
    
    [Tooltip("Animar saída em ordem reversa?")]
    public bool reverseOrderOnExit = true;

    public override void Enter(InterfaceManager manager)
    {
        base.Enter(manager);
        Debug.Log("[Interface State Padrão] Entrando no estado");
        
        // Iniciar todos os elementos com alpha 0
        foreach (var element in interfaceElements)
        {
            if (element != null)
            {
                element.alpha = 0f;
            }
        }
        
        // Iniciar animação de fade in sequencial
        StartCoroutine(FadeInElements());
    }

    public override void UpdateState(InterfaceManager manager)
    {
        base.UpdateState(manager);
        // Override em classes derivadas se necessário
    }

    public override IEnumerator Exit(InterfaceManager manager)
    {
        Debug.Log("[Interface State Padrão] Saindo do estado");
        
        // Animar fade out dos elementos
        yield return StartCoroutine(FadeOutElements());
        
        yield return base.Exit(manager);
    }
    
    /// <summary>
    /// Anima o fade in de todos os elementos com delay entre eles
    /// </summary>
    private IEnumerator FadeInElements()
    {
        for (int i = 0; i < interfaceElements.Count; i++)
        {
            var element = interfaceElements[i];
            if (element != null)
            {
                // Fazer fade in do elemento usando DOTween
                element.DOFade(1f, fadeInDuration)
                    .SetEase(fadeInEase);
                
                // Aguardar delay antes do próximo elemento
                if (i < interfaceElements.Count - 1)
                {
                    yield return new WaitForSecondsRealtime(delayBetweenElements);
                }
            }
        }
    }
    
    /// <summary>
    /// Anima o fade out de todos os elementos com delay entre eles
    /// </summary>
    private IEnumerator FadeOutElements()
    {
        // Criar lista na ordem desejada (normal ou reversa)
        List<CanvasGroup> elementsToFade = new List<CanvasGroup>(interfaceElements);
        if (reverseOrderOnExit)
        {
            elementsToFade.Reverse();
        }
        
        for (int i = 0; i < elementsToFade.Count; i++)
        {
            var element = elementsToFade[i];
            if (element != null)
            {
                // Fazer fade out do elemento usando DOTween
                element.DOFade(0f, fadeOutDuration)
                    .SetEase(fadeOutEase);
                
                // Aguardar delay antes do próximo elemento
                if (i < elementsToFade.Count - 1)
                {
                    yield return new WaitForSecondsRealtime(delayBetweenElementsOut);
                }
            }
        }
        
        // Aguardar a última animação terminar
        yield return new WaitForSecondsRealtime(fadeOutDuration);
    }
    
    private void OnDisable()
    {
        // Matar tweens ao desabilitar para evitar erros
        foreach (var element in interfaceElements)
        {
            if (element != null)
            {
                element.DOKill();
            }
        }
    }
}