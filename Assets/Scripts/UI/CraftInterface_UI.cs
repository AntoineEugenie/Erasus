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
        // 1. On s'assure que l'inventaire de craft existe bien
        if (GameManager.instance.playerController.craftInventory == null) return;

        var craftSlots = GameManager.instance.playerController.craftInventory.slots;

        // 2. On parcourt nos slots d'UI
        for (int i = 0; i < slots.Count; i++)
        {
            // On vérifie que l'index i existe bien dans les données (slots de l'inventaire)
            if (i < craftSlots.Count)
            {
                // Si l'item est vide, on vide visuellement la case
                if (string.IsNullOrEmpty(craftSlots[i].itemName))
                {
                    slots[i].SetEmpty();
                }
                else
                {
                    slots[i].SetItem(craftSlots[i]);
                }
            }
        }
    }
}