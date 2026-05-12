using Manager;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Collectable : MonoBehaviour
{
    //public AudioClip collectedClip;
  


    
    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            Item item = GetComponent<Item>();
            for (int i = 0; i < item.amount; i++)
            {
                player.playerInventory.Add(item);
            }
            
            Destroy(gameObject);
            if (GameManager.instance.inventoryUI != null)
            {
                GameManager.instance.inventoryUI.Refresh();
            }
            else
            {
                Debug.LogWarning("Impossible de refresh inventoryUI car il est null dans le GameManager");
            }
            
        }
            //controller.PlaySound(collectedClip);
            
        
    }

}



