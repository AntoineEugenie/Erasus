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
        public Slot(Slot other)
        {
            this.itemName = other.itemName;
            this.count = other.count;
            this.maxPerStack = other.maxPerStack;
            this.icon = other.icon;
            this.item = other.item;
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
            this.itemName = item.data.itemName;
            this.icon = item.data.icon;
            this.maxPerStack = item.data.maxStackSize;
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

    public Slot selectSlot = null;

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
            if (slot.itemName == item.data.itemName && slot.CanAddItem())
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
        
        GameManager.instance.playerController.DropItem(item);
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
        if (this.slots[DestinationId].itemName == this.slots[slotId].itemName)
        {
            Deplace(slotId, DestinationId, this.slots[slotId].count);
        }
        else
        {
            Slot temp = this.slots[DestinationId];
            this.slots[DestinationId] = this.slots[slotId];
            this.slots[slotId] = temp;
        }
    }

    public void Deplace(int slotId, int destinationId, int quantity)
    {//currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if (this.slots[destinationId].itemName == this.slots[slotId].itemName)
        {
            quantity = Mathf.Clamp(quantity, 0, this.slots[slotId].count);// ne dépasse le nombre d'item du slot envoyeur
            quantity = Mathf.Clamp(quantity, 0, this.slots[slotId].maxPerStack - this.slots[destinationId].count); // ne dépasse pas le stack max 
            Remove(slotId, quantity);
            this.slots[destinationId].count += quantity;
        }
        if (this.slots[destinationId].itemName == "")
        {
            this.slots[destinationId] = new Slot(this.slots[slotId]); 
            this.slots[destinationId].count = quantity;
            Remove(slotId, quantity);
        }

    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= slots.Count)  
        {
            Debug.LogWarning($"Tentative d'accès à un slot hors limite: {index}. Taille actuelle: {slots.Count}");
            return;
        }
        else {
            selectSlot = slots[index];
            Debug.Log($"Slot {index} sélectionné.");
        }

    
    }

}
