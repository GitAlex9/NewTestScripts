// ItemPickup.cs
using System;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    public ItemData item;
   
    
    public void OnInteract()
    {     
        bool wasPickedUp = Inventory.Instance.AddItem(item);

        if (wasPickedUp)
        {
            BoxPickedUp();
            Debug.Log($"{item.itemName} picked up");
            Destroy(gameObject);
        }
    }

    public void BoxPickedUp()
    {
        // Encontra o CollectBoxes na cena (assumindo que há apenas um)
        CollectBoxes collectBoxes = FindFirstObjectByType<CollectBoxes>();
        if (collectBoxes != null)
        {
            collectBoxes.BoxCollected();
        }
    }
}