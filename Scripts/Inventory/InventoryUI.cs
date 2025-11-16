using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    // public GameObject inventoryUI;

    Inventory inventory;

    InventorySlot[] slots;

    ItemData itemData;
    void Start()
    {
        inventory = Inventory.Instance;

        inventory.onInventoryChangedCallback += UpdateUI;

        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
    }


    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.I))
    //     {
    //         inventoryUI.SetActive(!inventoryUI.activeSelf);
    //     }
    // }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                ItemStack stack = inventory.items[i];
                slots[i].AddItem(stack.item, stack.quantity);
            }
            
            else
                {
                    slots[i].ClearSlot();
                }
            }
        }
    }

