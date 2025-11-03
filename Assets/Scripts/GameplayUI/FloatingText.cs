using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [Header("Float Settings")]
    public float floatSpeed = 1f;
    public float lifetime = 1.2f;
    
    [Header("Position Offset")]
    public Vector3 offset = new Vector3(0, 1.5f, 0); // ⬅️ smaller offset (closer to enemy)
    public Vector3 randomIntensity = new Vector3(0.3f, 0.3f, 0.3f); // ⬅️ reduced randomness

    [Header("Text Settings")]
    public float textScale = 0.5f; // ⬅️ 50% smaller
    public TextMeshPro textMesh;

    private float timer;

    void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshPro>();
    }

    void Start()
    {
        transform.localPosition += offset + new Vector3(
            Random.Range(-randomIntensity.x, randomIntensity.x),
            Random.Range(-randomIntensity.y, randomIntensity.y),
            Random.Range(-randomIntensity.z, randomIntensity.z)
        );

        if (textMesh)
            textMesh.transform.localScale = Vector3.one * textScale; // apply smaller scale

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        timer += Time.deltaTime;
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Always face the camera
        if (Camera.main)
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        // Smooth fade out
        if (textMesh)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / lifetime);
            Color c = textMesh.color;
            c.a = alpha;
            textMesh.color = c;
        }
    }

    public void SetText(string message, Color color)
    {
        if (textMesh)
        {
            textMesh.text = message;
            textMesh.color = color;
        }
    }
}
