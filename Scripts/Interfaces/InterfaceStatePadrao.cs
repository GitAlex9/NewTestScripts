using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI; 


public class InterfaceStatePadrao : InterfaceStateNavegavel // <-- MUDANÇA
{
    [Header("Animação de Interface")]
    [Tooltip("Elementos da interface que terão animação de fade (em ordem de aparição)")]
    public List<CanvasGroup> interfaceElements = new List<CanvasGroup>();

    [Header("Controlador de Conteúdo (Opcional)")]
    [Tooltip("Arraste aqui o script que gerencia o conteúdo desta UI, como o QuestLogUI.")]
    [SerializeField] private QuestLogUI uiContentController;
    
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

    /// <summary>
    /// Ao entrar no estado, define o primeiro botão selecionável e inicia as animações.
    /// </summary>
    public override void Enter(InterfaceManager manager)
    {

        if (uiContentController != null)
        {
            primeiroElementoSelecionado = uiContentController.FirstSelectedButton;
        }
        
        if (primeiroElementoSelecionado == null && interfaceElements.Count > 0)
        {
            primeiroElementoSelecionado = interfaceElements[0].GetComponent<Selectable>();
            if (primeiroElementoSelecionado == null)
            {
                foreach (var element in interfaceElements)
                {
                    Selectable selectable = element.GetComponent<Selectable>();
                    if (selectable != null)
                    {
                        primeiroElementoSelecionado = selectable;
                        break;
                    }
                }
            }
        }

        base.Enter(manager);
        
        foreach (var element in interfaceElements)
        {
            if (element != null)
            {
                element.alpha = 0f;
            }
        }

        StartCoroutine(FadeInElements());
    }

    public override IEnumerator Exit(InterfaceManager manager)
    {
        // Animar fade out dos elementos
        yield return StartCoroutine(FadeOutElements());
        
        // Chama o Exit() da classe base (InterfaceStateNavegavel) para limpar a navegação
        yield return base.Exit(manager);
    }
    
    private IEnumerator FadeInElements()
    {
        for (int i = 0; i < interfaceElements.Count; i++)
        {
            var element = interfaceElements[i];
            if (element != null)
            {
                element.DOFade(1f, fadeInDuration).SetEase(fadeInEase);
                if (i < interfaceElements.Count - 1)
                {
                    yield return new WaitForSecondsRealtime(delayBetweenElements);
                }
            }
        }
    }
    
    private IEnumerator FadeOutElements()
    {
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
                element.DOFade(0f, fadeOutDuration).SetEase(fadeOutEase);
                if (i < elementsToFade.Count - 1)
                {
                    yield return new WaitForSecondsRealtime(delayBetweenElementsOut);
                }
            }
        }
        
        yield return new WaitForSecondsRealtime(fadeOutDuration);
    }
    
    private void OnDisable()
    {
        foreach (var element in interfaceElements)
        {
            if (element != null)
            {
                element.DOKill();
            }
        }
    }
}
