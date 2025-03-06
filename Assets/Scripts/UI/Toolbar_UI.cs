using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Toolbar_UI : MonoBehaviour
{
    [SerializeField] private List<Slot_UI> slots = new();
    private Slot_UI selectedSlots;
    private void Start()
    {
        SelectSlot(0);
    }

    public void SelectSlot(int index)
    {
        if (slots.Count == 9)
        {   
            if(selectedSlots != null)
            {
               selectedSlots.SetHightlight(false);
            }    
            selectedSlots = slots[index];
            Debug.Log(selectedSlots);
            GameManager.instance.playerController.inventory.SelectSlot(index);
            selectedSlots.SetHightlight(true);
            
        }
    }
    public void OnToolbarOne()
    {
        SelectSlot(0);
    }

    public void OnToolbarTwo()
    {
        SelectSlot(1);
    }
    public void OnToolbarThree()
    {
        SelectSlot(2);
    }
    public void OnToolbarFour()
    {
        SelectSlot(3);
    }
    public void OnToolbarFive()
    {
        SelectSlot(4);
    }

    public void OnToolbarSix()
    {
        SelectSlot(5);
    }
    public void OnToolbarSeven()
    {
        SelectSlot(6);
    }
    public void OnToolbarEight()
    {
        SelectSlot(7);
    }
    public void OnToolbarNine()
    {
        SelectSlot(8);
    }

    

}
