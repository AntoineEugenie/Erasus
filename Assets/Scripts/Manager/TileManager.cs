using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] private Tile plowedTile;
    [SerializeField] private Tilemap map;
    [SerializeField] private List<TileData> tileDatas;

    private Dictionary<TileBase, TileData> dataFromTiles; // Associé aux types de tiles
    private Dictionary<Vector3Int, TileData> positionData; // Associé aux positions spécifiques

    private void Awake()
    {
        dataFromTiles = new();
        positionData = new();

        // Initialiser les données des tiles
        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }
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
        if (positionData.ContainsKey(position))
        {
            var data = positionData[position];
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
        if (!positionData.ContainsKey(position))
        {
            // Crée une nouvelle instance de TileData en utilisant ScriptableObject.CreateInstance
            positionData[position] = ScriptableObject.CreateInstance<TileData>();
        }

        positionData[position].isOccupied = true;

        Debug.Log($"Tile at position {position} is now occupied.");
    }
    public int GetWaterLevel(Vector3Int position)

    {
        TileBase tile = map.GetTile(position);
        TileData tileData = dataFromTiles[tile];
        if (!positionData.ContainsKey(position))
        {
            // Crée une nouvelle instance de TileData en utilisant ScriptableObject.CreateInstance
            positionData[position] = CreateTileDataInstance(position);
        }
        return positionData[position].WaterLevel;
    }

    public void ChangeWaterLevel(Vector3Int position, int amount)
    {
        if (map.GetTile(position) != null)
        {
            if (!positionData.ContainsKey(position))
            {
                // Crée une nouvelle instance de TileData en utilisant ScriptableObject.CreateInstance
                positionData[position] = CreateTileDataInstance(position);
                Debug.Log($"Tile at position is occupied {positionData[position].isOccupied}.");
                Debug.Log($"Tile at position is plowted {positionData[position].isPlowted}.");
            }

            positionData[position].WaterLevel += amount;

            Debug.Log($"Tile at position {position} have {positionData[position].WaterLevel}L.");
        }
    }

    public int GetTemperature(Vector3Int position)
    {
        if (!positionData.ContainsKey(position))
        {
            // Crée une nouvelle instance de TileData en utilisant ScriptableObject.CreateInstance
            positionData[position] = CreateTileDataInstance(position);
        }
        return positionData[position].Temperature;
    }

    public void ChangeTemperature(Vector3Int position, int amount)
    {
        if (map.GetTile(position) != null)
        {
            if (!positionData.ContainsKey(position))
            {
                // Crée une nouvelle instance de TileData en utilisant ScriptableObject.CreateInstance
                positionData[position] = CreateTileDataInstance(position);
            }

            positionData[position].WaterLevel += amount;

            Debug.Log($"Tile at position {position} is now at {positionData[position].Temperature}°c.");
        }
    }

    public void ResetWaterLevel()
    {
        foreach (var tile in positionData)
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
            positionData[position] = newTileData;

            Debug.Log($"Created new TileData instance for position {position}.");
            return newTileData;
        }

        Debug.LogWarning($"No TileData found for tile at position {position}. Cannot create instance.");
        return null;
    }


}
