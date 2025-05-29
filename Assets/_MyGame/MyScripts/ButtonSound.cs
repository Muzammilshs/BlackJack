using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    Button btn;
    void Start()
    {
        btn = GetComponent<Button>();
        if(btn)
            btn.onClick.AddListener(PlayButtonSound);
        else
            Debug.LogError("Button component is missing on " + gameObject.name);
    }

    void PlayButtonSound()
    {
        SoundManagerBJ.Instance.PlayAudioClip(SoundManagerBJ.AllSounds.ButtonSound);
    }
}
