using UnityEngine;


[CreateAssetMenu]
public class PlantData : ScriptableObject
{
    [SerializeField] public GrowthData growthData;
    [Space(25)]
    [SerializeField] public InventoryData inventoryData;
    public  HarvestData harvestData;
    [Space(25)]

    // [Space(25)]

    // [SerializeField]
    // private HarvestData harvestData;

    [Space(25)]

    [SerializeField]
    public EffectsData effectsData;

    [System.Serializable]
    public struct InventoryData
    {
        public Sprite inventoryIcon;
        public string itemName;
        [TextArea]
        public string description;
        [Min(1)]
        public int maxStackSize;

    }
    [System.Serializable]
    public struct GrowthData
    {
        [Tooltip("Number of cycle to grow to the next step")]
        [Min(0)]
        public int numberOfCycleToGrow;
        public Sprite[] growProgressSprites;
        // à changer en Sprite[] lorsque tous les sprites seront pret
        public Sprite deadSprite;
        [Space(10)]

        public int minTemperature;
        public int maxTemperature;

        [Space(10)]

        // [Min(0)]
        // public int proximityRadius;
        // [Min(0)]
        // public int maxNumberOfPlantsInProximity;

        // [Space(10)]

        [Min(0)]
        public int waterQuantityNeeded;
        [Min(1)]
        [Tooltip("The number of cycles it takes for this plant to reset its water level")]
        public int waterFrequencyNeeded;

        [Space(10)]

        //[Min(0)]
        public int maxHealth;
        public int healthRecoveredPerCycle;
        public bool isCopy;
    }
    [System.Serializable]
    public struct HarvestData
    {

        public PlantState plantState;
        public int growingLevels;
        public int cycleToGrow;
        public int health;
        public Vector3Int position;
        public string sceneName;

    }

    [System.Serializable]
    public struct EffectsData
    {
        public int temperatureEmission;
        [Min(0)]
        public int temperatureEmissionRadius;


        //     [Space(10)]

        //     [Tooltip("Amount of water that will be added or removed")]
        //     public int _waterChangeAmount;
        //     [Min(0)]
        //     public int _waterChangeRadius;

        //     [Space(10)]

        //     [Tooltip("The plant will copy the effect of other plants in this radius")]
        //     [Min(0)]
        //     public int _copyEffectsRadius;

        //     [Space(10)]

        //     [Tooltip("The temperature around this plant will be set to this value regardless of other temperature sources")]
        //     public int _temperatureOverwriteValue;
        //     [Min(0)]
        //     public int _temperatureOverwriteRadius;

    }
}
public enum PlantState
{
    //PREVIEWING,
    GROWING,
    READY_TO_HARVEST,
    //REGROWING,
    DEAD
}