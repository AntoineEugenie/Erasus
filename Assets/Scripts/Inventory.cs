using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    [System.Serializable]
    public class Slot
    {
        public string itemName;
        public int count;
        public int maxPerStack;
        public Sprite icon;

        public Slot()
        {
            itemName = "";
            count = 0;
            maxPerStack = 0;
        }

        public bool CanAddItem()
        {
            if (count < maxPerStack)
            {
                return true;
            }
            return false;
        }

        public void AddItem(Item item)
        {
            this.itemName = item.data.inventoryData.itemName;
            this.icon = item.data.inventoryData.icon;
            this.maxPerStack = item.data.inventoryData.maxStackSize;
            count++;
        }
        public void RemoveItem()
        {
            if (count > 0)
            {
                count--;
                if (count == 0)
                {
                    icon = null;
                    itemName = "";
                }
            }
        }
    }
    public List<Slot> slots = new();

    public Inventory(int numSlots)
    {
        for (int i = 0; i < numSlots; i++)
        {
            Slot slot = new();
            slots.Add(slot);
        }
    }

    public void Add(Item item)
    {
        foreach (Slot slot in slots) // check if stack already exist 
        {
            if (slot.itemName == item.data.inventoryData.itemName && slot.CanAddItem())
            {
                slot.AddItem(item);
                return;
            }
        }
        foreach (Slot slot in slots) // create if not 
        {
            if (slot.itemName == "")
            {
                slot.AddItem(item);
                return;
            }
        }
    }
    public void Remove(int index)
    {
        slots[index].RemoveItem();
    }
}
