using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTransform; // Assign this via the Unity editor
    public Vector3 offset; // Optional offset for the camera

    void LateUpdate()
    {
        if (playerTransform != null)
        {
            // Update the camera's position to follow the player, considering the offset
            transform.position = playerTransform.position + offset;
        }
    }
}
