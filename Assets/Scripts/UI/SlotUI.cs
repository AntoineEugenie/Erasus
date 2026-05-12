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
    public Inventory parentInventory;

    // Parent d'origine de l'icône, sauvegardé avant le drag
    private Transform _iconOriginalParent;

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
        quantityText.text = slot.count > 1 ? slot.count.ToString() : "";
    }

    public void SetEmpty()
    {
        itemIcon.sprite = baseItemIcon;
        itemIcon.color = new Color(1, 1, 1, 0);
        quantityText.text = "";
    }

    public void SetHightlight(bool isOn)
    {
        if (highlight != null) highlight.SetActive(isOn);
    }

    // --- GESTION DU DRAG & DROP ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        // On ne peut drag que s'il y a un item
        if (itemIcon.sprite == baseItemIcon || itemIcon.color.a == 0) return;

        // Désactive le raycast pour détecter le slot cible derrière l'icône
        itemIcon.raycastTarget = false;

        // Sauvegarde le parent d'origine et remonte l'icône au Canvas racine
        // pour qu'elle passe visuellement au-dessus de tout
        _iconOriginalParent = itemIcon.transform.parent;
        GameObject mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
        if (mainCanvas != null)
            itemIcon.transform.SetParent(mainCanvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        itemIcon.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Remet toujours l'icône dans son slot d'origine, même si le drop échoue
        itemIcon.raycastTarget = true;
        if (_iconOriginalParent != null)
            itemIcon.transform.SetParent(_iconOriginalParent, false);
        itemIcon.transform.localPosition = Vector2.zero;
        _iconOriginalParent = null;

        // Vérifier si on a relâché sur un autre slot
        GameObject droppedOn = eventData.pointerCurrentRaycast.gameObject;
        if (droppedOn != null)
        {
            SlotUI targetSlot = droppedOn.GetComponentInParent<SlotUI>();
            if (targetSlot != null && targetSlot != this)
            {
                InventoryManager.Instance.MoveItem(this, targetSlot);
            }
        }
    }
}
