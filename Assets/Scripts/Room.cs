using UnityEngine;

public class Room
{
    public Vector3 center;
    public int width;
    public int height;

    public Room(Vector3 center, int width, int height)
    {
        this.center = center;
        this.width = width;
        this.height = height;
    }
}