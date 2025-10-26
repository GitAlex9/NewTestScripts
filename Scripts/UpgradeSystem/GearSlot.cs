using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GearSlot : MonoBehaviour
{    
    public Image gearImage;
    public ItemStack gearItem;
    public TextMeshProUGUI gearStatsText;
    public TextMeshProUGUI requiredMaterialsText;
    

    
    public void SetGear(ItemStack gear)
    {
        gearItem = gear;
        if (gear != null)
        {        
            gearImage.sprite = gear.item.icon;
            gearImage.enabled = true;
            
            if(gear.item.upgradeLevel == 0)
            {
                requiredMaterialsText.text = $"{++gear.item.upgradeLevel} material necessary for upgrade";
            }else 
                requiredMaterialsText.text = ($"{gear.item.upgradeLevel.ToString()} materials necessary for upgrade");
        }
        else
        {
            ClearSlot();
        }
    }
    public void ClearSlot()
    {        
        gearItem = null;
        gearImage.sprite = null;
        gearImage.enabled = false;  
        gearStatsText.text = "";
        requiredMaterialsText.text = "";
    }

    public void ReturnGear()
    {
        if (gearItem == null) { return; }
        int quantity = gearItem.quantity;
        int moved = 0;

        for (int i = 0; i < quantity; i++)
        {
            bool added = Inventory.Instance.AddItem(gearItem.item);
            if (!added)
            {
                break;
            }
            moved++;

            if (moved == quantity) { ClearSlot(); }
        }
       

        
    }
}
