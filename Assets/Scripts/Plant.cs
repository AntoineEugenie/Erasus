using System;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Rigidbody2D))]
public class Plant : MonoBehaviour, IRaycastable
{
    public PlantData data;
    SpriteRenderer spriteRenderer;
    private int growingLevels;
    private int cycleToGrow;
    private int health;
    private Vector3Int position;
    [HideInInspector] public Rigidbody2D rb2d;
    List<Vector3Int> heatZone;


    public PlantState plantState;

    public enum PlantState
    {
        //PREVIEWING,
        GROWING,
        READY_TO_HARVEST,
        //REGROWING,
        DEAD
    }

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = data.growthData.growProgressSprites[0];
        health = data.growthData.maxHealth;
        plantState = PlantState.GROWING;
        growingLevels = 0;
        cycleToGrow = 0;
        position = Vector3Int.FloorToInt(rb2d.position );
        heatZone = new Zone().MakeZone(data.effectsData.temperatureEmissionRadius, position);
        Debug.Log(position);
        Debug.Log(String.Join(", ",heatZone));
        if (GameManager.instance == null || GameManager.instance.tileManager == null)
        {
            Debug.LogError("GameManager ou tileManager n'est pas assigné !");
            return;
        }

        for (int i = 0; i < heatZone.Count; i++)
        {
            GameManager.instance.tileManager.ChangeTemperature(heatZone[i],data.effectsData.temperatureEmission);
        }

    }


    public void Grow()
    {
        CheckCondition();

        if (plantState == PlantState.GROWING)
        {
            cycleToGrow++;

            if (cycleToGrow == data.growthData.numberOfCycleToGrow)
            {
                if (growingLevels < data.growthData.growProgressSprites.Length - 1)
                {

                    growingLevels++;
                    spriteRenderer.sprite = data.growthData.growProgressSprites[growingLevels];
                    if (growingLevels == data.growthData.growProgressSprites.Length - 1)
                    {
                        plantState = PlantState.READY_TO_HARVEST;
                    }
                }
                cycleToGrow = 0;
            }
        }

    }

    public void DropFruit()
    {
        if (plantState == PlantState.READY_TO_HARVEST)
        {
            Item item = GameManager.instance.itemManager.GetItembyName(data.inventoryData.itemName);
            if (item != null)
            {
                Vector2 spawnLocation = transform.position;
                Vector2 spawnOffset = UnityEngine.Random.insideUnitCircle * 1.25f;

                Item droppedItem = Instantiate(item, spawnLocation + spawnOffset, Quaternion.identity);
                droppedItem.rb2d.AddForce(spawnOffset * 2f, ForceMode2D.Impulse);
                GameManager.instance.tileManager.SetFree(position);
                for (int i = 0; i < heatZone.Count; i++)
                {
                    GameManager.instance.tileManager.ChangeTemperature(heatZone[i], -data.effectsData.temperatureEmission);
                }
                Destroy(gameObject);

            }
        }
        if (plantState == PlantState.DEAD)
        {
            GameManager.instance.tileManager.SetFree(position);
            for (int i = 0; i < heatZone.Count; i++)
            {
                GameManager.instance.tileManager.ChangeTemperature(heatZone[i], -data.effectsData.temperatureEmission);
            }
            Destroy(gameObject);
        }
    }

    public void CheckCondition()
    {
        
        int damage = 0;
        bool allGood = true;
        if (GameManager.instance.tileManager.GetWaterLevel(position) < data.growthData.waterQuantityNeeded)
        {
            damage++;
            allGood = false;
        }
        if (allGood)
        {
            health += data.growthData.healthRecoveredPerCycle;
        }
        else
        {
            health -= damage;
        }
        if (health <= 0)
        {
            plantState = PlantState.DEAD;
            spriteRenderer.sprite = data.growthData.deadSprite;

        }
    }
    public void OnHitByRaycast()
    {
        Debug.Log("Plante touchée ! Récolte en cours...");
        DropFruit(); // Appelle une fonction de récolte
    }


}

