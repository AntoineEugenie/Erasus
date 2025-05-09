
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using static Plant;

public class PlantManager : MonoBehaviour
{
    public GameObject basePlantPrefab;
    public PlantData[] plantsLibrary;
    public List<PlantData> activePlants;
    private Dictionary<string, PlantData> nameToPlantDict = new();




    

    private void Awake()
    {
        foreach (PlantData plant in plantsLibrary)
        {
            if (plant == null)
            {
                Debug.LogWarning("Null reference in plantsLibrary");
                continue;
            }
            if (plant.inventoryData.itemName == null)
            {
                Debug.LogError($"Un plant ({plant.name}) a une inventoryData corrompue !");
            }

            AddPlant(plant);
        }
        RegisterActivePlants();
        TimeEvents.newDay.AddListener(GrowAll);

    }

    public void InitializeScene(string sceneName)
    {

        
        foreach (PlantData plant in activePlants)
        {
            Debug.Log(plant.harvestData.sceneName == sceneName);
            if (plant.harvestData.sceneName == sceneName)
            {
                Vector3 intPosition = plant.harvestData.position;
                Vector3 centerPosition = new(intPosition.x + 0.5f, intPosition.y + 0.25f, 0f);
                // Instancie un prefab vide de plante
                GameObject newPlantGO = Instantiate(GameManager.instance.plantManager.basePlantPrefab, centerPosition, Quaternion.identity);

                // Et donne-lui sa data 
                Plant newPlant = newPlantGO.GetComponent<Plant>();
                newPlant.Initialize(plant);
            }
        }
    }

    // Ajoute les données d'une plantes au dictionnaire pour les associés à leurs noms
    private void AddPlant(PlantData plant)
    {
        if (!nameToPlantDict.ContainsKey(plant.inventoryData.itemName))
        {
            nameToPlantDict.Add(plant.inventoryData.itemName, plant);
        }
    }

    // Retourne les donnée des plantes
    public PlantData GetPlantbyName(string name)
    {
        if (nameToPlantDict.ContainsKey(name)   )
        {
            return nameToPlantDict[name];
        }
        return null;
    }

    // Méthode pour enregistrer les plantes comme écouteurs
    private void RegisterActivePlants()
    {
        GameObject[] plants = GameObject.FindGameObjectsWithTag("Plant");
        foreach (GameObject plantObject in plants)
        {
            Plant plant = plantObject.GetComponent<Plant>();
            if (plant != null)
            {
                activePlants.Add(plant.data);
            }
        }
    }

    // Ajoute de nouvelles plantes dynamiquement
    public void RegisterNewAcitvePlant(Plant plant)
    {
        activePlants.Add(plant.data);
    }

    // Fais grandir toutes les plantes
    public void GrowAll()
    {
        activePlants.RemoveAll(item => item == null);
        foreach (var plant in activePlants)
        {
            Grow(plant);
        }
    }

    public void Grow(PlantData data)
    {
        CheckCondition(data);

        if (data.harvestData.plantState == PlantState.GROWING)
        {
            data.harvestData.cycleToGrow++;

            if (data.harvestData.cycleToGrow == data.growthData.numberOfCycleToGrow)
            {
                if (data.harvestData.growingLevels < data.growthData.growProgressSprites.Length - 1)
                {

                    data.harvestData.growingLevels++;
                    
                    if (data.harvestData.growingLevels == data.growthData.growProgressSprites.Length - 1)
                    {
                        data.harvestData.plantState = PlantState.READY_TO_HARVEST;
                    }
                }
                data.harvestData.cycleToGrow = 0;
            }
        }

    }
    //ok theorique 
    public void CheckCondition(PlantData data)
    {

        int damage = 0;
        bool allGood = true;
        if (GameManager.instance.tileManager.GetWaterLevel(data.harvestData.position, data.harvestData.sceneName) < data.growthData.waterQuantityNeeded)
        {
            damage++;
            allGood = false;
        }
        if (allGood)
        {
            data.harvestData.health += data.growthData.healthRecoveredPerCycle;
        }
        else
        {
            data.harvestData.health -= damage;
        }
        if (data.harvestData.health <= 0)
        {
            data.harvestData.plantState = PlantState.DEAD;

        }
    }


















}



