using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Main game manager for the blackjack game. Handles scene transitions, UI panels, card design,
/// and global utility functions.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Inspector Fields

    [SerializeField] Rm refMgr;                         // Reference to the main resource manager
    [HideInInspector] public Sprite dummyCardSprite;    // The current card back sprite
    public GameObject shopPanel;                        // Shop panel UI
    [SerializeField] GameObject exitPanel;              // Exit confirmation panel UI

    #endregion

    #region Unity Methods

    /// <summary>
    /// Initializes the game manager, checks login, and hides the exit panel.
    /// </summary>
    private void Start()
    {
        QualitySettings.vSyncCount = 0; // Disable VSync
        Application.targetFrameRate = 120; // Set frame rate above 60fps
        if (LoginWithGoogle.instance == null)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        exitPanel.SetActive(false);
    }

    /// <summary>
    /// Handles escape key for showing/hiding the exit panel.
    /// </summary>
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

    #endregion

    #region Card Design

    /// <summary>
    /// Sets the card back design based on the selected index in settings.
    /// </summary>
    public void SetCardDesign()
    {
        dummyCardSprite = Rm.Instance.cardsManager.allCards.CardBackDesigns[LocalSettingBlackJack.SelectedDesignIndex];
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Destroys all GameObjects in the list and returns a cleared list.
    /// </summary>
    /// <param name="list">List of GameObjects to clear.</param>
    /// <returns>Cleared list.</returns>
    public List<GameObject> ClearList(List<GameObject> list)
    {
        if (list == null)
        {
            list = new List<GameObject>();
            return list;
        }
        if (list.Count > 0)
        {
            for (int i = list.Count - 1; i >= 0; i--)
                Destroy(list[i]);
            list.Clear();
        }
        return list;
    }

    /// <summary>
    /// Shows the shop panel UI.
    /// </summary>
    public void ShowShopPanel()
    {
        shopPanel.SetActive(true);
    }

    #endregion

    #region Button Functions

    /// <summary>
    /// Called when the Home button is clicked. Returns to the previous scene and destroys login instance.
    /// </summary>
    public void OnHomeBtnClick()
    {
        if (LoginWithGoogle.instance != null)
            DestroyImmediate(LoginWithGoogle.instance.gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    /// <summary>
    /// Called when the Exit button is clicked. Quits the application.
    /// </summary>
    public void OnExitButtonClick()
    {
        Application.Quit();
    }

    /// <summary>
    /// Called when the Restart button is clicked. Reloads the current scene.
    /// </summary>
    public void ResetartGameBtnClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion
}
