using Unity.VisualScripting;
using UnityEngine;

public class InteractableDetector : MonoBehaviour
{
    public float detectionRadius = 3f;
    public LayerMask interactableLayer;
    [SerializeField] private Collider nearestInteractable;

    private Entity entity;


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private void OnEnable()
    {
        GameManager.Instance.OnInteractPress += Interact;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnInteractPress -= Interact;
    }

    void Update()
    {
        DetectInteractable();

        // if (Input.GetKeyDown(KeyCode.R) && nearestInteractable != null)
        // {
        //     Interact();
        // }
                           
    }

    private void Awake()
    {
        entity = GetComponent<Entity>();
    }
    
    void DetectInteractable()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, interactableLayer);
        
        if (hitColliders.Length > 0)
        {
            nearestInteractable = hitColliders[0];
        }
        else
        {
            nearestInteractable = null;
        }
      
    }

    void Interact(InputEventContext inputEventContext)
    {
        if (!nearestInteractable || !inputEventContext.Equals(InputEventContext.DEFAULT)) return;

        IInteractable interactable = nearestInteractable.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.OnInteract();
        }
        entity.animationController?.TriggerAnimation("PickItem");
    }
}
