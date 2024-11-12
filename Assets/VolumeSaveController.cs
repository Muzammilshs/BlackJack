using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class VolumeSaveController : MonoBehaviour
{
    public Slider volumeSlider;         
    public TextMeshProUGUI volumeText;  
    private const string VolumePrefKey = "VolumeLevel";

    void Start()
    {
        // Load saved volume or set default to 0 if not set
        volumeSlider.value = PlayerPrefs.GetFloat(VolumePrefKey, 0);

        
        //UpdateVolume();
        
        volumeSlider.onValueChanged.AddListener(delegate { UpdateAndSaveVolume(); });
    }

    void UpdateAndSaveVolume()
    {
        
        AudioListener.volume = volumeSlider.value / 5f;

       
        volumeText.text = volumeSlider.value.ToString("0.000");

        
        PlayerPrefs.SetFloat(VolumePrefKey, volumeSlider.value);
        PlayerPrefs.Save();
    }
}
