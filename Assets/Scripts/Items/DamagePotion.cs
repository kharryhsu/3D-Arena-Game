using UnityEngine;

public class DamagePotion : MonoBehaviour
{
    [Header("Potion Settings")]
    public float damageBoost = 1.5f;   // 50% more damage
    public float duration = 10f;       // seconds
    public float rotateSpeed = 60f;    // spin speed

    [Header("Audio")]
    public AudioClip pickupSound;

    [Header("Floating Text")]
    public GameObject floatingTextPrefab; // assign your FloatingText prefab here

    void Update()
    {
        // Rotate slowly for visual feedback
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ✅ Get player component
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                // Apply the boost
                player.damageBoostAmount = damageBoost;
                player.boostDuration = duration;
                player.ApplyDamageBoost();

                // ✅ Show timer in UI
                if (GameManager.Instance)
                    GameManager.Instance.ShowBoosterTimer(duration);

                // ✅ Spawn floating text feedback
                if (floatingTextPrefab)
                {
                    Vector3 spawnPos = player.transform.position + Vector3.up * 2.2f;
                    GameObject ft = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
                    ft.GetComponent<FloatingText>().SetText($"x{damageBoost} Damage!", Color.cyan);
                }
            }

            // ✅ Play pickup sound if assigned
            if (pickupSound)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            // ✅ Destroy the potion after pickup
            Destroy(gameObject);
        }
    }
}
