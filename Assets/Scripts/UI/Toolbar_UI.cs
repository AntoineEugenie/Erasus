using System.Collections.Generic;
using Manager;
using UnityEngine;

public class Toolbar_UI : MonoBehaviour
{
    [SerializeField] private List<SlotUI> slots = new();
    private SlotUI _selectedSlots;
    private void Start()
    {
        if (GameManager.instance != null && GameManager.instance.playerInventory != null)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].slotID = i;
                slots[i].parentInventory = GameManager.instance.playerInventory;
            }
        }
        RefreshUI();
        SelectSlot(0);
    }

    //void Update()
    //{
    //    SelectSlot(GameManager.instance.playerController.player.selectSlot);
    //}

    public void SelectSlot(int index)
    {
        if (slots.Count == 9)
        {
            if (_selectedSlots != null)
            {
                _selectedSlots.SetHightlight(false);
            }
            _selectedSlots = slots[index];
            _selectedSlots.SetHightlight(true);
        }
    }
    public void OnToolbarOne()
    {
        SelectSlot(0);
    }

    public void OnToolbarTwo()
    {
        SelectSlot(1);
    }
    public void OnToolbarThree()
    {
        SelectSlot(2);
    }
    public void OnToolbarFour()
    {
        SelectSlot(3);
    }
    public void OnToolbarFive()
    {
        SelectSlot(4);
    }

    public void OnToolbarSix()
    {
        SelectSlot(5);
    }
    public void OnToolbarSeven()
    {
        SelectSlot(6);
    }
    public void OnToolbarEight()
    {
        SelectSlot(7);
    }
    public void OnToolbarNine()
    {
        SelectSlot(8);
    }

    public void RefreshUI()
    {
        if (GameManager.instance.playerInventory == null) return;

        for (int i = 0; i < slots.Count; i++)
        {
            // On vérifie que l'index existe dans les DATA avant de l'afficher dans l'UI
            if (i < GameManager.instance.playerInventory.slots.Count)
            {
                slots[i].SetItem(GameManager.instance.playerInventory.slots[i]);
            }
        }
    }
}
