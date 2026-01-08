using System.Collections.Generic;
using Manager;
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
        public bool RemoveItem()
        {
            if (count > 0)
            {
                count--;

                if (count == 0)
                {
                    icon = null;
                    itemName = "";
                    return true; // indique que le slot est maintenant vide
                }
            }
            return false;
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

    public void Deplace(int sourceID, int targetID, Inventory targetInventory = null)
    {
        // Si targetInventory est nul, on considère que c'est le même inventaire
        Inventory targetInv = targetInventory ?? this;

        Slot sourceSlot = this.slots[sourceID];
        Slot targetSlot = targetInv.slots[targetID];

        // 1. Si la source est vide, on ne fait rien
        if (string.IsNullOrEmpty(sourceSlot.itemName)) return;

        // 2. TENTATIVE DE STACK (Empilement)
        if (sourceSlot.itemName == targetSlot.itemName)
        {
            int spaceInTarget = targetSlot.maxPerStack - targetSlot.count;
            int amountToMove = Mathf.Min(sourceSlot.count, spaceInTarget);

            if (amountToMove > 0)
            {
                targetSlot.count += amountToMove;
                sourceSlot.count -= amountToMove;

                if (sourceSlot.count <= 0)
                {
                    // On vide le slot source s'il n'y a plus rien
                    this.slots[sourceID] = new Slot(); 
                }
                return; // Fin de l'opération
            }
        }

        // 3. LOGIQUE D'ÉCHANGE (Swap) ou Transfert simple
        // On n'échange que si les inventaires sont identiques ou si la cible est vide
        // Sinon, on intervertit les références des slots
        this.slots[sourceID] = targetSlot;
        targetInv.slots[targetID] = sourceSlot;
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= slots.Count)
        {
            Debug.LogWarning($"Tentative d'acc�s � un slot hors limite: {index}. Taille actuelle: {slots.Count}");
            return;
        }
        else
        {
            selectSlot = slots[index];
            Debug.Log($"Slot {index} s�lectionn�.");
        }


    }

}
