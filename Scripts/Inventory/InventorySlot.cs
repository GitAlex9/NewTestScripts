using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Button removeButton;
    public GameObject stack;
    public TextMeshProUGUI stackText;
    public GameObject discardPopup;
    public TextMeshProUGUI discardText;
    public GameObject itemDescription;
    private TextMeshProUGUI itemDescriptionText;
    



    ItemData item;
    ItemStack itemStack;
    UpgradeManager upgradeManager;

    public void AddItem(ItemData newItem, int quantity)
    {
        item = newItem;

        icon.sprite = item.icon;
        icon.enabled = true;
        removeButton.interactable = true;

        if (item.isStackable && quantity > 1)
        {
            stack.SetActive(true);
            stackText.text = quantity.ToString();
        }
        else
        {
            stack.SetActive(false);
            stackText.text = "";
        }


    }

    public void ClearSlot()
    {
        item = null;

        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
        stack.SetActive(false);
        stackText.text = "";

    }
    public void InventoryRemoveButton()
    {
        discardText.text = $"Deseja descartar? {item.name}?";
        discardPopup.SetActive(true);

        //Inventory.Instance.RemoveItem(item);
    }
    public void DiscardYesRemoveButton()
    {

        Inventory.Instance.RemoveItem(item);
        //ClearSlot();
        discardPopup.SetActive(false);
    }
    public void NotDiscard()
    {
        discardPopup.SetActive(false);
    }

    public void ItemDescription()
    {
        //OnMouseOver ou on mouse right click item description set active true e item equipped description tb
        itemDescriptionText.text = ($"{item.itemName} \n {item.itemDescription} \n ");
    }


    public void UseItem()
    {


        if (item != null && item is Consumables)
        {
            ItemStack stack = new ItemStack(item, Inventory.Instance.items.Find(x => x.item == item).quantity);
            bool assigned = HotbarManager.Instance.AssignHotbar(stack);

            if (assigned)
            {
                Inventory.Instance.RemoveItemQuantity(item, stack.quantity);
            }
        }
        else if (item != null && item is UpgradeMaterial)
        {
            ItemStack material = new ItemStack(item, Inventory.Instance.items.Find(x => x.item == item).quantity);
            bool assigned = UpgradeManager.Instance.AssignMaterial(material);

            if (assigned)
            {
                Inventory.Instance.RemoveItemQuantity(item, material.quantity);
            }
        }
        else if (item != null && item is Equippable && UpgradeManager.Instance.upgradeUI.activeSelf == true)
        {
            ItemStack gear = new ItemStack(item, Inventory.Instance.items.Find(x => x.item == item).quantity);       
           
            bool assigned = UpgradeManager.Instance.AssignEquipment(gear);

            if (assigned)
            {
                Inventory.Instance.RemoveItemQuantity(item, gear.quantity);               
            }
        }
        else if (item != null) { item.Use(); }
    
            //else if (item != null && UpgradeManager.Instance.upgradeUI.activeSelf == true)

        }
        
}
