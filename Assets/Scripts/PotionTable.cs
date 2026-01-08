using Manager;
using UnityEngine;

class PotionTable : MonoBehaviour, IRaycastable
{
    [SerializeField]
    private GameObject interfaceTable;

    public void ToggleTable()
    {
        interfaceTable.SetActive(!interfaceTable.activeSelf);
    }
    
    public void OnHitByRaycast()
    {
        Debug.Log("Ouverture de la table de craft de potion");
        ToggleTable(); 
    }
}