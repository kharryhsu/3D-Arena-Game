using UnityEngine;

public class DoorMechanism : MonoBehaviour
{
    private bool isVisible = true;

    void Start()
    {
        // Register this door with the GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterDoorMechanism(this);
        }
    }

    // Called by GameManager when all enemies are defeated
    public void HideDoor()
    {
        if (!isVisible) return;
        isVisible = false;

        gameObject.SetActive(false);
        Debug.Log("DoorMechanism: Door is now hidden!");
    }
}
