using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;


public class HotbarSlot : MonoBehaviour
{
    public Image icon;    
    public TextMeshProUGUI stackText;
    public Button button;
    
    public ItemStack itemStack;

    public void SetItem(ItemStack stack)
    {
        itemStack = stack;
        if (stack != null)
        {
            icon.sprite = stack.item.icon;
            icon.enabled = true;
            stackText.text = stack.quantity > 1 ? stack.quantity.ToString() : "";
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        itemStack = null;
        icon.sprite = null;
        icon.enabled = false;
        stackText.text = "";
    }

    public void OnClick()
    {
        if (itemStack == null) { return; }

        int quantity = itemStack.quantity;
        int moved = 0;
                        
            for (int i = 0; i < quantity; i++)
        {
            bool added = Inventory.Instance.AddItem(itemStack.item);
            if (!added)
            {
                break;
            }
            moved++;  
        }
        if (moved == quantity) 
        { 
            ClearSlot();
        }
                    
       }

      

    public void UseItem()
    {
        if (itemStack != null)
        {
            if (itemStack.item is Consumables consumable)
            {
                consumable.Use();

                Inventory.Instance.RemoveItem(itemStack.item);
                itemStack.quantity--;

                if (itemStack.quantity <= 0)
                {
                    ClearSlot();
                }
                else
                {
                    UpdateSlotUI();
                }               
            }                                          
        }
       
    }
    public void UpdateSlotUI()
    {
        if (itemStack != null)
        {
            icon.sprite = itemStack.item.icon;
            icon.enabled = true;
            stackText.text = itemStack.quantity > 1 ? itemStack.quantity.ToString() : "";
        }
        else
        {
            ClearSlot();
        }
    }
    
}

