using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    public Plant[] plantsLibrary;
    public List<Plant> activePlants;
    private Dictionary<string, Plant> nameToPlantDict = new();

    public GameManager gameManager;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
        foreach (Plant plant in plantsLibrary)
        {
            AddPlant(plant);
        }
        RegisterActivePlants();
    }

    private void AddPlant(Plant plant)
    {
        if (!nameToPlantDict.ContainsKey(plant.data.inventoryData.itemName))
        {
            nameToPlantDict.Add(plant.data.inventoryData.itemName, plant);
        }
    }

    public Plant GetPlantbyName(string name)
    {
        if (nameToPlantDict.ContainsKey(name))
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
                activePlants.Add(plant);
            }
        }
    }

    // Si vous ajoutez de nouvelles plantes dynamiquement, appelez cette méthode
    public void RegisterNewAcitvePlant(Plant plant)
    {
        activePlants.Add(plant);
    }
    public void GrowAll()
    {
        activePlants.RemoveAll(item => item == null);
        foreach (var plant in activePlants)
        {
            plant.Grow();
        }
    }
}
