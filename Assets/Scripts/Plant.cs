using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[RequireComponent(typeof(Rigidbody2D))]
public class Plant : MonoBehaviour
{
    public PlantData data;
    SpriteRenderer spriteRenderer;
    private int growingLevels;
    private int cycleToGrow;
    private int health;
    private Vector3Int position;
    [HideInInspector] public Rigidbody2D rb2d;

    public PlantState plantState;

    public enum PlantState
    {
        //PREVIEWING,
        GROWING,
        READY_TO_HARVEST,
        //REGROWING,
        DEAD
    }

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = data.growthData.growProgressSprites[0];
        health = data.growthData.maxHealth;
        plantState = PlantState.GROWING;
        growingLevels = 0;
        cycleToGrow = 0;
        position = Vector3Int.FloorToInt(rb2d.position + Vector2.up * 0.5f);

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
                Vector2 spawnOffset = Random.insideUnitCircle * 1.25f;

                Item droppedItem = Instantiate(item, spawnLocation + spawnOffset, Quaternion.identity);
                droppedItem.rb2d.AddForce(spawnOffset * 2f, ForceMode2D.Impulse);
                GameManager.instance.tileManager.SetFree(position);
                Destroy(gameObject);
            }
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



}

