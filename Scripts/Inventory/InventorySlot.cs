using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Image icon;
    public Button removeButton;
    public GameObject stack;
    public TextMeshProUGUI stackText;
    public GameObject discardPopup;
    public TextMeshProUGUI discardText;
    public GameObject itemDescription;
    public TextMeshProUGUI itemDescriptionText;

    ItemData item;
    ItemStack itemStack;
    UpgradeManager upgradeManager;

    private void Start()
    {
        // Garantir que a descrição esteja desativada no início
        if (itemDescription != null)
        {
            itemDescription.SetActive(false);
        }
        
        // Se itemDescriptionText não foi atribuído no Inspector, tenta encontrar
        if (itemDescriptionText == null && itemDescription != null)
        {
            itemDescriptionText = itemDescription.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

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

        // Desativar a descrição quando o slot for limpo
        if (itemDescription != null)
        {
            itemDescription.SetActive(false);
        }
    }

    public void InventoryRemoveButton()
    {
        discardText.text = $"Deseja descartar? {item.name}?";
        discardPopup.SetActive(true);
    }

    public void DiscardYesRemoveButton()
    {
        Inventory.Instance.RemoveItem(item);
        discardPopup.SetActive(false);
    }

    public void NotDiscard()
    {
        discardPopup.SetActive(false);
    }

    // Método chamado quando o botão é selecionado (navegação por teclado/controller)
    public void OnSelect(BaseEventData eventData)
    {
        ShowItemDescription();
    }

    // Método chamado quando o botão perde a seleção
    public void OnDeselect(BaseEventData eventData)
    {
        HideItemDescription();
    }

    // Método para mostrar a descrição do item
    public void ShowItemDescription()
    {
        if (item != null && itemDescription != null && itemDescriptionText != null)
        {
            itemDescriptionText.text = $"{item.itemName}\n{item.itemDescription}\n";
            itemDescription.SetActive(true);
        }
    }

    // Método para esconder a descrição do item
    public void HideItemDescription()
    {
        if (itemDescription != null)
        {
            itemDescription.SetActive(false);
        }
    }

    // Método manual para ativar/desativar a descrição (útil para mouse)
    public void ToggleItemDescription()
    {
        if (itemDescription != null && item != null)
        {
            if (!itemDescription.activeSelf)
            {
                ShowItemDescription();
            }
            else
            {
                HideItemDescription();
            }
        }
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
        else if (item != null) 
        { 
            item.Use(); 
        }
    }
}