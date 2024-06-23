using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DungeonType { Caverns, Rooms, WindingHalls }

public class DungeonGenerator : MonoBehaviour
{
    [Header("Values")]
    public float minX, minY, maxX, maxY;
    [Range(50, 1000)] public int totalFloorCount;
    private Vector2 hitSize;
    [Range(1, 100)] public int hallwayPercentage;
    public DungeonType dungeonType;

    [Space]
    [Header("Bools")]
    public bool useRoundedEdges;

    [Space]
    [Header("Lists & Arrays")]
    private List<Vector3> floorList = new List<Vector3>();
    public GameObject[] topWall;
    public GameObject[] randomItems;
    public GameObject wall, floor, exit, doorPrefab, tilePrefab;
    public LayerMask wallMask, floorMask;

    public List<Room> rooms = new List<Room>();
    public GameObject singleShootingEnemyPrefab; // Assign this in the Inspector
    public GameObject shotgunShootingEnemyPrefab; // Assign this in the Inspector

    private void Start()
    {
        Debug.Log("DungeonGenerator started");

        // Initialize bounds
        minX = minY = float.MaxValue;
        maxX = maxY = float.MinValue;

        switch (dungeonType)
        {
            case DungeonType.Caverns: RandomWalker(); break;
            case DungeonType.Rooms: RoomWalker(); break;
            case DungeonType.WindingHalls: HallRoomWalker(); break;
        }
        hitSize = Vector2.one * 0.8f;

        StartCoroutine(DelayProgress());
    }

