using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;


public class TileManager : MonoBehaviour
{
    [SerializeField] private Tile plowedTile;
    private Tilemap map;
    string sceneName;
    [SerializeField] private List<TileData> tileDatas;

    private Dictionary<TileBase, TileData> dataFromTiles; // Associé aux types de tiles

    
    private Dictionary<Vector3Int, TileData> positionData; // Associé aux positions spécifiques
    private Dictionary<string, Dictionary<Vector3Int, TileData>> scenemap;

    private void Awake()
    {
        scenemap = new();
        dataFromTiles = new();

        // Initialisation des données des tiles
        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }

        // Abonne l'événement de changement de scène
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Initialisation de la première scène
        InitializeScene();
    }

    private void OnDestroy()
    {
        // Désabonnement pour éviter les erreurs
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeScene();
    }

    private void InitializeScene()
    {
        sceneName = SceneManager.GetActiveScene().name;
        map = GameObject.Find("Ground")?.GetComponent<Tilemap>();

        if (map == null)
        {
            Debug.LogError("Aucun Tilemap nommé 'Ground' trouvé !");
            return;
        }

        if (!scenemap.ContainsKey(sceneName))
        {
            scenemap[sceneName] = new();
            Debug.Log($"Nouvelle scène ajoutée : {sceneName}");
        }

        Debug.Log($"Scène active : {sceneName}, Tilemap récupéré.");
    }
    public bool IsInteractable(Vector3Int position)
    {
        TileBase tile = map.GetTile(position);
        if (tile != null && dataFromTiles.ContainsKey(tile))
        {
            return dataFromTiles[tile].isPlowtable;
        }
        return false;
    }

    public void SetPlowed(Vector3Int position)
    {
        map.SetTile(position, plowedTile);
    }

    public bool CanPlant(Vector3Int position)
    {
        TileBase tile = map.GetTile(position);

        // Vérifie si cette position a des données spécifiques
        if (scenemap[sceneName].ContainsKey(position))
        {
            var data = scenemap[sceneName][position];
            return data.isPlowted && !data.isOccupied;
        }

        // Sinon, vérifie les données générales des tiles
        if (tile != null && dataFromTiles.ContainsKey(tile))
        {
            var data = dataFromTiles[tile];
            return data.isPlowted && !data.isOccupied;
        }

        return false;
    }

    public void SetOccupied(Vector3Int position)
    {
        if (!scenemap[sceneName].ContainsKey(position))
        {
            // Crée une nouvelle instance de TileData en utilisant ScriptableObject.CreateInstance
            scenemap[sceneName][position] = CreateTileDataInstance(position);
        }

        scenemap[sceneName][position].isOccupied = true;

        Debug.Log($"Tile at position {position} is now occupied.");
    }
    public void SetFree(Vector3Int position)
    {
        if (!scenemap[sceneName].ContainsKey(position))
        {
            // Crée une nouvelle instance de TileData en utilisant ScriptableObject.CreateInstance
            scenemap[sceneName][position] = CreateTileDataInstance(position);
        }

        scenemap[sceneName][position].isOccupied = false;
        Debug.Log(scenemap[sceneName][position].isOccupied);
        Debug.Log($"Tile at position {position} is now free.");
    }
    public int GetWaterLevel(Vector3Int position)

    {
        TileBase tile = map.GetTile(position);
        TileData tileData = dataFromTiles[tile];
        if (!scenemap[sceneName].ContainsKey(position))
        {
            // Crée une nouvelle instance de TileData en utilisant ScriptableObject.CreateInstance
            scenemap[sceneName][position] = CreateTileDataInstance(position);
        }
        return scenemap[sceneName][position].WaterLevel;
    }

    public void ChangeWaterLevel(Vector3Int position, int amount)
    {
        if (map.GetTile(position) != null)
        {
            if (!scenemap[sceneName].ContainsKey(position))
            {
                // Crée une nouvelle instance de TileData en utilisant ScriptableObject.CreateInstance
                scenemap[sceneName][position] = CreateTileDataInstance(position);
                Debug.Log($"Created new TileData instance for position {position}.");
            }

            scenemap[sceneName][position].WaterLevel += amount;

            Debug.Log($"Tile at position {position} have {scenemap[sceneName][position].WaterLevel}L.");
        }
    }

    public int GetTemperature(Vector3Int position)
    {
        if (!scenemap[sceneName].ContainsKey(position))
        {
            // Crée une nouvelle instance de TileData en utilisant ScriptableObject.CreateInstance
            scenemap[sceneName][position] = CreateTileDataInstance(position);
            Debug.Log($"Created new TileData instance for position {position}.");
        }
        return scenemap[sceneName][position].Temperature;
    }

    public void ChangeTemperature(Vector3Int position, int amount)
    {
        if (map.GetTile(position) != null)
        {
            if (!scenemap[sceneName].ContainsKey(position))
            {
                // Crée une nouvelle instance de TileData en utilisant ScriptableObject.CreateInstance
                scenemap[sceneName][position] = CreateTileDataInstance(position);
                Debug.Log($"Created new TileData instance for position {position}.");
            }

            scenemap[sceneName][position].Temperature += amount;

            Debug.Log($"Tile at position {position} is now at {scenemap[sceneName][position].Temperature}°c.");
        }
    }

    public void ResetWaterLevel()
    {
        foreach (var tile in scenemap[sceneName])
        {
            Vector3Int position = tile.Key;
            TileBase tileData = map.GetTile(position);
            tile.Value.WaterLevel = dataFromTiles[tileData].WaterLevel;
        }
    }

    public TileData CreateTileDataInstance(Vector3Int position)
    {
        TileBase tile = map.GetTile(position); // Récupère le TileBase à cette position

        if (tile != null && dataFromTiles.ContainsKey(tile))
        {
            // Récupère le TileData général correspondant à la tuile
            TileData baseTileData = dataFromTiles[tile];

            // Crée une nouvelle instance en dupliquant les données de base
            TileData newTileData = ScriptableObject.CreateInstance<TileData>();

            // Copie les propriétés de base
            newTileData.tiles = baseTileData.tiles;
            newTileData.isPlowtable = baseTileData.isPlowtable;
            newTileData.isPlowted = baseTileData.isPlowted;
            newTileData.isOccupied = baseTileData.isOccupied;
            newTileData.Temperature = baseTileData.Temperature;
            newTileData.WaterLevel = baseTileData.WaterLevel;

            // Stocke cette nouvelle instance dans le dictionnaire de données par position
            scenemap[sceneName][position] = newTileData;

            Debug.Log($"Created new TileData instance for position {position}.");
            return newTileData;
        }

        Debug.LogWarning($"No TileData found for tile at position {position}. Cannot create instance.");
        return null;
    }


}
