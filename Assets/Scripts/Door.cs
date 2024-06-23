using UnityEngine;

public class Door : MonoBehaviour
{
    private bool isPlayerNear;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Space))
        {
            // Logic to transition to the next room
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                DungeonGenerator generator = FindObjectOfType<DungeonGenerator>();
                if (generator != null && generator.rooms.Count > 1)
                {
                    // Find the current room of the player
                    Vector3 playerPos = player.transform.position;
                    Room currentRoom = generator.rooms.Find(r => Vector3.Distance(r.center, playerPos) < r.width);
                    int currentIndex = generator.rooms.IndexOf(currentRoom);

                    // Move to the next room if it exists
                    if (currentIndex >= 0 && currentIndex < generator.rooms.Count - 1)
                    {
                        player.transform.position = generator.rooms[currentIndex + 1].center;
                    }
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}
