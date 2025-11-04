using UnityEngine;

public class HealingItem : MonoBehaviour
{
    [Header("Healing Settings")]
    public float healAmount = 30f;
    public float floatHeight = 0.25f;
    public float floatSpeed = 2f;
    public float rotationSpeed = 80f;
    
    [Header("Audio")]
    public AudioClip pickupSound;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Floating + rotation animation
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = startPos + new Vector3(0, offset, 0);
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth)
            {
                SoundManager.Instance?.PlayHealthItemPickup();

                playerHealth.Heal(healAmount);

                if (pickupSound)
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                
                Destroy(gameObject);
            }
        }
    }
}
