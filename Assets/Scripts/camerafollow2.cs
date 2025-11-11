using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camerafollow2 : MonoBehaviour
{
   public Transform target;
    public float followSpeed = 6f;
    public float xOffset = 23.7f;
    public float yOffset = 6.5f;

    // Camera limits (adjust based on your level design)
    public float minX = -9f; // leftmost boundary
    public float maxX = 50f;   // rightmost boundary
    public float minY = 0f;     // bottom limit
    public float maxY = 20f;    // top limit

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position
        Vector3 desiredPosition = new Vector3(target.position.x - xOffset, target.position.y + yOffset, -10f);

        // Clamp to prevent showing background
        float clampedX = Mathf.Clamp(desiredPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(desiredPosition.y, minY, maxY);

        Vector3 smoothPosition = Vector3.Lerp(transform.position, new Vector3(clampedX, clampedY, -10f), followSpeed * Time.deltaTime);
        transform.position = smoothPosition;
    }
}