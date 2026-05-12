using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public Toolbar_UI toolbarInventoryUI;
    public CraftInterface_UI craftInventoryUI;
    public InventoryUI inventoryUI;

    void Awake()
    {
        Instance = this;

        // Fallback : si non assigné dans l'Inspector, on le cherche dans la scène (inclut les GO inactifs)
        if (inventoryUI == null)
            inventoryUI = FindAnyObjectByType<InventoryUI>(FindObjectsInactive.Include);
    }

    public void MoveItem(SlotUI sourceUI, SlotUI targetUI)
    {
        Inventory sourceInv = sourceUI.parentInventory;
        Inventory targetInv = targetUI.parentInventory;

        // Récupérer les vrais index (si c'est l'UI principale avec pagination)
        int sourceIdx = sourceUI.slotID;
        int targetIdx = targetUI.slotID;

        // Si on est dans l'InventoryUI principale, il faut convertir l'index
        // car le slot 10 de l'UI n'est pas forcément le slot 10 de la List<Slot>
        if (sourceUI.GetComponentInParent<InventoryUI>())
            sourceIdx = sourceUI.GetComponentInParent<InventoryUI>().GetRealIndex(sourceIdx);

        if (targetUI.GetComponentInParent<InventoryUI>())
            targetIdx = targetUI.GetComponentInParent<InventoryUI>().GetRealIndex(targetIdx);

        // Utilise la méthode Deplace de la classe Inventory pour gérer la logique
        sourceInv.Deplace(sourceIdx, targetIdx, targetInv);

        // Rafraîchir toutes les interfaces
        RefreshAllUIs();
    }

    /// <summary>
    /// Rafraîchit toutes les UIs liées à l'inventaire.
    /// </summary>
    public void RefreshAllUIs()
    {
        if (toolbarInventoryUI) toolbarInventoryUI.RefreshUI();
        if (craftInventoryUI) craftInventoryUI.RefreshUI();
        if (inventoryUI) inventoryUI.Refresh();
    }
}