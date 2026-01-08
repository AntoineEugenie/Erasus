using System.Collections.Generic;
using Manager;
using UnityEngine;

public class CraftInterface_UI : MonoBehaviour
{
    [SerializeField] private List<SlotUI> slots = new();
    private SlotUI _selectedSlots;
    private void Start()
    {
        if (GameManager.instance != null && GameManager.instance.craftInventory != null)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].slotID = i;
                slots[i].parentInventory = GameManager.instance.craftInventory;
            }
        }
        RefreshUI();
        SelectSlot(0);
    }

    public void SelectSlot(int index)
    {
        if (slots.Count == 3)
        {   
            if(_selectedSlots != null)
            {
                _selectedSlots.SetHightlight(false);
            }    
            _selectedSlots = slots[index];
            _selectedSlots.SetHightlight(true);
            Debug.Log(_selectedSlots);
        }
    }
    
    public void RefreshUI()
    {
        if (GameManager.instance.craftInventory == null) return;

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < GameManager.instance.craftInventory.slots.Count)
            {
                slots[i].SetItem(GameManager.instance.craftInventory.slots[i]);
            }
        }
    }
}