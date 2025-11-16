using UnityEngine;

public class CraftingMenuOpener : MonoBehaviour
{
    private GameObject craftingCanvas;
    private CraftingZone nearbyCraftingZone;

    void Start()
    {
        craftingCanvas = CraftingManager.Instance.craftingCanvas;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && nearbyCraftingZone != null && nearbyCraftingZone.playerIsNearby)
        {
            bool isActive = craftingCanvas.activeSelf;
            craftingCanvas.SetActive(!isActive);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CraftingZone zone = other.GetComponent<CraftingZone>();
        if (zone != null)
        {
            nearbyCraftingZone = zone;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CraftingZone zone = other.GetComponent<CraftingZone>();
        if (zone != null && nearbyCraftingZone == zone)
        {
            nearbyCraftingZone = null;
        }
    }
}
