using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBag : MonoBehaviour
{
    public GameObject droppedItemPrefab;
    public List<Loot> lootList = new List<Loot>();
    
    void Update()
    {
        Debug.Log("puila");
    }

    Loot GetDroppedItem()
    {
        int randomnr = Random.Range(1, 101);
        List<Loot> possibleItems = new List<Loot>();
        foreach (Loot item in lootList)
        {
            if (randomnr <= item.dropChance)
            {
                possibleItems.Add(item);
            }
        }
        if (possibleItems.Count > 0)
        {
            Loot droppedItem = possibleItems[Random.Range(0, possibleItems.Count)];
            return droppedItem;
        }
        Debug.Log("No loot dropped");
        return null;
    }

    public void InstantiateLoot(Vector3 spawnPosition)
    {
        Loot droppedItem = GetDroppedItem();
        if (droppedItem != null)
        {
            GameObject lootGameObject = Instantiate(droppedItemPrefab, spawnPosition, Quaternion.identity);
            lootGameObject.GetComponent<SpriteRenderer>().sprite = droppedItem.lootSprite;

            // Attach the appropriate behavior based on loot type
            if (droppedItem.lootName == "Potion")
            {
                lootGameObject.AddComponent<PotionBehavior>();
            }
            else if (droppedItem.lootName == "Hammer")
            {
                lootGameObject.AddComponent<HammerBehavior>();
            }
            else if (droppedItem.lootName == "Amulet")
            {
                lootGameObject.AddComponent<SpeedBoostBehavior>();
            }

            // Add a Collider2D component for collision detection
            lootGameObject.AddComponent<CircleCollider2D>().isTrigger = true;

            // Add the LootPickup script to handle collision detection
            lootGameObject.AddComponent<LootPickup>();
        }
    }
   

}
