using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public ItemManager itemManager;
    public PlantManager plantManager;
    public TileManager tileManager;
    public PlayerController playerController;
    public UnityEvent newDay;
    public float cycle = 5f;

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
        itemManager = GetComponent<ItemManager>();
        plantManager = GetComponent<PlantManager>();
        tileManager = GetComponent<TileManager>();
        instance.newDay.AddListener(plantManager.GrowAll);
        instance.newDay.AddListener(tileManager.ResetWaterLevel);
    }



    private void Update()
    {
        cycle -= Time.deltaTime;
        if (cycle <= 0)
        {
            newDay.Invoke(); // Invoque l'événement pour signaler un nouveau jour
            cycle = 2f;
        }
    }




}
