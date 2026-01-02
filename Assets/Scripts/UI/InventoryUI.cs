using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private List<SlotUI> slots = new();
    [SerializeField] private Canvas menu;
    private SlotUI draggedSlot;
    private Image draggedIcon;
    private bool dragSingle;
    private int pageNumber;
    private int maxPage;
    int numberOfItemPerPage = 27;

    void Start()
    {
        pageNumber = 1;
        if (GameManager.instance.playerController == null)
        {
            Debug.LogError("⚠️ UI: Le `player` n'est pas assigné !");
        }
        else if (GameManager.instance.playerController.inventory == null)
        {
            Debug.LogError("⚠️ UI: `player.inventory` est null !");
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift)) { dragSingle = true; } else {  dragSingle = false; }
        Refresh();
    }



    void Refresh()
    {
        var inventory = GameManager.instance.playerController.inventory;

        // Calcul du nombre de pages
        int inventoryCountWithoutToolbar = inventory.slots.Count - 9;
       
        if (inventoryCountWithoutToolbar <= 0) maxPage = 1;
        else maxPage = Mathf.CeilToInt((float)inventoryCountWithoutToolbar / numberOfItemPerPage);

        pageNumber = Mathf.Clamp(pageNumber, 1, maxPage);

        // --- GESTION DE LA TOOLBAR  ---
        for (int i = 0; i < 9; i++)
        {
            if (i < inventory.slots.Count)
            {
                if (inventory.slots[i].itemName != "")
                    slots[i].SetItem(inventory.slots[i]);
                else
                    slots[i].SetEmpty();
            }
            else
            {
                slots[i].SetEmpty();
            }
        }

        // --- GESTION DES PAGES  ---

        // Calcul de l'index de départ (27 item par page)
        // Ex: Page 1 = 9 + (0 * 27) = 9
        // Ex: Page 2 = 9 + (1 * 27) = 36
        int startDataIndex = 9 + ((pageNumber - 1) * numberOfItemPerPage);

        for (int i = 0; i < numberOfItemPerPage; i++)
        {
            int uiSlotIndex = 9 + i;        // L'index dans UI (9 à 35)
            int dataIndex = startDataIndex + i; // L'index dans inventory

            // SÉCURITÉ CRITIQUE : Vérifier si l'index de donnée existe réellement
            if (dataIndex < inventory.slots.Count)
            {
                if (inventory.slots[dataIndex].itemName != "")
                    slots[uiSlotIndex].SetItem(inventory.slots[dataIndex]);
                else
                    slots[uiSlotIndex].SetEmpty();
            }
            else
            {
                slots[uiSlotIndex].SetEmpty();
            }
        }
    }
    //public void Remove()
    //{

    //    Item itemToDrop = GameManager.instance.itemManager.GetItembyName(GameManager.instance.playerController.inventory.slots[draggedSlot.slotID].itemName);

    //    if (itemToDrop != null)
    //    {

    //        Debug.Log(dragSingle);
    //        if (dragSingle) 
    //        {
    //            GameManager.instance.playerController.DropItem(itemToDrop, 1);

    //            GameManager.instance.playerController.inventory.Remove(draggedSlot.slotID);
    //         }
    //        else
    //        {
    //            GameManager.instance.playerController.DropItem(itemToDrop);
    //            GameManager.instance.playerController.inventory.Remove(draggedSlot.slotID, GameManager.instance.playerController.inventory.slots[draggedSlot.slotID].count);
    //        }
    //    }
    //    draggedSlot = null;

    //}
    public void NextPage()
    {
        Debug.Log("Next :  {pageNumber}");
        if (pageNumber == maxPage)
        {
            pageNumber = 1;
        }
        else
        {
            pageNumber += 1; 
        }
    }

    public void PreviousPage()
    {
        Debug.Log("PreviousPage :  {pageNumber}");
        if (pageNumber == 1)
        {
            pageNumber = maxPage;
        }
        else
        {
            pageNumber -= 1;
        }
    }
    public void SlotBeginDrag(SlotUI slot)
    {
        draggedSlot = slot;
        draggedIcon = Instantiate(draggedSlot.itemIcon);
        draggedIcon.transform.SetParent(menu.transform);
        draggedIcon.raycastTarget = false;
        draggedIcon.rectTransform.sizeDelta = new Vector2(100,100);
        MoveToMousePosition(draggedIcon.gameObject);
        Debug.Log("Start Drag " );
    }

    public void SlotDrag()
    {
        MoveToMousePosition(draggedIcon.gameObject);
        Debug.Log(" Drag " );
    }

    public void SlotEndDrag()
    {
        Destroy(draggedIcon.gameObject);
      
        Debug.Log("End Drag " );
    }

    public void SlotDrop(SlotUI slot)
    {
        int fromIndex = GetRealIndex(draggedSlot.slotID);
        int toIndex = GetRealIndex(slot.slotID);
        int maxCount = GameManager.instance.playerController.inventory.slots.Count;

        if (fromIndex < maxCount && toIndex < maxCount)
        {
            if (dragSingle)
            {
                GameManager.instance.playerController.inventory.Deplace(fromIndex, toIndex, 1);
            }
            else
            {
                GameManager.instance.playerController.inventory.Deplace(fromIndex, toIndex);
            }
        }
        else
        {
            Debug.LogWarning("déplacement hors des limites de l'inventaire");
        }
    }

    // Convertit l'index du slot UI (0-35) en index réel dans l'inventaire (0-999)
    public int GetRealIndex(int uiSlotIndex)
    {
        // Si c'est dans la toolbar (0 à 8), l'index ne change jamais
        if (uiSlotIndex < 9)
        {
            return uiSlotIndex;
        }

        // Sinon, on ajoute le décalage des pages
        // Formule : IndexUI + ((PageActuelle - 1) * nombre d'item par page)
        // Exemple : Slot UI 9 à la page 2 devient : 9 + (1 * 27) = 36
        int offset = (pageNumber - 1) * numberOfItemPerPage;
        return uiSlotIndex + offset;
    }

    private void MoveToMousePosition(GameObject toMove)
    {
        if (menu != null)
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(menu.transform as RectTransform, Input.mousePosition,null, out position);
            toMove.transform.position = menu.transform.TransformPoint(position);
        }
    }
}
