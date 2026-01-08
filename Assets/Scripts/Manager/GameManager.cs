using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class GameManager : MonoBehaviour 
    {
        public static GameManager instance;
        public ItemManager itemManager;
        public PlantManager plantManager;
        public TileManager tileManager;
        public TimeManager timeManager;
        public PotionManager potionManager;
        public PlayerController playerController;
        public Inventory playerInventory;
        public Inventory craftInventory;
        public InventoryManager inventoryManager;
        
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
            potionManager = GetComponent<PotionManager>();
            timeManager = GetComponent<TimeManager>();
            inventoryManager = GetComponent<InventoryManager>();
            
            // Création des inventaires
            playerInventory = new Inventory(9);
            craftInventory = new Inventory(2);
            
            if (TimeEvents.newDay == null)
            {
                Debug.LogError("TimeEvents.newDay is null. Ensure it is initialized.");
            }

            SceneManager.sceneLoaded += OnSceneLoaded;

            // Initialisation de la première scène
            InitializeScene(SceneManager.GetActiveScene().name);

        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            InitializeScene(scene.name);
        }


        private void InitializeScene(string sceneName)
        {   
        
            tileManager.InitializeScene(sceneName);
            plantManager.InitializeScene(sceneName);
            playerController = GameObject.Find("Player")?.GetComponent<PlayerController>();

            if (playerController == null)
            {
                Debug.LogError("Aucun Player nommé trouvé !");
                return;
            }
            playerController.player.lastScene = sceneName;
            GameObject spawnPoint = GameObject.Find(playerController.player.spawnName);
            if (spawnPoint == null)
            {
                Debug.Log($"Aucun point de spawn nommé {playerController.player.spawnName} trouvé !");
        
            }
            else
            {
                playerController.transform.position = spawnPoint.transform.position;
            }


        }
    }
}
