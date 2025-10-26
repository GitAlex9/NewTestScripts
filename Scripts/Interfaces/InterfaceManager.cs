using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager Instance { get; private set; }

    [Header("Interface States")]
    [SerializeField] protected InterfaceState currentState;
    [SerializeField] protected InterfaceState defaultState; // Estado padrão (ex: HUD)
    [SerializeField] protected List<InterfaceState> allStates;
    
    protected bool isTransitioning = false;

    // Eventos para quando estados mudam
    public event Action<InterfaceState> OnStateChanged;
    public event Action<InterfaceState> OnStateEntered;
    public event Action<InterfaceState> OnStateExited;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Coletar todos os estados filhos se a lista estiver vazia
        if (allStates == null || allStates.Count == 0)
        {
            allStates = new List<InterfaceState>(GetComponentsInChildren<InterfaceState>(true));
        }
        
        // Desativar todos os estados inicialmente
        DisableAllStates();
    }

    private void Start()
    {
        // Iniciar com o estado padrão usando Enter
        if (defaultState != null)
        {
            // Reativar o GameObject do estado padrão antes de chamar Enter
            defaultState.gameObject.SetActive(true);
            
            currentState = defaultState;
            currentState.Enter(this);
            OnStateEntered?.Invoke(currentState);
            OnStateChanged?.Invoke(currentState);
            Debug.Log($"[InterfaceManager] Estado inicial: {currentState.name}");
        }
    }

    private void Update()
    {
        // Atualizar o estado atual
        currentState?.UpdateState(this);
    }

    public void SceneChange(int sceneIndex)
    {
        SceneTransition.Instance.transitionFade(sceneIndex);
    }

    public void ResumeSavedGame()
    {
        Metadados.Instance.ResumeSavedGame();
    }
    public void EndGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Transiciona para um novo estado de interface
    /// </summary>
    public void TransitionToState(InterfaceState newState)
    {
        if (isTransitioning)
        {
            Debug.LogWarning("[InterfaceManager] Já está em transição! Aguarde a transição atual terminar.");
            return;
        }

        if (newState == null)
        {
            Debug.LogWarning("[InterfaceManager] Tentou transicionar para um estado nulo!");
            return;
        }

        // Se já está neste estado, não fazer nada
        if (currentState == newState)
        {
            return;
        }

        StartCoroutine(TransitionCoroutine(newState));
    }

    private IEnumerator TransitionCoroutine(InterfaceState newState)
    {
        isTransitioning = true;
        InterfaceState previousState = currentState;

        // Sair do estado atual (aguarda animações)
        if (currentState != null)
        {
            yield return StartCoroutine(currentState.Exit(this));
            OnStateExited?.Invoke(currentState);
            
            // Desativar o GameObject do estado anterior
            currentState.gameObject.SetActive(false);
        }

        // Transicionar para o novo estado
        currentState = newState;
        
        // Reativar o GameObject do novo estado
        currentState.gameObject.SetActive(true);

        // Entrar no novo estado
        currentState.Enter(this);
        OnStateEntered?.Invoke(currentState);
        OnStateChanged?.Invoke(currentState);

        Debug.Log($"[InterfaceManager] Transicionou de {previousState?.name ?? "null"} para {currentState.name}");
        
        isTransitioning = false;
    }

    /// <summary>
    /// Retorna ao estado padrão
    /// </summary>
    public void ReturnToDefaultState()
    {
        if (defaultState != null)
        {
            TransitionToState(defaultState);
        }
    }

    /// <summary>
    /// Obtém o estado atual da interface
    /// </summary>
    public InterfaceState GetCurrentState()
    {
        return currentState;
    }

    /// <summary>
    /// Verifica se está em um estado específico
    /// </summary>
    public bool IsInState(InterfaceState state)
    {
        return currentState == state;
    }

    /// <summary>
    /// Verifica se está em um estado específico por tipo
    /// </summary>
    public bool IsInState<T>() where T : InterfaceState
    {
        return currentState is T;
    }

    /// <summary>
    /// Obtém um estado específico por tipo
    /// </summary>
    public T GetState<T>() where T : InterfaceState
    {
        return GetComponentInChildren<T>(true);
    }
    
    /// <summary>
    /// Desativa todos os estados de interface
    /// </summary>
    private void DisableAllStates()
    {
        foreach (var state in allStates)
        {
            if (state != null && state.gameObject != null)
            {
                // Desativar o GameObject do estado
                state.gameObject.SetActive(false);
            }
        }
        
        Debug.Log($"[InterfaceManager] Desativou {allStates.Count} estados de interface");
    }
}
