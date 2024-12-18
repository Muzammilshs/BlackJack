using UnityEngine;
using UnityEngine.UI;

public class Purchase : MonoBehaviour
{
    public Button[] buttons;

    void Start()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }


    }

    void Update()
    {


    }


    public void OnButtonClick(int buttonIndex)
    {
        SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ButtonSound);
        Debug.Log("Button" + buttonIndex + "This item has been purchased");
    }

}
