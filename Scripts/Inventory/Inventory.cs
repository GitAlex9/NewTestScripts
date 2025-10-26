using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public delegate void OnInventoryChanged();
    public OnInventoryChanged onInventoryChangedCallback;

    public delegate void OnEquipmentChanged();
    public OnEquipmentChanged onEquipmentChangedCallback;

    public int slots = 20;
    public List<ItemStack> items = new List<ItemStack>();
    public Equipment[] equipments = new Equipment[3];

    public event Action<int> OnItemEquipped; // Event to notify when an item is equipped
    public event Action<int> OnItemUnequipped; // Event to notify when an item is unequipped

    [System.Serializable]
    private struct SaveData
    {
        public List<ItemStackSave> inventory;
        public List<string> equipment;
    }

    [System.Serializable]
    private struct ItemStackSave
    {
        public string itemName;
        public int quantity;
    }

    #region Inventory Methods

    public bool AddItem(ItemData item)
    {
        if (item.isStackable)
        {
            foreach (ItemStack stack in items)
            {
                if (stack.item == item && stack.quantity < item.maxStack)
                {
                    stack.quantity++;
                    onInventoryChangedCallback?.Invoke();
                    return true;
                }
            }
        }
        if (items.Count < slots)
        {
            items.Add(new ItemStack(item, 1));
            onInventoryChangedCallback?.Invoke();
            return true;
        }
        Debug.Log("Inventory Full");
        return false;
    }

    public void RemoveItem(ItemData item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == item)
            {
                items.RemoveAt(i);
                onInventoryChangedCallback?.Invoke();
                return;
            }
        }
    }

    public bool AddItemQuantity(ItemData item, int quantity) //parte nova, retirar se der b.o
    {
        if (item.isStackable)
        {
            foreach (ItemStack stack in items)
            {
                if (stack.item == item && stack.quantity < item.maxStack)
                {
                    int space = item.maxStack - stack.quantity;
                    int add = Mathf.Min(space, quantity);
                    stack.quantity += add;
                    quantity -= add;
                    if (quantity <= 0)
                    {
                        onInventoryChangedCallback?.Invoke();
                        return true;
                    }
                }
            }
        }

        while (quantity > 0)
        {
            if (items.Count >= slots)
            {
                Debug.Log("Inv full");
                onInventoryChangedCallback?.Invoke();
                return false;
            }

            int stackAmount = item.isStackable ? Mathf.Min(quantity, item.maxStack) : 1;
            items.Add(new ItemStack(item, stackAmount));
            quantity -= stackAmount;
        }
        onInventoryChangedCallback?.Invoke();
        return true;
    }

    public bool RemoveItemQuantity(ItemData item, int quantity)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == item)
            {
                if (items[i].quantity >= quantity)
                {
                    items[i].quantity -= quantity;

                    if (items[i].quantity == 0)
                    {
                        items.RemoveAt(i);
                    }

                    onInventoryChangedCallback?.Invoke();
                    return true;
                }
                else
                {
                    // Quantidade insuficiente nessa pilha
                    quantity -= items[i].quantity;
                    items.RemoveAt(i);
                    i--; // Voltar um índice pois a lista mudou
                }
            }
        }

        onInventoryChangedCallback?.Invoke();
        return quantity <= 0;
    }

    #endregion

    #region Equipment Methods

    public void Equip(Equipment newItem, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= equipments.Length)
        {
            Debug.LogWarning("Invalid equipment slot index.");
            return;
        }

        UnequipItem(slotIndex);
        equipments[slotIndex] = newItem;

        // Invoca o evento de equipar
        OnItemEquipped?.Invoke(slotIndex);
        onEquipmentChangedCallback?.Invoke();
    }

    public void UnequipItem(int slot)
    {
        if (slot < 0 || slot >= equipments.Length)
        {
            Debug.LogWarning("Invalid equipment slot index.");
            return;
        }

        Equipment currentItem = equipments[slot];
        if (currentItem != null)
        {
            AddItem(currentItem);
            equipments[slot] = null;

            // Invoca o evento de desequipar
            OnItemUnequipped?.Invoke(slot);
            onEquipmentChangedCallback?.Invoke();
        }
    }

    public Equipment[] GetAllEquipments()
    {
        return equipments;
    }

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than one instance of inventory found!");
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }
    #endregion

    #region Save/Load System

    /// <summary>
    /// Exporta o inventário e equipamentos para uma string JSON
    /// </summary>
    public string ExportToJson()
    {
        SaveData saveData = new SaveData
        {
            inventory = new List<ItemStackSave>(),
            equipment = new List<string>()
        };

        // Exportar itens do inventário usando itemName como key
        foreach (ItemStack stack in items)
        {
            if (stack.item != null)
            {
                saveData.inventory.Add(new ItemStackSave
                {
                    itemName = stack.item.itemName,
                    quantity = stack.quantity
                });
            }
        }

        // Exportar equipamentos usando itemName
        foreach (Equipment equip in equipments)
        {
            saveData.equipment.Add(equip != null ? equip.itemName : "");
        }

        return JsonUtility.ToJson(saveData, true);
    }

    /// <summary>
    /// Importa o inventário e equipamentos de uma string JSON
    /// </summary>
    public bool ImportFromJson(string json, ItemDB itemDatabase)
    {
        if (string.IsNullOrEmpty(json) || itemDatabase == null) return false;

        try
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            // Limpar inventário
            items.Clear();

            // Importar itens usando itemName como busca
            foreach (var itemSave in saveData.inventory)
            {
                ItemData item = itemDatabase.GetItemByName(itemSave.itemName);
                if (item != null)
                {
                    items.Add(new ItemStack(item, itemSave.quantity));
                }
            }

            // Importar equipamentos
            for (int i = 0; i < saveData.equipment.Count && i < equipments.Length; i++)
            {
                if (!string.IsNullOrEmpty(saveData.equipment[i]))
                {
                    ItemData item = itemDatabase.GetItemByName(saveData.equipment[i]);
                    equipments[i] = item as Equipment;
                }
                else
                {
                    equipments[i] = null;
                }
            }

            onInventoryChangedCallback?.Invoke();
            onEquipmentChangedCallback?.Invoke();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to import inventory: {e.Message}");
            return false;
        }
    }

    #endregion

    
}