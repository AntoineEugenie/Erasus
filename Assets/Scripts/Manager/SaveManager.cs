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
        Debug.Log("Début de la sauvegarde...");
        GameManager gm = GameManager.instance;
        GameSaveData data = new GameSaveData();

        data.currentSceneName = SceneManager.GetActiveScene().name;
        if (gm.playerController != null)
            data.playerPosition = gm.playerController.transform.position;


        if (gm.playerController != null && gm.playerController.inventory != null)
        {
            foreach (var slot in gm.playerController.inventory.slots)
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
                ActivePlantSaveData pData = new ActivePlantSaveData();
                pData.plantNameID = plant.inventoryData.itemName;
                pData.harvestData = plant.harvestData;
                data.plantsData.Add(pData);
            }
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Sauvegarde terminée !");
    }


    public static void Load()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("Aucune sauvegarde trouvée.");
            return;
        }
        string json = File.ReadAllText(savePath);
        pendingData = JsonUtility.FromJson<GameSaveData>(json);

        SceneManager.sceneLoaded += OnSceneLoaded;

        SceneManager.LoadScene(pendingData.currentSceneName);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        Debug.Log("Scene chargée. Application des données...");
        ApplyData();
    }


    private static void ApplyData()
    {
        GameManager gm = GameManager.instance;


        if (gm.playerController == null)
            Debug.LogError("Pas de player");

     
        if (gm.playerController != null)
        {
            var inventory = gm.playerController.inventory;
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
            gm.plantManager.activePlants.Clear();

            foreach (ActivePlantSaveData savedPlant in pendingData.plantsData)
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

        Debug.Log("Chargement complet terminé !");
    }
}