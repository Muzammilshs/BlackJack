using com.muzamil;
using UnityEngine;


[CreateAssetMenu(fileName = "CardsSprites", menuName = "ScriptableObjects/CardsContainer", order = 1)]
public class CardsContainer : ScriptableObject
{
    public CardProperty[] Card;
    public Sprite[] CardBackDesigns;
}