    private void Update()
    {
        // Check for space key instead of enter to avoid regenerating the map
        if (Application.isEditor && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void RandomWalker()
    {
        Vector3 currentPos = Vector3.zero;
        floorList.Add(currentPos); // Add starting position to the list
        UpdateBounds(currentPos);

        while (floorList.Count < totalFloorCount)
        {
            currentPos += RandomDirection();
            if (!InFloorList(currentPos))
            {
                floorList.Add(currentPos);
                UpdateBounds(currentPos);
            }
        }
        CreateRoomsAndCorridors();
    }

    void RoomWalker()
    {
        Vector3 currentPos = Vector3.zero;
        floorList.Add(currentPos);
        UpdateBounds(currentPos);

        while (floorList.Count < totalFloorCount)
        {
            currentPos = LongWalk(currentPos);
            CreateRoom(currentPos);
        }
        CreateRoomsAndCorridors();
    }

    void HallRoomWalker()
    {
        Vector3 currentPos = Vector3.zero;
        floorList.Add(currentPos);
        UpdateBounds(currentPos);

        while (floorList.Count < totalFloorCount)
        {
            currentPos = LongWalk(currentPos);
            int roll = Random.Range(1, 100);
            if (roll > hallwayPercentage)
            {
                CreateRoom(currentPos);
            }
        }
        CreateRoomsAndCorridors();
    }

    void UpdateBounds(Vector3 position)
    {
        if (position.x < minX) minX = position.x;
        if (position.x > maxX) maxX = position.x;
        if (position.y < minY) minY = position.y;
        if (position.y > maxY) maxY = position.y;
    }

    Vector3 LongWalk(Vector3 myPos)
    {
        Vector3 walkDir = RandomDirection();
        int walkLength = Random.Range(9, 10);

        for (int i = 0; i < walkLength; i++)
        {
            if (!InFloorList(myPos + walkDir))
            {
                floorList.Add(myPos + walkDir);
                UpdateBounds(myPos + walkDir);
            }
            myPos += walkDir;
        }
        return myPos;
    }

    void CreateRoom(Vector3 roomCenter)
    {
        int width = Random.Range(1, 5);
        int height = Random.Range(1, 5);
        Room newRoom = new Room(roomCenter, width, height);

        for (int w = -width; w <= width; w++)
        {
            for (int h = -height; h <= height; h++)
            {
                Vector3 offset = new Vector3(w, h, 0);
                if (!InFloorList(roomCenter + offset))
                {
                    floorList.Add(roomCenter + offset);
                    UpdateBounds(roomCenter + offset);
                }
            }
        }
        rooms.Add(newRoom);
    }

    void CreateRoomsAndCorridors()
    {
        // Create corridors between rooms
        for (int i = 0; i < rooms.Count - 1; i++)
        {
            Vector3 start = rooms[i].center;
            Vector3 end = rooms[i + 1].center;
            CreateCorridor(start, end);
        }

        // Log number of rooms and corridors
        Debug.Log($"Total rooms: {rooms.Count}");
        Debug.Log($"Total corridors created");

        // Instantiate doors at the gaps between rooms
        PlaceDoors();

        StartCoroutine(DelayProgress());
    }

    void CreateCorridor(Vector3 start, Vector3 end)
    {
        Vector3 currentPos = start;
        while (currentPos != end)
        {
            if (!InFloorList(currentPos))
            {
                floorList.Add(currentPos);
                UpdateBounds(currentPos);
            }

            if (currentPos.x < end.x) currentPos.x++;
            else if (currentPos.x > end.x) currentPos.x--;
            else if (currentPos.y < end.y) currentPos.y++;
            else if (currentPos.y > end.y) currentPos.y--;
        }
    }

    void PlaceDoors()
    {
        HashSet<Vector3> doorPositions = new HashSet<Vector3>();

        foreach (Vector3 floorPos in floorList)
        {
            Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };

            foreach (Vector3 dir in directions)
            {
                Vector3 adjacentPos = floorPos + dir;
                Vector3 nextAdjacentPos = adjacentPos + dir;

                if (!InFloorList(adjacentPos) && InFloorList(nextAdjacentPos))
                {
                    if (IsWall(adjacentPos))
                    {
                        doorPositions.Add(adjacentPos);
                    }
                }
            }
        }

        Debug.Log($"Number of doors to be placed: {doorPositions.Count}");
        foreach (Vector3 pos in doorPositions)
        {
            Debug.Log($"Placing door at position: {pos}");
            Instantiate(doorPrefab, pos, Quaternion.identity).transform.SetParent(transform);
        }
    }

    bool InFloorList(Vector3 myPos)
    {
        return floorList.Contains(myPos);
    }

    bool IsWall(Vector3 position)
    {
        Collider2D hit = Physics2D.OverlapBox(position, hitSize, 0, wallMask);
        return hit != null;
    }

    Vector3 RandomDirection()
    {
        switch (Random.Range(1, 5))
        {
            case 1: return Vector3.up;
            case 2: return Vector3.right;
            case 3: return Vector3.down;
            case 4: return Vector3.left;
        }
        return Vector3.zero;
    }

    IEnumerator DelayProgress()
    {
        for (int i = 0; i < floorList.Count; i++)
        {
            GameObject goTile = Instantiate(tilePrefab, floorList[i], Quaternion.identity);
            goTile.name = tilePrefab.name;
            goTile.transform.SetParent(transform);
        }
        while (FindObjectsOfType<TileSpawner>().Length > 0)
        {
            yield return null;
        }
        ExitDoor();

        // Create walls around the dungeon after floor tiles are spawned
        CreateDungeonWalls();

        for (int x = (int)minX - 2; x <= (int)maxX + 2; x++)
        {
            for (int y = (int)minY - 2; y <= (int)maxY + 2; y++)
            {
                RoundedEdges(x, y);
            }
        }

        // Create enemies in each room
        foreach (Room room in rooms)
        {
            Debug.Log($"Spawning enemy in room at: {room.center}");
            SpawnEnemiesInRoom(room);
        }
    }

    void SpawnEnemiesInRoom(Room room)
    {
        if (room == null)
        {
            Debug.LogError("Room is null");
            return;
        }

        Vector3 roomCenter = room.center;
        float offset = 1f;

        // Define two spawn positions to avoid overlapping
        Vector3[] spawnPositions = new Vector3[]
        {
        roomCenter + new Vector3(-offset, -offset, 0),
        roomCenter + new Vector3(offset, offset, 0)
        };

        // Spawn a SingleShootingEnemy at the first position
        Instantiate(singleShootingEnemyPrefab, spawnPositions[0], Quaternion.identity);

        // Spawn a ShotgunShootingEnemy at the second position
        Instantiate(shotgunShootingEnemyPrefab, spawnPositions[1], Quaternion.identity);

        Debug.Log($"Enemies spawned in room at: {roomCenter}");
    }

    void CreateDungeonWalls()
    {
        HashSet<Vector3> wallPositions = new HashSet<Vector3>();

        foreach (Vector3 floorPos in floorList)
        {
            // Check all four directions (up, down, left, right) around each floor tile
            Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
            foreach (Vector3 dir in directions)
            {
                Vector3 wallPos = floorPos + dir;
                if (!InFloorList(wallPos))
                {
                    wallPositions.Add(wallPos);
                }
            }
        }

        foreach (Vector3 pos in wallPositions)
        {
            Instantiate(wall, pos, Quaternion.identity).transform.SetParent(transform);
        }
    }



    void RoundedEdges(int x, int y)
    {
        if (useRoundedEdges)
        {
            Collider2D hitWall = Physics2D.OverlapBox(new Vector2(x, y), hitSize, 0, wallMask);
            if (hitWall)
            {
                Collider2D hitTop = Physics2D.OverlapBox(new Vector2(x, y + 1), hitSize, 0, wallMask);
                Collider2D hitRight = Physics2D.OverlapBox(new Vector2(x + 1, y), hitSize, 0, wallMask);
                Collider2D hitBottom = Physics2D.OverlapBox(new Vector2(x, y - 1), hitSize, 0, wallMask);
                Collider2D hitLeft = Physics2D.OverlapBox(new Vector2(x - 1, y), hitSize, 0, wallMask);
                int bitValue = 0;
                if (!hitTop) { bitValue += 1; }
                if (!hitRight) { bitValue += 2; }
                if (!hitBottom) { bitValue += 4; }
                if (!hitLeft) { bitValue += 8; }

                if (bitValue == 2 || bitValue == 5 || bitValue == 12)
                {
                    GameObject goTop = Instantiate(topWall[Random.Range(0, topWall.Length)], new Vector2(x, y) - Vector2.one, Quaternion.identity);
                    goTop.name = wall.name;
                    goTop.transform.SetParent(transform);
                }
            }
        }
    }

    void ExitDoor()
    {
        Vector3 doorPos = floorList[floorList.Count - 1];
        GameObject goExit = Instantiate(exit, doorPos, Quaternion.identity);
        goExit.name = tilePrefab.name;
        goExit.transform.SetParent(transform);
    }
}