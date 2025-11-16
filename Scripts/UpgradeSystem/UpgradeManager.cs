using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class UpgradeManager : MonoBehaviour
{
    public GameObject upgradeUI;       
    
    Entity entity;    
    public GearSlot gearSlot;
    public MaterialSlot materialSlot;

    #region Singleton

    public static UpgradeManager Instance;
    private void Awake()
    {
        Instance = this;
                
    }
    #endregion


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            upgradeUI.SetActive(!upgradeUI.activeSelf);
        }
    }

    public void Upgrade()
    {
        //Call all methods below
        if (IsUpgradable())
        {
            Debug.Log("Upgrading Item");
            

            if (gearSlot.gearItem.item.upgradeLevel == 0) 
            { 
                materialSlot.materialStack.quantity--;
                materialSlot.UpdateUi();
            }
            else 
            {
                materialSlot.materialStack.quantity -= gearSlot.gearItem.item.upgradeLevel;
                materialSlot.UpdateUi();
            }          
            
            if (materialSlot.materialStack.quantity == 0)
            {
                materialSlot.ClearSlot();
                materialSlot.UpdateUi();
            }
           
                      
            Inventory.Instance.AddItem(gearSlot.gearItem.item.nextUpgrade);
            gearSlot.ClearSlot();



        }
        else 
        { 
            Debug.Log("Not upgrading");
            return;
        }
        



    }

    public bool IsUpgradable()
    {
        if (gearSlot.gearItem == null || materialSlot.materialStack == null || gearSlot == null || materialSlot == null || gearSlot.gearItem.item == null)
        {
            Debug.Log("please assign items to upgrade");
            return false;
        }
        if (gearSlot.gearItem.item.upgradeLevel == gearSlot.gearItem.item.maxUpgradeLevel || materialSlot.materialStack.quantity < gearSlot.gearItem.item.upgradeLevel || materialSlot.materialStack == null || gearSlot.gearItem == null)
        {
            Debug.Log("not upgradable");
            return false;
        }
        if (gearSlot.gearItem.item.upgradeLevel < gearSlot.gearItem.item.maxUpgradeLevel && materialSlot.materialStack.quantity >= gearSlot.gearItem.item.upgradeLevel)
        {
            Debug.Log("Upgrade Possible");
            return true;
        }
        return false;

        
    }

    public void UpgradeStats()
    {
//upgrade base item stats
        //upgrade base item name to contain +n
    }

    public void ConsumeResources()
    {
//consume upgrade item
    }

    public void UpgradeFeedback(bool success)
    {
        //true = "upgrade successfull"
        //false = "why failed message not enough materials or alreadya t max level"
    }

    public bool AssignEquipment(ItemStack gear)
    {
        
        if (gearSlot.gearItem != null)
        {
            if (gearSlot.gearItem.item != null)
            {

                Inventory.Instance.AddItem(gear.item);

                Debug.Log("item here");
            }
        }

        gearSlot.SetGear(gear);
        return true;



    }
    
    public bool AssignMaterial(ItemStack material)
    {
        if (materialSlot != null && materialSlot.materialStack.quantity < 1)
        {
            materialSlot.SetMaterial(material);
            return true;
        }
        else if (materialSlot != null && materialSlot.materialStack.quantity >= 1)
        {
            materialSlot.materialStack.quantity = materialSlot.materialStack.quantity + material.quantity;
            materialSlot.UpdateUi();
            return true;
        }
        else
        {
            return false;
        }
    }

}
