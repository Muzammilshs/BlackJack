using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    #region Fields

    // Reference to the Button component on this GameObject
    Button btn;

    #endregion

    #region Unity Methods

    // Called when the script instance is being loaded
    void Start()
    {
        btn = GetComponent<Button>();
        if (btn)
            btn.onClick.AddListener(PlayButtonSound);
        else
            Debug.LogError("Button component is missing on " + gameObject.name);
    }

    #endregion

    #region Sound Logic

    /// <summary>
    /// Plays the button click sound using the SoundManagerBJ.
    /// </summary>
    void PlayButtonSound()
    {
        SoundManagerBJ.Instance.PlayAudioClip(SoundManagerBJ.AllSounds.ButtonSound);
    }

    #endregion
}
