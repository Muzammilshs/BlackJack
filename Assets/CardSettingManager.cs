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

        selectedDesignIndex = LocalSetting.SelectedDesignIndex;
        SetInitialToggle();
        UpdateCheckmarks();
        //Debug.Log($"Loaded Design: {selectedDesignIndex} from PlayerPrefs");
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
        for (int i = 0; i < designToggles.Length; i++)
        {
            if (designToggles[i].isOn && i != selectedDesignIndex)
            {
                selectedDesignIndex = i;
                LocalSetting.SelectedDesignIndex = selectedDesignIndex;
                UpdateCheckmarks();
                Debug.LogError($"Saved Design: {selectedDesignIndex} to PlayerPrefs");
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
