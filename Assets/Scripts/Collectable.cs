using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Collectable : MonoBehaviour
{
    //public AudioClip collectedClip;

    public int amount;


    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            for (int i = 0; i < amount; i++)
            {
                Item item = GetComponent<Item>();
                player.inventory.Add(item);
            }
            //controller.PlaySound(collectedClip);
            Destroy(gameObject);
        }
    }

}



