using System.Collections.Generic;


using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CollectPoint : MonoBehaviour, IRaycastable
{
    public List<Item> loots;
    int hp;
    public int maxhp;
    Rigidbody2D rigidbody2d;

    private void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        hp = maxhp;
    }
    public void OnHitByRaycast()
    {
        if (hp > 0)
        {
            LootItem();
            hp -= 1;
        }
    }

    public void LootItem()
    {
        int randomIndex = Random.Range(0, loots.Count);
        //Vector3Int intPosition = Vector3Int.FloorToInt(rigidbody2d.position + Vector2.up * 0.5f);
        //Vector3 centerPosition = new(intPosition.x + 0.5f, intPosition.y + 0.25f, 0f);
        Vector2 spawnLocation = transform.position;
        Vector2 spawnOffset = Random.insideUnitCircle;
        Item droppedItem = Instantiate(loots[randomIndex], spawnLocation + spawnOffset, Quaternion.identity);
        droppedItem.rb2d.AddForce(spawnOffset * 2f, ForceMode2D.Impulse);

    }
}


