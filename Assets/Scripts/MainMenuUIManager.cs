using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    [Header("Menu Groups")]
    public GameObject mainMenuGroup;
    public GameObject rulesGroup;

    void Start()
    {
        ShowMainMenu();
    }

    public void ShowRules()
    {
        Debug.Log("ShowRules");
        mainMenuGroup.SetActive(false);
        rulesGroup.SetActive(true);
    }

    public void ShowMainMenu()
    {
        Debug.Log("ShowMainMenu");
        mainMenuGroup.SetActive(true);
        rulesGroup.SetActive(false);
    }
}
