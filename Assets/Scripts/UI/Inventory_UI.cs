using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI : MonoBehaviour
{
    InputAction menu;
    public GameObject inventoryPanel;
    bool isActive;
    public PlayerController player;
    [SerializeField] private List<Slot_UI> slots = new();

    void Start()
    {
        menu = InputSystem.actions.FindAction("Menu");
        menu.performed += ToggleInventory;
        //inventoryPanel.SetActive(false);
    }

    public void ToggleInventory(InputAction.CallbackContext context)
    {
        isActive = !inventoryPanel.activeSelf;
        Refresh();
        inventoryPanel.SetActive(isActive);
    }

    void Refresh()
    {//hmmm
        if (slots.Count == player.inventory.slots.Count)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (player.inventory.slots[i].itemName != "")
                {
                    slots[i].SetItem(player.inventory.slots[i]);
                }
                else
                {
                    slots[i].SetEmpty();
                }
            }
        }
    }
    public void Remove(int slotID)
    {
        Item itemToDrop = GameManager.instance.itemManager.GetItembyName(player.inventory.slots[slotID].itemName);
        if (itemToDrop != null)
        {
            player.DropItem(itemToDrop);
            player.inventory.Remove(slotID);
            Refresh();
        }

    }

}
