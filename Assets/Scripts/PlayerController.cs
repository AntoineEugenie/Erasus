using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Windows;

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
    public Inventory toolbar;

    //// interact 
    // InputAction InteractAction;
    //InputAction MoveAction;
    //InputAction SprintAction;
    /////
    // InputAction test;
    // InputAction plowing;
    // InputAction water;
    // InputAction toolbar;

    // Start is called before the first frame update
    void Start()
    {
       // InputSystem.actions.Enable();
       // // get input from imputsystem
       // MoveAction = InputSystem.actions.FindAction("Move");
       // SprintAction = InputSystem.actions.FindAction("Sprint");
       // InteractAction = InputSystem.actions.FindAction("Interact");

       // /////
       // test = InputSystem.actions.FindAction("Attack");
       // plowing = InputSystem.actions.FindAction("Crouch");
       // //water = InputSystem.actions.FindAction("Jump");
       // // assign actions to fonctions
       // SprintAction.performed += ToggleSprint;
       // SprintAction.canceled += ToggleSprint;
       // InteractAction.performed += Harvest;


       // ///
       // test.performed += Plant;
       //// water.performed += Watering;
       // plowing.performed += FindSoil;




        rigidbody2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        inventory = new(27);
        toolbar = new(9);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // FixedUpdate has the same call rate as the physics system
    void FixedUpdate()
    {
        Debug.Log((isSprinting, sprintMultiplier));
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
        Harvest();
    }
    public void OnCrouch(InputValue value)
    {
        Plowting();
    }
    public void OnAttack(InputValue value)
    {
        Planting();

    }
    public void DropItem(Item item)
    {
        if (item != null)
        {
            Vector2 spawnLocation = transform.position;
            Vector2 spawnOffset = Random.insideUnitCircle * 1.25f;

            Item droppedItem = Instantiate(item, spawnLocation + spawnOffset, Quaternion.identity);
            droppedItem.rb2d.AddForce(spawnOffset * 2f, ForceMode2D.Impulse);
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

    void Planting()
    {
        Plant Plant = GameManager.instance.plantManager.GetPlantbyName("Red Carrot");
        Vector3Int intPosition = Vector3Int.FloorToInt(rigidbody2d.position + Vector2.up * 0.5f);
        Vector3 centerPosition = new(intPosition.x + 0.5f, intPosition.y + 0.25f, 0f);
        if (GameManager.instance.tileManager.CanPlant(intPosition))
        {

            Plant newPlant = Instantiate(Plant, centerPosition, Quaternion.identity);
            GameManager.instance.plantManager.RegisterNewAcitvePlant(newPlant);
            GameManager.instance.tileManager.SetOccupied(intPosition);

        }
    }


}