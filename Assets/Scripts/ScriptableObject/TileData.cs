using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : ScriptableObject
{
    public TileBase[] tiles;
    public bool isPlowtable;
    public bool isPlowted;
    public bool isOccupied;
    public int Temperature = 0;

    public int WaterLevel = 0;
}
