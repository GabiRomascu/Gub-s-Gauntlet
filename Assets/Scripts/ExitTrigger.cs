using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    public GameObject winCanvas; // Reference to the win canvas (optional)
    public GameObject arenaPrefab; // The arena prefab to be instantiated
    public Vector3 arenaSpawnPosition = new Vector3(50, 50, 0); // Position where the arena will be generated

    private bool arenaGenerated = false;
    private BossArena arenaGenerator;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!arenaGenerated)
            {
                // Instantiate the arena at the specified position
                GameObject arenaInstance = Instantiate(arenaPrefab, arenaSpawnPosition, Quaternion.identity);
                arenaGenerator = arenaInstance.GetComponent<BossArena>();
                arenaGenerator.arenaPosition = arenaSpawnPosition; // Set the arena's position
                arenaGenerator.GenerateArena();
                arenaGenerated = true;

                // Show the health bar if a boss is spawned
                //if (arenaGenerator != null && arenaGenerator.boss != null)
                //{
                //       Boss bossScript = arenaGenerator.boss.GetComponent<Boss>();
                //   if (bossScript != null)
                //       {
                //             bossScript.ShowHealthBar();
                //         }
                //      }
                //    }

                // Teleport the player to the center of the arena
                if (arenaGenerator != null)
                {
                    other.transform.position = arenaGenerator.GetCenter();
                }

                // Display the win screen (optional)
                if (winCanvas != null)
                {
                    winCanvas.SetActive(true);
                    // Optionally, stop the game time
                    // Time.timeScale = 0;
                }
            }
        }
    }
}

