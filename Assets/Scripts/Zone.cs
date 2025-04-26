using System.Collections.Generic;
using UnityEngine;

public class Zone
{
    public List<Vector3Int> positionList = new();


    public List<Vector3Int> MakeZone(int size, Vector3Int basePosition)
    {
        positionList.Clear();

        for (int x = -size; x <= size; x++)
        {
            for (int y = -size; y <= size; y++)
            {
                // La condition pour un losange (distance de Manhattan)
                if (Mathf.Abs(x) + Mathf.Abs(y) <= size)
                {
                    Vector3Int pos = new Vector3Int(basePosition.x + x, basePosition.y + y, basePosition.z);
                    positionList.Add(pos);
                }
            }
        }

        return positionList;
    }
}
