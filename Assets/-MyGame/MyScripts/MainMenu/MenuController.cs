using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject[] menuController;
    public Button[] buttons;
    //public Button settingButton;
    public Button closeButton;

    [SerializeField] TMP_Text totalChipsTxt;

    private int mainMenuIndex = 0;
    private int settingButtonIndex = 1;
    private int loadingpPanelIndex = 2;

    void Start()
    {
        menuController[mainMenuIndex].SetActive(true);
        menuController[settingButtonIndex].SetActive(false);


        closeButton.onClick.AddListener(OpenSettingMenu);
        closeButton.onClick.AddListener(CloseSettingMenu);

        UpDateTotalChipsTxts();
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

    public void OnPlayBtnClick()
    {
        menuController[loadingpPanelIndex].SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    public void UpDateTotalChipsTxts()
    {
        totalChipsTxt.text = LocalSetting.GetTotalCash().ToString();
    }
}
