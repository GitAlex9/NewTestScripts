using UnityEngine;
using UnityEngine.InputSystem;
using System;

[CreateAssetMenu(menuName = "Input/Input Reader Game Manager", fileName = "InputReaderGameManager")]
public class InputReaderGameManager : ScriptableObject, UserInput.IGameManagerActions
{
    private UserInput _userInput;

    private void OnEnable()
    {
        if (_userInput == null)
        {
            _userInput = new UserInput();
            _userInput.GameManager.SetCallbacks(this);
        }

        _userInput.Enable();
    }

    private void OnDisable()
    {
        _userInput.Disable();
    }

    public event Action PauseEvent;
    public event Action QuestLogEvent;
    public event Action InteractEvent;
    public event Action CraftEvent;
    public event Action InventoryEvent;

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            PauseEvent?.Invoke();
        }
    }

    public void OnQuesLog(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            QuestLogEvent?.Invoke();
        }
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            InventoryEvent?.Invoke();
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            InteractEvent?.Invoke();
        }
    }

    public void OnCrafting(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            CraftEvent?.Invoke();
        }
    }
}