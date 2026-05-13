using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class SaveManager
{
    private static string savePath = Application.persistentDataPath + "/sauvegarde_erasus.json";

    private static GameSaveData pendingData;

    public static void DebugPath()
    {
        Debug.Log("Le fichier de sauvegarde sera ici : " + savePath);
    }
    public static void Save()
    {
        Debug.Log("D�but de la sauvegarde...");
        Manager.GameManager gm = Manager.GameManager.instance;
        GameSaveData data = new GameSaveData();

        data.currentSceneName = SceneManager.GetActiveScene().name;
        if (gm.playerController != null)
            data.playerPosition = gm.playerController.transform.position;


        if (gm.playerController != null && gm.playerController.playerInventory != null)
        {
            foreach (var slot in gm.playerController.playerInventory.slots)
            {
                InventorySlotData slotData = new InventorySlotData();
                slotData.itemName = slot.itemName;
                slotData.count = slot.count;
                data.inventoryData.Add(slotData);
            }
        }

        if (gm.plantManager != null)
        {
            foreach (PlantData plant in gm.plantManager.activePlants)
            {
                PlantSaveData pData = new PlantSaveData();
                pData.plantNameID = plant.inventoryData.itemName;
                pData.harvestData = plant.harvestData;
                data.plantsData.Add(pData);
            }
        }

        if (gm.tileManager != null)
        {
            var allScenesData = gm.tileManager.GetAllScenesData();

            foreach (var sceneEntry in allScenesData)
            {
                SceneTileSaveData sceneData = new SceneTileSaveData();
                sceneData.sceneName = sceneEntry.Key;
                foreach (var tileEntry in sceneEntry.Value)
                {
                    TileInfo tInfo = new TileInfo();
                    tInfo.position = tileEntry.Key;
                    tInfo.state = tileEntry.Value; 

                    sceneData.savedTiles.Add(tInfo);
                }

                
                data.scenesTileData.Add(sceneData);
            }
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Sauvegarde termin�e !");
    }


    public static void Load()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("Aucune sauvegarde trouv�e.");
            Save();
            return;
        }
        string json = File.ReadAllText(savePath);
        pendingData = JsonUtility.FromJson<GameSaveData>(json);

        Manager.GameManager.instance.isLoadingSave = true;
        SceneManager.sceneLoaded += OnSceneLoaded;

        SceneManager.LoadScene(pendingData.currentSceneName);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        Debug.Log("Scene charg�e. Application des donn�es...");
        ApplyData();
    }


    private static void ApplyData()
    {
        Manager.GameManager gm = Manager.GameManager.instance;


        if (gm.playerController == null)
            Debug.LogError("Pas de player");

     
        if (gm.playerController != null)
        {
            var inventory = gm.playerController.playerInventory;
            for (int i = 0; i < pendingData.inventoryData.Count; i++)
            {
                InventorySlotData savedSlot = pendingData.inventoryData[i];
                if (savedSlot.count > 0 && !string.IsNullOrEmpty(savedSlot.itemName))
                {
                    Item itemPrefab = gm.itemManager.GetItembyName(savedSlot.itemName);
                    if (itemPrefab != null)
                        inventory.LoadSlotFromSave(i, itemPrefab, savedSlot.count);
                }
                else
                {
                    inventory.ClearSlot(i);
                }
            }


            Rigidbody2D rb = gm.playerController.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.position = pendingData.playerPosition;
                rb.linearVelocity = Vector2.zero;
               
            }
        }


        if (gm.plantManager != null)
        {
            gm.plantManager.DestroyAllPlants();

            foreach (PlantSaveData savedPlant in pendingData.plantsData)
            {
                PlantData originalData = gm.plantManager.GetPlantbyName(savedPlant.plantNameID);
                if (originalData != null)
                {
                    PlantData runtimePlant = ScriptableObject.Instantiate(originalData);
                    runtimePlant.harvestData = savedPlant.harvestData;
                    runtimePlant.inventoryData = originalData.inventoryData;
                    gm.plantManager.activePlants.Add(runtimePlant);
                }
            }

            gm.plantManager.InitializeScene(pendingData.currentSceneName);
        }

        if (gm.tileManager != null)
        {
            
            Dictionary<string, Dictionary<Vector3Int, TileState>> reconstructedMap = new Dictionary<string, Dictionary<Vector3Int, TileState>>();

            foreach (SceneTileSaveData sceneData in pendingData.scenesTileData)
            {
                Dictionary<Vector3Int, TileState> sceneTiles = new Dictionary<Vector3Int, TileState>();

                foreach (TileInfo tInfo in sceneData.savedTiles)
                {
                    sceneTiles[tInfo.position] = tInfo.state;
                }

                reconstructedMap[sceneData.sceneName] = sceneTiles;
            }
            gm.tileManager.LoadAllScenesData(reconstructedMap);
            gm.tileManager.InitializeScene(pendingData.currentSceneName);
        
        }

        Manager.GameManager.instance.isLoadingSave = false;
        InventoryManager.Instance.RefreshAllUIs();
        Debug.Log("Chargement complet termin� !");
    }
}