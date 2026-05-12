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

    public Slot selectSlot;
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
        // Ajout d'une sécurité pour vérifier si l'index est bien dans la liste
        if (index < 0 || index >= slots.Count)
        {
            Debug.LogError($"Tentative de retrait à un index invalide : {index}. Taille inventaire : {slots.Count}");
            return;
        }

        slots[index].RemoveItem();
        GameManager.instance.inventoryUI.Refresh();
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
        // Si targetInventory est nul, on travaille dans le même inventaire
        Inventory targetInv = targetInventory ?? this;

        // Sécurité index
        if (sourceID < 0 || sourceID >= this.slots.Count || targetID < 0 || targetID >= targetInv.slots.Count)
            return;

        Slot sourceSlot = this.slots[sourceID];
        Slot targetSlot = targetInv.slots[targetID];

        // 1. Si la source est vide, rien à déplacer
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
                    // On réinitialise proprement le slot source
                    sourceSlot.itemName = "";
                    sourceSlot.item = null;
                    sourceSlot.icon = null;
                }
                return;
            }
        }

        // 3. ÉCHANGE DE CONTENU (Swap)
        // On crée une copie temporaire des DONNÉES du slot source
        string tempName = sourceSlot.itemName;
        int tempCount = sourceSlot.count;
        int tempMax = sourceSlot.maxPerStack;
        Sprite tempIcon = sourceSlot.icon;
        Item tempItem = sourceSlot.item;

        // On transfère les données de la cible vers la source
        sourceSlot.itemName = targetSlot.itemName;
        sourceSlot.count = targetSlot.count;
        sourceSlot.maxPerStack = targetSlot.maxPerStack;
        sourceSlot.icon = targetSlot.icon;
        sourceSlot.item = targetSlot.item;

        // On transfère les données temporaires (ex-source) vers la cible
        targetSlot.itemName = tempName;
        targetSlot.count = tempCount;
        targetSlot.maxPerStack = tempMax;
        targetSlot.icon = tempIcon;
        targetSlot.item = tempItem;
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= slots.Count)
        {
            Debug.LogWarning($"Tentative d'acc�s � un slot hors limite: {index + 1}. Taille actuelle: {slots.Count}");
        }
        else
        {
            selectSlot = slots[index];
            Debug.Log($"Slot {index + 1} s�lectionn�.");
        }
    }



    public void LoadSlotFromSave(int index, Item itemModel, int quantity)
    {
        if (index < slots.Count)
        {
            slots[index].itemName = itemModel.data.itemName;
            slots[index].icon = itemModel.data.icon;
            slots[index].maxPerStack = itemModel.data.maxStackSize;
            slots[index].item = itemModel;
            slots[index].count = quantity;
        }
    }


    public void ClearSlot(int index)
    {
        if (index < slots.Count)
        {
            slots[index].count = 0;
            slots[index].itemName = "";
            slots[index].icon = null;
            slots[index].item = null;
        }
    }

}
