using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item Data")]
public class ItemData : ScriptableObject
{
    public Sprite icon;
    public string itemName;
    public ItemType itemType;
    [TextArea] public string description;
    [Min(1)] public int maxStackSize = 1;

    [ShowIf("IsSeed")] 
    public PlantData associatedPlant;
    [DisableIf("IsntTool")][HideIf("IsntTool")] 
    public Action action;
    private bool IsSeed() => itemType == ItemType.Seed;
    private bool IsntTool() => itemType != ItemType.Tool;
}


public enum ItemType
{
    Seed,
    Tool,
    Object
}
public enum Action
{
    None,
    Plowting,
    Watering,
    Digging
}