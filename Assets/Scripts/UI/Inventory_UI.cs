using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Inventory;

public class UI : MonoBehaviour
{
    InputAction menu;
    public GameObject inventoryPanel;
    bool isActive;
    [SerializeField] private List<Slot_UI> slots = new();
    [SerializeField] private Canvas canvas;
    private Slot_UI draggedSlot;
    private Image draggedIcon;
    private bool dragSingle;

    void Start()
    {
        menu = InputSystem.actions.FindAction("Menu");
        menu.performed += ToggleInventory;
        inventoryPanel.SetActive(false);
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

    public void ToggleInventory(InputAction.CallbackContext context)
    {
        isActive = !inventoryPanel.activeSelf;
        
        inventoryPanel.SetActive(isActive);
    }

    void Refresh()
    {//hmmm


        if (slots.Count == GameManager.instance.playerController.inventory.slots.Count)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (GameManager.instance.playerController.inventory.slots[i].itemName != "")
                {
                    slots[i].SetItem(GameManager.instance.playerController.inventory.slots[i]);
                }
                else
                {
                    slots[i].SetEmpty();
                }
            }
        }
    }
    public void Remove()
    {
        
        Item itemToDrop = GameManager.instance.itemManager.GetItembyName(GameManager.instance.playerController.inventory.slots[draggedSlot.slotID].itemName);

        if (itemToDrop != null)
        {

            Debug.Log(dragSingle);
            if (dragSingle) 
            {
                GameManager.instance.playerController.DropItem(itemToDrop, 1);
                
                GameManager.instance.playerController.inventory.Remove(draggedSlot.slotID);
             }
            else
            {
                GameManager.instance.playerController.DropItem(itemToDrop);
                GameManager.instance.playerController.inventory.Remove(draggedSlot.slotID, GameManager.instance.playerController.inventory.slots[draggedSlot.slotID].count);
            }
        }
        draggedSlot = null;

    }

    public void SlotBeginDrag(Slot_UI slot)
    {
        draggedSlot = slot;
        draggedIcon = Instantiate(draggedSlot.itemIcon);
        draggedIcon.transform.SetParent(canvas.transform);
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

    public void SlotDrop(Slot_UI slot)
    {
        if (dragSingle)
        {
            GameManager.instance.playerController.inventory.Deplace(draggedSlot.slotID, slot.slotID,1);
        }
        else
        { GameManager.instance.playerController.inventory.Deplace(draggedSlot.slotID, slot.slotID); }

    }

    private void MoveToMousePosition(GameObject toMove)
    {
        if (canvas != null)
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition,null, out position);
            toMove.transform.position = canvas.transform.TransformPoint(position);
        }
    }
}
