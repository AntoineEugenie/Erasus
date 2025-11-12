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
                player.inventory.Add(item);
            }
            
            Destroy(gameObject);
        }
            //controller.PlaySound(collectedClip);
            
        
    }

}



