using UnityEngine;
using UnityEngine.UI;

public class CardSettingManager : MonoBehaviour
{
    public Sprite[] cardBackDesigns;
    public Toggle[] designToggles;
    public Image[] checkmarks;

    private int selectedDesignIndex;

    void Start()
    {

        selectedDesignIndex = PlayerPrefs.GetInt("CardBackDesign", 0);
        SetInitialToggle();
        UpdateCheckmarks();
        Debug.Log($"Loaded Design: {selectedDesignIndex} from PlayerPrefs");
    }

    private void SetInitialToggle()
    {

        if (selectedDesignIndex < designToggles.Length)
        {
            designToggles[selectedDesignIndex].isOn = true;
        }
    }


    public void OnToggleChanged()
    {

        //for (int i = 0; i < designToggles.Length; i++)
        //{
        //    if (designToggles[i].isOn)
        //    {
        //        if (i != selectedDesignIndex)
        //        {

        //            selectedDesignIndex = i;
        //            PlayerPrefs.SetInt("CardBackDesign", selectedDesignIndex);
        //            PlayerPrefs.Save();
        //            UpdateCheckmarks();
        //            Debug.Log($"Saved Design: {selectedDesignIndex} to PlayerPrefs");
        //            break;  

        //        }

        //    }
        //}
        for (int i = 0; i < designToggles.Length; i++)
        {
            if (designToggles[i].isOn && i != selectedDesignIndex)
            {
                selectedDesignIndex = i;
                PlayerPrefs.SetInt("CardBackDesign", selectedDesignIndex);
                PlayerPrefs.Save();
                UpdateCheckmarks();
                Debug.Log($"Saved Design: {selectedDesignIndex} to PlayerPrefs");
                break;
            }
        }

    }

    private void UpdateCheckmarks()
    {

        for (int i = 0; i < checkmarks.Length; i++)
        {
            checkmarks[i].gameObject.SetActive(i == selectedDesignIndex);
        }
    }
}
