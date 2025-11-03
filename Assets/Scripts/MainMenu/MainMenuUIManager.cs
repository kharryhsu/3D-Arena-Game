using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    [Header("Camera Mounts")]
    public Transform mainMenuMount;
    public Transform rulesMount;
    public Transform intro1Mount;
    public Transform intro2Mount;
    public Transform intro3Mount;

    [Header("Camera Follower")]
    public CameraMountFollow cameraFollow;

    public void ShowMainMenu()
    {
        if (cameraFollow && mainMenuMount && intro1Mount && intro2Mount && intro3Mount)
        {
            cameraFollow.SetMount(mainMenuMount);
            Debug.Log("Switched camera to Main Menu view");
        }
    }

    public void ShowRules()
    {
        if (cameraFollow && rulesMount && intro1Mount && intro2Mount && intro3Mount)
        {
            cameraFollow.SetMount(rulesMount);
            Debug.Log("Switched camera to Rules view");
        }
    }

    public void Next1()
    {
        if (cameraFollow && rulesMount && intro1Mount && intro2Mount && intro3Mount)
        {
            cameraFollow.SetMount(intro1Mount);
            Debug.Log("Switched camera to Intro_1 view");
        }
    }

    public void Next2()
    {
        if (cameraFollow && rulesMount && intro1Mount && intro2Mount && intro3Mount)
        {
            cameraFollow.SetMount(intro2Mount);
            Debug.Log("Switched camera to Intro_2 view");
        }
    }

    public void Next3()
    {
        if (cameraFollow && rulesMount && intro1Mount && intro2Mount && intro3Mount)
        {
            cameraFollow.SetMount(intro3Mount);
            Debug.Log("Switched camera to Intro_3 view");
        }
    }
}
