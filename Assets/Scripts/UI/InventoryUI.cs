using Manager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private List<SlotUI> slots = new();
    [SerializeField] private Canvas menu;
    [SerializeField] private TMPro.TMP_InputField inputField;
    [SerializeField] private Sprite baseItemIcon;
    private string currentSearchQuery = "";
    private List<int> displayedGridIndices = new List<int>();

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
        else if (GameManager.instance.playerController.playerInventory == null)
        {
            Debug.LogError("⚠️ UI: `playerInventory` est null !");
        }
        else if (GameManager.instance.playerController.craftInventory == null)
        {
            Debug.LogError("⚠️ UI: `craftInventory` est null !");
        }
    }
    
    public void Refresh()
    {
        var inventory = GameManager.instance.playerController.playerInventory;
        displayedGridIndices.Clear();

        // --- 1 : LA TOOLBAR (Ne change jamais) ---
        // On affiche toujours les items 0 à 8
        for (int i = 0; i < 9; i++)
        {
            if (i < inventory.slots.Count && inventory.slots[i].itemName != "")
                slots[i].SetItem(inventory.slots[i]);
            else
                slots[i].SetEmpty();
        }

        // --- 2 : LE FILTRAGE ---
        // On cherche tous les items correspondants dans le RESTE de l'inventaire (à partir de 9)
        List<int> foundIndices = new List<int>();

        for (int i = 9; i < inventory.slots.Count; i++)
        {
            var slot = inventory.slots[i];
            bool match = false;

            // Si la recherche est vide, on prend tout
            if (string.IsNullOrEmpty(currentSearchQuery))
            {
                match = true;
            }
            // Sinon, on vérifie si le nom 
            else if (slot.itemName != "" && slot.itemName.ToLower().Contains(currentSearchQuery))
            {
                match = true;
            }

            if (match)
            {
                foundIndices.Add(i); // On sauvegarde l'INDEX RÉEL (ex: item n°12)
            }
        }

        // --- 3: LA PAGINATION ---
        // On calcule les pages en se basant sur le nombre d'items trouvé
        int count = foundIndices.Count;
        if (count == 0) maxPage = 1;
        else maxPage = Mathf.CeilToInt((float)count / numberOfItemPerPage);

        pageNumber = Mathf.Clamp(pageNumber, 1, maxPage);

        // --- 4 : L'AFFICHAGE DE LA GRILLE ---
        int startOffset = (pageNumber - 1) * numberOfItemPerPage;

        for (int i = 0; i < numberOfItemPerPage; i++) // Boucle sur les 27 cases visuelles
        {
            int uiIndex = 9 + i; // L'index du SlotUI (visuel)
            int resultIndex = startOffset + i; // L'index dans notre liste de résultats filtrés

            if (resultIndex < foundIndices.Count)
            {
                int realIndex = foundIndices[resultIndex]; // On récupère le vrai ID 

                //  "La case i correspond à l'item realIndex"
                displayedGridIndices.Add(realIndex);
                slots[uiIndex].SetItem(inventory.slots[realIndex]);
            }
            else
            {
                // Case vide (fin de page ou pas de résultat)
                displayedGridIndices.Add(-1); // -1 pour le mettre à la fin
                slots[uiIndex].SetEmpty();
            }
        }
    }
    public void Remove()
    {

        Item itemToDrop = GameManager.instance.itemManager.GetItembyName(GameManager.instance.playerController.playerInventory.slots[draggedSlot.slotID].itemName);

        if (itemToDrop != null)
        {

            Debug.Log(dragSingle);
            if (dragSingle) 
            {
                GameManager.instance.playerController.DropItem(itemToDrop, 1);

                GameManager.instance.playerController.playerInventory.Remove(draggedSlot.slotID);
             }
            else
            {
                GameManager.instance.playerController.DropItem(itemToDrop);
                GameManager.instance.playerController.playerInventory.Remove(draggedSlot.slotID, GameManager.instance.playerController.playerInventory.slots[draggedSlot.slotID].count);
            }
        }
        draggedSlot = null;
        Refresh();
    }
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
        Refresh();
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
        Refresh();
    }
    public void SlotBeginDrag(SlotUI slot)
    {
        draggedSlot = slot;
        draggedIcon = Instantiate(draggedSlot.itemIcon);
        draggedIcon.transform.SetParent(menu.transform);
        draggedIcon.raycastTarget = false;
        draggedIcon.rectTransform.sizeDelta = new Vector2(100,100);
        MoveToMousePosition(draggedIcon.gameObject);
    }

    public void SlotDrag()
    {
        MoveToMousePosition(draggedIcon.gameObject);
    }

    public void SlotEndDrag()
    {
        Destroy(draggedIcon.gameObject);
        Refresh();
    }

    public void SlotDrop(SlotUI slot)
    {
        // 1. Récupérer les vrais index de la grille
        int fromIndex = GetRealIndex(draggedSlot.slotID);
        int toIndex = GetRealIndex(slot.slotID);

        // 2. Appeler le déplacement sur les DATA
        // Note: On passe l'inventaire du joueur pour les deux car c'est le même conteneur
        GameManager.instance.playerInventory.Deplace(fromIndex, toIndex);

        // 3. Forcer le rafraîchissement visuel de TOUTES les parties de l'UI
        // (Cela mettra à jour la toolbar ET la grille d'inventaire en même temps)
        Refresh(); 
        if(GameManager.instance.inventoryManager.toolbarInventoryUI)
            GameManager.instance.inventoryManager.toolbarInventoryUI.RefreshUI();
    }

    // Convertit l'index du slot UI (0-35) en index réel dans l'inventaire (0-n)
    public int GetRealIndex(int uiSlotIndex)
    {
        // Toolbar (0-8)
     
        if (uiSlotIndex < 9)
        {
            return uiSlotIndex;
        }

        // Inventaire (9-n)
        
        int indexInGrid = uiSlotIndex - 9; // On ramène l'index de 0 à 26

        if (indexInGrid >= 0 && indexInGrid < displayedGridIndices.Count)
        {
            int realIndex = displayedGridIndices[indexInGrid];

            // Si c'est -1, c'est une case vide générée par le filtre, on renvoie l'index UI par défaut
            // ou on bloque (ici je renvoie l'UI index pour éviter les crashs, mais attention au drop)
            if (realIndex == -1) return uiSlotIndex;

            return realIndex;
        }

        return uiSlotIndex; // Sécurité
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

    public void SearchValueChanged()
    {
        
        // On met en minuscule et on enlève les espaces inutiles pour faciliter la recherche
        currentSearchQuery = inputField.text.ToLower().Trim();
        pageNumber = 1;
        Refresh();
    }

    
}
 