using UnityEngine;

public class ForwardScrollCamera : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Camera Settings")]
    public float followSpeed = 2f;
    public float yOffset = 15f;
    public float zOffset = -6f;
    public float xOffset = 0f;
    public float cameraAngle = 75f;
    public bool lockX = true;

    private float fixedX; // store X position at start if lockX = true

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        transform.rotation = Quaternion.Euler(cameraAngle, 0f, 0f);

        // If X is locked, remember initial X position (so it doesn’t change later)
        if (lockX)
            fixedX = transform.position.x;
    }

    void LateUpdate()
    {
        if (!player) return;

        Vector3 targetPos = transform.position;

        // Smoothly follow player's Z
        targetPos.z = Mathf.Lerp(transform.position.z, player.position.z + zOffset, Time.deltaTime * followSpeed);

        // Maintain height
        targetPos.y = player.position.y + yOffset;

        // Handle X axis logic
        if (lockX)
        {
            // keep same X as when started (plus any set offset)
            targetPos.x = fixedX + xOffset;
        }
        else
        {
            // smoothly follow X if unlocked
            targetPos.x = Mathf.Lerp(transform.position.x, player.position.x + xOffset, Time.deltaTime * followSpeed);
        }

        transform.position = targetPos;
    }
}
