using UnityEngine;


public class GameManager : MonoBehaviour 
{
    public static GameManager instance;
    public ItemManager itemManager;
    public PlantManager plantManager;
    public TileManager tileManager;
    public TimeManager timeManager;
    public PlayerController playerController;
   

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;

        }

        DontDestroyOnLoad(this.gameObject);

        // Vérifiez que tous les managers sont correctement attachés
        tileManager = GetComponent<TileManager>();
        itemManager = GetComponent<ItemManager>();
        plantManager = GetComponent<PlantManager>();
        
        timeManager = GetComponent<TimeManager>();
        if (TimeEvents.newDay == null)
        {
            Debug.LogError("TimeEvents.newDay is null. Ensure it is initialized.");
        }
       


    }








}
