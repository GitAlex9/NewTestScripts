using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.Instance == null) return;

        // Só usar este spawn se não houver teleportIndex válido (spawn inicial da cena)
        if (GameManager.Instance.teleportIndex < 0)
        {
            GameManager.Instance.SpawnPlayer(this);
        }
        
        Destroy(this);
    }
}
