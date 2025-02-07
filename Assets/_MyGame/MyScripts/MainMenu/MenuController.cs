using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject[] menuController;
    //public Button[] buttons;
    public Button settingButton;
    public Button closeButton;

    [SerializeField] TMP_Text[] totalChipsTxt;

    private int mainMenuIndex = 0;
    // private int settingButtonIndex = 1;
    private int loadingpPanelIndex = 1;

    void Start()
    {
        menuController[mainMenuIndex].SetActive(true);
        UpDateTotalChipsTxts();
    }

    public void OnPlayBtnClick()
    {
        menuController[loadingpPanelIndex].SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    public void UpDateTotalChipsTxts()
    {
        foreach (var txt in totalChipsTxt)
            txt.text = LocalSetting.GetTotalCash().ToString();
    }
}
