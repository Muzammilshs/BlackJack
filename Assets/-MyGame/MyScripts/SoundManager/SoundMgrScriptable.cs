using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SoundMgrScriptable", order = 1)]
public class SoundMgrScriptable : ScriptableObject
{
    public GameObject AudioSourcePrefab;
    public AudioClip BGMusic,
        ButtonSound,
        CardFlip,
        Notification,
        WinFinal;

}