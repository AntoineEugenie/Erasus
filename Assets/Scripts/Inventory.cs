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
        public Item item;

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
            this.item = item;
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
    public void Remove(int index, int quantity)
    {
        quantity = Mathf.Clamp(quantity, 0, slots[index].count);
        for (int i = 0; i < quantity; i++)
        {
            Remove(index);
        }
    }

    public void Deplace(int slotId, int DestinationId)
    {
        Slot temp = this.slots[DestinationId];
        this.slots[DestinationId] = this.slots[slotId];
        this.slots[slotId] = temp;    
    }

    public void Deplace(int slotId, int DestinationId, int quantity)
    {//currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if (this.slots[DestinationId].itemName == this.slots[slotId].itemName)
        {
            quantity = this.slots[slotId].maxPerStack - Mathf.Clamp(quantity + this.slots[DestinationId].count, 0, this.slots[slotId].maxPerStack);
            Remove(slotId, quantity);
            this.slots[DestinationId].count += quantity;
        }
        if (this.slots[DestinationId].itemName =="")
        {
            this.slots[DestinationId] = this.slots[slotId];
            this.slots[DestinationId].count = quantity;
        }
    }
}
