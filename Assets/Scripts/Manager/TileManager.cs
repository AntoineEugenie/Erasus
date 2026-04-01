using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;



public class TileManager : MonoBehaviour
{
    [SerializeField] private Tile plowedTile;
    private Tilemap map;
    [SerializeField] private List<TileData> tileDatas;

    // Le dictionnaire des règles (ScriptableObjects intacts)
    [SerializeField] private Dictionary<TileBase, TileData> dataFromTiles;

    // 2. MODIFIÉ : On stocke des TileState (légers) au lieu de TileData (lourds)
    private Dictionary<string, Dictionary<Vector3Int, TileState>> scenemap;

    private void Awake()
    {
        scenemap = new Dictionary<string, Dictionary<Vector3Int, TileState>>();
        dataFromTiles = new Dictionary<TileBase, TileData>();

        // Initialisation des données des tiles de base
        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }
    }

    public void InitializeScene(string sceneName)
    {
        map = GameObject.Find("Ground")?.GetComponent<Tilemap>();

        if (map == null)
        {
            Debug.LogError("Aucun Tilemap nommé 'Ground' trouvé !");
            return;
        }

        if (!scenemap.ContainsKey(sceneName))
        {
            scenemap[sceneName] = new Dictionary<Vector3Int, TileState>();
            Debug.Log($"Nouvelle scène ajoutée : {sceneName}");
        }

        // On affiche visuellement les tuiles qui ont été labourées
        foreach (var tile in scenemap[sceneName])
        {
            if (tile.Value.isPlowted)
            {
                map.SetTile(tile.Key, plowedTile);
            }
        }
        Debug.Log($"Scène active : {sceneName}, Tilemap récupéré.");
    }

    public bool IsInteractable(Vector3Int position, string sceneName)
    {
        TileBase tile = map.GetTile(position);
        if (tile != null && dataFromTiles.ContainsKey(tile))
        {
            return dataFromTiles[tile].isPlowtable;
        }
        return false;
    }

    public void SetPlowed(Vector3Int position, string sceneName)
    {
        map.SetTile(position, plowedTile);

        if (!scenemap[sceneName].ContainsKey(position))
        {
            CreateTileState(position, sceneName);
        }

        scenemap[sceneName][position].isPlowted = true;
    }

    public bool CanPlant(Vector3Int position, string sceneName)
    {
        // 1. On regarde s'il y a des données modifiées à cette position
        if (scenemap[sceneName].ContainsKey(position))
        {
            var data = scenemap[sceneName][position];
            return data.isPlowted && !data.isOccupied;
        }

        // 2. Sinon on regarde la règle de base
        TileBase tile = map.GetTile(position);
        if (tile != null && dataFromTiles.ContainsKey(tile))
        {
            var data = dataFromTiles[tile];
            return data.isPlowted && !data.isOccupied;
        }

        return false;
    }

    public void SetOccupied(Vector3Int position, string sceneName)
    {
        if (!scenemap[sceneName].ContainsKey(position)) CreateTileState(position, sceneName);
        scenemap[sceneName][position].isOccupied = true;
    }

    public void SetFree(Vector3Int position, string sceneName)
    {
        if (!scenemap[sceneName].ContainsKey(position)) CreateTileState(position, sceneName);
        scenemap[sceneName][position].isOccupied = false;
    }

    public int GetWaterLevel(Vector3Int position, string sceneName)
    {
        if (!scenemap[sceneName].ContainsKey(position)) CreateTileState(position, sceneName);
        return scenemap[sceneName][position].WaterLevel;
    }

    public void ChangeWaterLevel(Vector3Int position, int amount, string sceneName)
    {
        if (map.GetTile(position) != null)
        {
            if (!scenemap[sceneName].ContainsKey(position)) CreateTileState(position, sceneName);
            scenemap[sceneName][position].WaterLevel += amount;
        }
    }

    public int GetTemperature(Vector3Int position, string sceneName)
    {
        if (!scenemap[sceneName].ContainsKey(position)) CreateTileState(position, sceneName);
        return scenemap[sceneName][position].Temperature;
    }

    public void ChangeTemperature(Vector3Int position, int amount, string sceneName)
    {
        if (map.GetTile(position) != null)
        {
            if (!scenemap[sceneName].ContainsKey(position)) CreateTileState(position, sceneName);
            scenemap[sceneName][position].Temperature += amount;
        }
    }

    public void ResetWaterLevel(string sceneName)
    {
        if (!scenemap.ContainsKey(sceneName)) return;

        foreach (var tile in scenemap[sceneName])
        {
            Vector3Int position = tile.Key;
            TileBase baseTile = map.GetTile(position);

            // On remet au niveau d'eau par défaut du TileData de base
            if (baseTile != null && dataFromTiles.ContainsKey(baseTile))
            {
                tile.Value.WaterLevel = dataFromTiles[baseTile].WaterLevel;
            }
        }
    }

    // 3. MODIFIÉ : Crée une classe légère au lieu d'un ScriptableObject
    private TileState CreateTileState(Vector3Int position, string sceneName)
    {
        TileBase tile = map.GetTile(position);

        if (tile != null && dataFromTiles.ContainsKey(tile))
        {
            TileData baseTileData = dataFromTiles[tile];

            // On copie les valeurs du ScriptableObject vers notre classe légère
            TileState newState = new TileState(
                baseTileData.isPlowted,
                baseTileData.isOccupied,
                baseTileData.Temperature,
                baseTileData.WaterLevel
            );

            scenemap[sceneName][position] = newState;
            return newState;
        }

        // Sécurité par défaut si la tuile n'a pas de règles
        TileState emptyState = new TileState(false, false, 0, 0);
        scenemap[sceneName][position] = emptyState;
        return emptyState;
    }

    // ---------------------------------------------------------
    // FONCTIONS POUR LE SAVE MANAGER
    // ---------------------------------------------------------

    // Pour donner la boîte de données au SaveManager
    public Dictionary<string, Dictionary<Vector3Int, TileState>> GetAllScenesData()
    {
        return scenemap;
    }

    // Pour recevoir la boîte de données du SaveManager
    public void LoadAllScenesData(Dictionary<string, Dictionary<Vector3Int, TileState>> savedData)
    {
        scenemap = savedData;
    }
}