using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject[] menuController;
    public Button[] buttons;
    public Button settingButton;
    public Button closeButton;

    private int mainMenuIndex = 0;
    private int settingButtonIndex = 1;

    void Start()
    {
        menuController[mainMenuIndex].SetActive(true);
        menuController[settingButtonIndex].SetActive(false); 

        
        settingButton.onClick.AddListener(OpenSettingMenu);
        closeButton.onClick.AddListener(CloseSettingMenu);


    }   

    public void OpenSettingMenu()
    {
        menuController[mainMenuIndex].SetActive(true);
        menuController[settingButtonIndex].SetActive(true);
    }

    public void CloseSettingMenu()
    {
        menuController[mainMenuIndex].SetActive(true);
        menuController[settingButtonIndex].SetActive(false);
    }


}
