using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public Item[] items;
    private Dictionary<string, Item> nameToItemDict = new();

    private void Awake()
    {
        foreach (Item item in items)
        {
            AddItem(item);
        }
    }

    private void AddItem(Item item)
    {
        if (!nameToItemDict.ContainsKey(item.data.itemName))
        {
            nameToItemDict.Add(item.data.itemName, item);
        }
    }

    public Item GetItembyName(string name)
    {
        //Debug.Log(name);
        //Debug.Log($"contains: {nameToItemDict.ContainsKey(name)}");
        //Debug.Log("Dict: " + string.Join(", ", nameToItemDict));


        if (nameToItemDict.ContainsKey(name))
        {
            //Debug.Log("name to dict", nameToItemDict[name]);
            return nameToItemDict[name];
        }
        return null;
    }
}
