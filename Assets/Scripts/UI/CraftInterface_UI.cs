using System.Collections.Generic;
using Manager;
using UnityEngine;

public class CraftInterface_UI : MonoBehaviour
{
    [SerializeField] private List<SlotUI> slots = new();
    private SlotUI _selectedSlots;
    private void Start()
    {
        if (GameManager.instance != null && GameManager.instance.playerController.craftInventory != null)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].slotID = i;
                slots[i].parentInventory = GameManager.instance.playerController.craftInventory;
            }
        }
        RefreshUI();
    }
    
    public void RefreshUI()
    {
        if (GameManager.instance.playerController.craftInventory == null) return;

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < GameManager.instance.playerController.craftInventory.slots.Count)
            {
                slots[i].SetItem(GameManager.instance.playerController.craftInventory.slots[i]);
            }
        }
        if  (GameManager.instance.playerController.craftInventory.slots[0].itemName == ""){slots[0].SetEmpty();}
        if  (GameManager.instance.playerController.craftInventory.slots[1].itemName == ""){slots[1].SetEmpty();}
    }
}