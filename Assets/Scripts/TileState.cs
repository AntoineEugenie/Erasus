[System.Serializable]
public class TileState
{
    public bool isPlowted;
    public bool isOccupied;
    public int Temperature;
    public int WaterLevel;

    public TileState(bool plowed, bool occupied, int temp, int water)
    {
        isPlowted = plowed;
        isOccupied = occupied;
        Temperature = temp;
        WaterLevel = water;
    }
}