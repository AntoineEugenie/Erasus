using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toolbar_UI : MonoBehaviour
{
    [SerializeField] private List<Slot_UI> slots = new();
    private Slot_UI selectedSlots;

    public void SelectSlot(int index)
    {
        if (slots.Count == 9)
        {
            selectedSlots = slots[index];
            Debug.Log("je suis sur :" + selectedSlots.name);
            
        }
    }

   

}
