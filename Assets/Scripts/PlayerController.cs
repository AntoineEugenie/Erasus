using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;



public class PlayerController : MonoBehaviour
{
    AudioSource audioSource;

    Animator animator;
    public Player player;

    // hitbox
    Rigidbody2D rigidbody2d;
    //  movement

    Vector2 move;
    Vector2 moveDirection;
    public float speed = 3.0f;
    private float sprintMultiplier = 2f;

    public bool isSprinting = false;

    //[System.NonSerialized]
    public Inventory inventory;
    


    // Start is called before the first frame update
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();


        if (player != null)
        {
            if (player.inventory == null)
            {
                player.inventory = new(36);
                Debug.Log("Inventaire cr�er");
                inventory = player.inventory;
            }
            else
            {
                inventory = player.inventory;
                Debug.Log("Inventaire copier");
            }
            inventory.SelectSlot(player.selectSlot);
            transform.position = player.lastPosition;


        }
        else {
            Debug.LogWarning("Les player data sont nulles");
        }
        

    }

    // Update is called once per frame
    private void Update()
    {
        player.lastPosition = transform.position;
    }


    // FixedUpdate has the same call rate as the physics system
    void FixedUpdate()
    {
        Vector2 position = isSprinting ? (Vector2)rigidbody2d.position + move * (speed * sprintMultiplier) * Time.deltaTime : (Vector2)rigidbody2d.position + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    
    public void OnMove(InputValue movementValue)
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

    public void OnAttack(InputValue value)
    {
        Harvest();
    }
    public void OnJump(InputValue value)
    {
        Watering();
    }
    public void OnInteract(InputValue value)
    {
        if (inventory.selectSlot.itemName != "")
        {
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
            
        }
        int layerMask = ~(LayerMask.GetMask("Player", "Confiner"));
        RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, moveDirection, 1.5f, layerMask);
        if (hit.collider != null)
        {
            IRaycastable raycastable = hit.collider.GetComponent<IRaycastable>();
            if (raycastable != null)
            {
                raycastable.OnHitByRaycast();
            }
        }


    }
  
    
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

        Vector3Int position = Vector3Int.FloorToInt(rigidbody2d.position); //+ Vector2.up * 0.5f);

        if (GameManager.instance.tileManager.IsInteractable(position, player.lastScene))
        {
            GameManager.instance.tileManager.SetPlowed(position, player.lastScene);
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
        GameManager.instance.tileManager.ChangeWaterLevel(position, 1, player.lastScene);

    }

    public void OnSprint()
    {
        isSprinting = !isSprinting;
    }

    void Planting(string plantName)
    {
        PlantData plantData = GameManager.instance.plantManager.GetPlantbyName(plantName);
        Debug.Log($"Data : {plantData} de  {plantName} ");
        

        Vector3Int intPosition = Vector3Int.FloorToInt(rigidbody2d.position + Vector2.up * 0.5f);
        Vector3 centerPosition = new(intPosition.x + 0.5f, intPosition.y + 0.25f, 0f);

        if (GameManager.instance.tileManager.CanPlant(intPosition, player.lastScene))
        {
            inventory.Remove(inventory.slots.IndexOf(inventory.selectSlot));

            // Instancie un prefab vide de plante
            GameObject newPlantGO = Instantiate(GameManager.instance.plantManager.basePlantPrefab, centerPosition, Quaternion.identity);

            // Et donne-lui sa data 
            Plant newPlant = newPlantGO.GetComponent<Plant>();
            newPlant.Initialize(plantData);

            GameManager.instance.plantManager.RegisterNewAcitvePlant(newPlant);
            GameManager.instance.tileManager.SetOccupied(intPosition, player.lastScene);
        }
    }


    public void OnToolbarOne()
    {   
        player.selectSlot = 0;
        inventory.SelectSlot(0);
    }

    public void OnToolbarTwo()
    {
        player.selectSlot = 1;
        inventory.SelectSlot(1);
    }
    public void OnToolbarThree()
    {
        player.selectSlot = 2;
        inventory.SelectSlot(2);
    }
    public void OnToolbarFour()
    {
        player.selectSlot = 3;
        inventory.SelectSlot(3);
    }
    public void OnToolbarFive()
    {
        player.selectSlot = 4;
        inventory.SelectSlot(4);
    }

    public void OnToolbarSix()
    {
        player.selectSlot = 5;
        inventory.SelectSlot(5);
    }
    public void OnToolbarSeven()
    {   
        player.selectSlot = 6;
        inventory.SelectSlot(6);
    }
    public void OnToolbarEight()
    {
        player.selectSlot = 7;
        inventory.SelectSlot(7);
    }
    public void OnToolbarNine()
    {
        player.selectSlot = 8;
        inventory.SelectSlot(8);
    }

}