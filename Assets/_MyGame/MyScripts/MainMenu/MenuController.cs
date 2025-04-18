using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    //public GameObject[] menuController;
    public GameObject exitPanel;
    //public Button settingButton;
    //public Button closeButton;

    [SerializeField] TMP_Text[] totalChipsTxt;

    private int mainMenuIndex = 0;
    private int loadingpPanelIndex = 1;


    [Header("Games Stats")]
    public TMP_Text totalGamesPlayedTxt;
    public TMP_Text totalGamesWonTxt;
    public TMP_Text totalGamesLostTxt;
    public TMP_Text totalTieGamesTxt;
    public TMP_Text totalJackpotsTxt;


    public static MenuController instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }


    void Start()
    {
        SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.BGMusic, true);
        SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.BridSound, true);
        exitPanel.SetActive(false);
        //menuController[mainMenuIndex].SetActive(true);
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


    public void OnExitButtonClick()
    {
        Application.Quit();
    }
    public void OnPlayBtnClick()
    {
        //menuController[loadingpPanelIndex].SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    public void UpDateTotalChipsTxts()
    {
        foreach (var txt in totalChipsTxt)
            txt.text = LocalSetting.GetTotalCash().ToString("N0");
    }

    public void UpdateInStartTxts()
    {
        foreach (var txt in totalChipsTxt)
            txt.text = LoginWithGoogle.instance.totalCash.ToString("N0");

        Debug.Log("Updating");
    }

    void UpdateGameStats()
    {
        totalGamesPlayedTxt.text = $"PLAYED : {LocalSetting.TotalGamesPlayed}";
        totalGamesWonTxt.text = $"WON : {LocalSetting.TotalGamesWon}";
        totalGamesLostTxt.text = $"LOST : {LocalSetting.TotalGamesLost}";
        totalTieGamesTxt.text = $"TIE : {LocalSetting.TotalTieGames}";
        totalJackpotsTxt.text = $"JACKPOT : {LocalSetting.TotalJackPOT}";
    }
}
