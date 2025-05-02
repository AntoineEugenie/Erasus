using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Toolbar_UI : MonoBehaviour
{
    [SerializeField] private List<Slot_UI> slots = new();
    private Slot_UI selectedSlots;

   void Update()
    {
        SelectSlot(GameManager.instance.playerController.player.selectSlot);
        
    }

    public void SelectSlot(int index)
    {
        if (slots.Count == 9)
        {
            if (selectedSlots != null)
            {
                selectedSlots.SetHightlight(false);
            }
            selectedSlots = slots[index];
            //Debug.Log("Index: " + index);
            //Debug.Log(selectedSlots);
            selectedSlots.SetHightlight(true);

        }
    }
}
