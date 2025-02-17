using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Collectable : MonoBehaviour
{
    //public AudioClip collectedClip;
  


    
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
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



