using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject[] menuController;
    public GameObject exitPanel;
    //public Button[] buttons;
    public Button settingButton;
    public Button closeButton;

    [SerializeField] TMP_Text[] totalChipsTxt;

    private int mainMenuIndex = 0;
    // private int settingButtonIndex = 1;
    private int loadingpPanelIndex = 1;


    [Header("Games Stats")]
    public TMP_Text totalGamesPlayedTxt;
    public TMP_Text totalGamesWonTxt;
    public TMP_Text totalGamesLostTxt;
    public TMP_Text totalTieGamesTxt;
    public TMP_Text totalJackpotsTxt;


    void Start()
    {
        exitPanel.SetActive(false);
        menuController[mainMenuIndex].SetActive(true);
        UpDateTotalChipsTxts();
        UpdateGameStats();
    }
    private void Update()
    {
        if (Keyboard.current.escapeKey.wasReleasedThisFrame)
        {
            if (exitPanel.activeInHierarchy)
                exitPanel.SetActive(false);
            else
                exitPanel.SetActive(true);
        }
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        if (exitPanel.activeInHierarchy)
    //            exitPanel.SetActive(false);
    //        else
    //            exitPanel.SetActive(true);
    //    }
    //}

    public void OnExitButtonClick()
    {
        Application.Quit();
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

    void UpdateGameStats()
    {
        totalGamesPlayedTxt.text = LocalSetting.TotalGamesPlayed.ToString();
        totalGamesWonTxt.text = LocalSetting.TotalGamesWon.ToString();
        totalGamesLostTxt.text = LocalSetting.TotalGamesLost.ToString();
        totalTieGamesTxt.text = LocalSetting.TotalTieGames.ToString();
        totalJackpotsTxt.text = LocalSetting.TotalJackPOT.ToString();
    }
}
