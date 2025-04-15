using UnityEngine;

public class GoogleAuth : MonoBehaviour
{
    public GameObject loginPanel;
    public GameObject mainPanel;

    private const string FirstTimeKey = "FirstTimeLaunch";

    void Start()
    {
        CheckFirstLaunch();
    }
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

    public void ChangeTheKey()
    {
        PlayerPrefs.SetInt(FirstTimeKey, 1);
        PlayerPrefs.Save();
        Debug.Log("Key changed to 1");
    }
}
