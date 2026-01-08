using Manager;
using UnityEngine;
using UnityEngine.Serialization;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    
    public Toolbar_UI toolbarInventoryUI;
    public CraftInterface_UI craftInventoryUI;
    void Awake()
    {
        Instance = this;
    }

    public void MoveItem(SlotUI sourceUI, SlotUI targetUI)
    {
        Inventory sourceData = sourceUI.parentInventory;
        Inventory targetData = targetUI.parentInventory;

        // VÉRIFICATION DE SÉCURITÉ
        // VÉRIFICATION DE SÉCURITÉ
        if (sourceData == targetData)
        {
            sourceData.Deplace(sourceUI.slotID, targetUI.slotID);
        }
        else if (sourceUI.slotID >= sourceData.slots.Count || targetUI.slotID >= targetData.slots.Count)
        {
            Debug.LogError($"Index hors limite ! Source: {sourceUI.slotID}/{sourceData.slots.Count}, Cible: {targetUI.slotID}/{targetData.slots.Count}");
            return;
        }

        // Récupération des données
        Inventory.Slot itemToMove = sourceData.slots[sourceUI.slotID];
        Inventory.Slot itemAtTarget = targetData.slots[targetUI.slotID];

        // Logique d'échange (Swap)
        Inventory.Slot temp = new Inventory.Slot(itemAtTarget);
        targetData.slots[targetUI.slotID] = new Inventory.Slot(itemToMove);
        sourceData.slots[sourceUI.slotID] = temp;

        // Rafraîchissement
        toolbarInventoryUI.RefreshUI();
        craftInventoryUI.RefreshUI();
    }
}