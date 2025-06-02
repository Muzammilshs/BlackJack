using UnityEngine;

public class GoogleAuth : MonoBehaviour
{
    #region Fields

    // Reference to the login UI panel
    public GameObject loginPanel;

    // Reference to the main UI panel
    public GameObject mainPanel;

    // Key used to check if the app is launched for the first time
    private const string FirstTimeKey = "FirstTimeLaunch";

    #endregion

    #region Unity Methods

    // Called on script start
    void Start()
    {
        CheckFirstLaunch();
    }

    #endregion

    #region First Launch Logic

    /// <summary>
    /// Checks if this is the first time the app is launched.
    /// Shows the login panel if so, otherwise shows the main panel.
    /// </summary>
    public void CheckFirstLaunch()
    {
        if (!PlayerPrefs.HasKey(FirstTimeKey))
        {
            Debug.Log("First time launch!");

            loginPanel.SetActive(true);
            if (mainPanel != null)
                mainPanel.SetActive(false);
        }
        else
        {
            Debug.Log("Not the first time.");

            loginPanel.SetActive(false);
            if (mainPanel != null)
                mainPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Sets the first time launch key in PlayerPrefs to indicate the app has been launched.
    /// </summary>
    public void ChangeTheKey()
    {
        PlayerPrefs.SetInt(FirstTimeKey, 1);
        PlayerPrefs.Save();
        Debug.Log("Key changed to 1");
    }

    #endregion
}
