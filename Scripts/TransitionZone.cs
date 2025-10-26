using UnityEngine;

public class TransitionZone : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private int connectionIndex; // ID de conexão (único para este portal e seu par)
    [SerializeField] private int sceneIndex; // Índice da cena de destino
    [SerializeField] private Transform spawnPoint; // Ponto de spawn do player ao chegar nesta zona

    private bool transitionBlocked = false;

    private void Start()
    {
        // Se este for o destino do teleporte, spawnar o player aqui
        if (GameManager.Instance != null && GameManager.Instance.teleportIndex == connectionIndex)
        {
            GameManager.Instance.SpawnPlayerAtPosition(spawnPoint.position, spawnPoint.rotation);
            GameManager.Instance.teleportIndex = -1; // Resetar
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") )
        {
            SceneTransition.Instance.transitionFade(sceneIndex, connectionIndex);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transitionBlocked = false;
        }
    }
}
