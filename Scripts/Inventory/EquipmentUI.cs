using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentUI : MonoBehaviour
{
    public Image[] slotImages;

    private void Start()
    {
        Inventory.Instance.onInventoryChangedCallback += UpdateUI;
    }

    void UpdateUI()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            Equipment item = Inventory.Instance.equipments[i];

            if (item != null)
            {
                slotImages[i].sprite = item.icon;
                slotImages[i].enabled = true;
            }
            else
            {
                slotImages[i].sprite = null;
                slotImages[i].enabled = false;
            }
        }
    }
}
