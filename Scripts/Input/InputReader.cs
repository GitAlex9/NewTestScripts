using UnityEngine;
using UnityEngine.InputSystem;
using System;

[CreateAssetMenu(menuName = "Input/Input Reader", fileName = "InputReader")]
public class InputReader : ScriptableObject, UserInput.IPlayerActions
{
    public event Action<Vector2> MoveEvent;
    // public event Action InteractEvent;

    public event Action FirstActionPressed;
    public event Action FirstActionReleased;

    public event Action SecondActionPressed;
    public event Action SecondActionReleased;

    public event Action ThirdActionPressed;
    public event Action ThirdActionReleased;

    public event Action DashPressed;
    public event Action DashReleased;
 
    public event Action FocusTargetPressed;
    public event Action FocusTargetReleased;

    private UserInput _userInput;

    // Propriedade para acessar o valor atual do movimento
    public Vector2 Move => _userInput?.Player.Move.ReadValue<Vector2>() ?? Vector2.zero;

    private void OnEnable()
    {
        if (_userInput == null)
        {
            _userInput = new UserInput();
            _userInput.Player.SetCallbacks(this);
        }

        _userInput.Enable();
    }

    private void OnDisable()
    {
        _userInput.Disable();
    }

    private bool CheckGameManagerBlock()
    {
        return GameManager.Instance != null && (GameManager.Instance.SoftPaused || GameManager.Instance.IsPaused || GameManager.Instance.IsQuestLog || GameManager.Instance.IsDialogue);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (CheckGameManagerBlock()) return;
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnFirstAction(InputAction.CallbackContext context)
    {
        if (CheckGameManagerBlock()) return;

        if (context.started)
        {
            FirstActionPressed?.Invoke();
        }
        else if (context.canceled)
        {
            FirstActionReleased?.Invoke();
        }
    }

    public void OnSecondAction(InputAction.CallbackContext context)
    {
        if (CheckGameManagerBlock()) return;

        if (context.started)
        {
            SecondActionPressed?.Invoke();
        }
        else if (context.canceled)
        {
            SecondActionReleased?.Invoke();
        }
    }

    public void OnThirdAction(InputAction.CallbackContext context)
    {
        if (CheckGameManagerBlock()) return;

        if (context.started)
        {
            ThirdActionPressed?.Invoke();
        }
        else if (context.canceled)
        {
            ThirdActionReleased?.Invoke();
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (CheckGameManagerBlock()) return;

        if (context.started)
        {
            DashPressed?.Invoke();
        }
        else if (context.canceled)
        {
            DashReleased?.Invoke();
        }
    }
    
    public void OnLookAt(InputAction.CallbackContext context)
    {
        if (CheckGameManagerBlock()) return;

        if (context.started)
        {
            FocusTargetPressed?.Invoke();
        }
        else if (context.canceled)
        {
            FocusTargetReleased?.Invoke();
        }
    }
}
