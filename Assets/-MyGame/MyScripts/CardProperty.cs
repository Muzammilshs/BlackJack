using UnityEngine;
using UnityEngine.UI;
namespace com.mani.muzamil.amjad
{
    public class CardProperty : MonoBehaviour
    {
        public CardState.SUIT Suit;
        public CardState.CARDVALUE Card;
        public int Power;
        public int CardIndexInArray;
        public int CardPowerHeartsGame;

        [ShowOnly]
        public int cardOwnerViewID;

        Sprite originalSprite;
        public void ShowDummySkin()
        {
            originalSprite = GetComponent<Image>().sprite;

        }

        public void ShowOriginalSprite() => GetComponent<Image>().sprite = originalSprite;
    }
}