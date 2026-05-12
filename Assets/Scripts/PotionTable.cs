using Manager;
using UnityEngine;

class PotionTable : MonoBehaviour, IRaycastable
{
    [SerializeField] private GameObject interfaceTable;
    public GameObject createButton;

    private void Update()
    {
        if (!interfaceTable.activeSelf) return;
        
        if (GameManager.instance.playerController.move.magnitude > 0.1f)
        {
            interfaceTable.SetActive(false);
        }
    }

    public void TryAddToRecipeInventory()
    {
        if (string.IsNullOrEmpty(GameManager.instance.craftInventory.slots[0].itemName))
        {
            Debug.LogWarning("Il n'y a pas d'item dans le slot 1");
        }
        else if (string.IsNullOrEmpty(GameManager.instance.craftInventory.slots[1].itemName))
        {
            Debug.LogWarning("Il n'y a pas d'item dans le slot 2");
        }
        else
        {
            GameManager.instance.potionManager.AddRecipeToInventory(GameManager.instance.craftInventory.slots[0].itemName, GameManager.instance.craftInventory.slots[1].itemName);
        }
    }

    private void ToggleTable()
    {
        interfaceTable.SetActive(!interfaceTable.activeSelf);
    }
    
    public void OnHitByRaycast()
    {
        Debug.Log("Ouverture de la table de craft de potion");
        ToggleTable(); 
    }
}