using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CharacterController))]
public class PlayerEntity : Entity
{
    [Header("Player References")]
    public InputReader _input;
    public Transform cameraTransform; // Referência para a câmera principal ou virtual
    private Vector3 _initialPosition;

    #region Global Events

    public void OnHealthChanged()
    {
        HealthEvents.TriggerCurrentHealthChanged(stats.Health);
    }

    public void OnMaxHealthChanged()
    {
        HealthEvents.TriggerMaxHealthChanged(stats.MaxHealth);
    }

    #endregion

    #region System Setup

    public override void EntityStart()
    {
        equipments = Inventory.Instance.equipments;
        base.EntityStart();     
    }

    private void OnEnable()
    {
        _initialPosition = transform.position;    

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        // Input
        _input.DashPressed += movementComponent.OnDash;

        _input.FirstActionPressed += actionComponent.OnFirstActionPressed;
        _input.FirstActionReleased += actionComponent.OnFirstActionReleased;
        _input.SecondActionPressed += actionComponent.OnSecondActionPressed;
        _input.SecondActionReleased += actionComponent.OnSecondActionReleased;
        _input.ThirdActionPressed += actionComponent.OnThirdActionPressed;
        _input.ThirdActionReleased += actionComponent.OnThirdActionReleased;

        _input.FocusTargetPressed += movementComponent.OnFocusTargetPressed;
        _input.FocusTargetReleased += movementComponent.OnFocusTargetReleased;

        // Equipments
        Inventory.Instance.OnItemEquipped += Equip;
        Inventory.Instance.OnItemUnequipped += Unequip;

        // Health
        HealthEvents.TriggerCurrentHealthChanged(stats.Health);
        HealthEvents.TriggerMaxHealthChanged(stats.MaxHealth);

        EntityHitted += OnHealthChanged;
        EntityStatsChanged += OnMaxHealthChanged;
    }

    private void OnDisable()
    {
        // _input.MoveEvent -= OnMove;
        _input.DashPressed -= movementComponent.OnDash;

        _input.FirstActionPressed -= actionComponent.OnFirstActionPressed;
        _input.FirstActionReleased -= actionComponent.OnFirstActionReleased;
        _input.SecondActionPressed -= actionComponent.OnSecondActionPressed;
        _input.SecondActionReleased -= actionComponent.OnSecondActionReleased;

        _input.FocusTargetPressed -= movementComponent.OnFocusTargetPressed;
        _input.FocusTargetReleased -= movementComponent.OnFocusTargetReleased;

        // Equipments
        Inventory.Instance.OnItemEquipped -= Equip;
        Inventory.Instance.OnItemUnequipped -= Unequip;

        EntityHitted -= OnHealthChanged;
        EntityStatsChanged -= OnMaxHealthChanged;
    }

    private void OnFellUnderWorld(Entity entity)
    {
        transform.position = _initialPosition;
    }

    private void Update()
    {
        // Verifica o input de movimento continuamente
        if (_input != null)
        {
            Vector2 currentMoveInput = _input.Move;
            OnMove(currentMoveInput);
        }
    }

    #endregion

    public void OnMove(Vector2 moveDir)
    {
        if (cameraTransform != null)
        {
            // Converte o movimento 2D para um vetor 3D no plano xz
            Vector3 moveDirection3D = new Vector3(moveDir.x, 0, moveDir.y);

            // Aplica apenas a rotação Y (horizontal) da câmera
            Quaternion cameraRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);

            // Rotaciona o vetor de movimento conforme a rotação da câmera
            Vector3 rotatedMovement = cameraRotation * moveDirection3D;

            // Converte de volta para um Vector2 para o sistema de movimento
            Vector2 adjustedMoveDir = new Vector2(rotatedMovement.x, rotatedMovement.z);

            // Chama o método da classe base com a direção ajustada
            movementComponent.OnMove(adjustedMoveDir);
        }
        else
        {
            // Fallback para o comportamento padrão se não houver câmera
            movementComponent.OnMove(moveDir);
        }
    }

}
