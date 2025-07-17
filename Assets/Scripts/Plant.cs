using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



[RequireComponent(typeof(Rigidbody2D))]
public class Plant : MonoBehaviour, IRaycastable
{

    
    public PlantData data;
    SpriteRenderer spriteRenderer;
    //private int growingLevels;
    //private int cycleToGrow;
    //private int health;
    //private Vector3Int position;
    [HideInInspector] public Rigidbody2D rb2d;
    List<Vector3Int> heatZone;


    void OnEnable()
    {
        TimeEvents.newDay.AddListener(SpriteChanger);
    }

    void OnDisable()
    {
        TimeEvents.newDay.RemoveListener(SpriteChanger);
    }



    public void Initialize(PlantData plantData)
    {
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        heatZone = new Zone().MakeZone(data.effectsData.temperatureEmissionRadius, data.harvestData.position);
        if (plantData.growthData.isCopy)
        {
            data = ScriptableObject.Instantiate(plantData);
            DontDestroyOnLoad(data);
            spriteRenderer.sprite = data.growthData.growProgressSprites[data.harvestData.growingLevels];
            data.harvestData.health = data.growthData.maxHealth;
            data.harvestData.cycleToGrow = 0;
            data.harvestData.position = Vector3Int.FloorToInt(transform.position);
            data.harvestData.sceneName = SceneManager.GetActiveScene().name;
            Debug.Log(SceneManager.GetActiveScene().name + " " + data.harvestData.sceneName);
            data.growthData.isCopy = false;

        }
        else
        {
            data = plantData;
        }
        Debug.Log(data.harvestData.position);
        Debug.Log(String.Join(", ",heatZone));
        if (GameManager.instance == null || GameManager.instance.tileManager == null)
        {
            Debug.LogError("GameManager ou tileManager n'est pas assigné !");
            return;
        }

        for (int i = 0; i < heatZone.Count; i++)
        {
            GameManager.instance.tileManager.ChangeTemperature(heatZone[i],data.effectsData.temperatureEmission, data.harvestData.sceneName);
        }

    }

    void SpriteChanger()
    {
        if(data.harvestData.plantState == PlantState.DEAD)
        {
            //spriteRenderer.sprite = data.growthData.deadSprite;
            spriteRenderer.color = Color.grey;
        }
        else
        {
            spriteRenderer.sprite = data.growthData.growProgressSprites[data.harvestData.growingLevels];
        }
        
    }

    //public void Grow()
    //{
    //    CheckCondition();

    //    if (data.harvestData.plantState == PlantState.GROWING)
    //    {
    //        data.harvestData.cycleToGrow++;

    //        if (data.harvestData.cycleToGrow == data.growthData.numberOfCycleToGrow)
    //        {
    //            if (data.harvestData.growingLevels < data.growthData.growProgressSprites.Length - 1)
    //            {

    //                data.harvestData.growingLevels++;
    //                spriteRenderer.sprite = data.growthData.growProgressSprites[data.harvestData.growingLevels];
    //                if (data.harvestData.growingLevels == data.growthData.growProgressSprites.Length - 1)
    //                {
    //                    data.harvestData.plantState = PlantState.READY_TO_HARVEST;
    //                }
    //            }
    //            data.harvestData.cycleToGrow = 0;
    //        }
    //    }

    //}

    public void DropFruit()
    {
        if (data.harvestData.plantState == PlantState.READY_TO_HARVEST)
        {
            Item item = GameManager.instance.itemManager.GetItembyName(data.inventoryData.itemName);
            if (item != null)
            {
                Vector2 spawnLocation = transform.position;
                Vector2 spawnOffset = UnityEngine.Random.insideUnitCircle * 1.25f;

                Item droppedItem = Instantiate(item, spawnLocation + spawnOffset, Quaternion.identity);
                droppedItem.rb2d.AddForce(spawnOffset * 2f, ForceMode2D.Impulse);
                GameManager.instance.tileManager.SetFree(data.harvestData.position, data.harvestData.sceneName);
                for (int i = 0; i < heatZone.Count; i++)
                {
                    GameManager.instance.tileManager.ChangeTemperature(heatZone[i], -data.effectsData.temperatureEmission, data.harvestData.sceneName);
                }
                GameManager.instance.plantManager.RemoveActivePlant(data.harvestData.position);
                Destroy(gameObject);

            }
        }
        if (data.harvestData.plantState == PlantState.DEAD)
        {
            GameManager.instance.tileManager.SetFree(data.harvestData.position, data.harvestData.sceneName);
            for (int i = 0; i < heatZone.Count; i++)
            {
                GameManager.instance.tileManager.ChangeTemperature(heatZone[i], -data.effectsData.temperatureEmission, data.harvestData.sceneName);
            }
            GameManager.instance.plantManager.RemoveActivePlant(data.harvestData.position);
            Destroy(gameObject);
        }
    }

    //public void CheckCondition()
    //{
        
    //    int damage = 0;
    //    bool allGood = true;
    //    if (GameManager.instance.tileManager.GetWaterLevel(data.harvestData.position) < data.growthData.waterQuantityNeeded)
    //    {
    //        damage++;
    //        allGood = false;
    //    }
    //    if (allGood)
    //    {
    //        data.harvestData.health += data.growthData.healthRecoveredPerCycle;
    //    }
    //    else
    //    {
    //        data.harvestData.health -= damage;
    //    }
    //    if (data.harvestData.health <= 0)
    //    {
    //        plantState = PlantState.DEAD;
    //        spriteRenderer.sprite = data.growthData.deadSprite;

    //    }
    //}
    public void OnHitByRaycast()
    {
        Debug.Log("Plante touchée ! Récolte en cours...");
        DropFruit(); 
    }


}

