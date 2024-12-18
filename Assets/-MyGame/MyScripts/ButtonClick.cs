using UnityEngine;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{
    Button thisBtn;
    void Start()
    {
        if (GetComponent<Button>() != null)
        {
            thisBtn = GetComponent<Button>();
            thisBtn.onClick.AddListener(() => PlayButtonSound());
        }
    }

    public void PlayButtonSound()
    {
        SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ButtonSound);
    }
}
