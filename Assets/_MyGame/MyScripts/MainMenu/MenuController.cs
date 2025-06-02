using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    #region Fields

    // Reference to the exit confirmation panel
    public GameObject exitPanel;

    // Array of text fields displaying total chips in various UI locations
    [SerializeField] TMP_Text[] totalChipsTxt;

    [Header("Games Stats")]
    public TMP_Text totalGamesPlayedTxt;
    public TMP_Text totalGamesWonTxt;
    public TMP_Text totalGamesLostTxt;
    public TMP_Text totalTieGamesTxt;
    public TMP_Text totalJackpotsTxt;

    // Singleton instance
    public static MenuController instance;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // Ensure only one instance exists (Singleton pattern)
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        // Play background music and bridge sound
        SoundManagerBJ.Instance.PlayAudioClip(SoundManagerBJ.AllSounds.BGMusic, true, 0.3f);
        SoundManagerBJ.Instance.PlayAudioClip(SoundManagerBJ.AllSounds.BridSound, true, 0.7f);

        // Hide exit panel at start
        exitPanel.SetActive(false);

        // Initialize UI with current values
        UpDateTotalChipsTxts();
        UpdateGameStats();
    }

    private void Update()
    {
        // Toggle exit panel when Escape key is released
        if (Keyboard.current.escapeKey.wasReleasedThisFrame)
        {
            if (exitPanel.activeInHierarchy)
                exitPanel.SetActive(false);
            else
                exitPanel.SetActive(true);
        }
    }

    #endregion

    #region UI Button Handlers

    // Called when the Exit button is clicked
    public void OnExitButtonClick()
    {
        Application.Quit();
    }

    // Called when the Play button is clicked
    public void OnPlayBtnClick()
    {
        // Load the next scene (gameplay)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    #endregion

    #region UI Update Methods

    // Updates all total chips text fields with local values
    public void UpDateTotalChipsTxtsWithLocalValues()
    {
        foreach (var txt in totalChipsTxt)
            txt.text = LocalSettingBlackJack.GetTotalCashLocal().ToString("N0");
    }

    // Updates all total chips text fields with server values
    public void UpDateTotalChipsTxts()
    {
        foreach (var txt in totalChipsTxt)
            txt.text = LocalSettingBlackJack.GetTotalCash().ToString("N0");
    }

    // Updates total chips text fields at start, using Google login if available
    public void UpdateInStartTxts()
    {
        if (totalChipsTxt == null || totalChipsTxt.Length == 0)
        {
            Debug.LogWarning("TotalChipsTxt array is empty or not assigned!");
            return;
        }

        int cash = LoginWithGoogle.instance != null ? LoginWithGoogle.instance.totalCash : 0;
        LocalSettingBlackJack.SetTotalCashLocal(cash);
        foreach (var txt in totalChipsTxt)
        {
            if (txt != null)
                txt.text = cash.ToString("N0");
        }
    }

    // Updates the game statistics UI fields
    void UpdateGameStats()
    {
        totalGamesPlayedTxt.text = $"PLAYED : {LocalSettingBlackJack.TotalGamesPlayed}";
        totalGamesWonTxt.text = $"WON : {LocalSettingBlackJack.TotalGamesWon}";
        totalGamesLostTxt.text = $"LOST : {LocalSettingBlackJack.TotalGamesLost}";
        totalTieGamesTxt.text = $"TIE : {LocalSettingBlackJack.TotalTieGames}";
        totalJackpotsTxt.text = $"JACKPOT : {LocalSettingBlackJack.TotalJackPOT}";
    }

    #endregion
}
