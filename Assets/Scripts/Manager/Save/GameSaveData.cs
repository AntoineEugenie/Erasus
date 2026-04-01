using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable] 
public class GameSaveData
{
    // --- player info ---
    public string currentSceneName;
    public Vector3 playerPosition;
    public List<InventorySlotData> inventoryData = new List<InventorySlotData>();

    // --- Plantes ---
    public List<PlantSaveData> plantsData = new List<PlantSaveData>();

    public List<SceneTileSaveData> scenesTileData = new List<SceneTileSaveData>();
}


[System.Serializable]
public struct InventorySlotData
{
    public string itemName; 
    public int count;       
}


[System.Serializable]
public struct PlantSaveData
{
    public string plantNameID; 
    public PlantData.HarvestData harvestData;
}

[System.Serializable]
public class SceneTileSaveData
{
    public string sceneName;
    public List<TileInfo> savedTiles = new List<TileInfo>();
}

// 2. La "Boîte" pour une seule tuile
[System.Serializable]
public struct TileInfo
{
    public Vector3Int position;
    public TileState state; 
}