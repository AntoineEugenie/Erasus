using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    [SerializeField] public InventoryData inventoryData;

    [System.Serializable]
    public struct InventoryData
    {
        public Sprite icon;
        public string itemName;
        [TextArea]
        public string description;
        [Min(1)]
        public int maxStackSize;

    }
}
