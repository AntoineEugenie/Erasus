using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI quantityText;
    [SerializeField] private GameObject highlight;
    [SerializeField] private Sprite baseItemIcon;

    [Header("Data")]
    public int slotID; 
    public Inventory parentInventory; // L'inventaire (Toolbar ou Craft) auquel ce slot appartient

    // --- TES ANCIENNES FONCTIONS (GARDÉES ET NETTOYÉES) ---

    public void SetItem(Inventory.Slot slot)
    {
        // Si le slot d'inventaire est vide ou n'a pas d'item
        if (slot == null || string.IsNullOrEmpty(slot.itemName) || slot.count <= 0)
        {
            SetEmpty();
            return;
        }

        itemIcon.sprite = slot.icon != null ? slot.icon : baseItemIcon;
        itemIcon.color = Color.white;
        quantityText.text = slot.count > 1 ? slot.count.ToString() : ""; // N'affiche "1" que si nécessaire
    }

    public void SetEmpty()
    {
        itemIcon.sprite = baseItemIcon;
        itemIcon.color = new Color(1, 1, 1, 0); // Optionnel: mettre l'alpha à 0 si pas d'icône
        quantityText.text = "";
    }

    public void SetHightlight(bool isOn)
    {
        if(highlight != null) highlight.SetActive(isOn);
    }

    // --- NOUVELLES FONCTIONS : GESTION DU DRAG & DROP ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        // On ne peut drag que s'il y a un item
        if (itemIcon.sprite == baseItemIcon || itemIcon.color.a == 0) return;

        itemIcon.raycastTarget = false; // Permet de détecter ce qu'il y a DERRIÈRE l'icône pendant qu'on glisse
        itemIcon.transform.SetParent(GameObject.FindGameObjectWithTag("MainCanvas").transform); // Sortir l'icône du slot pour qu'elle passe au dessus de tout
    }

    public void OnDrag(PointerEventData eventData)
    {
        itemIcon.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        itemIcon.raycastTarget = true;
        itemIcon.transform.SetParent(transform); // Remet l'icône dans son slot parent
        itemIcon.transform.localPosition = Vector2.zero; // Recentrer

        // Vérifier si on a relâché sur un autre slot
        GameObject droppedOn = eventData.pointerCurrentRaycast.gameObject;
        
        // Si l'objet touché est un SlotUI (ou un de ses enfants comme l'image)
        if (droppedOn != null)
        {
            SlotUI targetSlot = droppedOn.GetComponentInParent<SlotUI>();
            if (targetSlot != null && targetSlot != this)
            {
                // On appelle la logique de déplacement globale
                InventoryManager.Instance.MoveItem(this, targetSlot);
            }
        }
    }
}