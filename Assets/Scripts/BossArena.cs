using System.Collections.Generic;
using UnityEngine;

public class BossArena : MonoBehaviour
{
    public GameObject wallPrefab; // The wall prefab to be used for the arena
    public GameObject floorPrefab; // The floor prefab to be used for the arena
    public GameObject bossHealthBarCanvas; // The health bar canvas for the boss
    public float radius = 10f; // Radius of the circular arena
    public Vector3 arenaPosition = new Vector3(10000, 10000, 0); // The position where the arena will be generated

    [HideInInspector] public GameObject boss; // Reference to the existing boss

    private HashSet<Vector3> floorPositions = new HashSet<Vector3>(); // To track floor positions

    void Start()
    {
        boss = GameObject.FindGameObjectWithTag("Boss"); // Find the existing boss by tag
        if (boss == null)
        {
            Debug.LogError("Boss not found in the scene!");
            return;
        }

        GenerateArena();
    }

    public void GenerateArena()
    {
        GenerateFloor();
        GenerateWalls();
        MoveBossToArena();
    }

    void GenerateFloor()
    {
        float floorSize = 1f; // Size of each floor tile

        for (float x = -radius; x <= radius; x += floorSize)
        {
            for (float y = -radius; y <= radius; y += floorSize)
            {
                if (x * x + y * y <= radius * radius)
                {
                    Vector3 position = new Vector3(x, y, 0) + arenaPosition;
                    Instantiate(floorPrefab, position, Quaternion.identity).transform.SetParent(transform);
                    floorPositions.Add(position);
                }
            }
        }
    }

    void GenerateWalls()
    {
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };

        foreach (var floorPos in floorPositions)
        {
            foreach (var direction in directions)
            {
                Vector3 adjacentPos = floorPos + direction;
                if (!floorPositions.Contains(adjacentPos))
                {
                    Instantiate(wallPrefab, adjacentPos, Quaternion.identity).transform.SetParent(transform);
                }
            }
        }
    }

    void MoveBossToArena()
    {
        if (boss != null)
        {
            boss.transform.position = arenaPosition; // Move the existing boss to the center of the arena
            boss.transform.SetParent(transform);

            DoubleModeShooting bossScript = boss.GetComponent<DoubleModeShooting>();
            if (bossScript != null && bossHealthBarCanvas != null)
            {
                bossHealthBarCanvas.SetActive(true); // Ensure the health bar canvas is active
                bossScript.healthBarCanvas = bossHealthBarCanvas;
            }
        }
    }

    public Vector3 GetCenter()
    {
        return arenaPosition;
    }
}
