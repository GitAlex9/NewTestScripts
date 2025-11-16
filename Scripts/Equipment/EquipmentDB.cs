using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ItemDB", menuName = "Equipment/ItemDB")]
public class ItemDB : ScriptableObject
{
    [SerializeField] public List<ItemData> items = new List<ItemData>();
    
    private Dictionary<string, ItemData> itemDictionary = new Dictionary<string, ItemData>();
    
    private void OnEnable()
    {
        RebuildDictionary();
    }
    
    private void RebuildDictionary()
    {
        itemDictionary.Clear();
        foreach (var item in items)
        {
            itemDictionary.Add(item.itemName, item);
        }
    }
    
    // Método para buscar item por nome
    public ItemData GetItemByName(string itemName)
    {
        if (itemDictionary.Count == 0 && items.Count > 0)
            RebuildDictionary();

        if (itemDictionary.TryGetValue(itemName, out ItemData item))
            return item;
            
        return null;
    }
    
    // Verificar se item existe
    public bool ContainsItem(string itemName)
    {
        if (itemDictionary.Count == 0 && items.Count > 0)
            RebuildDictionary();
            
        return itemDictionary.ContainsKey(itemName);
    }
       
    
    // Obter todos os itens
    public IEnumerable<ItemData> GetAllItems()
    {
        if (itemDictionary.Count == 0 && items.Count > 0)
            RebuildDictionary();
            
        return itemDictionary.Values;
    }
}
