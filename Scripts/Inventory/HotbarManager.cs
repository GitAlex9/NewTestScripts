using System.ComponentModel.Design;
using System.Linq;
using UnityEngine;

public class HotbarManager : MonoBehaviour
{
    public static HotbarManager Instance;

    public HotbarSlot[] hotbarSlots;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                hotbarSlots[i].UseItem();
            }
        }
    }

    public  bool AssignHotbar(ItemStack stack)
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i] != null && hotbarSlots[i].icon.sprite == null) 
            {
                
                hotbarSlots[i].SetItem(stack);
                return true;
            }
        }
        Debug.Log("hotbar full");
        return false;
    }
}