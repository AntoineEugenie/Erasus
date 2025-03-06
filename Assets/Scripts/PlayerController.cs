using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Windows;
using static Inventory;

public class PlayerController : MonoBehaviour
{
    AudioSource audioSource;

    Animator animator;

    // hitbox
    Rigidbody2D rigidbody2d;
    //  movement

    Vector2 move;
    Vector2 moveDirection;
    public float speed = 3.0f;
    private float sprintMultiplier = 2f;

    public bool isSprinting = false;

    public Inventory inventory;




    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        inventory = new(36);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // FixedUpdate has the same call rate as the physics system
    void FixedUpdate()
    {
        Vector2 position = isSprinting ? (Vector2)rigidbody2d.position + move * (speed * sprintMultiplier) * Time.deltaTime : (Vector2)rigidbody2d.position + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    
    void OnMove(InputValue movementValue)
    {
        move = movementValue.Get<Vector2>();
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }
        animator.SetFloat("X", moveDirection.x);
        animator.SetFloat("Y", moveDirection.y);
        animator.SetFloat("Speed", move.magnitude);
    }
    public void OnJump(InputValue value)
    {
        Watering();
    }
    public void OnInteract(InputValue value)
    {
        if (inventory.selectSlot.itemName != "") {
            Item item = GameManager.instance.itemManager.GetItembyName(inventory.selectSlot.itemName);
            Debug.Log(item.name);
            if (item.data.itemType == ItemType.Seed)
            {
                Planting(item.data.associatedPlant.inventoryData.itemName);
            }
            switch (item.data.action)
            { 
                case Action.Plowting:
                    Plowting();
                    break;

                case Action.Watering:
                    Watering();
                    break;

                default:
                    Debug.LogWarning("Action non reconnue : " + item.data.action);
                    break;
            }
            Harvest();
            
        }
    }
    public void OnCrouch(InputValue value)
    {
        Plowting();
    }
    //public void OnAttack(InputValue value)
    //{
    //    Planting();

    //}
    public void DropItem(Item item)
    {
        Vector2 spawnLocation = transform.position;
        Vector2 spawnOffset = Random.insideUnitCircle * 2;
        Item droppedItem = Instantiate(item, spawnLocation + spawnOffset, Quaternion.identity);
        droppedItem.rb2d.AddForce(spawnOffset * 2f, ForceMode2D.Impulse);
        Debug.Log(item.amount.ToString()+"after");
    }

    public void DropItem(Item item, int quantity)
    {
        if (item != null)
        {
            int temp = item.amount;
            item.amount = quantity;
            DropItem(item);
            Debug.Log(temp.ToString()+"temp");

            item.amount = temp;
            Debug.Log(item.amount.ToString()+"amount after");
        }
        
    }

    void Plowting()
    {

        Vector3Int position = Vector3Int.FloorToInt(rigidbody2d.position + Vector2.up * 0.5f);

        if (GameManager.instance.tileManager.IsInteractable(position))
        {
            GameManager.instance.tileManager.SetPlowed(position);
        }
    }

    void Harvest()
    {
        RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, moveDirection, 1.5f, LayerMask.GetMask("Plant"));
        if (hit.collider != null)
        {
            Debug.Log("dans son front");
            Plant plant = hit.collider.GetComponent<Plant>();
            if (plant != null)
            {
                plant.DropFruit();
            }
        }
    }
    void Watering()
    {
        Vector3Int position = Vector3Int.FloorToInt(rigidbody2d.position + Vector2.up * 0.5f);
        GameManager.instance.tileManager.ChangeWaterLevel(position, 1);

    }

    public void OnSprint()
    {
        isSprinting = !isSprinting;
    }

    void Planting(string plantName)
    {
        Plant Plant = GameManager.instance.plantManager.GetPlantbyName(plantName);
        Debug.Log($"Plant {plantName}");
        Vector3Int intPosition = Vector3Int.FloorToInt(rigidbody2d.position + Vector2.up * 0.5f);
        Vector3 centerPosition = new(intPosition.x + 0.5f, intPosition.y + 0.25f, 0f);
        if (GameManager.instance.tileManager.CanPlant(intPosition))
        {
            inventory.Remove(inventory.slots.IndexOf(inventory.selectSlot));
            Plant newPlant = Instantiate(Plant, centerPosition, Quaternion.identity);
            GameManager.instance.plantManager.RegisterNewAcitvePlant(newPlant);
            GameManager.instance.tileManager.SetOccupied(intPosition);

        }
    }
     

}