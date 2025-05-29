using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SoundMgrScriptable", order = 1)]
public class SoundMgrScriptable : ScriptableObject
{
    public GameObject AudioSourcePrefab;
    public AudioClip BGMusic,
        BridSound,
        ButtonSound,
        cardFlip,
        cardDrop,
        cardSlightlyUp,
        cardCollect,
        chipBeting,
        chipCollect,
        coinAdding,
        coinRemoving,
        tieSound,
        winSound,
        jackpotSound,
        loseSound,
        clearAllChips;

}