using UnityEngine;

public class InterfaceGameplayManager : InterfaceManager
{
    [SerializeField] private InterfaceState pauseState;
    [SerializeField] private InterfaceState inventoryState;
    [SerializeField] private InterfaceState questLogState;
    [SerializeField] private InterfaceState dialogueState;
    [SerializeField] private InterfaceState gameOverState;

    public void HandlePauseEvent()
    {
        Debug.Log("Handling Pause Event in InterfaceGameplayManager");
        if (GameManager.Instance.IsPaused)
        {
            TransitionToState(pauseState);
        }
        else
        {
            TransitionToState(defaultState);
        }
    }

    public void HandleQuestLogEvent()
    {
        if (!questLogState.isActiveAndEnabled)
        {         
            TransitionToState(questLogState);
        }
        else
        {
            TransitionToState(defaultState);
        }
    }

    public void HandleDialogueEvent()
    {
        if (!dialogueState.isActiveAndEnabled)
        {
            TransitionToState(dialogueState);
        }
        else
        {
            TransitionToState(defaultState);
        }
    }

    public void HandleInventoryEvent()
    {
        if (!inventoryState.isActiveAndEnabled)
        {
            TransitionToState(inventoryState);
        }
        else
        {
            TransitionToState(defaultState);
        }
    }

    public void ShowGameOverMenu()
    {
        Debug.Log("Showing Game Over Menu");
        TransitionToState(gameOverState);
    }

    public void HidePauseMenu()
    {
        GameManager.Instance.PauseGame();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnGamePaused += HandlePauseEvent;
        GameManager.Instance.OnGameUnpaused += HandlePauseEvent;
        GameManager.Instance.OnQuestLogPressed += HandleQuestLogEvent;
        GameManager.Instance.dialogueEvents.onDialogueStarted += HandleDialogueEvent;
        GameManager.Instance.dialogueEvents.onDialogueFinished += HandleDialogueEvent;
        GameManager.Instance.OnGameOver += ShowGameOverMenu;
        GameManager.Instance.OnInventoryPressed += HandleInventoryEvent;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGamePaused -= HandlePauseEvent;
        GameManager.Instance.OnGameUnpaused -= HandlePauseEvent;
        GameManager.Instance.OnQuestLogPressed -= HandleQuestLogEvent;
        GameManager.Instance.dialogueEvents.onDialogueStarted -= HandleDialogueEvent;
        GameManager.Instance.dialogueEvents.onDialogueFinished -= HandleDialogueEvent;
        GameManager.Instance.OnGameOver -= ShowGameOverMenu;
        GameManager.Instance.OnInventoryPressed -= HandleInventoryEvent;
    }
}
