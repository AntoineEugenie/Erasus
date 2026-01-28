using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class GameSaveData
{
    // --- player info ---
    public string currentSceneName;
    public Vector3 playerPosition;
    public List<InventorySlotData> inventoryData = new List<InventorySlotData>();

    // --- Plantes ---
    public List<ActivePlantSaveData> plantsData = new List<ActivePlantSaveData>();
}


[System.Serializable]
public struct InventorySlotData
{
    public string itemName; 
    public int count;       
}


[System.Serializable]
public struct ActivePlantSaveData
{
    public string plantNameID; 
    public PlantData.HarvestData harvestData;
}
