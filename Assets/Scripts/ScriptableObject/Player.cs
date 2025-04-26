using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Player", menuName = "Scriptable Objects/Player")]
public class Player : ScriptableObject
{
    public Inventory inventory;
    public Vector3 lastPosition;
    public string LastScene;
    
}
