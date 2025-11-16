using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialSlot : MonoBehaviour
{
    public Image slotImg;
    public TextMeshProUGUI stackText;
    

    GearSlot gearSlot;
    public ItemStack materialStack;

    public void SetMaterial(ItemStack stack)
    {
        materialStack = stack;


        if (stack != null)
        {
            slotImg.sprite = stack.item.icon;
            slotImg.enabled = true;
            stackText.text = stack.quantity > 1 ? stack.quantity.ToString() : "";
            
        }

        else
        {
            ClearSlot();
        }
        
    }

    public void ClearSlot()
    {
        materialStack = null;
        slotImg.sprite = null;
        slotImg.enabled = false;
        stackText.text = "";
        
    }

    public void ReturnMaterial()
    {
        if (materialStack == null) { return; }
        int quantity = materialStack.quantity;
        int moved = 0;

        for (int i = 0; i < quantity; i++)
        {
            bool added = Inventory.Instance.AddItem(materialStack.item);
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
    public void UpdateUi()
    {
        if (materialStack != null)
        {            
            stackText.text = materialStack.quantity > 1 ? materialStack.quantity.ToString() : "";      


        }
    }
}


        